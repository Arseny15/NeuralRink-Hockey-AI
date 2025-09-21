using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using NeuralRink.Systems;
using NeuralRink.Agents;

namespace NeuralRink.Training
{
    /// <summary>
    /// Training manager for coordinating ML-Agents training sessions.
    /// Handles environment setup, episode management, and training metrics.
    /// </summary>
    public class TrainingManager : MonoBehaviour
    {
        [Header("Training Configuration")]
        [SerializeField] private TrainingSwitch trainingSwitch;
        [SerializeField] private GoalieAgent goalieAgent;
        [SerializeField] private RinkSetup rinkSetup;
        [SerializeField] private SalarySystem salarySystem;
        
        [Header("Training Statistics")]
        [SerializeField] private int totalEpisodes = 0;
        [SerializeField] private int successfulSaves = 0;
        [SerializeField] private int goalsConceded = 0;
        [SerializeField] private float averageReward = 0f;
        [SerializeField] private float bestReward = float.MinValue;
        
        [Header("Training Metrics")]
        [SerializeField] private float trainingStartTime;
        [SerializeField] private float lastEpisodeTime;
        [SerializeField] private float averageEpisodeLength = 0f;
        [SerializeField] private float savePercentage = 0f;
        
        // Training state
        private bool isTrainingActive = false;
        private float totalReward = 0f;
        private int episodeCount = 0;
        
        /// <summary>
        /// Initialize training manager.
        /// </summary>
        private void Start()
        {
            InitializeTrainingManager();
        }
        
        /// <summary>
        /// Initialize training systems and setup.
        /// </summary>
        private void InitializeTrainingManager()
        {
            // Find required components
            FindRequiredComponents();
            
            // Setup training environment
            SetupTrainingEnvironment();
            
            // Start training if in training mode
            if (trainingSwitch != null && trainingSwitch.IsTrainingMode())
            {
                StartTraining();
            }
            
            Debug.Log("Training Manager initialized");
        }
        
        /// <summary>
        /// Find and cache required training components.
        /// </summary>
        private void FindRequiredComponents()
        {
            if (trainingSwitch == null)
            {
                trainingSwitch = FindObjectOfType<TrainingSwitch>();
            }
            
            if (goalieAgent == null)
            {
                goalieAgent = FindObjectOfType<GoalieAgent>();
            }
            
            if (rinkSetup == null)
            {
                rinkSetup = FindObjectOfType<RinkSetup>();
            }
            
            if (salarySystem == null)
            {
                salarySystem = FindObjectOfType<SalarySystem>();
            }
        }
        
        /// <summary>
        /// Setup training environment for optimal ML training.
        /// </summary>
        private void SetupTrainingEnvironment()
        {
            // Ensure deterministic physics
            Random.InitState(42); // Fixed seed for reproducible training
            
            // Set time scale for training
            Time.timeScale = 1f;
            
            // Disable unnecessary systems for performance
            DisableNonEssentialSystems();
            
            // Configure ML-Agents environment
            ConfigureMLAgentsEnvironment();
        }
        
        /// <summary>
        /// Disable non-essential systems during training for performance.
        /// </summary>
        private void DisableNonEssentialSystems()
        {
            // Disable audio during training
            AudioListener.volume = 0f;
            
            // Disable UI during training
            var hud = FindObjectOfType<NeuralRink.UI.GameHUD>();
            if (hud != null)
            {
                hud.gameObject.SetActive(false);
            }
            
            // Disable telemetry during training (optional)
            var telemetry = FindObjectOfType<NeuralRink.Utils.TelemetryLogger>();
            if (telemetry != null && !trainingSwitch.ShouldEnableDebugLogs())
            {
                telemetry.SetLoggingEnabled(false);
            }
        }
        
        /// <summary>
        /// Configure ML-Agents environment settings.
        /// </summary>
        private void ConfigureMLAgentsEnvironment()
        {
            // Set ML-Agents to training mode
            Academy.Instance.AutomaticSteppingEnabled = true;
            
            // Configure environment parameters
            var envParams = Academy.Instance.EnvironmentParameters;
            
            if (envParams != null)
            {
                // Set episode length
                float episodeLength = trainingSwitch?.GetEpisodeLength() ?? 30f;
                envParams.Set("episode_length", episodeLength);
                
                // Set reward parameters
                envParams.Set("save_bonus", 1.0f);
                envParams.Set("goal_penalty", -1.0f);
                envParams.Set("distance_penalty", -0.01f);
                envParams.Set("time_penalty", -0.001f);
            }
        }
        
        /// <summary>
        /// Start training session.
        /// </summary>
        public void StartTraining()
        {
            if (isTrainingActive) return;
            
            isTrainingActive = true;
            trainingStartTime = Time.time;
            totalEpisodes = 0;
            successfulSaves = 0;
            goalsConceded = 0;
            totalReward = 0f;
            episodeCount = 0;
            
            Debug.Log("Training session started");
        }
        
