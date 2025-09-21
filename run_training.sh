#!/bin/bash

# Neural Rink Training Runner
# This script activates the virtual environment and runs training

echo "🏒 Starting Neural Rink Training..."
echo "=================================="

# Activate virtual environment
source neural-rink-env/bin/activate

# Check if training is already running
if pgrep -f "alternative_train.py" > /dev/null; then
    echo "⚠️  Training is already running!"
    echo "Check progress with: python check_status.py"
    exit 1
fi

# Start training
echo "🚀 Starting 100k step training..."
python alternative_train.py --run-id neural_rink_full --max-steps 100000

echo "✅ Training completed!"
echo "📊 View results: tensorboard --logdir results/"
