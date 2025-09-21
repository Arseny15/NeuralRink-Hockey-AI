#!/bin/bash

# Neural Rink Status Checker
# This script checks the current status of training, Unity setup, and builds

echo "🏒 Neural Rink Status Checker"
echo "============================="

# Check if virtual environment exists
if [ ! -d "neural-rink-env" ]; then
    echo "❌ Virtual environment not found!"
    echo "Please run: pip install -r requirements_alternative.txt"
    exit 1
fi

# Activate virtual environment and run status checker
source neural-rink-env/bin/activate
python check_status.py
