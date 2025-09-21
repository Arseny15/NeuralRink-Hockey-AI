#!/bin/bash

# Neural Rink Unity Project Setup
# This script sets up Unity project in the current folder

echo "🎮 Setting up Unity project in current folder..."
echo "=============================================="

# Check if Unity project already exists
if [ -d "Neural Rink" ]; then
    echo "⚠️  Unity project 'Neural Rink' already exists!"
    echo "Please remove it first or choose a different location"
    exit 1
fi

# Create Unity project structure
echo "📁 Creating Unity project structure..."
mkdir -p "Neural Rink/Assets"
mkdir -p "Neural Rink/Library"
mkdir -p "Neural Rink/Packages"
mkdir -p "Neural Rink/ProjectSettings"

# Copy our Assets to Unity project
echo "📦 Copying Neural Rink files to Unity project..."
cp -r Assets/* "Neural Rink/Assets/"

# Create Models folder and copy trained model
echo "🤖 Setting up trained model..."
mkdir -p "Neural Rink/Assets/Models"
if [ -f "results/neural_rink_full/final_model.zip" ]; then
    cp "results/neural_rink_full/final_model.zip" "Neural Rink/Assets/Models/Goalie_Final.zip"
    echo "✅ Trained model copied to Unity project"
else
    echo "⚠️  No trained model found - you can copy it later"
fi

# Create basic Unity project files
echo "⚙️  Creating Unity project configuration..."

# Create ProjectVersion.txt
cat > "Neural Rink/ProjectSettings/ProjectVersion.txt" << EOF
m_EditorVersion: 6002.0.0f1
m_EditorVersionWithRevision: 6002.0.0f1 (a7b0c7e2b8b9)
EOF

# Create ProjectSettings.asset
cat > "Neural Rink/ProjectSettings/ProjectSettings.asset" << EOF
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!129 &1
PlayerSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 24
  productGUID: $(uuidgen | tr '[:upper:]' '[:lower:]' | tr -d '-')
  AndroidProfiler: 0
  AndroidFilterTouchesWhenObscured: 0
  AndroidEnableSustainedPerformanceMode: 0
  defaultScreenOrientation: 3
  targetDevice: 2
  useOnDemandResources: 0
  accelerometerFrequency: 60
  companyName: Neural Rink
  productName: Neural Rink
  defaultCursor: {fileID: 0}
  cursorHotspot: {x: 0, y: 0}
  m_SplashScreenBackgroundColor: {r: 0.13725491, g: 0.12156863, b: 0.1254902, a: 1}
  m_ShowUnitySplashScreen: 1
  m_ShowUnitySplashLogo: 1
  m_SplashScreenOverlayOpacity: 1
  m_SplashScreenAnimation: 1
  m_SplashScreenLogoStyle: 1
  m_SplashScreenDrawMode: 0
  m_SplashScreenBackgroundAnimationZoom: 1
  m_SplashScreenLogoAnimationZoom: 1
  m_SplashScreenBackgroundLandscapeAspectRatio: 2
  m_SplashScreenBackgroundPortraitAspectRatio: 1
  m_SplashScreenBackgroundLandscapeUvs:
    serializedVersion: 2
    x: 0
    y: 0
    width: 1
    height: 1
  m_SplashScreenBackgroundPortraitUvs:
    serializedVersion: 2
    x: 0
    y: 0
    width: 1
    height: 1
  m_SplashScreenLogos: []
  m_VirtualRealitySDKs: []
  m_TargetPixelDensity: 30
  m_SplashScreenLogos: []
  m_VirtualRealitySDKs: []
  m_TargetPixelDensity: 30
  m_DefaultScreenWidth: 1920
  m_DefaultScreenHeight: 1080
  m_DefaultScreenWidthWeb: 960
  m_DefaultScreenHeightWeb: 600
  m_RunInBackground: 1
  m_CaptureSingleScreen: 0
  m_MuteOtherAudioSources: 0
  m_Prepare iOS For Recording: 0
  m_Force iOS Speakers When Recording: 0
  m_deferSystemGesturesMode: 0
  m_hideHomeButton: 0
  m_SubmitAnalytics: 1
  m_UsePlayerLog: 1
  m_VirtualRealitySupported: 0
  m_BuildTargetVREnabled:
  - Android: 0
  - Standalone: 0
  - iPhone: 0
  - tvOS: 0
  - WebGL: 0
  - Windows Store Apps: 0
  - Tizen: 0
  - PS4: 0
  - XboxOne: 0
  - Nintendo 3DS: 0
  - Nintendo Switch: 0
  - Stadia: 0
  - CloudRendering: 0
  - GameCoreScarlett: 0
  - GameCoreXboxOne: 0
  - PS5: 0
  - EmbeddedLinux: 0
  - QNX: 0
  - VisionOS: 0
  m_VRConfigTemplate:
  - Android: {fileID: 0}
  - Standalone: {fileID: 0}
  - iPhone: {fileID: 0}
  - tvOS: {fileID: 0}
  - WebGL: {fileID: 0}
  - Windows Store Apps: {fileID: 0}
  - Tizen: {fileID: 0}
  - PS4: {fileID: 0}
  - XboxOne: {fileID: 0}
  - Nintendo 3DS: {fileID: 0}
  - Nintendo Switch: {fileID: 0}
  - Stadia: {fileID: 0}
  - CloudRendering: {fileID: 0}
  - GameCoreScarlett: {fileID: 0}
  - GameCoreXboxOne: {fileID: 0}
  - PS5: {fileID: 0}
  - EmbeddedLinux: {fileID: 0}
  - QNX: {fileID: 0}
  - VisionOS: {fileID: 0}
  m_DefaultScreenWidthWeb: 960
  m_DefaultScreenHeightWeb: 600
  m_RunInBackground: 1
  m_CaptureSingleScreen: 0
  m_MuteOtherAudioSources: 0
  m_Prepare iOS For Recording: 0
  m_Force iOS Speakers When Recording: 0
  m_deferSystemGesturesMode: 0
  m_hideHomeButton: 0
  m_SubmitAnalytics: 1
  m_UsePlayerLog: 1
  m_VirtualRealitySupported: 0
  m_BuildTargetVREnabled:
  - Android: 0
  - Standalone: 0
  - iPhone: 0
  - tvOS: 0
  - WebGL: 0
  - Windows Store Apps: 0
  - Tizen: 0
  - PS4: 0
  - XboxOne: 0
  - Nintendo 3DS: 0
  - Nintendo Switch: 0
  - Stadia: 0
  - CloudRendering: 0
  - GameCoreScarlett: 0
  - GameCoreXboxOne: 0
  - PS5: 0
  - EmbeddedLinux: 0
  - QNX: 0
  - VisionOS: 0
  m_VRConfigTemplate:
  - Android: {fileID: 0}
  - Standalone: {fileID: 0}
  - iPhone: {fileID: 0}
  - tvOS: {fileID: 0}
  - WebGL: {fileID: 0}
  - Windows Store Apps: {fileID: 0}
  - Tizen: {fileID: 0}
  - PS4: {fileID: 0}
  - XboxOne: {fileID: 0}
  - Nintendo 3DS: {fileID: 0}
  - Nintendo Switch: {fileID: 0}
  - Stadia: {fileID: 0}
  - CloudRendering: {fileID: 0}
  - GameCoreScarlett: {fileID: 0}
  - GameCoreXboxOne: {fileID: 0}
  - PS5: {fileID: 0}
  - EmbeddedLinux: {fileID: 0}
  - QNX: {fileID: 0}
  - VisionOS: {fileID: 0}
  m_DefaultScreenWidthWeb: 960
  m_DefaultScreenHeightWeb: 600
  m_RunInBackground: 1
  m_CaptureSingleScreen: 0
  m_MuteOtherAudioSources: 0
  m_Prepare iOS For Recording: 0
  m_Force iOS Speakers When Recording: 0
  m_deferSystemGesturesMode: 0
  m_hideHomeButton: 0
  m_SubmitAnalytics: 1
  m_UsePlayerLog: 1
  m_VirtualRealitySupported: 0
  m_BuildTargetVREnabled:
  - Android: 0
  - Standalone: 0
  - iPhone: 0
  - tvOS: 0
  - WebGL: 0
  - Windows Store Apps: 0
  - Tizen: 0
  - PS4: 0
  - XboxOne: 0
  - Nintendo 3DS: 0
  - Nintendo Switch: 0
  - Stadia: 0
  - CloudRendering: 0
  - GameCoreScarlett: 0
  - GameCoreXboxOne: 0
  - PS5: 0
  - EmbeddedLinux: 0
  - QNX: 0
  - VisionOS: 0
  m_VRConfigTemplate:
  - Android: {fileID: 0}
  - Standalone: {fileID: 0}
  - iPhone: {fileID: 0}
  - tvOS: {fileID: 0}
  - WebGL: {fileID: 0}
  - Windows Store Apps: {fileID: 0}
  - Tizen: {fileID: 0}
  - PS4: {fileID: 0}
  - XboxOne: {fileID: 0}
  - Nintendo 3DS: {fileID: 0}
  - Nintendo Switch: {fileID: 0}
  - Stadia: {fileID: 0}
  - CloudRendering: {fileID: 0}
  - GameCoreScarlett: {fileID: 0}
  - GameCoreXboxOne: {fileID: 0}
  - PS5: {fileID: 0}
  - EmbeddedLinux: {fileID: 0}
  - QNX: {fileID: 0}
  - VisionOS: {fileID: 0}
EOF

echo ""
echo "✅ Unity project setup complete!"
echo ""
echo "🎮 Next steps:"
echo "1. Open Unity Hub"
echo "2. Click 'Add' and select the 'Neural Rink' folder"
echo "3. Open the project in Unity"
echo "4. Run: Neural Rink → Setup Project"
echo ""
echo "📁 Project location: $(pwd)/Neural Rink"
