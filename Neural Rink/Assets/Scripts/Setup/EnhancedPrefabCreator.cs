using UnityEngine;
using UnityEditor;
using NeuralRink.Visuals;

namespace NeuralRink.Setup
{
    /// <summary>
    /// Enhanced prefab creator that uses visual builders for detailed hockey player and goalie models.
    /// Creates production-ready prefabs with proper visuals, physics, and component setup.
    /// </summary>
    public class EnhancedPrefabCreator : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material jerseyMat;
        [SerializeField] private Material blackMat;
        [SerializeField] private Material skinMat;
        [SerializeField] private Material padsMat;
        [SerializeField] private Material iceMat;
        [SerializeField] private Material goalMetalMat;

        [Header("Physics Materials")]
        [SerializeField] private PhysicsMaterial icePhysicMat;
        [SerializeField] private PhysicsMaterial puckPhysicMat;
        [SerializeField] private PhysicsMaterial wallPhysicMat;

        [Header("Prefab Creation")]
        [SerializeField] private bool createDetailedVisuals = true;
        [SerializeField] private bool createPhysicsObjects = true;
        [SerializeField] private string prefabOutputPath = "Assets/Prefabs/";

        /// <summary>
        /// Create all enhanced prefabs with detailed visuals.
        /// </summary>
        [ContextMenu("Create All Enhanced Prefabs")]
        public void CreateAllEnhancedPrefabs()
        {
            Debug.Log("Creating enhanced prefabs with detailed visuals...");

            // Create materials if not assigned
            CreateDefaultMaterials();

            // Create physics materials if not assigned
            CreateDefaultPhysicsMaterials();

            // Create enhanced prefabs
            CreateEnhancedPlayerPrefab();
            CreateEnhancedGoaliePrefab();
            CreateEnhancedPuckPrefab();
            CreateEnhancedGoalPrefab();
            CreateEnhancedRinkPrefab();

            Debug.Log("All enhanced prefabs created successfully!");
        }

        /// <summary>
        /// Create enhanced player prefab with detailed visuals.
        /// </summary>
        private void CreateEnhancedPlayerPrefab()
        {
            Debug.Log("Creating enhanced Player prefab...");

            // Create root player object
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            player.layer = LayerMask.NameToLayer("Player");

            // Add physics components
            if (createPhysicsObjects)
            {
                Rigidbody rb = player.AddComponent<Rigidbody>();
                rb.mass = 80f;
                rb.linearDamping = 0f;
                rb.angularDamping = 0.05f;
                rb.useGravity = true;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                CapsuleCollider collider = player.AddComponent<CapsuleCollider>();
                collider.center = new Vector3(0, 0.9f, 0);
                collider.radius = 0.35f;
                collider.height = 1.8f;
                collider.material = icePhysicMat;
            }

            // Add player controller
            player.AddComponent<NeuralRink.Gameplay.PlayerController>();

            // Add visual builder
            var visualBuilder = player.AddComponent<PrimitiveSkaterBuilder>();
            if (createDetailedVisuals)
            {
                visualBuilder.jerseyMat = jerseyMat;
                visualBuilder.blackMat = blackMat;
                visualBuilder.skinMat = skinMat;
            }

            // Add visual follower for training optimization
            var visualFollower = player.AddComponent<NeuralRink.Utils.VisualFollower>();
            visualFollower.SetTarget(player.transform);
            visualFollower.SetFollowingOptions(true, false, false);

            // Save as prefab
            string prefabPath = prefabOutputPath + "Player_Enhanced.prefab";
            PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
            DestroyImmediate(player);

            Debug.Log($"Enhanced Player prefab saved to: {prefabPath}");
        }

