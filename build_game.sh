#!/bin/bash

# Neural Rink Game Builder
# This script builds the game for all platforms

echo "🏗️  Neural Rink Game Builder"
echo "============================"

# Check if virtual environment exists
if [ ! -d "neural-rink-env" ]; then
    echo "❌ Virtual environment not found!"
    echo "Please run: pip install -r requirements_alternative.txt"
    exit 1
fi

# Activate virtual environment
echo "🔧 Activating virtual environment..."
source neural-rink-env/bin/activate

# Check if Unity is available
if ! command -v unity &> /dev/null; then
    echo "⚠️  Unity command not found in PATH"
    echo "Make sure Unity is installed and accessible"
fi

# Build for all platforms
echo "🚀 Building game for all platforms..."
echo "This will take several minutes..."
echo ""

python build_complete.py --all-platforms

echo ""
echo "✅ Build completed!"
echo "📁 Check the 'builds' folder for your game packages"
