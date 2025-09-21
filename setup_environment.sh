#!/bin/bash

# Neural Rink Environment Setup
# This script sets up the complete environment for Neural Rink

echo "🔧 Neural Rink Environment Setup"
echo "==============================="

# Create virtual environment if it doesn't exist
if [ ! -d "neural-rink-env" ]; then
    echo "📦 Creating virtual environment..."
    python3 -m venv neural-rink-env
    echo "✅ Virtual environment created"
else
    echo "✅ Virtual environment already exists"
fi

# Activate virtual environment
echo "🔧 Activating virtual environment..."
source neural-rink-env/bin/activate

# Install dependencies
echo "📦 Installing dependencies..."
pip install --upgrade pip
pip install -r requirements_alternative.txt

# Verify installation
echo "🔍 Verifying installation..."
python -c "import stable_baselines3; print('✅ Stable-Baselines3:', stable_baselines3.__version__)"
python -c "import torch; print('✅ PyTorch:', torch.__version__)"
python -c "import tensorboard; print('✅ TensorBoard: INSTALLED')"

echo ""
echo "🎉 Environment setup complete!"
echo ""
echo "Next steps:"
echo "1. Start training: ./start_training.sh"
echo "2. Monitor progress: ./start_tensorboard.sh"
echo "3. Check status: ./check_status.sh"
