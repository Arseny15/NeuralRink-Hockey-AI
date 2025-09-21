#!/usr/bin/env python3
"""
Alternative Training Script for Neural Rink
Uses Stable-Baselines3 instead of ML-Agents for Python 3.13 compatibility
"""

import os
import sys
import argparse
import numpy as np
import torch
import gymnasium as gym
from stable_baselines3 import PPO
from stable_baselines3.common.env_util import make_vec_env
from stable_baselines3.common.callbacks import EvalCallback, StopTrainingOnRewardThreshold
import matplotlib.pyplot as plt

def create_hockey_environment():
    """
    Create a simplified hockey environment for training
    This is a placeholder - in reality, you'd connect to Unity
    """
    class HockeyEnv(gym.Env):
        def __init__(self):
            super().__init__()
            # Define action and observation space
            self.action_space = gym.spaces.Box(
                low=np.array([-1, -1]),  # X, Z movement
                high=np.array([1, 1]),
                dtype=np.float32
            )
            
            self.observation_space = gym.spaces.Box(
                low=-np.inf,
                high=np.inf,
                shape=(12,),  # 12 observations as defined in GoalieAgent
                dtype=np.float32
            )
            
            self.reset()
        
        def reset(self, seed=None, options=None):
            super().reset(seed=seed)
            # Initialize game state
            self.goalie_pos = np.array([0.0, 0.0])
            self.puck_pos = np.array([-5.0, 0.0])
            # Puck starts with velocity toward goal (simulating a shot)
            self.puck_vel = np.array([2.0, np.random.uniform(-0.5, 0.5)])
            self.episode_step = 0
            self.max_steps = 300  # 6 seconds at 50 FPS
            self.puck_shot = False
            
            return self._get_observation(), {}
        
        def step(self, action):
            # Apply action (goal movement)
            self.goalie_pos += action * 0.1  # Scale movement
            self.goalie_pos = np.clip(self.goalie_pos, [-2.0, -1.5], [2.0, 1.5])
            
            # Update puck physics (simplified)
            self.puck_pos += self.puck_vel * 0.02  # 50 FPS timestep
            self.puck_vel *= 0.99  # Friction
            
            # Calculate rewards
            reward = 0
            done = False
            info = {}
            
            # Intermediate reward: stay close to puck (encourages positioning)
            distance_to_puck = np.linalg.norm(self.puck_pos - self.goalie_pos)
            reward += max(0, 10 - distance_to_puck) * 0.1  # Small positive reward for being close
            
            # Check for end conditions
            if abs(self.puck_pos[0]) > 8:  # Out of bounds (miss)
                reward = 50  # Miss bonus
                done = True
                info['result'] = 'miss'
            elif abs(self.puck_pos[1]) < 0.5 and self.puck_pos[0] > 7:  # Goal
                reward = -200  # Goal penalty
                done = True
                info['result'] = 'goal'
            elif self._check_save():  # Save
                reward = 200  # Save bonus
                done = True
                info['result'] = 'save'
            
            self.episode_step += 1
            if self.episode_step >= self.max_steps:
                # Timeout - no clear result, small penalty
                reward = -10
                done = True
                info['result'] = 'timeout'
            
            return self._get_observation(), reward, done, False, info
        
        def _get_observation(self):
            """Get current observation vector"""
            return np.array([
                self.goalie_pos[0],  # Goalie X position
                self.goalie_pos[1],  # Goalie Z position
                self.puck_pos[0],    # Puck X position
                self.puck_pos[1],    # Puck Z position
                self.puck_vel[0],    # Puck X velocity
                self.puck_vel[1],    # Puck Z velocity
                0.0, 0.0, 0.0, 0.0, 0.0, 0.0  # Placeholder for other observations
            ], dtype=np.float32)
        
        def _check_save(self):
            """Check if goalie made a save"""
            distance = np.linalg.norm(self.puck_pos - self.goalie_pos)
            # Save if close enough AND puck is in goal area
            in_goal_area = self.puck_pos[0] > 6 and abs(self.puck_pos[1]) < 1.0
            return distance < 1.0 and in_goal_area
        
        def render(self, mode='human'):
            """Simple text rendering"""
            print(f"Step {self.episode_step}: Goalie at {self.goalie_pos}, Puck at {self.puck_pos}")
    
    return HockeyEnv()

def train_goalie(run_id="neural_rink_alt", max_steps=100000, save_freq=10000):
    """
    Train the goalie using Stable-Baselines3 PPO
    """
    print(f"🏒 Starting Neural Rink Training (Alternative Method)")
    print(f"Run ID: {run_id}")
    print(f"Max Steps: {max_steps}")
    print("=" * 50)
    
    # Create environment
    env = create_hockey_environment()
    
    # Create PPO model
    model = PPO(
        "MlpPolicy",
        env,
        verbose=1,
        learning_rate=3e-4,
        n_steps=2048,
        batch_size=64,
        n_epochs=10,
        gamma=0.99,
        gae_lambda=0.95,
        clip_range=0.2,
        ent_coef=0.01,
        vf_coef=0.5,
        max_grad_norm=0.5,
        tensorboard_log=f"./results/{run_id}/"
    )
    
    # Create results directory
    os.makedirs(f"./results/{run_id}", exist_ok=True)
    
    # Setup callbacks
    eval_callback = EvalCallback(
        env,
        best_model_save_path=f"./results/{run_id}/best_model",
        log_path=f"./results/{run_id}/eval_logs",
        eval_freq=save_freq,
        deterministic=True,
        render=False
    )
    
    # Start training
    print("🚀 Starting training...")
    model.learn(
        total_timesteps=max_steps,
        callback=eval_callback,
        progress_bar=True
    )
    
    # Save final model
    model.save(f"./results/{run_id}/final_model")
    print(f"✅ Training complete! Model saved to ./results/{run_id}/")
    
    # Test the trained model
    print("\n🧪 Testing trained model...")
    obs, _ = env.reset()
    total_reward = 0
    
    for i in range(10):  # Test 10 episodes
        obs, _ = env.reset()
        episode_reward = 0
        
        while True:
            action, _ = model.predict(obs, deterministic=True)
            obs, reward, done, truncated, info = env.step(action)
            episode_reward += reward
            
            if done or truncated:
                result = info.get('result', 'timeout')
                print(f"Episode {i+1}: {result.upper()} (Reward: {episode_reward:.1f})")
                total_reward += episode_reward
                break
    
    avg_reward = total_reward / 10
    print(f"\n📊 Average Test Reward: {avg_reward:.1f}")
    
    if avg_reward > 100:
        print("🎉 Goalie is performing well!")
    elif avg_reward > 0:
        print("👍 Goalie is learning!")
    else:
        print("📚 Goalie needs more training!")
    
    return model

def main():
    parser = argparse.ArgumentParser(description="Train Neural Rink Goalie (Alternative Method)")
    parser.add_argument("--run-id", default="neural_rink_alt", help="Training run identifier")
    parser.add_argument("--max-steps", type=int, default=100000, help="Maximum training steps")
    parser.add_argument("--save-freq", type=int, default=10000, help="Model save frequency")
    
    args = parser.parse_args()
    
    try:
        model = train_goalie(args.run_id, args.max_steps, args.save_freq)
        print("\n🎯 Training completed successfully!")
        print(f"📈 View results: tensorboard --logdir ./results/{args.run_id}/")
        
    except Exception as e:
        print(f"❌ Training failed: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()
