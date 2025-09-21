using UnityEngine;

namespace NeuralRink.Systems
{
    [CreateAssetMenu(fileName = "TrainingSwitch", menuName = "NeuralRink/TrainingSwitch", order = 0)]
    public sealed class TrainingSwitch : ScriptableObject
    {
        [Header("Global Training Flag")]
        public bool IsTraining = false;

        [Header("Optional Time Scale When Training")]
        [Min(1f)] public float TrainingTimeScale = 20f;

        [Header("Disable expensive VFX/SFX in training")]
        public bool DisableEffects = true;
        
        [Header("Episode Management")]
        [SerializeField] private int currentEpisode = 0;
        [SerializeField] private int maxEpisodes = 1000;
        
        [Header("Audio Settings")]
        [SerializeField] private bool shouldEnableSound = false;
        
        /// <summary>
        /// Check if currently in training mode
        /// </summary>
        public bool IsTrainingMode()
        {
            return IsTraining;
        }
        
        /// <summary>
        /// Check if debug logs should be enabled (for compatibility with existing code)
        /// </summary>
        public bool ShouldEnableDebugLogs()
        {
            return IsTraining;
        }
        
        /// <summary>
        /// Start a new training episode
        /// </summary>
        public void StartEpisode()
        {
            if (IsTraining)
            {
                currentEpisode++;
                Debug.Log($"Started episode {currentEpisode}/{maxEpisodes}");
            }
        }
        
        /// <summary>
        /// End current training episode
        /// </summary>
        public void EndEpisode()
        {
            if (IsTraining)
            {
                Debug.Log($"Ended episode {currentEpisode}");
            }
        }
        
        /// <summary>
        /// Get current episode number
        /// </summary>
        public int GetCurrentEpisode()
        {
            return currentEpisode;
        }
        
        /// <summary>
        /// Get maximum episodes
        /// </summary>
        public int GetMaxEpisodes()
        {
            return maxEpisodes;
        }
        
        /// <summary>
        /// Get time scale for training
        /// </summary>
        public float GetTimeScale()
        {
            return TrainingTimeScale;
        }
        
        /// <summary>
        /// Check if sound should be enabled
        /// </summary>
        public bool ShouldEnableSound()
        {
            return shouldEnableSound;
        }
        
        /// <summary>
        /// Set training mode
        /// </summary>
        public void SetTrainingMode(bool training)
        {
            IsTraining = training;
        }
        
        /// <summary>
        /// Reset episode counter
        /// </summary>
        public void ResetEpisodes()
        {
            currentEpisode = 0;
        }
    }
}