        /// <summary>
        /// Create enhanced goalie prefab with detailed visuals.
        /// </summary>
        private void CreateEnhancedGoaliePrefab()
        {
            Debug.Log("Creating enhanced Goalie prefab...");

            // Create root goalie object
            GameObject goalie = new GameObject("Goalie");
            goalie.tag = "Goalie";
            goalie.layer = LayerMask.NameToLayer("Goalie");

            // Add physics components
            if (createPhysicsObjects)
            {
                Rigidbody rb = goalie.AddComponent<Rigidbody>();
                rb.mass = 90f;
                rb.linearDamping = 0f;
                rb.angularDamping = 0.05f;
                rb.useGravity = true;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                BoxCollider collider = goalie.AddComponent<BoxCollider>();
                collider.size = new Vector3(1.2f, 1.8f, 0.8f);
                collider.center = new Vector3(0, 0.9f, 0);
                collider.material = icePhysicMat;
            }

            // Add goalie agent
            goalie.AddComponent<NeuralRink.Agents.GoalieAgent>();

            // Add visual builder
            var visualBuilder = goalie.AddComponent<PrimitiveGoalieBuilder>();
            if (createDetailedVisuals)
            {
                visualBuilder.jerseyMat = jerseyMat;
                visualBuilder.padsMat = padsMat;
                visualBuilder.blackMat = blackMat;
            }

            // Add visual follower for training optimization
            var visualFollower = goalie.AddComponent<NeuralRink.Utils.VisualFollower>();
            visualFollower.SetTarget(goalie.transform);
            visualFollower.SetFollowingOptions(true, false, false);

            // Save as prefab
            string prefabPath = prefabOutputPath + "Goalie_Enhanced.prefab";
            PrefabUtility.SaveAsPrefabAsset(goalie, prefabPath);
            DestroyImmediate(goalie);

            Debug.Log($"Enhanced Goalie prefab saved to: {prefabPath}");
        }

        /// <summary>
        /// Create enhanced puck prefab.
        /// </summary>
        private void CreateEnhancedPuckPrefab()
        {
            Debug.Log("Creating enhanced Puck prefab...");

            // Create puck object
            GameObject puck = new GameObject("Puck");
            puck.tag = "Puck";
            puck.layer = LayerMask.NameToLayer("Puck");

            // Add physics components
            if (createPhysicsObjects)
            {
                Rigidbody rb = puck.AddComponent<Rigidbody>();
                rb.mass = 0.17f; // Standard hockey puck mass
                rb.linearDamping = 0.05f;
                rb.angularDamping = 0.05f;
                rb.useGravity = true;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                SphereCollider collider = puck.AddComponent<SphereCollider>();
                collider.radius = 0.15f;
                collider.material = puckPhysicMat;
                collider.isTrigger = true;
            }

            // Add visual representation
            var visual = PV.MakeVisualOnly(puck.transform, "PuckVisual", PrimitiveType.Cylinder, 
                Vector3.zero, new Vector3(0.3f, 0.1f, 0.3f), blackMat);

            // Add puck controller
            puck.AddComponent<NeuralRink.Gameplay.PuckController>();

            // Save as prefab
            string prefabPath = prefabOutputPath + "Puck_Enhanced.prefab";
            PrefabUtility.SaveAsPrefabAsset(puck, prefabPath);
            DestroyImmediate(puck);

            Debug.Log($"Enhanced Puck prefab saved to: {prefabPath}");
        }

        /// <summary>
        /// Create enhanced goal prefab.
        /// </summary>
        private void CreateEnhancedGoalPrefab()
        {
            Debug.Log("Creating enhanced Goal prefab...");

            // Create root goal object
            GameObject goal = new GameObject("Goal");
            goal.tag = "Goal";
            goal.layer = LayerMask.NameToLayer("Rink");

            // Create goal frame
            var frame = PV.Ensure(goal.transform, "Frame");
            
            // Left post
            PV.MakeWithPhysicsMaterial(frame, "LeftPost", PrimitiveType.Cylinder,
                new Vector3(0, 1f, -1.5f), new Vector3(0.1f, 2f, 0.1f), wallPhysicMat, goalMetalMat);

            // Right post
            PV.MakeWithPhysicsMaterial(frame, "RightPost", PrimitiveType.Cylinder,
                new Vector3(0, 1f, 1.5f), new Vector3(0.1f, 2f, 0.1f), wallPhysicMat, goalMetalMat);

            // Crossbar
            PV.MakeWithPhysicsMaterial(frame, "Crossbar", PrimitiveType.Cylinder,
                new Vector3(0, 2f, 0), new Vector3(3f, 0.1f, 0.1f), wallPhysicMat, goalMetalMat, Quaternion.Euler(0, 0, 90));

            // Goal trigger
            var goalTrigger = PV.CreateBoxTrigger(goal.transform, "GoalTrigger", 
                Vector3.zero, new Vector3(0.5f, 2f, 4f), "Goal");
            
            // Add trigger relay
            var triggerRelay = goalTrigger.AddComponent<NeuralRink.Gameplay.TriggerRelay>();
            triggerRelay.SetTriggerType(NeuralRink.Gameplay.TriggerRelay.TriggerType.Goal);

            // Save as prefab
            string prefabPath = prefabOutputPath + "Goal_Enhanced.prefab";
            PrefabUtility.SaveAsPrefabAsset(goal, prefabPath);
            DestroyImmediate(goal);

            Debug.Log($"Enhanced Goal prefab saved to: {prefabPath}");
        }

