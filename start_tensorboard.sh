#!/bin/bash

# Neural Rink TensorBoard Starter
# This script ensures the virtual environment is activated and starts TensorBoard

echo "📊 Neural Rink TensorBoard Starter"
echo "=================================="

# Check if TensorBoard is already running
if pgrep -f "tensorboard.*6006" > /dev/null; then
    echo "✅ TensorBoard is already running!"
    echo "🌐 Open your browser to: http://localhost:6006"
    echo ""
    echo "To stop TensorBoard, run: ./stop_tensorboard.sh"
    exit 0
fi

# Check if virtual environment exists
if [ ! -d "neural-rink-env" ]; then
    echo "❌ Virtual environment not found!"
    echo "Please run: ./setup_environment.sh"
    exit 1
fi

# Activate virtual environment
echo "🔧 Activating virtual environment..."
source neural-rink-env/bin/activate

# Check if results directory exists
if [ ! -d "results" ]; then
    echo "❌ No training results found!"
    echo "Please run training first: ./start_training.sh"
    exit 1
fi

# Start TensorBoard
echo "🚀 Starting TensorBoard..."
echo "🌐 Open your browser to: http://localhost:6006"
echo ""
echo "Press Ctrl+C to stop TensorBoard"
echo ""

tensorboard --logdir results/ --port 6006