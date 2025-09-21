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
            rb.useGravity = false;
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
            
            Debug.Log($"✅ Configured puck: {puck.name}");
        }
        
        /// <summary>
        /// Setup wall objects with proper physics materials.
        /// </summary>
        private void SetupWalls()
        {
            if (walls == null) return;
            
            foreach (GameObject wall in walls)
            {
                if (wall == null) continue;
                
                // Setup collider with physics material
                Collider col = wall.GetComponent<Collider>();
                if (col != null && wallMaterial != null)
                {
                    col.material = wallMaterial;
                }
                
                // Set tag
                wall.tag = "Wall";
                
                Debug.Log($"✅ Configured wall: {wall.name}");
            }
        }
        
        /// <summary>
        /// Setup ice surface with air hockey texture.
        /// </summary>
        private void SetupIceSurface()
        {
            if (iceSurface == null) return;
            
            // Load air hockey texture
            if (airHockeyTexture == null)
            {
                airHockeyTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/air_hockey_full.png");
            }
            
            if (airHockeyTexture != null)
            {
                // Create or update ice material
                Renderer renderer = iceSurface.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material iceMaterial = new Material(Shader.Find("Standard"));
                    iceMaterial.name = "Ice Surface";
                    iceMaterial.mainTexture = airHockeyTexture;
                    iceMaterial.color = Color.white;
                    iceMaterial.SetFloat("_Smoothness", 0.9f);
                    iceMaterial.SetFloat("_Metallic", 0.1f);
                    
                    renderer.material = iceMaterial;
                    
                    // Save material as asset
                    AssetDatabase.CreateAsset(iceMaterial, "Assets/Materials/IceSurface.mat");
                }
                
                Debug.Log($"✅ Configured ice surface: {iceSurface.name}");
            }
            else
            {
                Debug.LogWarning("Air hockey texture not found at Assets/Materials/air_hockey_full.png");
            }
        }
        
        /// <summary>
        /// Auto-find game objects in the scene.
        /// </summary>
        [ContextMenu("Auto-Find Game Objects")]
        public void AutoFindGameObjects()
        {
            // Find players
            var playerObjects = GameObject.FindGameObjectsWithTag("Player");
            if (playerObjects.Length > 0)
            {
                players = playerObjects;
                Debug.Log($"Found {players.Length} player objects");
            }
            
            // Find goalkeeper
            var goalkeepers = GameObject.FindGameObjectsWithTag("Goalkeeper");
            if (goalkeepers.Length > 0)
            {
                goalkeeper = goalkeepers[0];
                Debug.Log($"Found goalkeeper: {goalkeeper.name}");
            }
            
            // Find puck
            var pucks = GameObject.FindGameObjectsWithTag("Puck");
            if (pucks.Length > 0)
            {
                puck = pucks[0];
                Debug.Log($"Found puck: {puck.name}");
            }
            
            // Find walls
            var wallObjects = GameObject.FindGameObjectsWithTag("Wall");
            if (wallObjects.Length > 0)
            {
                walls = wallObjects;
                Debug.Log($"Found {walls.Length} wall objects");
            }
            
            // Find ice surface (look for large flat object)
            var renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            foreach (var renderer in renderers)
            {
                if (renderer.bounds.size.x > 10f && renderer.bounds.size.z > 10f && renderer.bounds.size.y < 2f)
                {
                    iceSurface = renderer.gameObject;
                    Debug.Log($"Found ice surface: {iceSurface.name}");
                    break;
                }
            }
        }
        
        /// <summary>
        /// Load physics materials from air hockey assets.
        /// </summary>
        [ContextMenu("Load Air Hockey Physics Materials")]
        public void LoadPhysicsMaterials()
        {
            playerMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Physics Materials/Handle.physicMaterial");
            goalkeeperMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Physics Materials/Handle.physicMaterial");
            puckMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Physics Materials/Puck.physicMaterial");
            wallMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Physics Materials/Wall.physicMaterial");
            
            Debug.Log("✅ Loaded air hockey physics materials");
        }
    }
}
#else
namespace NeuralRink.Setup
{
    // Runtime stub
    public class AirHockeyPhysicsSetup : MonoBehaviour { }
}
#endif