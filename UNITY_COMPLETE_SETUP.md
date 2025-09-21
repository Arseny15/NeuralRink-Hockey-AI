# 🏒 Complete Unity Setup Guide

## 🎯 **Step-by-Step Unity Scene Creation**

Follow this guide to create both Training and Play scenes with all necessary components.

## 📋 **Prerequisites**
- Unity 6 (6000.x)
- ML-Agents 4.0 package installed
- All scripts from this project

## 🏗️ **Step 1: Project Setup**

### **A. Package Manager Setup**
1. Open Unity Package Manager
2. Install these packages:
   - `com.unity.inputsystem` (1.5+)
   - `com.unity.ml-agents` (4.0)
   - `com.unity.render-pipelines.universal` (optional, for better graphics)

### **B. Project Settings**
1. **Edit → Project Settings → Time**
   - Fixed Timestep: `0.02`
   - Maximum Allowed Timestep: `0.33`

2. **Edit → Project Settings → Physics**
   - Default Solver Iterations: `12`
   - Solver Velocity Iterations: `2`
   - Continuous Collision Detection: `ON`

### **C. Tags & Layers**
**Layers:**
- Player (Layer 8)
- Puck (Layer 9)
- Goalie (Layer 10)
- Rink (Layer 11)
- TriggersUI (Layer 12)

**Tags:**
- Player
- Puck
- Goal
- SaveZone
- OutOfPlay

## 🎨 **Step 2: Materials Creation**

Create these materials in `Assets/Materials/`:

### **Ice.physicMaterial**
- Dynamic Friction: `0.02`
- Static Friction: `0.02`
- Bounciness: `0`
- Friction Combine: `Minimum`

### **Puck.physicMaterial**
- Dynamic Friction: `0.01`
- Static Friction: `0.01`
- Bounciness: `0.05`
- Friction Combine: `Minimum`

### **Wall.physicMaterial**
- Dynamic Friction: `0.8`
- Static Friction: `0.8`
- Bounciness: `0`
- Friction Combine: `Average`

### **Visual Materials**
- `Mat_Ice` (slightly blue tint)
- `Mat_Red` (team colors)
- `Mat_Blue` (team colors)
- `Mat_White`
- `Mat_GoalMetal`

## 🎮 **Step 3: ScriptableObjects**

### **TrainingSwitch.asset**
1. Right-click in Project → Create → NeuralRink → Training Switch
2. Name: `TrainingSwitch`
3. **TrainingMode**: `true` for Training scene, `false` for Play scene

### **SalarySystem.asset**
1. Right-click in Project → Create → NeuralRink → Salary System
2. Name: `SalarySystem`
3. Configure default values (base salary: 100, bonuses: 200, etc.)

## 🏒 **Step 4: Prefab Creation**

### **A. Puck Prefab**
```
Hierarchy: Puck (Layer: Puck, Tag: Puck)
Components:
- Rigidbody
  - Mass: 0.17
  - Drag: 0.05
  - Angular Drag: 0.05
  - Use Gravity: true
  - Collision Detection: Continuous Dynamic
- SphereCollider
  - Radius: 0.15
  - Material: Puck.physicMaterial
- MeshRenderer (Sphere) → Mat_Black
- PuckController
```

### **B. Player Prefab**
```
Hierarchy: Player (Layer: Player, Tag: Player)
├─ StickRoot (empty)
│ └─ StickTip (empty)
└─ Visual (optional)

Components on root:
- Rigidbody
  - Mass: 80
  - Drag: 0
  - Angular Drag: 0.05
  - Use Gravity: true
  - Collision Detection: Continuous
- CapsuleCollider
  - Center: (0, 0.9, 0)
  - Radius: 0.35
  - Height: 1.8
  - Material: Ice.physicMaterial
- PlayerController
  - Move Speed: 6
  - Sprint Multiplier: 1.5
  - Max Shoot Force: 20
- Player Input (if using Input System)
  - Actions: Assets/Input/Controls.inputactions
```

### **C. Goalie Prefab**
```
Hierarchy: Goalie (Layer: Goalie)
├─ SaveZone (BoxCollider isTrigger, Tag: SaveZone)
├─ GoalCenter (empty)
└─ Visual (optional)

Components on root:
- Rigidbody
  - Mass: 90
  - Drag: 0
  - Angular Drag: 0.05
  - Use Gravity: true
  - Collision Detection: Continuous
- BoxCollider
  - Size: (1.2, 1.8, 0.8)
  - Material: Ice.physicMaterial
- GoalieAgent
  - xzBounds: (2.0, 1.5)
  - Max Speed: 10
- Behavior Parameters (Training scene only)
  - Behavior Name: Goalie
  - Vector Observation: 12
  - Actions: Continuous 2
- Decision Requester (Training scene only)
  - Decision Interval: 1
```

### **D. Goal Prefab**
```
Hierarchy: Goal (Layer: Rink, Tag: Goal)
├─ Frame (mesh + colliders)
└─ GoalTrigger (BoxCollider isTrigger, Tag: Goal)

Components:
- GoalTrigger: BoxCollider (Is Trigger: ON)
- TriggerRelay
  - Type: Goal
  - Director: [will be assigned in scene]
```

