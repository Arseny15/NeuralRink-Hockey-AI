#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using NeuralRink.Gameplay;
using NeuralRink.Agents;
using NeuralRink.Systems;
using System.IO;

namespace NeuralRink.Setup
{
    /// <summary>
    /// Instant game setup that creates everything needed for immediate play.
    /// Run this once and your game will be fully playable!
    /// </summary>
    public class InstantGameSetup : EditorWindow
    {
        [MenuItem("NeuralRink/🚀 INSTANT SETUP - Make Game Playable Now!")]
        public static void ShowWindow()
        {
            GetWindow<InstantGameSetup>("Neural Rink Instant Setup");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🏒 Neural Rink - Instant Game Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            GUILayout.Label("This will create everything needed for immediate play:", EditorStyles.helpBox);
            GUILayout.Label("• Player and Goalkeeper prefabs");
            GUILayout.Label("• Puck and Goal prefabs");
            GUILayout.Label("• Complete rink with ice surface");
            GUILayout.Label("• Spawn points and game objects");
            GUILayout.Label("• UI system");
            GUILayout.Label("• ML model integration");
            
            GUILayout.Space(20);
            
            if (GUILayout.Button("🚀 CREATE COMPLETE PLAYABLE GAME", GUILayout.Height(40)))
            {
                CreateCompletePlayableGame();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("🎯 Create Prefabs Only", GUILayout.Height(30)))
            {
                CreateAllPrefabs();
            }
            
            if (GUILayout.Button("🎬 Setup Play Scene Only", GUILayout.Height(30)))
            {
                SetupPlayScene();
            }
            
            if (GUILayout.Button("🤖 Deploy ML Model Only", GUILayout.Height(30)))
            {
                DeployBestMLModel();
            }
        }
        
        /// <summary>
        /// Create complete playable game with one click.
        /// </summary>
        private void CreateCompletePlayableGame()
        {
            Debug.Log("🚀 Creating complete playable Neural Rink game...");
            
            try
            {
                // Step 1: Create all prefabs
                CreateAllPrefabs();
                
                // Step 2: Setup Play scene
                SetupPlayScene();
                
                // Step 3: Deploy ML model
                DeployBestMLModel();
                
                // Step 4: Final configuration
                ConfigureGameSettings();
                
                // Success message
                EditorUtility.DisplayDialog("🎉 Success!", 
                    "Your Neural Rink game is now fully playable!" +
                    "\n\nNext steps:" +
                    "\n1. Open Play.unity scene" +
                    "\n2. Press Play ▶️" +
                    "\n3. Use WASD to move, mouse to aim and shoot!" +
                    "\n\nControls:" +
                    "\n• WASD: Move player" +
                    "\n• Mouse: Aim" +
                    "\n• Click: Shoot" +
                    "\n• R: Reset game", 
                    "Start Playing!");
                
                Debug.Log("✅ Complete playable game created successfully!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Setup failed: {e.Message}");
                EditorUtility.DisplayDialog("Setup Error", $"Setup failed: {e.Message}", "OK");
            }
        }
        
        /// <summary>
        /// Create all game prefabs.
        /// </summary>
        private void CreateAllPrefabs()
        {
            Debug.Log("🎯 Creating all game prefabs...");
            
            // Ensure Prefabs directory exists
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
            
            CreatePlayerPrefabInstant();
            CreateGoalkeeperPrefabInstant();
            CreatePuckPrefabInstant();
            CreateGoalPrefabInstant();
            CreateIceSurfacePrefabInstant();
            
            AssetDatabase.Refresh();
            Debug.Log("✅ All prefabs created!");
        }
        
        /// <summary>
        /// Create player prefab with air hockey movement.
        /// </summary>
        private void CreatePlayerPrefabInstant()
        {
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            
            // Visual
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(player.transform);
            visual.transform.localPosition = new Vector3(0, 1f, 0);
            visual.GetComponent<Renderer>().material.color = Color.blue;
            
            // Physics
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.mass = 2.5f;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            CapsuleCollider collider = player.AddComponent<CapsuleCollider>();
            collider.height = 2f;
            collider.radius = 0.5f;
            collider.center = new Vector3(0, 1f, 0);
            
            // Scripts
            player.AddComponent<PlayerMovement>();
            player.AddComponent<GameReset>();
            
            // Save
            PrefabUtility.SaveAsPrefabAsset(player, "Assets/Prefabs/Player.prefab");
            DestroyImmediate(player);
        }
        
