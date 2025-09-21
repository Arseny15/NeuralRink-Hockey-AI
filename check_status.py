#!/usr/bin/env python3
"""
Status checker for Neural Rink project
Shows current progress of training, Unity setup, and builds
"""

import os
import sys
from pathlib import Path
import subprocess

def check_training_status():
    """Check if training is running and show progress"""
    print("🏒 Neural Rink Project Status")
    print("=" * 50)
    
    # Check if training is running
    try:
        result = subprocess.run(['ps', 'aux'], capture_output=True, text=True)
        if 'alternative_train.py' in result.stdout:
            print("✅ Training: RUNNING (100k steps in progress)")
            
            # Check results directory
            results_path = Path("results")
            if results_path.exists():
                training_runs = [d for d in results_path.iterdir() if d.is_dir()]
                if training_runs:
                    latest_run = max(training_runs, key=lambda x: x.stat().st_mtime)
                    print(f"📊 Latest run: {latest_run.name}")
                    
                    # Check for model files
                    model_files = list(latest_run.glob("*.zip"))
                    if model_files:
                        print(f"🤖 Model files: {len(model_files)} found")
                    else:
                        print("⏳ Model files: Training in progress...")
        else:
            print("⏸️  Training: NOT RUNNING")
            
            # Check for completed training
            results_path = Path("results/neural_rink_full")
            if results_path.exists():
                model_files = list(results_path.glob("*.zip"))
                if model_files:
                    print("✅ Training: COMPLETED (neural_rink_full)")
                    print(f"🤖 Model files: {len(model_files)} found")
                else:
                    print("✅ Training: COMPLETED (neural_rink_full) - No model files yet")
            else:
                print("❌ Training: NOT STARTED")
                
    except Exception as e:
        print(f"❌ Training: Error checking status - {e}")

def check_unity_setup():
    """Check Unity setup status"""
    print("\n🎮 Unity Setup Status:")
    
    # Check for Unity project files
    unity_files = [
        "Assets/Scenes/Training.unity",
        "Assets/Scenes/Play.unity",
        "Assets/Scripts/Agents/GoalieAgent.cs",
        "Assets/Scripts/Gameplay/PlayerController.cs",
        "Assets/Models/Goalie_Final.zip"
    ]
    
    found_files = 0
    for file_path in unity_files:
        if Path(file_path).exists():
            found_files += 1
    
    if found_files == len(unity_files):
        print("✅ Unity Setup: COMPLETE (all files present)")
    elif found_files >= 4:
        print(f"✅ Unity Setup: COMPLETE ({found_files}/{len(unity_files)} files)")
    elif found_files > 0:
        print(f"🚧 Unity Setup: PARTIAL ({found_files}/{len(unity_files)} files)")
    else:
        print("❌ Unity Setup: NOT STARTED")

def check_build_status():
    """Check build status"""
    print("\n🏗️  Build Status:")
    
    builds_path = Path("builds")
    if builds_path.exists():
        build_dirs = [d for d in builds_path.iterdir() if d.is_dir()]
        if build_dirs:
            print(f"✅ Builds: {len(build_dirs)} packages found")
            for build_dir in build_dirs:
                print(f"   📦 {build_dir.name}")
        else:
            print("⏳ Builds: Directory exists but no packages")
    else:
        print("❌ Builds: NOT STARTED")

def check_dependencies():
    """Check if required dependencies are installed"""
    print("\n📦 Dependencies Status:")
    
    try:
        import stable_baselines3
        print("✅ Stable-Baselines3: INSTALLED")
    except ImportError:
        print("❌ Stable-Baselines3: NOT INSTALLED")
    
    try:
        import torch
        print("✅ PyTorch: INSTALLED")
    except ImportError:
        print("❌ PyTorch: NOT INSTALLED")
    
    try:
        import tensorboard
        print("✅ TensorBoard: INSTALLED")
    except ImportError:
        print("❌ TensorBoard: NOT INSTALLED")

def show_next_steps():
    """Show recommended next steps"""
    print("\n🎯 Recommended Next Steps:")
    
    # Check training status
    results_path = Path("results/neural_rink_full")
    if not results_path.exists():
        print("1. ⏳ Wait for training to complete (100k steps)")
    else:
        print("1. ✅ Training completed - ready for Unity setup")
    
    # Check Unity setup
    unity_files = [
        "Assets/Scenes/Training.unity",
        "Assets/Scenes/Play.unity"
    ]
    
    if not any(Path(f).exists() for f in unity_files):
        print("2. 🎮 Start Unity setup:")
        print("   - Open Unity Hub")
        print("   - Create new 3D project")
        print("   - Import project files")
        print("   - Run automated setup")
    else:
        print("2. ✅ Unity setup in progress")
    
    # Check builds
    builds_path = Path("builds")
    if not builds_path.exists():
        print("3. 🏗️  Ready for building:")
        print("   - Complete Unity setup first")
        print("   - Run: python build_complete.py --all-platforms")
    else:
        print("3. ✅ Builds available")

def main():
    check_training_status()
    check_unity_setup()
    check_build_status()
    check_dependencies()
    show_next_steps()
    
    print("\n" + "=" * 50)
    print("📚 For detailed instructions, see:")
    print("   - COMPLETE_STEP_BY_STEP_GUIDE.md")
    print("   - UNITY_COMPLETE_SETUP.md")
    print("   - ML_TRAINING_GUIDE.md")

if __name__ == "__main__":
    main()
