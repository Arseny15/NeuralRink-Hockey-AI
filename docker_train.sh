#!/bin/bash

# Docker-based ML-Agents training for Neural Rink
echo "🐳 Building Docker image for ML-Agents training..."

# Build the Docker image
docker build -t neural-rink-ml .

if [ $? -eq 0 ]; then
    echo "✅ Docker image built successfully"
    echo ""
    echo "🚀 Starting training..."
    docker run -it --rm \
        -v "$(pwd)/results:/app/results" \
        -v "$(pwd)/Assets:/app/Assets" \
        neural-rink-ml \
        python train.py --run-id neural_rink_v1 --max-steps 100000
    
    echo ""
    echo "📊 Training complete! Results saved to ./results/"
    echo "To view training progress: tensorboard --logdir results/"
else
    echo "❌ Docker build failed"
    exit 1
fi
