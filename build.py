#!/usr/bin/env python3
"""
Neural Rink Build Script
Automated build script for creating playable builds and training environments.

Usage:
    python build.py [options]

Examples:
    python build.py --platform Windows --scene Play
    python build.py --platform Mac --scene Training
    python build.py --all-platforms
"""

import argparse
import os
import sys
import subprocess
import shutil
from pathlib import Path
import time

def main():
    parser = argparse.ArgumentParser(description='Build Neural Rink for different platforms and scenes')
    
    # Build options
    parser.add_argument('--platform', type=str, choices=['Windows', 'Mac', 'Linux'],
                       help='Target platform for build')
    parser.add_argument('--scene', type=str, choices=['Play', 'Training', 'Both'],
                       default='Both', help='Scene to build')
    parser.add_argument('--all-platforms', action='store_true',
                       help='Build for all supported platforms')
    parser.add_argument('--output-dir', type=str, default='Builds',
                       help='Output directory for builds')
    
    # Build configuration
    parser.add_argument('--development', action='store_true',
                       help='Create development build')
    parser.add_argument('--release', action='store_true',
                       help='Create release build (default)')
    parser.add_argument('--headless', action='store_true',
                       help='Create headless build for training')
    
    # Unity settings
    parser.add_argument('--unity-path', type=str, default=None,
                       help='Path to Unity executable')
    parser.add_argument('--project-path', type=str, default='.',
                       help='Path to Unity project')
    
    # Additional options
    parser.add_argument('--clean', action='store_true',
                       help='Clean build directory before building')
    parser.add_argument('--compress', action='store_true',
                       help='Compress builds after creation')
    parser.add_argument('--verbose', action='store_true',
                       help='Verbose build output')
    
    args = parser.parse_args()
    
    # Validate environment
    if not validate_build_environment(args):
        sys.exit(1)
    
    # Determine platforms to build
    platforms = get_build_platforms(args)
    
    # Determine scenes to build
    scenes = get_build_scenes(args)
    
    # Clean build directory if requested
    if args.clean:
        clean_build_directory(args.output_dir)
    
    # Create output directory
    os.makedirs(args.output_dir, exist_ok=True)
    
    # Build for each platform and scene combination
    for platform in platforms:
        for scene in scenes:
            build_platform_scene(platform, scene, args)
    
    # Post-build tasks
    post_build_tasks(args)
    
    print("Build process completed!")

def validate_build_environment(args):
    """Validate that the build environment is properly set up."""
    print("Validating build environment...")
    
    # Check if we're in the right directory
    if not os.path.exists('Assets/Scenes/Play.unity'):
        print("ERROR: Play.unity scene not found!")
        print("Please run this script from the Unity project root directory.")
        return False
    
    # Check if Unity executable exists
    if args.unity_path and not os.path.exists(args.unity_path):
        print(f"ERROR: Unity executable not found at: {args.unity_path}")
        return False
    
    print("✓ Build environment validation passed")
    return True

def get_build_platforms(args):
    """Get list of platforms to build for."""
    if args.all_platforms:
        return ['Windows', 'Mac', 'Linux']
    elif args.platform:
        return [args.platform]
    else:
        # Default to current platform
        if sys.platform.startswith('win'):
            return ['Windows']
        elif sys.platform.startswith('darwin'):
            return ['Mac']
        else:
            return ['Linux']

def get_build_scenes(args):
    """Get list of scenes to build."""
    if args.scene == 'Both':
        return ['Play', 'Training']
    else:
        return [args.scene]

def clean_build_directory(output_dir):
    """Clean the build directory."""
    print(f"Cleaning build directory: {output_dir}")
    
    if os.path.exists(output_dir):
        shutil.rmtree(output_dir)
    
    os.makedirs(output_dir, exist_ok=True)

def build_platform_scene(platform, scene, args):
    """Build for a specific platform and scene."""
    print(f"Building {scene} scene for {platform}...")
    
    # Determine build target
    build_target = get_build_target(platform)
    
    # Determine scene path
    scene_path = f"Assets/Scenes/{scene}.unity"
    
    # Determine output path
    output_path = os.path.join(args.output_dir, f"NeuralRink_{scene}_{platform}")
    
    # Add platform-specific extension
    if platform == 'Windows':
        output_path += ".exe"
    elif platform == 'Mac':
        output_path += ".app"
    # Linux builds typically don't need extension
    
    # Build Unity command
    unity_cmd = build_unity_command(args, build_target, scene_path, output_path)
    
    # Execute build
    try:
        result = subprocess.run(unity_cmd, check=True, capture_output=True, text=True)
        
        if args.verbose:
            print(f"Build output: {result.stdout}")
        
        print(f"✓ Successfully built {scene} for {platform}")
        
        # Compress if requested
        if args.compress:
            compress_build(output_path, platform)
            
    except subprocess.CalledProcessError as e:
        print(f"✗ Failed to build {scene} for {platform}")
        if args.verbose:
            print(f"Error output: {e.stderr}")
        return False
    
    return True