        /// <summary>
        /// Create goalkeeper prefab with AI.
        /// </summary>
        private void CreateGoalkeeperPrefabInstant()
        {
            GameObject goalkeeper = new GameObject("Goalkeeper");
            goalkeeper.tag = "Goalkeeper";
            
            // Visual
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(goalkeeper.transform);
            visual.transform.localPosition = new Vector3(0, 1.25f, 0);
            visual.transform.localScale = new Vector3(1.5f, 1.25f, 1.5f);
            visual.GetComponent<Renderer>().material.color = Color.red;
            
            // Physics
            Rigidbody rb = goalkeeper.AddComponent<Rigidbody>();
            rb.mass = 3.5f;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            CapsuleCollider collider = goalkeeper.AddComponent<CapsuleCollider>();
            collider.height = 2.5f;
            collider.radius = 0.75f;
            collider.center = new Vector3(0, 1.25f, 0);
            
            // Scripts
            goalkeeper.AddComponent<GoalkeeperMovement>();
            goalkeeper.AddComponent<GameReset>();
            goalkeeper.AddComponent<GoalieAgent>();
            
            // Save
            PrefabUtility.SaveAsPrefabAsset(goalkeeper, "Assets/Prefabs/Goalkeeper.prefab");
            DestroyImmediate(goalkeeper);
        }
        
        /// <summary>
        /// Create puck prefab with air hockey physics.
        /// </summary>
        private void CreatePuckPrefabInstant()
        {
            GameObject puck = new GameObject("Puck");
            puck.tag = "Puck";
            
            // Visual
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.name = "Visual";
            visual.transform.SetParent(puck.transform);
            visual.transform.localScale = new Vector3(1f, 0.1f, 1f);
            visual.GetComponent<Renderer>().material.color = Color.black;
            
            // Physics
            Rigidbody rb = puck.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            
            CapsuleCollider collider = puck.AddComponent<CapsuleCollider>();
            collider.height = 0.2f;
            collider.radius = 0.5f;
            collider.direction = 1;
            
            // Scripts
            puck.AddComponent<PuckController>();
            puck.AddComponent<GameReset>();
            
            // Save
            PrefabUtility.SaveAsPrefabAsset(puck, "Assets/Prefabs/Puck.prefab");
            DestroyImmediate(puck);
        }
        
        /// <summary>
        /// Create goal prefab.
        /// </summary>
        private void CreateGoalPrefabInstant()
        {
            GameObject goal = new GameObject("Goal");
            goal.tag = "Goal";
            
            // Goal posts
            var leftPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leftPost.name = "Left Post";
            leftPost.transform.SetParent(goal.transform);
            leftPost.transform.localPosition = new Vector3(0, 1, -1.5f);
            leftPost.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
            
            var rightPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rightPost.name = "Right Post";
            rightPost.transform.SetParent(goal.transform);
            rightPost.transform.localPosition = new Vector3(0, 1, 1.5f);
            rightPost.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
            
            var crossbar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crossbar.name = "Crossbar";
            crossbar.transform.SetParent(goal.transform);
            crossbar.transform.localPosition = new Vector3(0, 2, 0);
            crossbar.transform.localScale = new Vector3(3f, 0.1f, 0.1f);
            crossbar.transform.rotation = Quaternion.Euler(0, 0, 90);
            
            // Goal trigger
            BoxCollider trigger = goal.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(0.5f, 2f, 4f);
            
            // Save
            PrefabUtility.SaveAsPrefabAsset(goal, "Assets/Prefabs/Goal.prefab");
            DestroyImmediate(goal);
        }
        
        /// <summary>
        /// Create ice surface prefab with air hockey texture.
        /// </summary>
        private void CreateIceSurfacePrefabInstant()
        {
            GameObject ice = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ice.name = "Ice Surface";
            ice.tag = "Ice";
            ice.transform.localScale = new Vector3(20f, 0.1f, 12f);
            
            // Apply air hockey texture if available
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/air_hockey_full.png");
            if (texture != null)
            {
                var material = new Material(Shader.Find("Standard"));
                material.name = "Ice Surface";
                material.mainTexture = texture;
                material.color = Color.white;
                material.SetFloat("_Smoothness", 0.9f);
                ice.GetComponent<Renderer>().material = material;
                AssetDatabase.CreateAsset(material, "Assets/Materials/IceSurface.mat");
            }
            
            // Save
            PrefabUtility.SaveAsPrefabAsset(ice, "Assets/Prefabs/IceSurface.prefab");
            DestroyImmediate(ice);
        }
        
        /// <summary>
        /// Setup Play scene with complete game.
        /// </summary>
        private void SetupPlayScene()
        {
            Debug.Log("🎬 Setting up Play scene...");
            
            // Open Play scene
            EditorSceneManager.OpenScene("Assets/Scenes/Play.unity");
            
            // Clear existing objects (except camera and light)
            var existingObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var obj in existingObjects)
            {
                if (obj.GetComponent<Camera>() == null && obj.GetComponent<Light>() == null)
                {
                    DestroyImmediate(obj);
                }
            }
            