        /// <summary>
        /// Create enhanced rink prefab.
        /// </summary>
        private void CreateEnhancedRinkPrefab()
        {
            Debug.Log("Creating enhanced Rink prefab...");

            // Create root rink object
            GameObject rink = new GameObject("Rink");
            rink.layer = LayerMask.NameToLayer("Rink");

            // Create ice surface
            var ice = PV.MakeWithPhysicsMaterial(rink.transform, "Ice", PrimitiveType.Plane,
                Vector3.zero, new Vector3(10f, 1f, 20f), icePhysicMat, iceMat);

            // Create rink boards
            CreateRinkBoards(rink.transform);

            // Create spawn points
            CreateSpawnPoints(rink.transform);

            // Create out of play trigger
            var outOfPlay = PV.CreateBoxTrigger(rink.transform, "OutOfPlay",
                Vector3.zero, new Vector3(25f, 3f, 15f), "OutOfPlay");
            
            var outOfPlayRelay = outOfPlay.AddComponent<NeuralRink.Gameplay.TriggerRelay>();
            outOfPlayRelay.SetTriggerType(NeuralRink.Gameplay.TriggerRelay.TriggerType.Miss);

            // Add rink setup component
            rink.AddComponent<NeuralRink.Gameplay.RinkSetup>();

            // Save as prefab
            string prefabPath = prefabOutputPath + "Rink_Enhanced.prefab";
            PrefabUtility.SaveAsPrefabAsset(rink, prefabPath);
            DestroyImmediate(rink);

            Debug.Log($"Enhanced Rink prefab saved to: {prefabPath}");
        }

        /// <summary>
        /// Create rink boards around the ice surface.
        /// </summary>
        private void CreateRinkBoards(Transform rink)
        {
            float boardHeight = 1.2f;
            float boardThickness = 0.2f;

            // North board
            PV.MakeWithPhysicsMaterial(rink, "Board_North", PrimitiveType.Cube,
                new Vector3(0, boardHeight/2, 10.1f), new Vector3(20.4f, boardHeight, boardThickness), wallPhysicMat, blackMat);

            // South board
            PV.MakeWithPhysicsMaterial(rink, "Board_South", PrimitiveType.Cube,
                new Vector3(0, boardHeight/2, -10.1f), new Vector3(20.4f, boardHeight, boardThickness), wallPhysicMat, blackMat);

            // East board
            PV.MakeWithPhysicsMaterial(rink, "Board_East", PrimitiveType.Cube,
                new Vector3(10.1f, boardHeight/2, 0), new Vector3(boardThickness, boardHeight, 20f), wallPhysicMat, blackMat);

            // West board
            PV.MakeWithPhysicsMaterial(rink, "Board_West", PrimitiveType.Cube,
                new Vector3(-10.1f, boardHeight/2, 0), new Vector3(boardThickness, boardHeight, 20f), wallPhysicMat, blackMat);
        }

