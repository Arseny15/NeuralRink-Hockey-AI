# Unity Setup Guide for Neural Rink

This guide provides step-by-step instructions for setting up the Neural Rink project in Unity 6.

## Prerequisites

- Unity 6 (6000.x)
- Unity ML-Agents 4.0
- Unity Input System 1.5+

## 1. Project Setup

### A) Import ML-Agents Package
1. Open Unity Package Manager (Window → Package Manager)
2. Click "+" → "Add package from git URL"
3. Enter: `com.unity.ml-agents`
4. Wait for installation to complete

### B) Import Input System Package
1. In Package Manager, click "+" → "Add package from git URL"
2. Enter: `com.unity.inputsystem`
3. Wait for installation to complete

## 2. Project Settings Configuration

### Time Settings
1. Go to Edit → Project Settings → Time
2. Set **Fixed Timestep**: `0.02` (50 FPS for consistent physics)
3. Set **Maximum Allowed Timestep**: `0.33`

### Physics Settings
1. Go to Edit → Project Settings → Physics
2. Set **Default Solver Iterations**: `12`
3. Set **Solver Velocity Iterations**: `2`
4. Enable **Continuous Collision Detection**

### Tags and Layers
1. Go to Edit → Project Settings → Tags and Layers
2. Add these **Layers**:
   - `Player`
   - `Puck`
   - `Goalie`
   - `Rink`
   - `TriggersUI` (optional)

3. Add these **Tags**:
   - `Player`
   - `Puck`
   - `Goal`
   - `SaveZone`
   - `OutOfPlay`

## 3. Create ScriptableObjects

### Training Switch
1. Right-click in Project window
2. Create → Neural Rink → Training Switch
3. Name it `TrainingSwitch.asset`
4. Set **Training Mode**: `true` for Training scene, `false` for Play scene

### Salary System
1. Right-click in Project window
2. Create → Neural Rink → Salary System
3. Name it `SalarySystem.asset`

## 4. Create Physics Materials

Create these in `Assets/Materials/`:

### Ice.physicMaterial
- **Dynamic Friction**: `0.02`
- **Static Friction**: `0.02`
- **Bounciness**: `0`
- **Friction Combine**: `Minimum`
- **Bounce Combine**: `Minimum`

### Puck.physicMaterial
- **Dynamic Friction**: `0.01`
- **Static Friction**: `0.01`
- **Bounciness**: `0.05`
- **Friction Combine**: `Minimum`
- **Bounce Combine**: `Minimum`

### Wall.physicMaterial
- **Dynamic Friction**: `0.8`
- **Static Friction**: `0.8`
- **Bounciness**: `0`
- **Friction Combine**: `Maximum`
- **Bounce Combine**: `Maximum`

## 5. Create Visual Materials

Create these in `Assets/Materials/`:

### Mat_Ice
- **Shader**: URP Lit or Standard
- **Color**: Light blue `(0.8, 0.9, 1.0)`
- **Metallic**: `0.1`
- **Smoothness**: `0.8`

### Mat_Red
- **Color**: Red `(1.0, 0.2, 0.2)`

### Mat_Blue
- **Color**: Blue `(0.2, 0.4, 1.0)`

### Mat_White
- **Color**: White `(1.0, 1.0, 1.0)`

### Mat_GoalMetal
- **Color**: Dark gray `(0.3, 0.3, 0.3)`
- **Metallic**: `0.8`
- **Smoothness**: `0.6`

## 6. Create Prefabs

### Option A: Enhanced Prefabs with Visual Builders (Recommended)

1. **Create Enhanced Prefabs**:
   - Add `EnhancedPrefabCreator` component to any GameObject in scene
   - Right-click component → "Create All Enhanced Prefabs"
   - This creates detailed hockey player/goalie visuals automatically

2. **Manual Visual Builder Setup** (if needed):
   - Add `PrimitiveSkaterBuilder` to Player prefab
   - Add `PrimitiveGoalieBuilder` to Goalie prefab
   - Right-click components → "Build Visual" to generate detailed models

### Option B: Basic Prefabs (Simple Setup)

### Puck.prefab
1. Create empty GameObject named "Puck"
2. Set **Layer**: `Puck`, **Tag**: `Puck`
3. Add **Rigidbody**:
   - Mass: `0.17`
   - Drag: `0.05`
   - Angular Drag: `0.05`
   - Use Gravity: `true`
   - Collision Detection: `Continuous Dynamic`
4. Add **SphereCollider**:
   - Radius: `0.15`
   - Material: `Puck.physicMaterial`
5. Add **MeshFilter** + **MeshRenderer** (Sphere)
6. Set material to dark gray/black
7. Add **PuckController** script
8. Save as prefab in `Assets/Prefabs/`

### Player.prefab
1. Create empty GameObject named "Player"
2. Set **Layer**: `Player`, **Tag**: `Player`
3. Add child "StickRoot" (empty)
4. Add child "StickTip" to StickRoot (empty)
5. Add **Rigidbody**:
   - Mass: `80`
   - Drag: `0`
   - Angular Drag: `0.05`
   - Use Gravity: `true`
   - Collision Detection: `Continuous`
6. Add **CapsuleCollider**:
   - Center: `(0, 0.9, 0)`
   - Radius: `0.35`
   - Height: `1.8`
   - Material: `Ice.physicMaterial`
7. Add **PlayerController** script
8. Assign StickTip reference
9. Add **Player Input** component
10. Set Actions to `Controls.inputactions`
11. **Add Visual Builder** (Optional):
    - Add `PrimitiveSkaterBuilder` component
    - Assign materials (jersey, black, skin)
    - Right-click → "Build Visual (Forward)"
