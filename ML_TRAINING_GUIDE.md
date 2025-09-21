# Neural Rink ML Training Guide

## 🎯 **Complete ML-Agents Training Process**

This guide walks you through the entire ML training process from setup to deployment.

## 📋 **Prerequisites**

### **1. Python Version Compatibility**
⚠️ **IMPORTANT**: ML-Agents requires Python 3.8-3.10. Python 3.13 is not supported yet.

**Option A: Use Python 3.10 (Recommended)**
```bash
# Using pyenv (if installed)
pyenv install 3.10.12
pyenv local 3.10.12

# Or using conda
conda create -n neural-rink python=3.10
conda activate neural-rink
```

**Option B: Use Docker (No Python version issues)**
```bash
# Use the provided Docker setup
chmod +x docker_train.sh
./docker_train.sh
```

**Option C: Manual Installation**
```bash
# Install Python dependencies (Python 3.8-3.10 only)
pip install -r requirements.txt

# Verify installation
mlagents-learn --help
```

### **2. Unity Setup**
- Unity 6 (6000.x)
- ML-Agents 4.0 package installed
- Training.unity scene created with proper setup

## 🏗️ **Training Environment Setup**

### **Step 1: Create Training Scene**
1. Open Unity and create `Assets/Scenes/Training.unity`
2. Follow the Unity Setup Guide to create the scene
3. Ensure all components are properly configured:
   - GoalieAgent with Behavior Parameters
   - TrainingSwitch (TrainingMode: true)
   - BootstrapTraining script
   - GameDirector with proper references

### **Step 2: Configure ML-Agents**
1. Select the Goalie in Training scene
2. Verify **Behavior Parameters**:
   - Behavior Name: `Goalie` (must match YAML)
   - Vector Observation: `12` (from CollectObservations)
   - Actions: `Continuous 2` (X and Z movement)
3. Add **Decision Requester** component
4. Set Decision Interval: `1` (every frame for training)

### **Step 3: Validate Training Setup**
```bash
# Test ML-Agents connection
mlagents-learn Assets/ML-Agents/NeuralRink_PPO.yaml --test
```

## 🚀 **Training Process**

### **Phase 1: Basic Training (Start Here)**

#### **1.1 Initial Training Run**
```bash
# Start basic training
python train.py --run-id neural_rink_v1 --max-steps 100000

# Monitor with TensorBoard
tensorboard --logdir neural_rink_logs
```

#### **1.2 Training Parameters**
- **Episode Length**: 30 seconds
- **Max Steps**: 100,000 (initial)
- **Learning Rate**: 3.0e-4
- **Batch Size**: 64
- **Buffer Size**: 2048

#### **1.3 Expected Results**
- **Early Training** (0-25K steps): Random movement, low save rate
- **Mid Training** (25K-75K steps): Basic positioning, 20-40% save rate
- **Late Training** (75K+ steps): Improved positioning, 40-60% save rate

### **Phase 2: Advanced Training**

#### **2.1 Curriculum Learning**
The YAML config includes 5 progressive lessons:

```yaml
curriculum:
  thresholds: [0.1, 0.3, 0.5, 0.7, 0.9]
  
  lesson_1: # Basic positioning
    episode_length: 15
    save_bonus: 2.0
    
  lesson_2: # Movement and timing
    episode_length: 25
    save_bonus: 1.5
    
  lesson_3: # Advanced positioning
    episode_length: 30
    save_bonus: 1.0
    
  lesson_4: # Full difficulty
    episode_length: 30
    time_penalty: -0.001
    
  lesson_5: # Randomized physics
    randomize_physics: true
    physics_randomization_range: 0.15
```

#### **2.2 Extended Training**
```bash
# Extended training run
python train.py --run-id neural_rink_v2 --max-steps 500000 --resume

# Multi-environment training (faster)
python train.py --run-id neural_rink_v3 --num-envs 4 --max-steps 1000000
```

### **Phase 3: Specialized Training**

#### **3.1 Human Player Training**
```bash
# Train against human players (requires Play scene)
python train.py --run-id neural_rink_human --env-path "Builds/NeuralRink_Play_Windows.exe"
```

#### **3.2 Robustness Training**
```bash
# Train with physics randomization
python train.py --run-id neural_rink_robust --max-steps 750000
```

## 📊 **Training Monitoring**

### **TensorBoard Metrics**
Monitor these key metrics:

1. **Cumulative Reward**: Should increase over time
2. **Episode Length**: Should stabilize around 30 seconds
3. **Policy Loss**: Should decrease and stabilize
4. **Value Loss**: Should decrease and stabilize
5. **Save Rate**: Target 60-80% for good performance

### **Training Console Output**
```
INFO:mlagents.trainers:Goalie: Step: 10000. Time Elapsed: 123.456 s. Mean Reward: -0.234
INFO:mlagents.trainers:Goalie: Step: 20000. Time Elapsed: 245.789 s. Mean Reward: -0.156
INFO:mlagents.trainers:Goalie: Step: 30000. Time Elapsed: 368.123 s. Mean Reward: -0.089
```

