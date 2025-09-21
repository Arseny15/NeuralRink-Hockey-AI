using UnityEngine;
using NeuralRink.Systems;
using NeuralRink.Agents;
using NeuralRink.Gameplay;

namespace NeuralRink.Training
{
    /// <summary>
    /// Smart training manager that works with or without ML-Agents.
    /// Automatically detects ML-Agents availability and adapts accordingly.
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
        [SerializeField] private float totalReward = 0f;
        
        [Header("Training Settings")]
        [SerializeField] private bool enableTraining = true;
        [SerializeField] private float episodeTimeout = 30f;
        
        // ML-Agents detection
        private bool mlAgentsAvailable = false;
        private object mlTrainingManager = null;
        
        /// <summary>
        /// Initialize the training manager.
        /// </summary>
        private void Start()
        {
            // Detect ML-Agents availability
            DetectMLAgents();
            
            if (mlAgentsAvailable)
            {
                Debug.Log("🤖 ML-Agents Training Manager initialized");
                InitializeMLTraining();
            }
            else
            {
                Debug.Log("🎮 Simple Training Manager initialized");
                InitializeSimpleTraining();
            }
        }
        
        /// <summary>
        /// Detect if ML-Agents is available.
        /// </summary>
        private void DetectMLAgents()
        {
            try
            {
                // Try to find ML-Agents components
                var trainingComponents = GetComponents<MonoBehaviour>();
                foreach (var component in trainingComponents)
                {
                    string componentName = component.GetType().Name;
                    if (componentName.Contains("ML") || componentName.Contains("Agent"))
                    {
                        mlAgentsAvailable = true;
                        mlTrainingManager = component;
                        break;
                    }
                }
            }
            catch
            {
                mlAgentsAvailable = false;
            }
        }
        
        /// <summary>
        /// Initialize ML-Agents training.
        /// </summary>
        private void InitializeMLTraining()
        {
            Debug.Log("🚀 ML-Agents training system ready");
            // ML-Agents will handle the training pipeline
        }
        
        /// <summary>
        /// Initialize simple training system.
        /// </summary>
        private void InitializeSimpleTraining()
        {
            Debug.Log("🎯 Simple training system ready");
            // Simple training tracking without ML-Agents
        }
        
        /// <summary>
        /// Start a new training episode.
        /// </summary>
        public void StartEpisode()
        {
            if (!enableTraining) return;
            
            totalEpisodes++;
            
            if (mlAgentsAvailable)
            {
                StartMLTrainingEpisode();
            }
            else
            {
                StartSimpleTrainingEpisode();
            }
        }
        
        /// <summary>
        /// Start ML-Agents training episode.
        /// </summary>
        private void StartMLTrainingEpisode()
        {
            // ML-Agents will handle episode management
            Debug.Log($"🤖 ML-Agents Episode {totalEpisodes} started");
        }
        
        /// <summary>
        /// Start simple training episode.
        /// </summary>
        private void StartSimpleTrainingEpisode()
        {
            Debug.Log($"🎮 Simple Episode {totalEpisodes} started");
            
            // Reset goalie
            if (goalieAgent != null)
            {
                goalieAgent.ResetGoalie();
            }
            
            // Start episode timeout
            StartCoroutine(EpisodeTimeoutCoroutine());
        }
        
        /// <summary>
        /// Handle episode timeout.
        /// </summary>
        private System.Collections.IEnumerator EpisodeTimeoutCoroutine()
        {
            yield return new WaitForSeconds(episodeTimeout);
            EndEpisode();
        }
        
        /// <summary>
        /// End current training episode.
        /// </summary>
        public void EndEpisode()
        {
            if (mlAgentsAvailable)
            {
                EndMLTrainingEpisode();
            }
            else
            {
                EndSimpleTrainingEpisode();
            }
        }
        
        /// <summary>
        /// End ML-Agents training episode.
        /// </summary>
        private void EndMLTrainingEpisode()
        {
            Debug.Log($"🤖 ML-Agents Episode {totalEpisodes} ended");
            // ML-Agents will handle episode ending
        }
        
        /// <summary>
        /// End simple training episode.
        /// </summary>
        private void EndSimpleTrainingEpisode()
        {
            Debug.Log($"🎮 Simple Episode {totalEpisodes} ended");
            
            // Log episode statistics
            float episodeReward = CalculateEpisodeReward();
            totalReward += episodeReward;
            
            Debug.Log($"Episode Reward: {episodeReward:F2}, Total: {totalReward:F2}");
            
            // Start next episode after delay
            Invoke(nameof(StartEpisode), 1f);
        }
        
        /// <summary>
        /// Calculate episode reward for simple training.
        /// </summary>
        private float CalculateEpisodeReward()
        {
            float reward = 0f;
            
            if (successfulSaves > 0)
            {
                reward += successfulSaves * 10f;
            }
            
            if (goalsConceded > 0)
            {
                reward -= goalsConceded * 5f;
            }
            
            return reward;
        }
        
        /// <summary>
        /// Record a successful save.
        /// </summary>
        public void RecordSave()
        {
            successfulSaves++;
            Debug.Log($"Save recorded! Total saves: {successfulSaves}");
        }
        
        /// <summary>
        /// Record a goal conceded.
        /// </summary>
        public void RecordGoal()
        {
            goalsConceded++;
            Debug.Log($"Goal conceded! Total goals: {goalsConceded}");
        }
        
        /// <summary>
        /// Get training statistics.
        /// </summary>
        public string GetTrainingStats()
        {
            float savePercentage = totalEpisodes > 0 ? (float)successfulSaves / totalEpisodes * 100f : 0f;
            float avgReward = totalEpisodes > 0 ? totalReward / totalEpisodes : 0f;
            
            return $"Episodes: {totalEpisodes}, Saves: {successfulSaves} ({savePercentage:F1}%), " +
                   $"Goals: {goalsConceded}, Avg Reward: {avgReward:F2}, AI Type: {GetAIType()}";
        }
        
        /// <summary>
        /// Get current AI type.
        /// </summary>
        public string GetAIType()
        {
            return mlAgentsAvailable ? "ML-Agents (Advanced)" : "Simple AI (Fallback)";
        }
        
        /// <summary>
        /// Check if ML-Agents is available.
        /// </summary>
        public bool IsMLAgentsAvailable()
        {
            return mlAgentsAvailable;
        }
        
        /// <summary>
        /// Reset training statistics.
        /// </summary>
        public void ResetStats()
        {
            totalEpisodes = 0;
            successfulSaves = 0;
            goalsConceded = 0;
            totalReward = 0f;
            Debug.Log("Training statistics reset");
        }
    }
}