12. Save as prefab in `Assets/Prefabs/`

### Goalie.prefab
1. Create empty GameObject named "Goalie"
2. Set **Layer**: `Goalie`
3. Add child "SaveZone" (empty)
4. Add child "GoalCenter" (empty)
5. Add **Rigidbody**:
   - Mass: `90`
   - Drag: `0`
   - Angular Drag: `0.05`
   - Use Gravity: `true`
   - Collision Detection: `Continuous`
6. Add **BoxCollider**:
   - Size: `(1.2, 1.8, 0.8)`
   - Material: `Ice.physicMaterial`
7. Add **GoalieAgent** script
8. Add **Behavior Parameters** (Training scene only):
   - Behavior Name: `Goalie`
   - Vector Observation: `12` (from CollectObservations)
   - Actions: `Continuous 2`
9. Add **Decision Requester** (Training scene only)
10. **Add Visual Builder** (Optional):
    - Add `PrimitiveGoalieBuilder` component
    - Assign materials (jersey, pads, black)
    - Right-click → "Build Visual (Goalie)"
11. Configure SaveZone child:
    - Add **BoxCollider** (Is Trigger: `true`)
    - Size: `(1.0, 1.0, 0.2)`
    - Tag: `SaveZone`
    - Add **TriggerRelay** script (Type: `Save`)
12. Save as prefab in `Assets/Prefabs/`

### Goal.prefab
1. Create empty GameObject named "Goal"
2. Set **Layer**: `Rink`, **Tag**: `Goal`
3. Add child "Frame" (empty)
4. Add child "GoalTrigger" (empty)
5. Configure Frame child:
   - Add goal post meshes (Cylinders)
   - Add **BoxColliders** for posts/crossbar
   - Set material to `Mat_GoalMetal`
6. Configure GoalTrigger child:
   - Add **BoxCollider** (Is Trigger: `true`)
   - Size: `(0.5, 2, 4)`
   - Tag: `Goal`
   - Add **TriggerRelay** script (Type: `Goal`)
7. Save as prefab in `Assets/Prefabs/`

## 7. Create Scenes

### Training.unity
1. Create new scene
2. Save as `Assets/Scenes/Training.unity`
3. Add **Directional Light**
4. Add **Camera** (position for top-down view)
5. Create "RinkRoot" GameObject:
   - Add **Plane** child (Ice surface, 10×20m, material: `Mat_Ice`)
   - Add **BoxCollider** walls around perimeter
   - Add **OutOfPlay** trigger volume
   - Add **Spawn Points** (PlayerSpawn, PuckSpawn, GoalieSpawn)
6. Add **RinkSetup** script to RinkRoot
7. Place **Goal** prefab at rink end
8. Place **Goalie** prefab at GoalieSpawn
9. Place **Player** prefab at PlayerSpawn
10. Place **Puck** prefab at PuckSpawn
11. Create "Bootstrap" GameObject:
    - Add **BootstrapTraining** script
    - Assign **TrainingSwitch.asset** (TrainingMode: `true`)
12. Create "Director" GameObject:
    - Add **GameDirector** script
    - Assign all references
13. Wire all trigger relays to Director
14. **No UI/Canvas** in training scene

### Play.unity
1. Duplicate Training.unity
2. Save as `Assets/Scenes/Play.unity`
3. Change **TrainingSwitch.asset** to TrainingMode: `false`
4. Add **Canvas** with **NeuralRinkHUD** prefab
5. Add **EventFeed**, **SalaryHUD**, **PopupSpawner** components
6. Wire all UI references in GameDirector

## 8. Final Configuration

### ML-Agents Setup
1. In Training.unity, select Goalie
2. Verify **Behavior Parameters**:
   - Behavior Name: `Goalie` (matches YAML)
   - Vector Observation: `12`
   - Actions: `Continuous 2`
3. Verify **Decision Requester** is present

### Input System
1. Verify **Controls.inputactions** is assigned to Player
2. Test input mapping in Play scene

### Physics Validation
1. Test puck physics (should slide realistically)
2. Test player movement (should feel responsive)
3. Test goalie movement (should be constrained to goal area)

## 9. Testing Checklist

### Training Scene
- [ ] Puck resets to spawn point
- [ ] Goalie makes decisions (check Console)
- [ ] No Canvas/UI drawn
- [ ] ML-Agents connects successfully
- [ ] Behavior name matches YAML

### Play Scene
- [ ] HUD shows salary ($100 base)
- [ ] Save/Goal/Miss updates and animates
- [ ] Player controls work (WASD + Space)
- [ ] Shoot applies velocity to puck
- [ ] Goalie reacts to puck movement
- [ ] Out-of-Play triggers as MISS

## 10. Build Configuration

### Development Build
1. File → Build Settings
2. Add scenes: Training.unity, Play.unity
3. Select platform
4. Enable "Development Build"
5. Build

### Training Build
1. Use Training.unity only
2. Enable "Headless Mode"
3. Disable all UI elements
4. Build for target platform

## Troubleshooting

### Common Issues
- **ML-Agents not training**: Check Behavior name matches YAML
- **Physics inconsistencies**: Verify FixedUpdate usage and Time settings
- **Input not working**: Check Input System package and action assignments
- **Prefabs not saving**: Ensure all components are properly configured

### Debug Tips
- Enable debug logs in TrainingSwitch
- Check Console for ML-Agents messages
- Verify all references are assigned in GameDirector
- Test individual components before full integration

---

**Your Neural Rink project is now ready for training and play! 🏒🤖**
