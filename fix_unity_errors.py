#!/usr/bin/env python3
"""
Fix Unity compilation errors by installing required packages
"""

import subprocess
import sys
from pathlib import Path

def run_unity_command(command):
    """Run Unity command line"""
    try:
        # Try to find Unity installation
        unity_paths = [
            "/Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/MacOS/Unity",
            "/Applications/Unity/Hub/Editor/6000.2.5f1/Unity.app/Contents/MacOS/Unity",
            "/Applications/Unity/Unity.app/Contents/MacOS/Unity"
        ]
        
        unity_exe = None
        for path in unity_paths:
            if Path(path).exists():
                unity_exe = path
                break
        
        if not unity_exe:
            print("❌ Unity not found. Please install Unity 6000.2.5f1")
            return False
            
        cmd = [unity_exe, "-batchmode", "-quit", "-projectPath", str(Path.cwd()), "-executeMethod", command]
        result = subprocess.run(cmd, capture_output=True, text=True)
        
        if result.returncode == 0:
            print(f"✅ {command} completed successfully")
            return True
        else:
            print(f"❌ {command} failed: {result.stderr}")
            return False
            
    except Exception as e:
        print(f"❌ Error running Unity command: {e}")
        return False

def main():
    print("🔧 Fixing Unity Compilation Errors...")
    print("=" * 50)
    
    # Step 1: Install TextMeshPro
    print("\n1. Installing TextMeshPro...")
    if run_unity_command("UnityEditor.TextMeshPro.Editor.TextMeshProEditor.ImportTextMeshProEssentials"):
        print("✅ TextMeshPro installed")
    else:
        print("❌ Failed to install TextMeshPro")
    
    # Step 2: Install Input System
    print("\n2. Installing Input System...")
    if run_unity_command("UnityEditor.PackageManager.Client.Add"):
        print("✅ Input System installed")
    else:
        print("❌ Failed to install Input System")
    
    print("\n🎯 Next Steps:")
    print("1. Close Unity completely")
    print("2. Reopen Unity Hub")
    print("3. Open the Neural Rink project")
    print("4. The errors should be resolved!")

if __name__ == "__main__":
    main()