### **E. Rink Prefab**
```
Hierarchy: RinkRoot (Layer: Rink)
├─ Ice (Plane, Material: Mat_Ice)
├─ Boards (BoxColliders with Wall.physicMaterial)
├─ OutOfPlay (BoxCollider isTrigger, Tag: OutOfPlay)
├─ Spawns
│ ├─ PlayerSpawn (empty)
│ ├─ PuckSpawn (empty)
│ └─ GoalieSpawn (empty)
└─ Lighting (Directional Light)

Components:
- RinkSetup
  - Player Spawn: PlayerSpawn
  - Puck Spawn: PuckSpawn
  - Goalie Spawn: GoalieSpawn
  - Goal Trigger: [Goal/GoalTrigger from scene]
```

## 🎬 **Step 5: Scene Creation**

### **A. Training Scene**
1. **Create New Scene**: `Assets/Scenes/Training.unity`
2. **Add Rink**: Place RinkRoot prefab
3. **Add Goal**: Place Goal prefab at one end
4. **Add GameObjects**: Place Player, Goalie, Puck at spawn points
5. **Add Director**: Create empty GameObject "GameDirector"
   - Add `GameDirector` component
   - Assign references: agent, salary, training, goalieVisualAnchor, goalCenter
6. **Add Bootstrap**: Create empty GameObject "Bootstrap"
   - Add `BootstrapTraining` component
   - Assign TrainingSwitch.asset (TrainingMode: true)
7. **Configure Triggers**: Add `TriggerRelay` components to triggers
8. **Camera**: Simple static camera (top-down or behind shooter)
9. **No UI**: Keep scene minimal for training speed

### **B. Play Scene**
1. **Duplicate Training**: Duplicate Training.unity → `Play.unity`
2. **Update TrainingSwitch**: Set TrainingMode: false
3. **Add UI**: Create Canvas with NeuralRinkHUD
   - SalaryHUD component
   - EventFeed component
   - PopupSpawner component
4. **Add Audio**: Add AudioManager and ParticleEffectManager
5. **Enhanced Camera**: Dynamic camera system
6. **Visual Polish**: Enable all visual effects

## 🔧 **Step 6: Component Wiring**

### **GameDirector Setup**
```csharp
// Assign these references in both scenes:
GameDirector:
- agent: Goalie (from scene)
- salary: SalarySystem.asset
- training: TrainingSwitch.asset
- eventFeed: EventFeed (Play scene only)
- salaryHUD: SalaryHUD (Play scene only)
- popups: PopupSpawner (Play scene only)
- goalieVisualAnchor: Goalie/Visual
- goalCenter: Goalie/GoalCenter
- mainCam: Main Camera
```

### **TriggerRelay Setup**
```csharp
// Goal Trigger:
TriggerRelay:
- Type: Goal
- Director: GameDirector (from scene)

// Save Zone:
TriggerRelay:
- Type: Save
- Director: GameDirector (from scene)

// Out of Play:
TriggerRelay:
- Type: Miss
- Director: GameDirector (from scene)
```

## ✅ **Step 7: Validation**

### **Training Scene Test**
1. Press Play
2. Check Console for ML-Agents connection
3. Verify GoalieAgent is making decisions
4. Confirm no UI elements are drawn
5. Test episode flow (goal/save/miss)

### **Play Scene Test**
1. Press Play
2. Verify HUD shows salary information
3. Test player controls
4. Confirm UI feedback on events
5. Check audio and visual effects

## 🚀 **Step 8: Build Preparation**

### **Build Settings**
1. **File → Build Settings**
2. **Add Scenes**: Training.unity, Play.unity
3. **Platform**: Switch to target platform
4. **Player Settings**:
   - Company Name: Neural Rink
   - Product Name: Neural Rink
   - Version: 1.0.0

### **Build Scripts**
Use the provided `build.py` script:
```bash
python build.py --all-platforms
```

## 📊 **Step 9: Training Integration**

### **ML-Agents Training**
1. **Training Mode**: Use Training.unity scene
2. **Command**: `mlagents-learn Assets/ML-Agents/NeuralRink_PPO.yaml`
3. **Monitor**: `tensorboard --logdir results/`

### **Model Integration**
1. **Trained Model**: Copy from `results/neural_rink_full/`
2. **Unity Import**: Place in `Assets/Models/`
3. **Behavior Parameters**: Assign model file
4. **Test**: Switch to Play scene and test trained AI

## 🎉 **Complete Setup Checklist**

- [ ] Unity 6 project created
- [ ] All packages installed
- [ ] Project settings configured
- [ ] Tags and layers created
- [ ] Materials created
- [ ] ScriptableObjects created
- [ ] All prefabs created
- [ ] Training scene built
- [ ] Play scene built
- [ ] Components wired
- [ ] Scenes validated
- [ ] Build settings configured
- [ ] Training completed
- [ ] Model integrated
- [ ] Final builds created

## 🔧 **Troubleshooting**

### **Common Issues**
1. **ML-Agents not connecting**: Check Behavior Name matches YAML
2. **Prefabs not working**: Verify all components are assigned
3. **Triggers not firing**: Check TriggerRelay setup
4. **Training not learning**: Verify reward system and observations
5. **Build fails**: Check for missing references or scripts

### **Performance Tips**
1. **Training Scene**: Keep minimal for speed
2. **Play Scene**: Full features for presentation
3. **Physics**: Use FixedUpdate for deterministic behavior
4. **Graphics**: Optimize for target hardware

---

**Ready to create your Unity scenes? Follow this guide step by step!**
