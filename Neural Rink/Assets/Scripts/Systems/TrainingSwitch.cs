using UnityEngine;

namespace NeuralRink.Systems
{
    /// <summary>
    /// ScriptableObject for managing training vs play mode configuration.
    /// Provides centralized control over game behavior for ML training scenarios.
    /// </summary>
    [CreateAssetMenu(fileName = "TrainingSwitch", menuName = "Neural Rink/Training Switch")]
    public class TrainingSwitch : ScriptableObject
    {
        [Header("Training Configuration")]
        [SerializeField] private bool isTrainingMode = true;
        [SerializeField] private float episodeLength = 30f;
        [SerializeField] private int maxEpisodes = 1000;
        [SerializeField] private bool randomizePhysics = false;
        [SerializeField] private float physicsRandomizationRange = 0.1f;
        
        [Header("Play Mode Configuration")]
        [SerializeField] private bool showUI = true;
        [SerializeField] private bool enableSound = true;
        [SerializeField] private bool enableEffects = true;
        [SerializeField] private float timeScale = 1f;
        
        [Header("Debug Configuration")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool showTrainingMetrics = false;
        
        // Runtime state
        private int currentEpisode = 0;
        private float currentEpisodeTime = 0f;
        private bool isEpisodeActive = false;
        
        /// <summary>
        /// Check if currently in training mode.
        /// </summary>
        public bool IsTrainingMode()
        {
            return isTrainingMode;
        }
        
        /// <summary>
        /// Set training mode state.
        /// </summary>
        public void SetTrainingMode(bool training)
        {
            isTrainingMode = training;
            OnModeChanged();
        }
        
        /// <summary>
        /// Get current episode number.
        /// </summary>
        public int GetCurrentEpisode()
        {
            return currentEpisode;
        }
        
        /// <summary>
        /// Get current episode elapsed time.
        /// </summary>
        public float GetCurrentEpisodeTime()
        {
            return currentEpisodeTime;
        }
        
        /// <summary>
        /// Get maximum episode length.
        /// </summary>
        public float GetEpisodeLength()
        {
            return episodeLength;
        }
        
        /// <summary>
        /// Get maximum number of episodes.
        /// </summary>
        public int GetMaxEpisodes()
        {
            return maxEpisodes;
        }
        
        /// <summary>
        /// Check if episode should be randomized.
        /// </summary>
        public bool ShouldRandomizePhysics()
        {
            return randomizePhysics && isTrainingMode;
        }
        
        /// <summary>
        /// Get physics randomization range.
        /// </summary>
        public float GetPhysicsRandomizationRange()
        {
            return physicsRandomizationRange;
        }
        
        /// <summary>
        /// Check if UI should be shown.
        /// </summary>
        public bool ShouldShowUI()
        {
            return showUI && !isTrainingMode;
        }
        
        /// <summary>
        /// Check if sound should be enabled.
        /// </summary>
        public bool ShouldEnableSound()
        {
            return enableSound && !isTrainingMode;
        }
        
        /// <summary>
        /// Check if effects should be enabled.
        /// </summary>
        public bool ShouldEnableEffects()
        {
            return enableEffects && !isTrainingMode;
        }
        
        /// <summary>
        /// Get current time scale.
        /// </summary>
        public float GetTimeScale()
        {
            return timeScale;
        }
        
        /// <summary>
        /// Check if debug logs should be enabled.
        /// </summary>
        public bool ShouldEnableDebugLogs()
        {
            return enableDebugLogs;
        }
        
        /// <summary>
        /// Check if training metrics should be shown.
        /// </summary>
        public bool ShouldShowTrainingMetrics()
        {
            return showTrainingMetrics && isTrainingMode;
        }
        
        /// <summary>
        /// Start a new training episode.
        /// </summary>
        public void StartEpisode()
        {
            if (isTrainingMode)
            {
                currentEpisode++;
                currentEpisodeTime = 0f;
                isEpisodeActive = true;
                
                if (ShouldEnableDebugLogs())
                {
                    Debug.Log($"Starting episode {currentEpisode}/{maxEpisodes}");
                }
            }
        }
        
        /// <summary>
        /// End the current training episode.
        /// </summary>
        public void EndEpisode()
        {
            if (isTrainingMode && isEpisodeActive)
            {
                isEpisodeActive = false;
                
                if (ShouldEnableDebugLogs())
                {
                    Debug.Log($"Ended episode {currentEpisode} at time {currentEpisodeTime:F2}s");
                }
                
                // Check if we've reached max episodes
                if (currentEpisode >= maxEpisodes)
                {
                    if (ShouldEnableDebugLogs())
                    {
                        Debug.Log("Training completed - max episodes reached");
                    }
                }
            }
        }
        
        /// <summary>
        /// Update episode timing.
        /// </summary>
        public void UpdateEpisodeTime(float deltaTime)
        {
            if (isTrainingMode && isEpisodeActive)
            {
                currentEpisodeTime += deltaTime;
                
                // Auto-end episode if time limit reached
                if (currentEpisodeTime >= episodeLength)
                {
                    EndEpisode();
                }
            }
        }
        
        /// <summary>
        /// Check if current episode is active.
        /// </summary>
        public bool IsEpisodeActive()
        {
            return isEpisodeActive && isTrainingMode;
        }
        
        /// <summary>
        /// Reset training state.
        /// </summary>
        public void ResetTrainingState()
        {
            currentEpisode = 0;
            currentEpisodeTime = 0f;
            isEpisodeActive = false;
        }
        
        /// <summary>
        /// Get randomized physics value within range.
        /// </summary>
        public float GetRandomizedPhysicsValue(float baseValue)
        {
            if (!ShouldRandomizePhysics())
            {
                return baseValue;
            }
            
            float randomFactor = Random.Range(1f - physicsRandomizationRange, 1f + physicsRandomizationRange);
            return baseValue * randomFactor;
        }
        
        /// <summary>
        /// Called when training mode is changed.
        /// </summary>
        private void OnModeChanged()
        {
            // Update Unity time scale based on mode
            Time.timeScale = isTrainingMode ? 1f : timeScale;
            
            // Reset episode state when switching modes
            if (!isTrainingMode)
            {
                ResetTrainingState();
            }
            
            if (ShouldEnableDebugLogs())
            {
                Debug.Log($"Training mode changed to: {(isTrainingMode ? "Training" : "Play")}");
            }
        }
        
        /// <summary>
        /// Validate configuration on script reload.
        /// </summary>
        private void OnValidate()
        {
            // Ensure positive values
            episodeLength = Mathf.Max(1f, episodeLength);
            maxEpisodes = Mathf.Max(1, maxEpisodes);
            physicsRandomizationRange = Mathf.Clamp01(physicsRandomizationRange);
            timeScale = Mathf.Max(0.1f, timeScale);
        }
    }
}
