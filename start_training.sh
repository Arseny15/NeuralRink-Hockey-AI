#!/bin/bash

# Neural Rink Training Starter
# This script ensures the virtual environment is activated and starts training

echo "🏒 Neural Rink Training Starter"
echo "==============================="

# Check if virtual environment exists
if [ ! -d "neural-rink-env" ]; then
    echo "❌ Virtual environment not found!"
    echo "Please run: pip install -r requirements_alternative.txt"
    exit 1
fi

# Activate virtual environment
echo "🔧 Activating virtual environment..."
source neural-rink-env/bin/activate

# Check if training is already running
if pgrep -f "alternative_train.py" > /dev/null; then
    echo "⚠️  Training is already running!"
    echo "Check progress with: ./check_status.sh"
    exit 1
fi

# Start training
echo "🚀 Starting 100k step training..."
echo "This will take approximately 20-30 minutes..."
echo ""

python alternative_train.py --run-id neural_rink_full --max-steps 100000

echo ""
echo "✅ Training completed!"
echo "📊 View results: ./start_tensorboard.sh"
