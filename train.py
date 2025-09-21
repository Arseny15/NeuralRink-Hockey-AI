#!/usr/bin/env python3
"""
Neural Rink Training Script
Automated training script for Unity ML-Agents goalie agent.

Usage:
    python train.py [options]

Examples:
    python train.py --run-id neural_rink_v1
    python train.py --run-id neural_rink_v2 --num-envs 4
    python train.py --run-id neural_rink_v3 --resume
"""

import argparse
import os
import sys
import subprocess
import json
import time
from pathlib import Path

def main():
    parser = argparse.ArgumentParser(description='Train Neural Rink goalie agent with ML-Agents')
    
    # Training parameters
    parser.add_argument('--run-id', type=str, default='neural_rink_default',
                       help='Unique identifier for this training run')
    parser.add_argument('--config', type=str, default='Assets/ML-Agents/NeuralRink_PPO.yaml',
                       help='Path to ML-Agents configuration file')
    parser.add_argument('--num-envs', type=int, default=1,
                       help='Number of parallel environments')
    parser.add_argument('--max-steps', type=int, default=1000000,
                       help='Maximum training steps')
    parser.add_argument('--resume', action='store_true',
                       help='Resume training from last checkpoint')
    parser.add_argument('--force', action='store_true',
                       help='Force overwrite existing run directory')
    
    # Environment parameters
    parser.add_argument('--env-path', type=str, default=None,
                       help='Path to Unity executable (for builds)')
    parser.add_argument('--base-port', type=int, default=5005,
                       help='Base port for environment communication')
    
    # Logging and monitoring
    parser.add_argument('--log-dir', type=str, default='neural_rink_logs',
                       help='Directory for training logs')
    parser.add_argument('--tensorboard', action='store_true',
                       help='Launch TensorBoard for monitoring')
    parser.add_argument('--no-save', action='store_true',
                       help='Disable model saving')
    
    # Advanced options
    parser.add_argument('--debug', action='store_true',
                       help='Enable debug logging')
    parser.add_argument('--timeout', type=int, default=60,
                       help='Environment timeout in seconds')
    
    args = parser.parse_args()
    
    # Validate environment
    if not validate_environment():
        sys.exit(1)
    
    # Prepare training command
    cmd = build_training_command(args)
    
    # Create log directory
    os.makedirs(args.log_dir, exist_ok=True)
    
    # Start training
    print(f"Starting Neural Rink training with run ID: {args.run_id}")
    print(f"Command: {' '.join(cmd)}")
    print("-" * 50)
    
    try:
        # Run training
        process = subprocess.run(cmd, check=True, capture_output=False)
        
        print("-" * 50)
        print("Training completed successfully!")
        
        # Post-training tasks
        post_training_tasks(args)
        
    except subprocess.CalledProcessError as e:
        print(f"Training failed with exit code: {e.returncode}")
        sys.exit(1)
    except KeyboardInterrupt:
        print("\nTraining interrupted by user")
        sys.exit(1)

def validate_environment():
    """Validate that the training environment is properly set up."""
    print("Validating training environment...")
    
    # Check if we're in the right directory
    if not os.path.exists('Assets/ML-Agents/NeuralRink_PPO.yaml'):
        print("ERROR: NeuralRink_PPO.yaml not found!")
        print("Please run this script from the Unity project root directory.")
        return False
    
    # Check if mlagents-learn is available
    try:
        result = subprocess.run(['mlagents-learn', '--help'], 
                              capture_output=True, text=True, timeout=10)
        if result.returncode != 0:
            print("ERROR: mlagents-learn command not found!")
            print("Please install Unity ML-Agents: pip install mlagents")
            return False
    except (subprocess.TimeoutExpired, FileNotFoundError):
        print("ERROR: mlagents-learn command not found!")
        print("Please install Unity ML-Agents: pip install mlagents")
        return False
    
    # Check Python version
    if sys.version_info < (3, 8):
        print("ERROR: Python 3.8 or higher is required!")
        return False
    
    print("✓ Environment validation passed")
    return True

def build_training_command(args):
    """Build the ML-Agents training command."""
    cmd = ['mlagents-learn', args.config, '--run-id', args.run_id]
    
    # Add environment parameters
    if args.env_path:
        cmd.extend(['--env', args.env_path])
    else:
        cmd.extend(['--base-port', str(args.base_port)])
    
    # Add training parameters
    cmd.extend(['--num-envs', str(args.num_envs)])
    cmd.extend(['--max-steps', str(args.max_steps)])
    
    # Add resume flag
    if args.resume:
        cmd.append('--resume')
    
    # Add force flag
    if args.force:
        cmd.append('--force')
    
    # Add timeout
    cmd.extend(['--timeout', str(args.timeout)])
    
    # Add debug flag
    if args.debug:
        cmd.append('--debug')
    
    # Add no-save flag
    if args.no_save:
        cmd.append('--no-save')
    
    return cmd

def post_training_tasks(args):
    """Perform post-training tasks."""
    print("Performing post-training tasks...")
    
    # Launch TensorBoard if requested
    if args.tensorboard:
        launch_tensorboard(args.log_dir)
    
    # Generate training report
    generate_training_report(args)
    
    # Copy best model to models directory
    copy_best_model(args.run_id)

def launch_tensorboard(log_dir):
    """Launch TensorBoard for training monitoring."""
    print("Launching TensorBoard...")
    
    try:
        # Start TensorBoard in background
        subprocess.Popen(['tensorboard', '--logdir', log_dir, '--port', '6006'],
                        stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        print("TensorBoard launched at http://localhost:6006")
    except FileNotFoundError:
        print("Warning: TensorBoard not found. Install with: pip install tensorboard")

def generate_training_report(args):
    """Generate a training report."""
    report_path = os.path.join(args.log_dir, f"{args.run_id}_report.json")
    
    report = {
        'run_id': args.run_id,
        'config_file': args.config,
        'num_environments': args.num_envs,
        'max_steps': args.max_steps,
        'timestamp': time.strftime('%Y-%m-%d %H:%M:%S'),
        'python_version': sys.version,
        'platform': sys.platform
    }
    
    with open(report_path, 'w') as f:
        json.dump(report, f, indent=2)
    
    print(f"Training report saved to: {report_path}")

def copy_best_model(run_id):
    """Copy the best trained model to the models directory."""
    models_dir = Path('models')
    models_dir.mkdir(exist_ok=True)
    
    # Look for the best model in the results directory
    results_dir = Path('results') / run_id
    if results_dir.exists():
        # Find the best model file
        model_files = list(results_dir.glob('*.onnx'))
        if model_files:
            best_model = max(model_files, key=lambda x: x.stat().st_mtime)
            
            # Copy to models directory
            dest_path = models_dir / f"{run_id}_best.onnx"
            import shutil
            shutil.copy2(best_model, dest_path)
            print(f"Best model copied to: {dest_path}")

if __name__ == '__main__':
    main()
