#!/usr/bin/env python3
"""
Complete Build Script for Neural Rink
Builds the game for Windows and Mac with trained AI integration
"""

import os
import sys
import subprocess
import shutil
import argparse
from pathlib import Path

class NeuralRinkBuilder:
    def __init__(self):
        self.project_path = Path(__file__).parent
        self.unity_path = self.find_unity_executable()
        self.build_path = self.project_path / "builds"
        self.models_path = self.project_path / "Assets" / "Models"
        
    def find_unity_executable(self):
        """Find Unity executable path"""
        possible_paths = [
            "/Applications/Unity/Hub/Editor/*/Unity.app/Contents/MacOS/Unity",
            "C:/Program Files/Unity/Hub/Editor/*/Editor/Unity.exe",
            "C:/Program Files (x86)/Unity/Hub/Editor/*/Editor/Unity.exe"
        ]
        
        for path_pattern in possible_paths:
            import glob
            matches = glob.glob(path_pattern)
            if matches:
                # Get the latest version
                latest = sorted(matches)[-1]
                return latest
                
        return None
    
    def check_prerequisites(self):
        """Check if all prerequisites are met"""
        print("🔍 Checking prerequisites...")
        
        # Check Unity
        if not self.unity_path or not os.path.exists(self.unity_path):
            print("❌ Unity executable not found!")
            print("Please install Unity 6 and ensure it's in the default location")
            return False
            
        # Check project files
        required_files = [
            "Assets/Scenes/Training.unity",
            "Assets/Scenes/Play.unity",
            "Assets/Scripts/Agents/GoalieAgent.cs",
            "Assets/Scripts/Gameplay/PlayerController.cs"
        ]
        
        for file_path in required_files:
            if not (self.project_path / file_path).exists():
                print(f"❌ Missing required file: {file_path}")
                return False
                
        # Check trained model
        model_files = list(self.models_path.glob("*.onnx"))
        if not model_files:
            print("⚠️  No trained model found in Assets/Models/")
            print("Training will be skipped, but you can add models later")
            
        print("✅ Prerequisites check passed!")
        return True
    
    def copy_trained_model(self):
        """Copy trained model from results to Unity"""
        print("📦 Copying trained model...")
        
        # Find the latest trained model
        results_path = self.project_path / "results"
        if not results_path.exists():
            print("⚠️  No training results found")
            return
            
        # Look for the latest training run
        training_runs = [d for d in results_path.iterdir() if d.is_dir()]
        if not training_runs:
            print("⚠️  No training runs found")
            return
            
        latest_run = max(training_runs, key=lambda x: x.stat().st_mtime)
        
        # Copy model files
        model_files = list(latest_run.glob("*.onnx"))
        if model_files:
            self.models_path.mkdir(exist_ok=True)
            for model_file in model_files:
                dest = self.models_path / f"Goalie_{latest_run.name}.onnx"
                shutil.copy2(model_file, dest)
                print(f"✅ Copied model: {dest}")
        else:
            print("⚠️  No .onnx model files found in training results")
    
    def build_unity_project(self, platform, target_path):
        """Build Unity project for specific platform"""
        print(f"🏗️  Building for {platform}...")
        
        # Unity build command
        cmd = [
            self.unity_path,
            "-batchmode",
            "-quit",
            "-projectPath", str(self.project_path),
            "-buildTarget", platform,
            "-executeMethod", "NeuralRink.Build.BuildScript.BuildProject",
            "-buildPath", str(target_path),
            "-logFile", str(self.project_path / "build.log")
        ]
        
        try:
            result = subprocess.run(cmd, capture_output=True, text=True, timeout=600)
            
            if result.returncode == 0:
                print(f"✅ {platform} build completed successfully!")
                return True
            else:
                print(f"❌ {platform} build failed!")
                print("Error output:", result.stderr)
                return False
                
        except subprocess.TimeoutExpired:
            print(f"❌ {platform} build timed out!")
            return False
        except Exception as e:
            print(f"❌ {platform} build error: {e}")
            return False
    
    def create_build_info(self):
        """Create build information file"""
        build_info = {
            "version": "1.0.0",
            "build_date": subprocess.check_output(["date"]).decode().strip(),
            "python_version": sys.version,
            "training_status": "Completed" if list(self.models_path.glob("*.onnx")) else "No trained model",
            "features": [
                "1v1 Hockey Shootout",
                "RL-trained AI Goalie",
                "Salary/Bonus Economy",
                "Training and Play Modes",
                "Audio and Visual Effects"
            ]
        }
        
        import json
        with open(self.build_path / "build_info.json", "w") as f:
            json.dump(build_info, f, indent=2)
    
    def build_all_platforms(self):
        """Build for all supported platforms"""
        print("🚀 Starting complete build process...")
        
        if not self.check_prerequisites():
            return False
            
        # Create build directory
        self.build_path.mkdir(exist_ok=True)
        
        # Copy trained model
        self.copy_trained_model()
        
        # Build for each platform
        platforms = {
            "StandaloneWindows64": "Windows",
            "StandaloneOSX": "Mac"
        }
        
        success_count = 0
        for unity_platform, display_name in platforms.items():
            target_path = self.build_path / f"NeuralRink_{display_name}"
            
            if self.build_unity_project(unity_platform, target_path):
                success_count += 1
                
                # Create platform-specific package
                self.create_platform_package(target_path, display_name)
        
        # Create build info
        self.create_build_info()
        
        print(f"\n🎉 Build process completed!")
        print(f"✅ Successfully built: {success_count}/{len(platforms)} platforms")
        print(f"📁 Builds saved to: {self.build_path}")
        
        return success_count > 0
    
    def create_platform_package(self, build_path, platform):
        """Create a complete package for the platform"""
        print(f"📦 Creating {platform} package...")
        
        # Create package directory
        package_path = self.build_path / f"NeuralRink_{platform}_Package"
        package_path.mkdir(exist_ok=True)
        
        # Copy build files
        if build_path.exists():
            shutil.copytree(build_path, package_path / "Game", dirs_exist_ok=True)
        
        # Copy additional files
        additional_files = [
            "README.md",
            "PROJECT_SUMMARY.md",
            "ML_TRAINING_GUIDE.md",
            "UNITY_SETUP_GUIDE.md"
        ]
        
        for file_name in additional_files:
            src = self.project_path / file_name
            if src.exists():
                shutil.copy2(src, package_path / file_name)
        
        # Create run script
        if platform == "Windows":
            run_script = package_path / "Run_Neural_Rink.bat"
            with open(run_script, "w") as f:
                f.write("@echo off\n")
                f.write("echo Starting Neural Rink...\n")
                f.write("cd Game\n")
                f.write("NeuralRink.exe\n")
                f.write("pause\n")
        else:  # Mac
            run_script = package_path / "Run_Neural_Rink.command"
            with open(run_script, "w") as f:
                f.write("#!/bin/bash\n")
                f.write("echo 'Starting Neural Rink...'\n")
                f.write("cd Game\n")
                f.write("open NeuralRink.app\n")
            os.chmod(run_script, 0o755)
        
        print(f"✅ {platform} package created: {package_path}")

