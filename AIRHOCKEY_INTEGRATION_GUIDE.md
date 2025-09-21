# Air Hockey Movement Integration Guide

## 🏒 Overview
This guide shows how to integrate the proven air hockey movement system into your Neural Rink hockey game. The air hockey physics provide smooth, responsive movement that's perfect for hockey gameplay.

## 📋 New Movement Scripts Created

### 1. PlayerMovement.cs
- **Location**: `Assets/Scripts/Gameplay/PlayerMovement.cs`
- **Controls**: WASD keys
- **Features**: 
  - Physics-based movement with velocity limiting
  - Configurable speed and acceleration
  - Y-position and rotation constraints
  - Reset functionality
  - Telemetry support

### 2. GoalkeeperMovement.cs
- **Location**: `Assets/Scripts/Gameplay/GoalkeeperMovement.cs`
- **Controls**: Arrow keys
- **Features**:
  - Slightly slower than players (12 vs 14 speed)
  - Optional movement bounds for goal area
  - Same physics system as players
  - Visual bounds gizmo in editor

### 3. GameReset.cs
- **Location**: `Assets/Scripts/Gameplay/GameReset.cs`
- **Controls**: R key to reset
- **Features**:
  - Resets any object to initial position/rotation/velocity
  - Can be attached to players, goalkeeper, puck
  - Configurable reset behavior

### 4. Updated PuckController.cs
- **Enhanced with**: Air hockey physics mode
- **Features**:
  - Toggle between ice physics and air hockey physics
  - Very low friction for smooth gliding
  - Maintains existing functionality

## 🎯 Physics Materials Copied

From the proven air hockey game:
- **Handle.physicMaterial** → Use for Player and Goalkeeper
- **Puck.physicMaterial** → Use for Puck (zero friction, full bounce)
- **Wall.physicMaterial** → Use for Boards/Walls

## 🎨 Visual Assets

- **air_hockey_full.png** → Copied to `Assets/Materials/` for ice surface texture

## 🔧 Setup Instructions

### Step 1: Configure Game Objects
1. Open your hockey scene
2. Add `AirHockeyPhysicsSetup` component to any GameObject
3. Use the context menu "Auto-Find Game Objects" to detect scene objects
4. Use "Load Air Hockey Physics Materials" to load the copied materials
5. Run "NeuralRink/Setup/Configure Air Hockey Physics" from the menu

### Step 2: Manual Configuration (Alternative)

#### For Players:
```csharp
// Add these components:
- Rigidbody (mass: 2-3, freeze Y position and rotation)
- PlayerMovement script
- GameReset script
- Apply "Player" physics material to collider
- Set tag to "Player"
```

#### For Goalkeeper:
```csharp
// Add these components:
- Rigidbody (mass: 3-4, freeze Y position and rotation)  
- GoalkeeperMovement script
- GameReset script
- Apply "Goalkeeper" physics material to collider
- Set tag to "Goalkeeper"
```

#### For Puck:
```csharp
// Configure existing PuckController:
- Set useAirHockeyPhysics = true
- Apply "Puck" physics material to collider
- Add GameReset script
- Rigidbody: mass 1, no gravity, freeze Y position
```

#### For Walls/Boards:
```csharp
// Apply to all wall objects:
- Apply "Wall" physics material to colliders
- Set tag to "Wall"
```

## 🎮 Controls

### Player Controls (WASD):
- **W**: Move Forward
- **S**: Move Backward  
- **A**: Move Left
- **D**: Move Right

### Goalkeeper Controls (Arrow Keys):
- **↑**: Move Forward
- **↓**: Move Backward
- **←**: Move Left  
- **→**: Move Right

### Reset Controls:
- **R**: Reset all objects to initial positions

## ⚙️ Configuration Options

### PlayerMovement Settings:
- `playerSpeed`: Maximum movement speed (default: 14)
- `acceleration`: Force applied for movement (default: 60)
- `freezeYPosition`: Keep player on ice surface
- `freezeRotation`: Prevent unwanted rotation

### GoalkeeperMovement Settings:
- `goalkeeperSpeed`: Maximum movement speed (default: 12)
- `useBounds`: Enable goal area restriction
- `boundsCenter/boundsSize`: Define goalkeeper movement area

### PuckController Settings:
- `useAirHockeyPhysics`: Enable air hockey style movement
- `airHockeyFriction`: Very low friction value (default: 0.02)

## 🔬 Physics Material Specifications

### Player/Goalkeeper Materials:
- Dynamic Friction: 4.0
- Static Friction: 0.5
- Bounciness: 0.7 (Player) / 0.6 (Goalkeeper)
- Combine Mode: Average

### Puck Material:
- Dynamic Friction: 0.0
- Static Friction: 0.0
- Bounciness: 1.0
- Combine Mode: Average

### Wall Material:
- Dynamic Friction: 0.0
- Static Friction: 0.0
- Bounciness: 0.5
- Combine Mode: Average

## 🎨 Visual Integration

The air hockey texture (`air_hockey_full.png`) can be applied to your ice surface for a polished look that matches the physics behavior.

## 🧪 Testing

1. **Movement Test**: Players should move smoothly with WASD, goalkeeper with arrows
2. **Physics Test**: Puck should glide smoothly with minimal friction
3. **Collision Test**: Bouncing should feel responsive and realistic
4. **Reset Test**: R key should reset all objects to starting positions

## 🔄 Integration with Existing Systems

The new movement scripts are designed to work alongside your existing:
- ML-Agents training system
- Salary/reward system
- Telemetry logging
- UI systems

They provide the movement layer while preserving all your game logic and AI training capabilities.
