using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace NeuralRink.Setup
{
    /// <summary>
    /// Project setup utility for Neural Rink.
    /// Ensures all required packages and dependencies are properly configured.
    /// </summary>
    public class ProjectSetup : MonoBehaviour
    {
        [Header("Setup Configuration")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private bool createDefaultPrefabs = true;
        [SerializeField] private bool setupInputActions = true;
        
        /// <summary>
        /// Initialize project setup on start.
        /// </summary>
        private void Start()
        {
            if (autoSetupOnStart)
            {
                PerformProjectSetup();
            }
        }
        
        /// <summary>
        /// Perform complete project setup.
        /// </summary>
        public void PerformProjectSetup()
        {
            Debug.Log("Starting Neural Rink project setup...");
            
            // Check Unity version
            CheckUnityVersion();
            
            // Verify required packages
            VerifyRequiredPackages();
            
            // Create default prefabs if needed
            if (createDefaultPrefabs)
            {
                CreateDefaultPrefabs();
            }
            
            // Setup input actions
            if (setupInputActions)
            {
                SetupInputActions();
            }
            
            // Create default ScriptableObjects
            CreateDefaultScriptableObjects();
            
            Debug.Log("Project setup completed successfully!");
        }
        
        /// <summary>
        /// Check if Unity version is compatible.
        /// </summary>
        private void CheckUnityVersion()
        {
            string unityVersion = Application.unityVersion;
            Debug.Log($"Unity Version: {unityVersion}");
            
            if (!unityVersion.StartsWith("6000."))
            {
                Debug.LogWarning($"Unity 6 (6000.x) is recommended. Current version: {unityVersion}");
            }
        }
        
        /// <summary>
        /// Verify required packages are installed.
        /// </summary>
        private void VerifyRequiredPackages()
        {
            Debug.Log("Verifying required packages...");
            
            // Check for ML-Agents
            if (!IsPackageInstalled("com.unity.ml-agents"))
            {
                Debug.LogError("ML-Agents package not found! Please install com.unity.ml-agents via Package Manager.");
            }
            else
            {
                Debug.Log("✓ ML-Agents package found");
            }
            
            // Check for Input System
            if (!IsPackageInstalled("com.unity.inputsystem"))
            {
                Debug.LogError("Input System package not found! Please install com.unity.inputsystem via Package Manager.");
            }
            else
            {
                Debug.Log("✓ Input System package found");
            }
        }
        
        /// <summary>
        /// Check if a package is installed by looking at manifest.json.
        /// </summary>
        private bool IsPackageInstalled(string packageName)
        {
            string manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
            
            if (File.Exists(manifestPath))
            {
                string manifestContent = File.ReadAllText(manifestPath);
                return manifestContent.Contains(packageName);
            }
            
            return false;
        }
        
        /// <summary>
        /// Create default prefabs for the game.
        /// </summary>
        private void CreateDefaultPrefabs()
        {
            Debug.Log("Creating default prefabs...");
            
            // Create Prefabs directory if it doesn't exist
            string prefabsPath = "Assets/Prefabs";
#if UNITY_EDITOR
            if (!AssetDatabase.IsValidFolder(prefabsPath))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
#endif
            
            // Create default player prefab
            CreatePlayerPrefab();
            
            // Create default goalie prefab
            CreateGoaliePrefab();
            
            // Create default puck prefab
            CreatePuckPrefab();
            
            // Create default goal prefab
            CreateGoalPrefab();
            
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        
        /// <summary>
        /// Create default player prefab.
        /// </summary>
        private void CreatePlayerPrefab()
        {
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            
            // Add visual representation
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.transform.SetParent(player.transform);
            visual.transform.localScale = new Vector3(1f, 2f, 1f);
            visual.transform.localPosition = new Vector3(0, 1f, 0);
            
            // Add physics
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.freezeRotation = true;
            
            // Add collider
            CapsuleCollider collider = player.AddComponent<CapsuleCollider>();
            collider.height = 2f;
            collider.radius = 0.5f;
            
            // Add player controller
            player.AddComponent<NeuralRink.Gameplay.PlayerController>();
            
            // Save as prefab
            string prefabPath = "Assets/Prefabs/Player.prefab";
#if UNITY_EDITOR
            PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
#endif
            DestroyImmediate(player);
            
            Debug.Log($"Created player prefab: {prefabPath}");
        }
        
        /// <summary>
        /// Create default goalie prefab.
        /// </summary>
        private void CreateGoaliePrefab()
        {
            GameObject goalie = new GameObject("Goalie");
            goalie.tag = "Goalie";
            
            // Add visual representation
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.transform.SetParent(goalie.transform);
            visual.transform.localScale = new Vector3(1.5f, 2.5f, 1.5f);
            visual.transform.localPosition = new Vector3(0, 1.25f, 0);
            
            // Add physics
            Rigidbody rb = goalie.AddComponent<Rigidbody>();
            rb.mass = 2f;
            rb.freezeRotation = true;
            rb.useGravity = false;
            
            // Add collider
            CapsuleCollider collider = goalie.AddComponent<CapsuleCollider>();
            collider.height = 2.5f;
            collider.radius = 0.75f;
            
            // Add goalie agent
            goalie.AddComponent<NeuralRink.Agents.GoalieAgent>();
            
            // Save as prefab
            string prefabPath = "Assets/Prefabs/Goalie.prefab";
#if UNITY_EDITOR
            PrefabUtility.SaveAsPrefabAsset(goalie, prefabPath);
#endif
            DestroyImmediate(goalie);
            
            Debug.Log($"Created goalie prefab: {prefabPath}");
        }
        
        /// <summary>
        /// Create default puck prefab.
        /// </summary>
        private void CreatePuckPrefab()
        {
            GameObject puck = new GameObject("Puck");
            puck.tag = "Puck";
            
            // Add visual representation
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.transform.SetParent(puck.transform);
            visual.transform.localScale = new Vector3(1f, 0.2f, 1f);
            
            // Add physics
            Rigidbody rb = puck.AddComponent<Rigidbody>();
            rb.mass = 0.17f; // Standard hockey puck mass
            rb.useGravity = false;
            
            // Add collider (using CapsuleCollider as CylinderCollider doesn't exist in Unity 6)
            CapsuleCollider collider = puck.AddComponent<CapsuleCollider>();
            collider.height = 0.2f;
            collider.radius = 0.5f;
            collider.direction = 1; // Y-axis, approximates a cylinder
            collider.isTrigger = true;
            
            // Add puck controller
            puck.AddComponent<NeuralRink.Gameplay.PuckController>();
            
            // Save as prefab
            string prefabPath = "Assets/Prefabs/Puck.prefab";
#if UNITY_EDITOR
            PrefabUtility.SaveAsPrefabAsset(puck, prefabPath);
#endif
            DestroyImmediate(puck);
            
            Debug.Log($"Created puck prefab: {prefabPath}");
        }
        
        /// <summary>
        /// Create default goal prefab.
        /// </summary>
        private void CreateGoalPrefab()
        {
            GameObject goal = new GameObject("Goal");
            goal.tag = "Goal";
            
            // Create goal posts
            GameObject leftPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leftPost.transform.SetParent(goal.transform);
            leftPost.transform.localPosition = new Vector3(0, 1, -1.5f);
            leftPost.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
            
            GameObject rightPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rightPost.transform.SetParent(goal.transform);
            rightPost.transform.localPosition = new Vector3(0, 1, 1.5f);
            rightPost.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
            
            GameObject crossbar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crossbar.transform.SetParent(goal.transform);
            crossbar.transform.localPosition = new Vector3(0, 2, 0);
            crossbar.transform.localScale = new Vector3(3f, 0.1f, 0.1f);
            crossbar.transform.rotation = Quaternion.Euler(0, 0, 90);
            
            // Add goal trigger
            BoxCollider goalTrigger = goal.AddComponent<BoxCollider>();
            goalTrigger.isTrigger = true;
            goalTrigger.size = new Vector3(0.5f, 2f, 4f);
            goalTrigger.center = Vector3.zero;
            
            // Save as prefab
            string prefabPath = "Assets/Prefabs/Goal.prefab";
#if UNITY_EDITOR
            PrefabUtility.SaveAsPrefabAsset(goal, prefabPath);
#endif
            DestroyImmediate(goal);
            
            Debug.Log($"Created goal prefab: {prefabPath}");
        }
        
        /// <summary>
        /// Setup input actions configuration.
        /// </summary>
        private void SetupInputActions()
        {
            Debug.Log("Setting up input actions...");
            
            // Check if input actions file exists
            string inputActionsPath = "Assets/Input/Controls.inputactions";
            if (File.Exists(inputActionsPath))
            {
                Debug.Log("✓ Input actions file found");
            }
            else
            {
                Debug.LogWarning("Input actions file not found. Please ensure Controls.inputactions exists in Assets/Input/");
            }
        }
        
        /// <summary>
        /// Create default ScriptableObjects.
        /// </summary>
        private void CreateDefaultScriptableObjects()
        {
            Debug.Log("Creating default ScriptableObjects...");
            
            // Create ScriptableObjects directory if it doesn't exist
            string soPath = "Assets/ScriptableObjects";
#if UNITY_EDITOR
            if (!AssetDatabase.IsValidFolder(soPath))
            {
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            }
#endif
            
            // Create default TrainingSwitch
            CreateDefaultTrainingSwitch();
            
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        
        /// <summary>
        /// Create default TrainingSwitch ScriptableObject.
        /// </summary>
        private void CreateDefaultTrainingSwitch()
        {
            string trainingSwitchPath = "Assets/ScriptableObjects/DefaultTrainingSwitch.asset";
            
            if (!File.Exists(trainingSwitchPath))
            {
                NeuralRink.Systems.TrainingSwitch trainingSwitch = ScriptableObject.CreateInstance<NeuralRink.Systems.TrainingSwitch>();
                
                // Set default values
                // Note: These would be set in the Inspector, but we can set some defaults here
                
#if UNITY_EDITOR
                AssetDatabase.CreateAsset(trainingSwitch, trainingSwitchPath);
#endif
                Debug.Log($"Created default TrainingSwitch: {trainingSwitchPath}");
            }
            else
            {
                Debug.Log("✓ Default TrainingSwitch already exists");
            }
        }
        
        /// <summary>
        /// Validate project setup.
        /// </summary>
        public bool ValidateSetup()
        {
            bool isValid = true;
            
            // Check for required scripts
            if (FindFirstObjectByType<NeuralRink.Agents.GoalieAgent>() == null)
            {
                Debug.LogError("GoalieAgent not found in scene!");
                isValid = false;
            }
            
            if (FindFirstObjectByType<NeuralRink.Gameplay.PlayerController>() == null)
            {
                Debug.LogError("PlayerController not found in scene!");
                isValid = false;
            }
            
            if (FindFirstObjectByType<NeuralRink.Gameplay.PuckController>() == null)
            {
                Debug.LogError("PuckController not found in scene!");
                isValid = false;
            }
            
            if (isValid)
            {
                Debug.Log("✓ Project setup validation passed!");
            }
            else
            {
                Debug.LogError("✗ Project setup validation failed!");
            }
            
            return isValid;
        }
    }
}