### **Expected Training Timeline**
- **0-1 hour**: Basic movement patterns
- **1-3 hours**: Positioning improvements
- **3-6 hours**: Save technique development
- **6-12 hours**: Advanced positioning and timing
- **12+ hours**: Master-level performance

## 🎮 **Training Scenarios**

### **Scenario 1: Basic Positioning**
- **Goal**: Learn to stay in front of goal
- **Reward**: High save bonus, low time penalty
- **Duration**: 50K steps

### **Scenario 2: Movement and Timing**
- **Goal**: Learn to move toward puck
- **Reward**: Moderate save bonus, distance penalty
- **Duration**: 100K steps

### **Scenario 3: Advanced Positioning**
- **Goal**: Learn optimal positioning angles
- **Reward**: Standard rewards, time pressure
- **Duration**: 150K steps

### **Scenario 4: Full Difficulty**
- **Goal**: Handle all shot types and speeds
- **Reward**: Full reward structure
- **Duration**: 200K steps

### **Scenario 5: Robustness**
- **Goal**: Handle physics variations
- **Reward**: Randomized physics parameters
- **Duration**: 250K steps

## 🔧 **Training Troubleshooting**

### **Common Issues**

#### **1. Training Not Starting**
```bash
# Check Unity connection
mlagents-learn Assets/ML-Agents/NeuralRink_PPO.yaml --test

# Verify scene setup
# - TrainingSwitch.TrainingMode = true
# - GoalieAgent has Behavior Parameters
# - Behavior name matches YAML
```

#### **2. Poor Performance**
```bash
# Check reward structure
# - Save bonus: +1.0
# - Goal penalty: -1.0
# - Distance penalty: -0.01
# - Time penalty: -0.001

# Verify observations
# - 12 observations from CollectObservations
# - All values normalized and bounded
```

#### **3. Training Instability**
```bash
# Reduce learning rate
learning_rate: 1.0e-4

# Increase batch size
batch_size: 128

# Add reward smoothing
reward_signals:
  extrinsic:
    gamma: 0.95
```

### **Performance Optimization**

#### **1. Faster Training**
```bash
# Use multiple environments
python train.py --num-envs 8 --max-steps 1000000

# Reduce episode length for faster iterations
episode_length: 15

# Increase decision frequency
decision_interval: 1
```

#### **2. Better Performance**
```bash
# Increase network size
hidden_units: 512
num_layers: 3

# Use curriculum learning
curriculum: enabled

# Add reward shaping
reward_signals:
  extrinsic:
    strength: 1.0
```

## 📈 **Training Evaluation**

### **Performance Metrics**
- **Save Rate**: Percentage of shots saved
- **Average Reward**: Mean reward per episode
- **Episode Length**: Time per episode
- **Positioning Accuracy**: Distance from optimal position

### **Evaluation Process**
```bash
# Test trained model
python train.py --run-id neural_rink_test --resume --max-steps 10000

# Compare with baseline
python train.py --run-id neural_rink_baseline --max-steps 10000
```

### **Success Criteria**
- **Good Goalie**: 60-70% save rate
- **Great Goalie**: 70-80% save rate
- **Expert Goalie**: 80%+ save rate

## 🚀 **Deployment**

### **1. Export Trained Model**
```bash
# Find best model
ls results/neural_rink_v1/models/

# Copy to deployment folder
cp results/neural_rink_v1/models/Goalie.onnx models/Goalie_Best.onnx
```

### **2. Update Unity Scene**
1. Open Training.unity
2. Select Goalie GameObject
3. In Behavior Parameters, set **Model**: `Goalie_Best.onnx`
4. Set **Behavior Type**: `Inference Only`

### **3. Test in Play Scene**
1. Open Play.unity
2. Ensure Goalie uses trained model
3. Test against human player
4. Verify performance matches training

## 📊 **Training Analytics**

### **Key Performance Indicators**
- **Training Time**: Hours to reach target performance
- **Sample Efficiency**: Steps per performance improvement
- **Final Performance**: Save rate and reward metrics
- **Robustness**: Performance across different scenarios

### **Training Logs**
```bash
# View training logs
tensorboard --logdir neural_rink_logs

# Export metrics
python -c "
import pandas as pd
import json

# Load training data
with open('neural_rink_logs/neural_rink_v1/run_logs.json') as f:
    data = json.load(f)

# Create performance report
df = pd.DataFrame(data['metrics'])
df.to_csv('training_report.csv')
"
```

## 🎯 **Training Best Practices**

### **1. Start Simple**
- Begin with basic positioning
- Use high rewards for simple behaviors
- Gradually increase complexity

### **2. Monitor Closely**
- Watch TensorBoard regularly
- Check for training instability
- Adjust hyperparameters as needed

### **3. Validate Performance**
- Test against human players
- Compare with baseline performance
- Ensure generalization across scenarios

### **4. Iterate and Improve**
- Analyze failure cases
- Adjust reward structure
- Retrain with improvements

---

**Your Neural Rink goalie is now ready for training! 🏒🤖**

Follow this guide step-by-step to train a world-class AI goalie that can compete with human players.