        /// <summary>
        /// Create spawn points for players and puck.
        /// </summary>
        private void CreateSpawnPoints(Transform rink)
        {
            var spawns = PV.Ensure(rink, "Spawns");

            // Player spawn
            var playerSpawn = PV.Ensure(spawns, "PlayerSpawn");
            playerSpawn.localPosition = new Vector3(-8f, 0.1f, 0f);

            // Puck spawn
            var puckSpawn = PV.Ensure(spawns, "PuckSpawn");
            puckSpawn.localPosition = new Vector3(-6f, 0.1f, 0f);

            // Goalie spawn
            var goalieSpawn = PV.Ensure(spawns, "GoalieSpawn");
            goalieSpawn.localPosition = new Vector3(8f, 0.1f, 0f);
        }

        /// <summary>
        /// Create default materials if not assigned.
        /// </summary>
        private void CreateDefaultMaterials()
        {
            if (jerseyMat == null)
                jerseyMat = PV.GetDefaultMaterial("Mat_Jersey", new Color(0.2f, 0.4f, 1f));
            
            if (blackMat == null)
                blackMat = PV.GetDefaultMaterial("Mat_Black", Color.black);
            
            if (skinMat == null)
                skinMat = PV.GetDefaultMaterial("Mat_Skin", new Color(1f, 0.8f, 0.6f));
            
            if (padsMat == null)
                padsMat = PV.GetDefaultMaterial("Mat_Pads", new Color(0.8f, 0.8f, 0.8f));
            
            if (iceMat == null)
                iceMat = PV.GetDefaultMaterial("Mat_Ice", new Color(0.8f, 0.9f, 1f));
            
            if (goalMetalMat == null)
                goalMetalMat = PV.GetDefaultMaterial("Mat_GoalMetal", new Color(0.3f, 0.3f, 0.3f));
        }

        /// <summary>
        /// Create default physics materials if not assigned.
        /// </summary>
        private void CreateDefaultPhysicsMaterials()
        {
            if (icePhysicMat == null)
            {
                icePhysicMat = new PhysicsMaterial("Ice");
                icePhysicMat.dynamicFriction = 0.02f;
                icePhysicMat.staticFriction = 0.02f;
                icePhysicMat.bounciness = 0f;
                icePhysicMat.frictionCombine = PhysicsMaterialCombine.Minimum;
                icePhysicMat.bounceCombine = PhysicsMaterialCombine.Minimum;
            }

            if (puckPhysicMat == null)
            {
                puckPhysicMat = new PhysicsMaterial("Puck");
                puckPhysicMat.dynamicFriction = 0.01f;
                puckPhysicMat.staticFriction = 0.01f;
                puckPhysicMat.bounciness = 0.05f;
                puckPhysicMat.frictionCombine = PhysicsMaterialCombine.Minimum;
                puckPhysicMat.bounceCombine = PhysicsMaterialCombine.Minimum;
            }

            if (wallPhysicMat == null)
            {
                wallPhysicMat = new PhysicsMaterial("Wall");
                wallPhysicMat.dynamicFriction = 0.8f;
                wallPhysicMat.staticFriction = 0.8f;
                wallPhysicMat.bounciness = 0f;
                wallPhysicMat.frictionCombine = PhysicsMaterialCombine.Maximum;
                wallPhysicMat.bounceCombine = PhysicsMaterialCombine.Maximum;
            }
        }

        /// <summary>
        /// Create a complete scene setup with all enhanced prefabs.
        /// </summary>
        [ContextMenu("Create Complete Scene Setup")]
        public void CreateCompleteSceneSetup()
        {
            Debug.Log("Creating complete scene setup...");

            // Create all prefabs first
            CreateAllEnhancedPrefabs();

            // Create scene objects
            CreateSceneObjects();

            Debug.Log("Complete scene setup created!");
        }

        /// <summary>
        /// Create scene objects for testing.
        /// </summary>
        private void CreateSceneObjects()
        {
            // Create main camera
            var camera = PV.CreateCamera(transform, "Main Camera", 
                new Vector3(0, 15f, -12f), Quaternion.Euler(45f, 0f, 0f), 60f);

            // Create directional light
            var light = PV.CreateDirectionalLight(transform, "Directional Light",
                Vector3.zero, Quaternion.Euler(45f, 45f, 0f), Color.white, 1f);

            Debug.Log("Scene objects created for testing");
        }
    }
}
