#!/bin/bash

# Neural Rink TensorBoard Stopper
# This script stops any running TensorBoard instances

echo "🛑 Stopping TensorBoard..."

# Find and kill TensorBoard processes
pkill -f "tensorboard.*6006"

if [ $? -eq 0 ]; then
    echo "✅ TensorBoard stopped successfully!"
else
    echo "ℹ️  No TensorBoard processes found running"
fi

echo "🌐 TensorBoard is no longer available at http://localhost:6006"
