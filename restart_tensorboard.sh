#!/bin/bash

# Neural Rink TensorBoard Restarter
# This script stops and restarts TensorBoard

echo "🔄 Restarting TensorBoard..."

# Stop existing TensorBoard
echo "🛑 Stopping existing TensorBoard..."
pkill -f "tensorboard.*6006"

# Wait a moment
sleep 2

# Start new TensorBoard
echo "🚀 Starting new TensorBoard..."
./start_tensorboard.sh
