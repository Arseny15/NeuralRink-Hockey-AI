#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using NeuralRink.Gameplay;
using NeuralRink.Agents;

namespace NeuralRink.Setup
{
    /// <summary>
    /// Creates complete game prefabs with air hockey movement integration.
    /// </summary>
    public class CreateGamePrefabs : MonoBehaviour
    {
        [Header("Physics Materials")]
        [SerializeField] private PhysicsMaterial playerPhysicsMaterial;
        [SerializeField] private PhysicsMaterial goalkeeperPhysicsMaterial;
        [SerializeField] private PhysicsMaterial puckPhysicsMaterial;
        
        [MenuItem("NeuralRink/Create/All Game Prefabs")]
        public static void CreateAllPrefabs()
        {
            CreatePlayerPrefab();
            CreateGoalkeeperPrefab();
            CreatePuckPrefab();
            CreateGoalPrefab();
            
            AssetDatabase.Refresh();
            Debug.Log("✅ All game prefabs created with air hockey movement!");
        }
        
        [MenuItem("NeuralRink/Create/Player Prefab")]
        public static void CreatePlayerPrefab()
        {
            // Create player object
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            
            // Add visual representation
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(player.transform);
            visual.transform.localPosition = new Vector3(0, 1f, 0);
            visual.transform.localScale = new Vector3(1f, 1f, 1f);
            
            // Add physics
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.mass = 2.5f;
            rb.constraints = RigidbodyConstraints.FreezePositionY | 
                            RigidbodyConstraints.FreezeRotationX | 
                            RigidbodyConstraints.FreezeRotationZ;
            
            // Add collider
            CapsuleCollider collider = player.AddComponent<CapsuleCollider>();
            collider.height = 2f;
            collider.radius = 0.5f;
            collider.center = new Vector3(0, 1f, 0);
            
            // Apply physics material
            var playerMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Physics Materials/Handle.physicMaterial");
            if (playerMaterial != null)
            {
                collider.material = playerMaterial;
            }
            
            // Add movement scripts
            player.AddComponent<PlayerMovement>();
            player.AddComponent<GameReset>();
            
            // Add original player controller for compatibility
            player.AddComponent<PlayerController>();
            
            // Save as prefab
            string prefabPath = "Assets/Prefabs/Player.prefab";
            PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
            DestroyImmediate(player);
            
            Debug.Log($"✅ Player prefab created: {prefabPath}");
        }
        
        [MenuItem("NeuralRink/Create/Goalkeeper Prefab")]
        public static void CreateGoalkeeperPrefab()
        {
            // Create goalkeeper object
            GameObject goalkeeper = new GameObject("Goalkeeper");
            goalkeeper.tag = "Goalkeeper";
            
            // Add visual representation
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(goalkeeper.transform);
            visual.transform.localPosition = new Vector3(0, 1.25f, 0);
            visual.transform.localScale = new Vector3(1.5f, 1.25f, 1.5f);
            
            // Add physics
            Rigidbody rb = goalkeeper.AddComponent<Rigidbody>();
            rb.mass = 3.5f;
            rb.useGravity = false; // Goalkeepers can float slightly
            rb.constraints = RigidbodyConstraints.FreezePositionY | 
                            RigidbodyConstraints.FreezeRotationX | 
                            RigidbodyConstraints.FreezeRotationZ;
            
            // Add collider
            CapsuleCollider collider = goalkeeper.AddComponent<CapsuleCollider>();
            collider.height = 2.5f;
            collider.radius = 0.75f;
            collider.center = new Vector3(0, 1.25f, 0);
            
            // Apply physics material
            var goalkeeperMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Physics Materials/Handle.physicMaterial");
            if (goalkeeperMaterial != null)
            {
                collider.material = goalkeeperMaterial;
            }
            
            // Add movement scripts
            goalkeeper.AddComponent<GoalkeeperMovement>();
            goalkeeper.AddComponent<GameReset>();
            
            // Add AI agent
            goalkeeper.AddComponent<GoalieAgent>();
            
            // Save as prefab
            string prefabPath = "Assets/Prefabs/Goalkeeper.prefab";
            PrefabUtility.SaveAsPrefabAsset(goalkeeper, prefabPath);
            DestroyImmediate(goalkeeper);
            
            Debug.Log($"✅ Goalkeeper prefab created: {prefabPath}");
        }
        
        [MenuItem("NeuralRink/Create/Puck Prefab")]
        public static void CreatePuckPrefab()
        {
            // Create puck object
            GameObject puck = new GameObject("Puck");
            puck.tag = "Puck";
            
            // Add visual representation
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.name = "Visual";
            visual.transform.SetParent(puck.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(1f, 0.1f, 1f);
            
            // Add physics
            Rigidbody rb = puck.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.useGravity = false; // Air hockey style
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            
            // Add collider
            CapsuleCollider collider = puck.AddComponent<CapsuleCollider>();
            collider.height = 0.2f;
            collider.radius = 0.5f;
            collider.direction = 1; // Y-axis
            
            // Apply physics material
            var puckMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Physics Materials/Puck.physicMaterial");
            if (puckMaterial != null)
            {
                collider.material = puckMaterial;
            }
            
            // Add puck controller
            puck.AddComponent<PuckController>();
            puck.AddComponent<GameReset>();
            
            // Save as prefab
            string prefabPath = "Assets/Prefabs/Puck.prefab";
            PrefabUtility.SaveAsPrefabAsset(puck, prefabPath);
            DestroyImmediate(puck);
            
            Debug.Log($"✅ Puck prefab created: {prefabPath}");
        }
        
        [MenuItem("NeuralRink/Create/Goal Prefab")]
        public static void CreateGoalPrefab()
        {
            // Create goal object
            GameObject goal = new GameObject("Goal");
            goal.tag = "Goal";
            
            // Create goal posts
            GameObject leftPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leftPost.name = "Left Post";
            leftPost.transform.SetParent(goal.transform);
            leftPost.transform.localPosition = new Vector3(0, 1, -1.5f);
            leftPost.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
            
            GameObject rightPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rightPost.name = "Right Post";
            rightPost.transform.SetParent(goal.transform);
            rightPost.transform.localPosition = new Vector3(0, 1, 1.5f);
            rightPost.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
            
            GameObject crossbar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crossbar.name = "Crossbar";
            crossbar.transform.SetParent(goal.transform);
            crossbar.transform.localPosition = new Vector3(0, 2, 0);
            crossbar.transform.localScale = new Vector3(3f, 0.1f, 0.1f);
            crossbar.transform.rotation = Quaternion.Euler(0, 0, 90);
            
            // Add goal trigger
            BoxCollider goalTrigger = goal.AddComponent<BoxCollider>();
            goalTrigger.isTrigger = true;
            goalTrigger.size = new Vector3(0.5f, 2f, 4f);
            goalTrigger.center = Vector3.zero;
            
            // Apply wall physics material to posts
            var wallMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Physics Materials/Wall.physicMaterial");
            if (wallMaterial != null)
            {
                leftPost.GetComponent<Collider>().material = wallMaterial;
                rightPost.GetComponent<Collider>().material = wallMaterial;
                crossbar.GetComponent<Collider>().material = wallMaterial;
            }
            
            // Save as prefab
            string prefabPath = "Assets/Prefabs/Goal.prefab";
            PrefabUtility.SaveAsPrefabAsset(goal, prefabPath);
            DestroyImmediate(goal);
            
            Debug.Log($"✅ Goal prefab created: {prefabPath}");
        }
    }
}
#endif
