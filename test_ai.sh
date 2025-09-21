#!/bin/bash

# Neural Rink AI Tester
# This script tests the trained AI goalie in the simplified environment

echo "🤖 Neural Rink AI Tester"
echo "======================="

# Check if virtual environment exists
if [ ! -d "neural-rink-env" ]; then
    echo "❌ Virtual environment not found!"
    echo "Please run: ./setup_environment.sh"
    exit 1
fi

# Activate virtual environment
echo "🔧 Activating virtual environment..."
source neural-rink-env/bin/activate

# Check if trained model exists
if [ ! -f "results/neural_rink_full/final_model.zip" ]; then
    echo "❌ No trained model found!"
    echo "Please run training first: ./start_training.sh"
    exit 1
fi

echo "🏒 Testing trained AI goalie..."
echo "This will show the AI goalie in action!"
echo ""

# Test the trained AI
python alternative_train.py --run-id test_ai --max-steps 1000

echo ""
echo "✅ AI test completed!"
echo "🎮 For the full Unity game, follow QUICK_UNITY_SETUP.md"
