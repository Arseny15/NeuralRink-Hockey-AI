#!/bin/bash

# Alternative ML Training Setup for Neural Rink
# Uses Stable-Baselines3 instead of ML-Agents (Python 3.13 compatible)

echo "🏒 Neural Rink Alternative Training Setup"
echo "=========================================="
echo "Using Stable-Baselines3 (Python 3.13 compatible)"
echo ""

# Check Python version
python_version=$(python --version 2>&1 | awk '{print $2}')
echo "✅ Python version: $python_version"

# Install dependencies
echo "📦 Installing training dependencies..."
pip install --upgrade pip
pip install -r requirements_alternative.txt

# Verify installation
echo ""
echo "🔍 Verifying installation..."
python -c "import stable_baselines3; print('✅ Stable-Baselines3 version:', stable_baselines3.__version__)"
python -c "import torch; print('✅ PyTorch version:', torch.__version__)"
python -c "import numpy; print('✅ NumPy version:', numpy.__version__)"

echo ""
echo "🎉 Setup complete! You can now run:"
echo "   python alternative_train.py --run-id neural_rink_alt --max-steps 100000"
echo ""
echo "📊 To monitor training:"
echo "   tensorboard --logdir results/"
echo ""
echo "📝 Note: This uses a simplified environment instead of Unity ML-Agents"
echo "   For full Unity integration, you'll need Python 3.10 with ML-Agents"
