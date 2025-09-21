using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NeuralRink.Utils
{
    /// <summary>
    /// Telemetry logging system for tracking game performance, training metrics, and player behavior.
    /// Provides comprehensive data collection for analysis and optimization.
    /// </summary>
    public class TelemetryLogger : MonoBehaviour
    {
        [Header("Logging Configuration")]
        [SerializeField] private bool enableLogging = true;
        [SerializeField] private string logDirectory = "TelemetryLogs";
        [SerializeField] private float logInterval = 0.1f; // Log every 100ms
        [SerializeField] private bool logToFile = true;
        [SerializeField] private bool logToConsole = false;
        
        [Header("Data Collection")]
        [SerializeField] private bool logPlayerData = true;
        [SerializeField] private bool logGoalieData = true;
        [SerializeField] private bool logPuckData = true;
        [SerializeField] private bool logPerformanceData = true;
        
        [Header("File Configuration")]
        [SerializeField] private string fileNamePrefix = "neural_rink_telemetry";
        [SerializeField] private bool includeTimestamp = true;
        [SerializeField] private bool compressLogs = false;
        
        // Data structures
        private TelemetryData currentData;
        private Queue<TelemetryData> dataQueue;
        private StringBuilder csvBuilder;
        private string csvHeader;
        
        // File handling
        private string currentLogFile;
        private StreamWriter logWriter;
        private float lastLogTime;
        private bool isLogging = false;
        
        // References
        private Transform playerTransform;
        private Transform goalieTransform;
        private Transform puckTransform;
        private Rigidbody playerRigidbody;
        private Rigidbody goalieRigidbody;
        private Rigidbody puckRigidbody;
        
        /// <summary>
        /// Telemetry data structure for comprehensive logging.
        /// </summary>
        [Serializable]
        public struct TelemetryData
        {
            public float timestamp;
            public float frameTime;
            public float fps;
            
            // Player data
            public Vector3 playerPosition;
            public Vector3 playerVelocity;
            public float playerSpeed;
            public bool playerCanShoot;
            
            // Goalie data
            public Vector3 goaliePosition;
            public Vector3 goalieVelocity;
            public float goalieSpeed;
            public float goalieToGoalDistance;
            public float goalieToPuckDistance;
            
            // Puck data
            public Vector3 puckPosition;
            public Vector3 puckVelocity;
            public float puckSpeed;
            public bool puckIsStationary;
            
            // Game state
            public int totalShots;
            public int totalSaves;
            public int totalGoals;
            public float savePercentage;
            public float currentSalary;
            
            // Performance metrics
            public float memoryUsage;
            public int drawCalls;
            public float physicsTime;
        }
        
        /// <summary>
        /// Initialize telemetry logging system.
        /// </summary>
        private void Start()
        {
            if (!enableLogging) return;
            
            InitializeTelemetrySystem();
            FindGameObjects();
            SetupLogging();
        }
        
        /// <summary>
        /// Initialize telemetry system components.
        /// </summary>
        private void InitializeTelemetrySystem()
        {
            dataQueue = new Queue<TelemetryData>();
            csvBuilder = new StringBuilder();
            currentData = new TelemetryData();
            
            // Create CSV header
            CreateCSVHeader();
            
            Debug.Log("Telemetry logging system initialized");
        }
        
        /// <summary>
        /// Find and cache references to game objects.
        /// </summary>
        private void FindGameObjects()
        {
            // Find player
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerRigidbody = player.GetComponent<Rigidbody>();
            }
            
            // Find goalie
            var goalie = GameObject.FindGameObjectWithTag("Goalie");
            if (goalie != null)
            {
                goalieTransform = goalie.transform;
                goalieRigidbody = goalie.GetComponent<Rigidbody>();
            }
            
            // Find puck
            var puck = GameObject.FindGameObjectWithTag("Puck");
            if (puck != null)
            {
                puckTransform = puck.transform;
                puckRigidbody = puck.GetComponent<Rigidbody>();
            }
        }
        
        /// <summary>
        /// Setup logging file and directory.
        /// </summary>
        private void SetupLogging()
        {
            if (!logToFile) return;
            
            try
            {
                // Create log directory
                string fullLogPath = Path.Combine(Application.dataPath, "..", logDirectory);
                if (!Directory.Exists(fullLogPath))
                {
                    Directory.CreateDirectory(fullLogPath);
                }
                
                // Generate filename
                string timestamp = includeTimestamp ? DateTime.Now.ToString("yyyyMMdd_HHmmss") : "";
                currentLogFile = Path.Combine(fullLogPath, $"{fileNamePrefix}_{timestamp}.csv");
                
                // Create log file
                logWriter = new StreamWriter(currentLogFile);
                logWriter.WriteLine(csvHeader);
                logWriter.Flush();
                
                isLogging = true;
                Debug.Log($"Telemetry logging to file: {currentLogFile}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to setup telemetry logging: {e.Message}");
                enableLogging = false;
            }
        }
        
        /// <summary>
        /// Create CSV header for telemetry data.
        /// </summary>
        private void CreateCSVHeader()
        {
            csvHeader = "timestamp,frameTime,fps," +
                       "playerPosX,playerPosY,playerPosZ,playerVelX,playerVelY,playerVelZ,playerSpeed,playerCanShoot," +
                       "goaliePosX,goaliePosY,goaliePosZ,goalieVelX,goalieVelY,goalieVelZ,goalieSpeed,goalieToGoalDist,goalieToPuckDist," +
                       "puckPosX,puckPosY,puckPosZ,puckVelX,puckVelY,puckVelZ,puckSpeed,puckStationary," +
                       "totalShots,totalSaves,totalGoals,savePercentage,currentSalary," +
                       "memoryUsage,drawCalls,physicsTime";
        }
        
        /// <summary>
        /// Update telemetry data collection.
        /// </summary>
        private void Update()
        {
            if (!enableLogging || !isLogging) return;
            
            // Collect data at specified intervals
            if (Time.time - lastLogTime >= logInterval)
            {
                CollectTelemetryData();
                ProcessTelemetryData();
                lastLogTime = Time.time;
            }
        }
        
        /// <summary>
        /// Collect current telemetry data from all sources.
        /// </summary>
        private void CollectTelemetryData()
        {
            // Basic timing data
            currentData.timestamp = Time.time;
            currentData.frameTime = Time.deltaTime;
            currentData.fps = 1f / Time.deltaTime;
            
            // Player data
            if (logPlayerData && playerTransform != null)
            {
                currentData.playerPosition = playerTransform.position;
                if (playerRigidbody != null)
                {
                    currentData.playerVelocity = playerRigidbody.linearVelocity;
                    currentData.playerSpeed = playerRigidbody.linearVelocity.magnitude;
                }
                
                var playerController = playerTransform.GetComponent<NeuralRink.Gameplay.PlayerController>();
                currentData.playerCanShoot = playerController?.CanShoot() ?? false;
            }
            
            // Goalie data
            if (logGoalieData && goalieTransform != null)
            {
                currentData.goaliePosition = goalieTransform.position;
                if (goalieRigidbody != null)
                {
                    currentData.goalieVelocity = goalieRigidbody.linearVelocity;
                    currentData.goalieSpeed = goalieRigidbody.linearVelocity.magnitude;
                }
                
                // Calculate distances
                var goal = GameObject.FindGameObjectWithTag("Goal");
                if (goal != null)
                {
                    currentData.goalieToGoalDistance = Vector3.Distance(goalieTransform.position, goal.transform.position);
                }
                
                if (puckTransform != null)
                {
                    currentData.goalieToPuckDistance = Vector3.Distance(goalieTransform.position, puckTransform.position);
                }
            }
            
            // Puck data
            if (logPuckData && puckTransform != null)
            {
                currentData.puckPosition = puckTransform.position;
                if (puckRigidbody != null)
                {
                    currentData.puckVelocity = puckRigidbody.linearVelocity;
                    currentData.puckSpeed = puckRigidbody.linearVelocity.magnitude;
                    currentData.puckIsStationary = puckRigidbody.linearVelocity.magnitude < 0.1f;
                }
            }
            
            // Game state data
            var salarySystem = FindFirstObjectByType<NeuralRink.Systems.SalarySystem>();
            if (salarySystem != null)
            {
                var stats = salarySystem.GetPerformanceStats();
                currentData.totalShots = stats.shots;
                currentData.totalSaves = stats.saves;
                currentData.totalGoals = stats.goals;
                currentData.savePercentage = stats.percentage;
                currentData.currentSalary = salarySystem.GetCurrentSalary();
            }
            
            // Performance data
            if (logPerformanceData)
            {
                currentData.memoryUsage = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f); // MB
                currentData.drawCalls = 0; // Draw calls not available through simple API in Unity 6
                currentData.physicsTime = Time.fixedDeltaTime; // Use fixed delta time as physics time approximation
            }
        }
        
        /// <summary>
        /// Process and log telemetry data.
        /// </summary>
        private void ProcessTelemetryData()
        {
            // Add to queue
            dataQueue.Enqueue(currentData);
            
            // Log to console if enabled
            if (logToConsole)
            {
                LogToConsole();
            }
            
            // Log to file if enabled
            if (logToFile && logWriter != null)
            {
                LogToFile();
            }
        }
        
        /// <summary>
        /// Log data to console.
        /// </summary>
        private void LogToConsole()
        {
            Debug.Log($"Telemetry: FPS={currentData.fps:F1}, " +
                     $"Player Speed={currentData.playerSpeed:F1}, " +
                     $"Goalie Speed={currentData.goalieSpeed:F1}, " +
                     $"Puck Speed={currentData.puckSpeed:F1}, " +
                     $"Save%={currentData.savePercentage:F1}%");
        }
        
        /// <summary>
        /// Log data to file in CSV format.
        /// </summary>
        private void LogToFile()
        {
            try
            {
                csvBuilder.Clear();
                csvBuilder.Append($"{currentData.timestamp:F3},{currentData.frameTime:F6},{currentData.fps:F1},");
                csvBuilder.Append($"{currentData.playerPosition.x:F3},{currentData.playerPosition.y:F3},{currentData.playerPosition.z:F3},");
                csvBuilder.Append($"{currentData.playerVelocity.x:F3},{currentData.playerVelocity.y:F3},{currentData.playerVelocity.z:F3},");
                csvBuilder.Append($"{currentData.playerSpeed:F3},{currentData.playerCanShoot},");
                csvBuilder.Append($"{currentData.goaliePosition.x:F3},{currentData.goaliePosition.y:F3},{currentData.goaliePosition.z:F3},");
                csvBuilder.Append($"{currentData.goalieVelocity.x:F3},{currentData.goalieVelocity.y:F3},{currentData.goalieVelocity.z:F3},");
                csvBuilder.Append($"{currentData.goalieSpeed:F3},{currentData.goalieToGoalDistance:F3},{currentData.goalieToPuckDistance:F3},");
                csvBuilder.Append($"{currentData.puckPosition.x:F3},{currentData.puckPosition.y:F3},{currentData.puckPosition.z:F3},");
                csvBuilder.Append($"{currentData.puckVelocity.x:F3},{currentData.puckVelocity.y:F3},{currentData.puckVelocity.z:F3},");
                csvBuilder.Append($"{currentData.puckSpeed:F3},{currentData.puckIsStationary},");
                csvBuilder.Append($"{currentData.totalShots},{currentData.totalSaves},{currentData.totalGoals},{currentData.savePercentage:F1},{currentData.currentSalary:F0},");
                csvBuilder.Append($"{currentData.memoryUsage:F1},{currentData.drawCalls},{currentData.physicsTime:F3}");
                
                logWriter.WriteLine(csvBuilder.ToString());
                logWriter.Flush();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write telemetry data: {e.Message}");
            }
        }
        
        /// <summary>
        /// Stop logging and cleanup resources.
        /// </summary>
        private void OnDestroy()
        {
            StopLogging();
        }
        
        /// <summary>
        /// Stop telemetry logging and cleanup.
        /// </summary>
        public void StopLogging()
        {
            if (logWriter != null)
            {
                logWriter.Close();
                logWriter = null;
            }
            
            isLogging = false;
            Debug.Log("Telemetry logging stopped");
        }
        
        /// <summary>
        /// Get current telemetry data for external access.
        /// </summary>
        public TelemetryData GetCurrentData()
        {
            return currentData;
        }
        
        /// <summary>
        /// Enable or disable telemetry logging.
        /// </summary>
        public void SetLoggingEnabled(bool enabled)
        {
            enableLogging = enabled;
            if (enabled && !isLogging)
            {
                SetupLogging();
            }
            else if (!enabled && isLogging)
            {
                StopLogging();
            }
        }
        
        /// <summary>
        /// Set logging interval.
        /// </summary>
        public void SetLogInterval(float interval)
        {
            logInterval = Mathf.Max(0.01f, interval);
        }
        
        /// <summary>
        /// Get logging statistics.
        /// </summary>
        public string GetLoggingStats()
        {
            return $"Logging: {(isLogging ? "Active" : "Inactive")}, " +
                   $"Queue Size: {dataQueue.Count}, " +
                   $"File: {currentLogFile}, " +
                   $"Interval: {logInterval:F2}s";
        }
    }
}
