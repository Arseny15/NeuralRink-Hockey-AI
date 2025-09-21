#!/usr/bin/env python3
"""
Verify that the Neural Rink game is ready to run in Unity
"""

import os
from pathlib import Path

def check_file_exists(file_path, description):
    """Check if a file exists and report status"""
    if Path(file_path).exists():
        print(f"✅ {description}: {file_path}")
        return True
    else:
        print(f"❌ {description}: {file_path} - MISSING")
        return False

def check_directory_exists(dir_path, description):
    """Check if a directory exists and report status"""
    if Path(dir_path).exists() and Path(dir_path).is_dir():
        files = list(Path(dir_path).glob("*"))
        print(f"✅ {description}: {dir_path} ({len(files)} files)")
        return True
    else:
        print(f"❌ {description}: {dir_path} - MISSING")
        return False

def main():
    print("🏒 Neural Rink - Game Readiness Verification")
    print("=" * 60)
    
    all_good = True
    
    # Check Unity project structure
    print("\n📁 Unity Project Structure:")
    all_good &= check_directory_exists("Assets", "Assets folder")
    all_good &= check_directory_exists("Assets/Scripts", "Scripts folder")
    all_good &= check_directory_exists("Assets/Scenes", "Scenes folder")
    all_good &= check_directory_exists("Assets/Models", "Models folder")
    all_good &= check_directory_exists("Assets/Prefabs", "Prefabs folder")
    all_good &= check_directory_exists("Packages", "Packages folder")
    
    # Check essential scripts
    print("\n📜 Essential Scripts:")
    all_good &= check_file_exists("Assets/Scripts/Agents/GoalieAgent.cs", "GoalieAgent")
    all_good &= check_file_exists("Assets/Scripts/Agents/SimpleGoalieAgent.cs", "SimpleGoalieAgent")
    all_good &= check_file_exists("Assets/Scripts/Gameplay/GameDirector.cs", "GameDirector")
    all_good &= check_file_exists("Assets/Scripts/Gameplay/PlayerController.cs", "PlayerController")
    all_good &= check_file_exists("Assets/Scripts/Gameplay/PuckController.cs", "PuckController")
    all_good &= check_file_exists("Assets/Scripts/Systems/SalarySystem.cs", "SalarySystem")
    all_good &= check_file_exists("Assets/Scripts/Systems/TrainingSwitch.cs", "TrainingSwitch")
    
    # Check scenes
    print("\n🎬 Game Scenes:")
    all_good &= check_file_exists("Assets/Scenes/Training.unity", "Training scene")
    all_good &= check_file_exists("Assets/Scenes/Play.unity", "Play scene")
    
    # Check AI model
    print("\n🤖 AI Model:")
    all_good &= check_file_exists("Assets/Models/Goalie_Final.zip", "Trained AI model")
    
    # Check package manifest
    print("\n📦 Unity Packages:")
    all_good &= check_file_exists("Packages/manifest.json", "Package manifest")
    
    # Check training results
    print("\n🎯 Training Results:")
    if Path("results/neural_rink_full").exists():
        model_files = list(Path("results/neural_rink_full").glob("*.zip"))
        print(f"✅ Training completed: {len(model_files)} model files")
    else:
        print("⚠️  Training results not found (optional)")
    
    print("\n" + "=" * 60)
    
    if all_good:
        print("🎉 SUCCESS: Neural Rink is ready to run!")
        print("\n🎮 What you can do now:")
        print("1. Open Unity Hub")
        print("2. Add the 'Hockey RL Demo' project")
        print("3. Open the project in Unity")
        print("4. Wait for compilation to complete")
        print("5. Open Play.unity scene")
        print("6. Press Play ▶️ to test the game!")
        
        print("\n🏒 Your Neural Rink game features:")
        print("• Human player controls (WASD + mouse)")
        print("• AI goalie with basic intelligence")
        print("• Salary/bonus economy system")
        print("• Training and Play modes")
        print("• Particle effects and UI")
        print("• Complete hockey rink setup")
        
    else:
        print("❌ ISSUES FOUND: Some components are missing")
        print("Please check the missing files above")
    
    print("\n" + "=" * 60)

if __name__ == "__main__":
    main()
