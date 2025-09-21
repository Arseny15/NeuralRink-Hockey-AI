#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace NeuralRink.Setup
{
    /// <summary>
    /// Setup script to configure game objects with air hockey physics.
    /// Applies the proven physics materials and movement scripts for optimal gameplay.
    /// </summary>
    public class AirHockeyPhysicsSetup : MonoBehaviour
    {
        [Header("Physics Materials")]
        [SerializeField] private PhysicsMaterial playerMaterial;
        [SerializeField] private PhysicsMaterial goalkeeperMaterial;
        [SerializeField] private PhysicsMaterial puckMaterial;
        [SerializeField] private PhysicsMaterial wallMaterial;
        
        [Header("Game Objects")]
        [SerializeField] private GameObject[] players;
        [SerializeField] private GameObject goalkeeper;
        [SerializeField] private GameObject puck;
        [SerializeField] private GameObject[] walls;
        
        [Header("Ice Surface")]
        [SerializeField] private GameObject iceSurface;
        [SerializeField] private Texture2D airHockeyTexture;
        
        [MenuItem("NeuralRink/Setup/Configure Air Hockey Physics")]
        public static void ConfigurePhysics()
        {
            var setup = FindFirstObjectByType<AirHockeyPhysicsSetup>();
            if (setup != null)
            {
                setup.SetupAllObjects();
            }
            else
            {
                Debug.LogError("AirHockeyPhysicsSetup component not found in scene!");
            }
        }
        
        /// <summary>
        /// Setup all game objects with proper physics.
        /// </summary>
        public void SetupAllObjects()
        {
            CreatePhysicsMaterials();
            SetupPlayers();
            SetupGoalkeeper();
            SetupPuck();
            SetupWalls();
            SetupIceSurface();
            
            Debug.Log("✅ Air hockey physics setup completed!");
        }
        
        /// <summary>
        /// Create physics materials with air hockey specifications.
        /// </summary>
        private void CreatePhysicsMaterials()
        {
            string materialsPath = "Assets/Physics Materials";
            
            // Player Physics Material
            if (playerMaterial == null)
            {
                playerMaterial = new PhysicsMaterial("Player");
                playerMaterial.dynamicFriction = 4f;
                playerMaterial.staticFriction = 0.5f;
                playerMaterial.bounciness = 0.7f;
                playerMaterial.frictionCombine = PhysicsMaterialCombine.Average;
                playerMaterial.bounceCombine = PhysicsMaterialCombine.Average;
                AssetDatabase.CreateAsset(playerMaterial, $"{materialsPath}/Player.physicMaterial");
            }
            
            // Goalkeeper Physics Material
            if (goalkeeperMaterial == null)
            {
                goalkeeperMaterial = new PhysicsMaterial("Goalkeeper");
                goalkeeperMaterial.dynamicFriction = 4f;
                goalkeeperMaterial.staticFriction = 0.5f;
                goalkeeperMaterial.bounciness = 0.6f;
                goalkeeperMaterial.frictionCombine = PhysicsMaterialCombine.Average;
                goalkeeperMaterial.bounceCombine = PhysicsMaterialCombine.Average;
                AssetDatabase.CreateAsset(goalkeeperMaterial, $"{materialsPath}/Goalkeeper.physicMaterial");
            }
            
            // Puck Physics Material
            if (puckMaterial == null)
            {
                puckMaterial = new PhysicsMaterial("Puck");
                puckMaterial.dynamicFriction = 0f;
                puckMaterial.staticFriction = 0f;
                puckMaterial.bounciness = 1f;
                puckMaterial.frictionCombine = PhysicsMaterialCombine.Average;
                puckMaterial.bounceCombine = PhysicsMaterialCombine.Average;
                AssetDatabase.CreateAsset(puckMaterial, $"{materialsPath}/Puck.physicMaterial");
            }
            
            // Wall Physics Material
            if (wallMaterial == null)
            {
                wallMaterial = new PhysicsMaterial("Wall");
                wallMaterial.dynamicFriction = 0f;
                wallMaterial.staticFriction = 0f;
                wallMaterial.bounciness = 0.5f;
                wallMaterial.frictionCombine = PhysicsMaterialCombine.Average;
                wallMaterial.bounceCombine = PhysicsMaterialCombine.Average;
                AssetDatabase.CreateAsset(wallMaterial, $"{materialsPath}/Wall.physicMaterial");
            }
            
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// Setup player objects with proper components and physics.
        /// </summary>
        private void SetupPlayers()
        {
            if (players == null) return;
            
            foreach (GameObject player in players)
            {
                if (player == null) continue;
                
                // Ensure required components
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = player.AddComponent<Rigidbody>();
                }
                
                // Configure rigidbody
                rb.mass = 2.5f;
                rb.constraints = RigidbodyConstraints.FreezePositionY | 
                                RigidbodyConstraints.FreezeRotationX | 
                                RigidbodyConstraints.FreezeRotationZ;
                
                // Setup collider with physics material
                Collider col = player.GetComponent<Collider>();
                if (col != null && playerMaterial != null)
                {
                    col.material = playerMaterial;
                }
                
                // Add movement script
                if (player.GetComponent<PlayerMovement>() == null)
                {
                    player.AddComponent<PlayerMovement>();
                }
                
                // Add reset script
                if (player.GetComponent<GameReset>() == null)
                {
                    player.AddComponent<GameReset>();
                }
                
                // Set tag
                player.tag = "Player";
                
                Debug.Log($"✅ Configured player: {player.name}");
            }
        }
        
        /// <summary>
        /// Setup goalkeeper object with proper components and physics.
        /// </summary>
        private void SetupGoalkeeper()
        {
            if (goalkeeper == null) return;
            
            // Ensure required components
            Rigidbody rb = goalkeeper.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = goalkeeper.AddComponent<Rigidbody>();
            }
            
            // Configure rigidbody
            rb.mass = 3.5f;
            rb.constraints = RigidbodyConstraints.FreezePositionY | 
                            RigidbodyConstraints.FreezeRotationX | 
                            RigidbodyConstraints.FreezeRotationZ;
            
            // Setup collider with physics material
            Collider col = goalkeeper.GetComponent<Collider>();
            if (col != null && goalkeeperMaterial != null)
            {
                col.material = goalkeeperMaterial;
            }
            
            // Add movement script
            if (goalkeeper.GetComponent<GoalkeeperMovement>() == null)
            {
                goalkeeper.AddComponent<GoalkeeperMovement>();
            }
            
            // Add reset script
            if (goalkeeper.GetComponent<GameReset>() == null)
            {
                goalkeeper.AddComponent<GameReset>();
            }
            
            // Set tag
            goalkeeper.tag = "Goalkeeper";
            
            Debug.Log($"✅ Configured goalkeeper: {goalkeeper.name}");
        }
        
        /// <summary>
        /// Setup puck object with proper physics.
        /// </summary>
        private void SetupPuck()
        {
            if (puck == null) return;
            
            // Ensure required components
            Rigidbody rb = puck.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = puck.AddComponent<Rigidbody>();
            }
            
            // Configure rigidbody for air hockey physics
            rb.mass = 1f;
            rb.useGravity = false; // Air hockey pucks float
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            
            // Setup collider with physics material
            Collider col = puck.GetComponent<Collider>();
            if (col != null && puckMaterial != null)
            {
                col.material = puckMaterial;
            }
            
            // Add reset script
            if (puck.GetComponent<GameReset>() == null)
            {
                puck.AddComponent<GameReset>();
            }
            
            // Set tag
            puck.tag = "Puck";
            
            // Enable air hockey physics in PuckController
            var puckController = puck.GetComponent<PuckController>();
            if (puckController != null)
            {
                var field = typeof(PuckController).GetField("useAirHockeyPhysics", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(puckController, true);
                }
            }
            
            Debug.Log($"✅ Configured puck: {puck.name}");\n        }\n        \n        /// <summary>\n        /// Setup wall objects with proper physics materials.\n        /// </summary>\n        private void SetupWalls()\n        {\n            if (walls == null) return;\n            \n            foreach (GameObject wall in walls)\n            {\n                if (wall == null) continue;\n                \n                // Setup collider with physics material\n                Collider col = wall.GetComponent<Collider>();\n                if (col != null && wallMaterial != null)\n                {\n                    col.material = wallMaterial;\n                }\n                \n                // Set tag\n                wall.tag = \"Wall\";\n                \n                Debug.Log($\"✅ Configured wall: {wall.name}\");\n            }\n        }\n        \n        /// <summary>\n        /// Setup ice surface with air hockey texture.\n        /// </summary>\n        private void SetupIceSurface()\n        {\n            if (iceSurface == null) return;\n            \n            // Load air hockey texture\n            if (airHockeyTexture == null)\n            {\n                airHockeyTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(\"Assets/Materials/air_hockey_full.png\");\n            }\n            \n            if (airHockeyTexture != null)\n            {\n                // Create or update ice material\n                Renderer renderer = iceSurface.GetComponent<Renderer>();\n                if (renderer != null)\n                {\n                    Material iceMaterial = new Material(Shader.Find(\"Standard\"));\n                    iceMaterial.name = \"Ice Surface\";\n                    iceMaterial.mainTexture = airHockeyTexture;\n                    iceMaterial.color = Color.white;\n                    iceMaterial.SetFloat(\"_Smoothness\", 0.9f);\n                    iceMaterial.SetFloat(\"_Metallic\", 0.1f);\n                    \n                    renderer.material = iceMaterial;\n                    \n                    // Save material as asset\n                    AssetDatabase.CreateAsset(iceMaterial, \"Assets/Materials/IceSurface.mat\");\n                }\n                \n                Debug.Log($\"✅ Configured ice surface: {iceSurface.name}\");\n            }\n            else\n            {\n                Debug.LogWarning(\"Air hockey texture not found at Assets/Materials/air_hockey_full.png\");\n            }\n        }\n        \n        /// <summary>\n        /// Auto-find game objects in the scene.\n        /// </summary>\n        [ContextMenu(\"Auto-Find Game Objects\")]\n        public void AutoFindGameObjects()\n        {\n            // Find players\n            var playerObjects = GameObject.FindGameObjectsWithTag(\"Player\");\n            if (playerObjects.Length > 0)\n            {\n                players = playerObjects;\n                Debug.Log($\"Found {players.Length} player objects\");\n            }\n            \n            // Find goalkeeper\n            var goalkeepers = GameObject.FindGameObjectsWithTag(\"Goalkeeper\");\n            if (goalkeepers.Length > 0)\n            {\n                goalkeeper = goalkeepers[0];\n                Debug.Log($\"Found goalkeeper: {goalkeeper.name}\");\n            }\n            \n            // Find puck\n            var pucks = GameObject.FindGameObjectsWithTag(\"Puck\");\n            if (pucks.Length > 0)\n            {\n                puck = pucks[0];\n                Debug.Log($\"Found puck: {puck.name}\");\n            }\n            \n            // Find walls\n            var wallObjects = GameObject.FindGameObjectsWithTag(\"Wall\");\n            if (wallObjects.Length > 0)\n            {\n                walls = wallObjects;\n                Debug.Log($\"Found {walls.Length} wall objects\");\n            }\n            \n            // Find ice surface (look for large flat object)\n            var renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);\n            foreach (var renderer in renderers)\n            {\n                if (renderer.bounds.size.x > 10f && renderer.bounds.size.z > 10f && renderer.bounds.size.y < 2f)\n                {\n                    iceSurface = renderer.gameObject;\n                    Debug.Log($\"Found ice surface: {iceSurface.name}\");\n                    break;\n                }\n            }\n        }\n        \n        /// <summary>\n        /// Load physics materials from air hockey assets.\n        /// </summary>\n        [ContextMenu(\"Load Air Hockey Physics Materials\")]\n        public void LoadPhysicsMaterials()\n        {\n            playerMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(\"Assets/Physics Materials/Handle.physicMaterial\");\n            goalkeeperMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(\"Assets/Physics Materials/Handle.physicMaterial\");\n            puckMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(\"Assets/Physics Materials/Puck.physicMaterial\");\n            wallMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(\"Assets/Physics Materials/Wall.physicMaterial\");\n            \n            Debug.Log(\"✅ Loaded air hockey physics materials\");\n        }\n    }\n}\n#else\nnamespace NeuralRink.Setup\n{\n    // Runtime stub\n    public class AirHockeyPhysicsSetup : MonoBehaviour { }\n}\n#endif
