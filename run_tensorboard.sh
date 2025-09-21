#!/bin/bash

# Neural Rink TensorBoard Runner
# This script activates the virtual environment and runs TensorBoard

echo "📊 Starting TensorBoard for Neural Rink..."
echo "========================================="

# Activate virtual environment
source neural-rink-env/bin/activate

# Start TensorBoard
echo "🚀 Starting TensorBoard on port 6006..."
echo "Open your browser to: http://localhost:6006"
echo ""
echo "Press Ctrl+C to stop TensorBoard"
echo ""

tensorboard --logdir results/ --port 6006