def get_build_target(platform):
    """Get Unity build target for platform."""
    targets = {
        'Windows': 'Win64',
        'Mac': 'OSXUniversal',
        'Linux': 'Linux64'
    }
    return targets.get(platform, 'Win64')

def build_unity_command(args, build_target, scene_path, output_path):
    """Build Unity command for building."""
    cmd = []
    
    # Add Unity executable path
    if args.unity_path:
        cmd.append(args.unity_path)
    else:
        # Try to find Unity automatically
        cmd.append('unity')  # This assumes Unity is in PATH
    
    # Add batch mode
    cmd.extend(['-batchmode', '-quit'])
    
    # Add project path
    cmd.extend(['-projectPath', args.project_path])
    
    # Add build target
    cmd.extend(['-buildTarget', build_target])
    
    # Add scene path
    cmd.extend(['-buildPath', output_path])
    
    # Add build method
    if args.development:
        cmd.append('-developmentBuild')
    elif args.headless:
        cmd.append('-headless')
    
    # Add logging
    if args.verbose:
        cmd.extend(['-logFile', '-'])
    
    # Add custom build method
    cmd.extend(['-executeMethod', 'NeuralRink.Build.BuildScript.BuildGame'])
    
    # Add build parameters
    cmd.extend([scene_path, build_target, output_path])
    
    return cmd

def compress_build(output_path, platform):
    """Compress build for distribution."""
    print(f"Compressing {platform} build...")
    
    try:
        if platform == 'Windows':
            # Create ZIP archive for Windows
            archive_path = output_path + '.zip'
            shutil.make_archive(output_path, 'zip', output_path)
        elif platform == 'Mac':
            # Create ZIP archive for Mac
            archive_path = output_path + '.zip'
            shutil.make_archive(output_path, 'zip', output_path)
        else:
            # Create TAR.GZ for Linux
            archive_path = output_path + '.tar.gz'
            shutil.make_archive(output_path, 'gztar', output_path)
        
        print(f"✓ Compressed build: {archive_path}")
        
    except Exception as e:
        print(f"✗ Failed to compress build: {e}")

def post_build_tasks(args):
    """Perform post-build tasks."""
    print("Performing post-build tasks...")
    
    # Create build info file
    create_build_info(args)
    
    # Copy training models if they exist
    copy_training_models(args)
    
    # Create distribution package
    create_distribution_package(args)

def create_build_info(args):
    """Create build information file."""
    build_info = {
        'build_time': time.strftime('%Y-%m-%d %H:%M:%S'),
        'platforms': get_build_platforms(args),
        'scenes': get_build_scenes(args),
        'development_build': args.development,
        'headless_build': args.headless,
        'python_version': sys.version,
        'platform': sys.platform
    }
    
    build_info_path = os.path.join(args.output_dir, 'build_info.json')
    
    import json
    with open(build_info_path, 'w') as f:
        json.dump(build_info, f, indent=2)
    
    print(f"Build info saved to: {build_info_path}")

def copy_training_models(args):
    """Copy trained models to build directory."""
    models_dir = Path('models')
    if models_dir.exists():
        build_models_dir = Path(args.output_dir) / 'models'
        build_models_dir.mkdir(exist_ok=True)
        
        # Copy all model files
        for model_file in models_dir.glob('*.onnx'):
            shutil.copy2(model_file, build_models_dir)
            print(f"Copied model: {model_file.name}")

def create_distribution_package(args):
    """Create final distribution package."""
    print("Creating distribution package...")
    
    # Create README for builds
    create_build_readme(args)
    
    # Copy additional files
    copy_additional_files(args)

def create_build_readme(args):
    """Create README file for builds."""
    readme_content = """# Neural Rink - Build Package

This package contains the Neural Rink hockey game builds.

## Contents

- **Play Build**: Full game with UI and effects
- **Training Build**: Headless version for ML training
- **Models**: Pre-trained goalie models (if available)

## Quick Start

1. Extract the build for your platform
2. Run the executable
3. Use WASD to move, Space to shoot

## Training

To train your own goalie:
1. Use the Training build
2. Run: `python train.py --run-id my_goalie`
3. Monitor progress with TensorBoard

## System Requirements

- Windows 10/11, macOS 10.15+, or Linux Ubuntu 18.04+
- 4GB RAM minimum
- DirectX 11 compatible graphics card

## Support

For issues or questions, refer to the main project README.

---
Built with Unity 6 and ML-Agents 4.0
"""
    
    readme_path = os.path.join(args.output_dir, 'README_BUILD.txt')
    with open(readme_path, 'w') as f:
        f.write(readme_content)
    
    print(f"Build README created: {readme_path}")

def copy_additional_files(args):
    """Copy additional files to build directory."""
    # Copy training script
    if os.path.exists('train.py'):
        shutil.copy2('train.py', args.output_dir)
    
    # Copy main README
    if os.path.exists('README.md'):
        shutil.copy2('README.md', args.output_dir)

if __name__ == '__main__':
    main()