def main():
    parser = argparse.ArgumentParser(description="Build Neural Rink for all platforms")
    parser.add_argument("--platform", choices=["Windows", "Mac", "all"], default="all",
                       help="Platform to build for")
    parser.add_argument("--all-platforms", action="store_true",
                       help="Build for all platforms (same as --platform all)")
    parser.add_argument("--skip-model", action="store_true",
                       help="Skip copying trained model")
    
    args = parser.parse_args()
    
    # Handle --all-platforms flag
    if args.all_platforms:
        args.platform = "all"
    
    builder = NeuralRinkBuilder()
    
    if args.platform == "all":
        success = builder.build_all_platforms()
    else:
        # Single platform build
        if not builder.check_prerequisites():
            sys.exit(1)
            
        builder.build_path.mkdir(exist_ok=True)
        
        if not args.skip_model:
            builder.copy_trained_model()
        
        platform_map = {"Windows": "StandaloneWindows64", "Mac": "StandaloneOSX"}
        unity_platform = platform_map[args.platform]
        target_path = builder.build_path / f"NeuralRink_{args.platform}"
        
        success = builder.build_unity_project(unity_platform, target_path)
        
        if success:
            builder.create_platform_package(target_path, args.platform)
            builder.create_build_info()
    
    if success:
        print("\n🎯 Build completed successfully!")
        print("📁 Check the 'builds' folder for your game packages")
        print("🎮 Ready to play Neural Rink!")
    else:
        print("\n❌ Build failed!")
        sys.exit(1)

if __name__ == "__main__":
    main()