        /// <summary>
        /// Stop training session.
        /// </summary>
        public void StopTraining()
        {
            if (!isTrainingActive) return;
            
            isTrainingActive = false;
            
            float trainingDuration = Time.time - trainingStartTime;
            Debug.Log($"Training session stopped. Duration: {trainingDuration:F2}s, Episodes: {totalEpisodes}");
        }
        
        /// <summary>
        /// Handle episode start event.
        /// </summary>
        public void OnEpisodeStarted()
        {
            if (!isTrainingActive) return;
            
            totalEpisodes++;
            lastEpisodeTime = Time.time;
            
            if (trainingSwitch.ShouldEnableDebugLogs())
            {
                Debug.Log($"Episode {totalEpisodes} started");
            }
        }
        
        /// <summary>
        /// Handle episode end event.
        /// </summary>
        public void OnEpisodeEnded(float episodeReward)
        {
            if (!isTrainingActive) return;
            
            episodeCount++;
            totalReward += episodeReward;
            
            // Update statistics
            float episodeLength = Time.time - lastEpisodeTime;
            averageEpisodeLength = (averageEpisodeLength * (episodeCount - 1) + episodeLength) / episodeCount;
            averageReward = totalReward / episodeCount;
            
            // Track best performance
            if (episodeReward > bestReward)
            {
                bestReward = episodeReward;
            }
            
            // Update save percentage
            if (salarySystem != null)
            {
                var stats = salarySystem.GetPerformanceStats();
                savePercentage = stats.percentage;
                successfulSaves = stats.saves;
                goalsConceded = stats.goals;
            }
            
            if (trainingSwitch.ShouldEnableDebugLogs())
            {
                Debug.Log($"Episode {totalEpisodes} ended. Reward: {episodeReward:F2}, " +
                         $"Avg Reward: {averageReward:F2}, Save%: {savePercentage:F1}%");
            }
            
            // Check curriculum progression
            CheckCurriculumProgression();
        }
        
        /// <summary>
        /// Check if curriculum should advance to next lesson.
        /// </summary>
        private void CheckCurriculumProgression()
        {
            // Simple curriculum based on save percentage
            if (savePercentage >= 90f && totalEpisodes > 100)
            {
                // Advance to harder difficulty
                if (trainingSwitch != null)
                {
                    // Increase physics randomization or reduce rewards
                    Debug.Log("Curriculum: Advancing to harder difficulty");
                }
            }
        }
        
        /// <summary>
        /// Reset training statistics.
        /// </summary>
        public void ResetTrainingStats()
        {
            totalEpisodes = 0;
            successfulSaves = 0;
            goalsConceded = 0;
            averageReward = 0f;
            bestReward = float.MinValue;
            totalReward = 0f;
            episodeCount = 0;
            averageEpisodeLength = 0f;
            savePercentage = 0f;
            
            Debug.Log("Training statistics reset");
        }
        
        /// <summary>
        /// Get current training statistics.
        /// </summary>
        public (int episodes, int saves, int goals, float savePercent, float avgReward, float bestReward, float avgEpisodeLength) GetTrainingStats()
        {
            return (totalEpisodes, successfulSaves, goalsConceded, savePercentage, 
                   averageReward, bestReward, averageEpisodeLength);
        }
        
        /// <summary>
        /// Get training session duration.
        /// </summary>
        public float GetTrainingDuration()
        {
            return isTrainingActive ? Time.time - trainingStartTime : 0f;
        }
        
        /// <summary>
        /// Check if training is currently active.
        /// </summary>
        public bool IsTrainingActive()
        {
            return isTrainingActive;
        }
        
        /// <summary>
        /// Get formatted training report.
        /// </summary>
        public string GetTrainingReport()
        {
            float duration = GetTrainingDuration();
            return $"Training Report:\n" +
                   $"Duration: {duration:F1}s\n" +
                   $"Episodes: {totalEpisodes}\n" +
                   $"Saves: {successfulSaves}\n" +
                   $"Goals: {goalsConceded}\n" +
                   $"Save %: {savePercentage:F1}%\n" +
                   $"Avg Reward: {averageReward:F2}\n" +
                   $"Best Reward: {bestReward:F2}\n" +
                   $"Avg Episode Length: {averageEpisodeLength:F1}s";
        }
        
        /// <summary>
        /// Update training manager each frame.
        /// </summary>
        private void Update()
        {
            if (!isTrainingActive) return;
            
            // Update training switch episode timing
            if (trainingSwitch != null)
            {
                trainingSwitch.UpdateEpisodeTime(Time.deltaTime);
            }
            
            // Log periodic training updates
            if (Time.time - lastEpisodeTime > 30f && totalEpisodes > 0)
            {
                if (trainingSwitch.ShouldShowTrainingMetrics())
                {
                    Debug.Log($"Training Progress - Episodes: {totalEpisodes}, " +
                             $"Save%: {savePercentage:F1}%, Avg Reward: {averageReward:F2}");
                }
            }
        }
        
        /// <summary>
        /// Handle application quit to save training progress.
        /// </summary>
        private void OnApplicationQuit()
        {
            if (isTrainingActive)
            {
                Debug.Log("Saving training progress...");
                Debug.Log(GetTrainingReport());
                StopTraining();
            }
        }
    }
}
