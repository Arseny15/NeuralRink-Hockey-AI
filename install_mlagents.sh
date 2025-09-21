#!/bin/bash

# ML-Agents Installation Script for Neural Rink
# This script sets up ML-Agents with the correct Python version

echo "🏒 Neural Rink ML-Agents Setup"
echo "==============================="

# Check if pyenv is available
if command -v pyenv &> /dev/null; then
    echo "✅ pyenv found - installing Python 3.10"
    pyenv install 3.10.12 --skip-existing
    pyenv local 3.10.12
    echo "✅ Python 3.10.12 activated"
elif command -v conda &> /dev/null; then
    echo "✅ conda found - creating environment"
    conda create -n neural-rink python=3.10 -y
    conda activate neural-rink
    echo "✅ Conda environment 'neural-rink' created"
else
    echo "❌ Neither pyenv nor conda found"
    echo "Please install Python 3.10 manually or use Docker"
    exit 1
fi

# Install ML-Agents
echo "📦 Installing ML-Agents..."
pip install --upgrade pip
pip install -r requirements.txt

# Verify installation
echo "🔍 Verifying installation..."
python -c "import mlagents; print('✅ ML-Agents version:', mlagents.__version__)"
mlagents-learn --help > /dev/null && echo "✅ mlagents-learn command working" || echo "❌ mlagents-learn not found"

echo ""
echo "🎉 Setup complete! You can now run:"
echo "   python train.py --run-id neural_rink_v1"
echo ""
echo "📊 To monitor training:"
echo "   tensorboard --logdir results/"
