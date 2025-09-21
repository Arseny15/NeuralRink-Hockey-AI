using UnityEngine;
using NeuralRink.Systems;

namespace NeuralRink.Gameplay
{
    /// <summary>
    /// Manages rink setup, spawn points, and game state initialization.
    /// Handles both training and play mode configurations.
    /// </summary>
    public class RinkSetup : MonoBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private Transform puckSpawnPoint;
        [SerializeField] private Transform goalieSpawnPoint;
        [SerializeField] private Transform goalSpawnPoint;
        
        [Header("Game Objects")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject puckPrefab;
        [SerializeField] private GameObject goaliePrefab;
        [SerializeField] private GameObject goalPrefab;
        
        [Header("Rink Configuration")]
        [SerializeField] private Vector3 rinkCenter = Vector3.zero;
        [SerializeField] private Vector3 rinkSize = new Vector3(20f, 1f, 10f);
        [SerializeField] private Material iceMaterial;
        [SerializeField] private Material boardsMaterial;
        
        [Header("Training Configuration")]
        [SerializeField] private TrainingSwitch trainingSwitch;
        [SerializeField] private bool randomizeSpawnPoints = true;
        [SerializeField] private float spawnRandomizationRadius = 2f;
        
        private GameObject currentPlayer;
        private GameObject currentPuck;
        private GameObject currentGoalie;
        private GameObject currentGoal;
        
        /// <summary>
        /// Initialize rink setup and create game objects.
        /// </summary>
        private void Awake()
        {
            CreateRinkEnvironment();
            SpawnGameObjects();
        }
        
        /// <summary>
        /// Create the physical rink environment with ice and boards.
        /// </summary>
        private void CreateRinkEnvironment()
        {
            // Create ice surface
            GameObject iceSurface = GameObject.CreatePrimitive(PrimitiveType.Cube);
            iceSurface.name = "Ice Surface";
            iceSurface.transform.position = rinkCenter;
            iceSurface.transform.localScale = rinkSize;
            
            if (iceMaterial != null)
            {
                iceSurface.GetComponent<Renderer>().material = iceMaterial;
            }
            
            // Create rink boards
            CreateRinkBoards();
        }
        
        /// <summary>
        /// Create rink boards around the ice surface.
        /// </summary>
        private void CreateRinkBoards()
        {
            float boardHeight = 1.2f;
            float boardThickness = 0.2f;
            
            // Create boards for each side of the rink
            Vector3[] boardPositions = {
                new Vector3(0, boardHeight/2, rinkSize.z/2 + boardThickness/2), // North
                new Vector3(0, boardHeight/2, -rinkSize.z/2 - boardThickness/2), // South
                new Vector3(rinkSize.x/2 + boardThickness/2, boardHeight/2, 0), // East
                new Vector3(-rinkSize.x/2 - boardThickness/2, boardHeight/2, 0) // West
            };
            
            Vector3[] boardScales = {
                new Vector3(rinkSize.x + boardThickness*2, boardHeight, boardThickness), // North/South
                new Vector3(rinkSize.x + boardThickness*2, boardHeight, boardThickness),
                new Vector3(boardThickness, boardHeight, rinkSize.z), // East/West
                new Vector3(boardThickness, boardHeight, rinkSize.z)
            };
            
            for (int i = 0; i < boardPositions.Length; i++)
            {
                GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
                board.name = $"Rink Board {i + 1}";
                board.transform.position = rinkCenter + boardPositions[i];
                board.transform.localScale = boardScales[i];
                
                if (boardsMaterial != null)
                {
                    board.GetComponent<Renderer>().material = boardsMaterial;
                }
                
                // Add physics collider
                Collider boardCollider = board.GetComponent<Collider>();
                if (boardCollider == null)
                {
                    boardCollider = board.AddComponent<BoxCollider>();
                }
            }
        }
        
        /// <summary>
        /// Spawn all game objects (player, puck, goalie, goal).
        /// </summary>
        private void SpawnGameObjects()
        {
            // Spawn goal first (needed for other objects)
            SpawnGoal();
            
            // Spawn other objects
            SpawnPlayer();
            SpawnPuck();
            SpawnGoalie();
        }
        
        /// <summary>
        /// Spawn the goal object.
        /// </summary>
        private void SpawnGoal()
        {
            if (goalPrefab != null && goalSpawnPoint != null)
            {
                currentGoal = Instantiate(goalPrefab, goalSpawnPoint.position, goalSpawnPoint.rotation);
                currentGoal.name = "Goal";
            }
            else
            {
                // Create default goal if no prefab
                currentGoal = CreateDefaultGoal();
            }
        }
        
        /// <summary>
        /// Create a default goal object if no prefab is provided.
        /// </summary>
        private GameObject CreateDefaultGoal()
        {
            GameObject goal = new GameObject("Goal");
            goal.tag = "Goal";
            
            // Position goal at one end of rink
            Vector3 goalPosition = rinkCenter + new Vector3(rinkSize.x/2 - 1f, 0, 0);
            goal.transform.position = goalPosition;
            
            // Create goal posts
            CreateGoalPosts(goal);
            
            // Add goal trigger
            BoxCollider goalTrigger = goal.AddComponent<BoxCollider>();
            goalTrigger.isTrigger = true;
            goalTrigger.size = new Vector3(0.5f, 2f, 4f);
            goalTrigger.center = Vector3.zero;
            
            return goal;
        }
        
        /// <summary>
        /// Create goal posts for visual representation.
        /// </summary>
        private void CreateGoalPosts(GameObject goal)
        {
            // Left post
            GameObject leftPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leftPost.transform.SetParent(goal.transform);
            leftPost.transform.localPosition = new Vector3(0, 1, -1.5f);
            leftPost.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
            
            // Right post
            GameObject rightPost = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rightPost.transform.SetParent(goal.transform);
            rightPost.transform.localPosition = new Vector3(0, 1, 1.5f);
            rightPost.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
            
            // Crossbar
            GameObject crossbar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crossbar.transform.SetParent(goal.transform);
            crossbar.transform.localPosition = new Vector3(0, 2, 0);
            crossbar.transform.localScale = new Vector3(3f, 0.1f, 0.1f);
            crossbar.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        
        /// <summary>
        /// Spawn the player object.
        /// </summary>
        private void SpawnPlayer()
        {
            if (playerPrefab != null && playerSpawnPoint != null)
            {
                Vector3 spawnPos = GetRandomizedSpawnPoint(playerSpawnPoint.position);
                currentPlayer = Instantiate(playerPrefab, spawnPos, playerSpawnPoint.rotation);
                currentPlayer.name = "Player";
            }
        }
        
        /// <summary>
        /// Spawn the puck object.
        /// </summary>
        private void SpawnPuck()
        {
            if (puckPrefab != null && puckSpawnPoint != null)
            {
                Vector3 spawnPos = GetRandomizedSpawnPoint(puckSpawnPoint.position);
                currentPuck = Instantiate(puckPrefab, spawnPos, puckSpawnPoint.rotation);
                currentPuck.name = "Puck";
            }
        }
        
        /// <summary>
        /// Spawn the goalie object.
        /// </summary>
        private void SpawnGoalie()
        {
            if (goaliePrefab != null && goalieSpawnPoint != null)
            {
                Vector3 spawnPos = GetRandomizedSpawnPoint(goalieSpawnPoint.position);
                currentGoalie = Instantiate(goaliePrefab, spawnPos, goalieSpawnPoint.rotation);
                currentGoalie.name = "Goalie";
            }
        }
        
        /// <summary>
        /// Get randomized spawn point if randomization is enabled.
        /// </summary>
        private Vector3 GetRandomizedSpawnPoint(Vector3 basePosition)
        {
            if (!randomizeSpawnPoints || !IsTrainingMode())
            {
                return basePosition;
            }
            
            // Add random offset within radius
            Vector2 randomOffset = Random.insideUnitCircle * spawnRandomizationRadius;
            return basePosition + new Vector3(randomOffset.x, 0, randomOffset.y);
        }
        
        /// <summary>
        /// Check if currently in training mode.
        /// </summary>
        private bool IsTrainingMode()
        {
            return trainingSwitch != null && trainingSwitch.IsTrainingMode();
        }
        
        /// <summary>
        /// Reset all game objects to their spawn positions.
        /// </summary>
        public void ResetRink()
        {
            // Reset player
            if (currentPlayer != null && playerSpawnPoint != null)
            {
                Vector3 spawnPos = GetRandomizedSpawnPoint(playerSpawnPoint.position);
                currentPlayer.transform.position = spawnPos;
                currentPlayer.transform.rotation = playerSpawnPoint.rotation;
                
                var playerController = currentPlayer.GetComponent<PlayerController>();
                playerController?.ResetPlayer();
            }
            
            // Reset puck
            if (currentPuck != null && puckSpawnPoint != null)
            {
                Vector3 spawnPos = GetRandomizedSpawnPoint(puckSpawnPoint.position);
                var puckController = currentPuck.GetComponent<PuckController>();
                puckController?.ResetPuck(spawnPos);
            }
            
            // Reset goalie
            if (currentGoalie != null && goalieSpawnPoint != null)
            {
                Vector3 spawnPos = GetRandomizedSpawnPoint(goalieSpawnPoint.position);
                currentGoalie.transform.position = spawnPos;
                currentGoalie.transform.rotation = goalieSpawnPoint.rotation;
                
                var goalieAgent = currentGoalie.GetComponent<NeuralRink.Agents.GoalieAgent>();
                goalieAgent?.ResetPosition();
            }
        }
        
        /// <summary>
        /// Get current game objects for external access.
        /// </summary>
        public GameObject GetPlayer() => currentPlayer;
        public GameObject GetPuck() => currentPuck;
        public GameObject GetGoalie() => currentGoalie;
        public GameObject GetGoal() => currentGoal;
        
        /// <summary>
        /// Get rink center position.
        /// </summary>
        public Vector3 GetRinkCenter() => rinkCenter;
        
        /// <summary>
        /// Get rink dimensions.
        /// </summary>
        public Vector3 GetRinkSize() => rinkSize;
    }
}