            // Create ice surface
            var icePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/IceSurface.prefab");
            if (icePrefab != null)
            {
                var ice = PrefabUtility.InstantiatePrefab(icePrefab) as GameObject;
                ice.transform.position = Vector3.zero;
            }
            
            // Create walls
            CreateRinkWalls();
            
            // Create player
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
            if (playerPrefab != null)
            {
                var player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
                player.transform.position = new Vector3(-8f, 1f, 0f);
            }
            
            // Create goalkeeper
            var goalkeeperPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Goalkeeper.prefab");
            if (goalkeeperPrefab != null)
            {
                var goalkeeper = PrefabUtility.InstantiatePrefab(goalkeeperPrefab) as GameObject;
                goalkeeper.transform.position = new Vector3(8f, 1.25f, 0f);
            }
            
            // Create puck
            var puckPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Puck.prefab");
            if (puckPrefab != null)
            {
                var puck = PrefabUtility.InstantiatePrefab(puckPrefab) as GameObject;
                puck.transform.position = new Vector3(0f, 0.5f, 0f);
            }
            
            // Create goal
            var goalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Goal.prefab");
            if (goalPrefab != null)
            {
                var goal = PrefabUtility.InstantiatePrefab(goalPrefab) as GameObject;
                goal.transform.position = new Vector3(9f, 0f, 0f);
            }
            
            // Create game systems
            CreateGameSystems();
            
            // Save scene
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            
            Debug.Log("✅ Play scene setup completed!");
        }
        
        /// <summary>
        /// Create rink walls.
        /// </summary>
        private void CreateRinkWalls()
        {
            Vector3[] wallPositions = {
                new Vector3(0, 1, 6.5f),   // North
                new Vector3(0, 1, -6.5f),  // South
                new Vector3(10.5f, 1, 0),  // East
                new Vector3(-10.5f, 1, 0)  // West
            };
            
            Vector3[] wallScales = {
                new Vector3(20f, 2f, 1f),
                new Vector3(20f, 2f, 1f),
                new Vector3(1f, 2f, 12f),
                new Vector3(1f, 2f, 12f)
            };
            
            for (int i = 0; i < wallPositions.Length; i++)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = $"Wall {i + 1}";
                wall.tag = "Wall";
                wall.transform.position = wallPositions[i];
                wall.transform.localScale = wallScales[i];
                wall.GetComponent<Renderer>().material.color = Color.gray;
            }
        }
        
        /// <summary>
        /// Create game systems (managers, UI, etc.).
        /// </summary>
        private void CreateGameSystems()
        {
            // Game Manager
            GameObject gameManager = new GameObject("Game Manager");
            gameManager.AddComponent<SalarySystem>();
            gameManager.AddComponent<GameDirector>();
            
            // UI Canvas
            GameObject canvas = new GameObject("Game UI");
            var canvasComp = canvas.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            canvas.AddComponent<GameHUD>();
            
            // Training Switch
            var trainingSwitch = ScriptableObject.CreateInstance<TrainingSwitch>();
            trainingSwitch.IsTraining = false; // Play mode
            AssetDatabase.CreateAsset(trainingSwitch, "Assets/Resources/PlayModeSwitch.asset");
        }
        
        /// <summary>
        /// Deploy the best trained ML model.
        /// </summary>
        private void DeployBestMLModel()
        {
            Debug.Log("🤖 Deploying best ML model...");
            
            string[] modelPaths = {
                "results/neural_rink_full/best_model/best_model.zip",
                "results/neural_rink_fixed/best_model/best_model.zip",
                "results/neural_rink_full/final_model.zip"
            };
            
            string bestModel = null;
            long bestSize = 0;
            
            foreach (string path in modelPaths)
            {
                if (File.Exists(path))
                {
                    var info = new FileInfo(path);
                    if (info.Length > bestSize)
                    {
                        bestSize = info.Length;
                        bestModel = path;
                    }
                }
            }
            
            if (bestModel != null)
            {
                string destPath = "Assets/Models/DeployedGoalieModel.zip";
                File.Copy(bestModel, destPath, true);
                AssetDatabase.Refresh();
                
                Debug.Log($"✅ Best ML model deployed: {bestModel} ({bestSize} bytes)");
            }
            else
            {
                Debug.LogWarning("No trained models found. AI will use simple fallback.");
            }
        }
        
        /// <summary>
        /// Configure final game settings.
        /// </summary>
        private void ConfigureGameSettings()
        {
            Debug.Log("⚙️ Configuring game settings...");
            
            // Physics settings
            Physics.defaultSolverIterations = 6;
            Physics.defaultSolverVelocityIterations = 1;
            Time.fixedDeltaTime = 0.02f;
            
            // Quality settings
            Application.targetFrameRate = 60;
            
            Debug.Log("✅ Game settings configured");
        }
    }
}
#endif