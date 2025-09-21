using UnityEngine;
using NeuralRink.Systems;
using NeuralRink.Agents;
using NeuralRink.Utils;

namespace NeuralRink.Gameplay
{
    /// <summary>
    /// Central game director that coordinates all scene events and systems.
    /// Handles save/goal/miss events and coordinates between all game systems.
    /// </summary>
    public class GameDirector : MonoBehaviour
    {
        [Header("Core References")]
        [SerializeField] private GoalieAgent agent;
        [SerializeField] private SalarySystem salary;
        [SerializeField] private TrainingSwitch training;
        
        [Header("UI References (Play Scene Only)")]
        [SerializeField] private EventFeed eventFeed;
        [SerializeField] private SalaryHUD salaryHUD;
        [SerializeField] private PopupSpawner popups;
        
        [Header("Visual References")]
        [SerializeField] private Transform goalieVisualAnchor;
        [SerializeField] private Transform goalCenter;
        [SerializeField] private Camera mainCam;
        
        [Header("Event Colors")]
        [SerializeField] private Color goalColor = Color.red;
        [SerializeField] private Color saveColor = Color.green;
        [SerializeField] private Color missColor = new Color(1f, 0.75f, 0f); // Amber
        
        [Header("Reward Values")]
        [SerializeField] private float goalPenalty = -200f;
        [SerializeField] private float saveReward = 200f;
        [SerializeField] private float missReward = 50f;
        
        private bool isEpisodeActive = false;
        
        /// <summary>
        /// Initialize game director and start first episode.
        /// </summary>
        private void Start()
        {
            // Validate references
            ValidateReferences();
            
            // Start first episode
            OnEpisodeBegin();
        }
        
        /// <summary>
        /// Validate that all required references are assigned.
        /// </summary>
        private void ValidateReferences()
        {
            if (agent == null)
            {
                Debug.LogError("GameDirector: GoalieAgent reference not assigned!");
            }
            
            if (salary == null)
            {
                Debug.LogError("GameDirector: SalarySystem reference not assigned!");
            }
            
            if (training == null)
            {
                Debug.LogError("GameDirector: TrainingSwitch reference not assigned!");
            }
            
            if (goalCenter == null)
            {
                Debug.LogWarning("GameDirector: GoalCenter reference not assigned - popup positioning may not work correctly");
            }
            
            if (mainCam == null)
            {
                mainCam = Camera.main;
            }
        }
        
        /// <summary>
        /// Start a new episode.
        /// </summary>
        public void OnEpisodeBegin()
        {
            if (isEpisodeActive) return;
            
            isEpisodeActive = true;
            
            // Initialize salary system for new episode
            salary?.OnEpisodeBegin();
            
            // Start training episode if in training mode
            if (training != null && training.IsTrainingMode())
            {
                training.StartEpisode();
            }
            
            Debug.Log("Episode started");
        }
        
        /// <summary>
        /// End the current episode.
        /// </summary>
        public void OnEpisodeEnd()
        {
            if (!isEpisodeActive) return;
            
            isEpisodeActive = false;
            
            // End salary system episode
            salary?.OnEpisodeEnd();
            
            // End training episode if in training mode
            if (training != null && training.IsTrainingMode())
            {
                training.EndEpisode();
            }
            
            Debug.Log("Episode ended");
        }
        
        /// <summary>
        /// Handle goal scored event.
        /// </summary>
        public void OnGoal()
        {
            if (!isEpisodeActive) return;
            
            Debug.Log("GOAL SCORED!");
            
            // Process salary penalty
            salary?.ProcessGoalConceded();
            
            // Show UI feedback (Play scene only)
            if (!IsTrainingMode())
            {
                ShowGoalFeedback();
            }
            
            // Notify goalie agent
            agent?.OnGoalConceded();
            
            // End current episode and start new one
            OnEpisodeEnd();
            Invoke(nameof(OnEpisodeBegin), 1f); // Brief delay before next episode
        }
        
        /// <summary>
        /// Handle save event.
        /// </summary>
        public void OnSave()
        {
            if (!isEpisodeActive) return;
            
            Debug.Log("SAVE!");
            
            // Process salary reward
            salary?.ProcessSave();
            
            // Show UI feedback (Play scene only)
            if (!IsTrainingMode())
            {
                ShowSaveFeedback();
            }
            
            // Notify goalie agent
            agent?.OnSave();
            
            // End current episode and start new one
            OnEpisodeEnd();
            Invoke(nameof(OnEpisodeBegin), 1f); // Brief delay before next episode
        }
        
        /// <summary>
        /// Handle miss event (puck goes out of play).
        /// </summary>
        public void OnMiss()
        {
            if (!isEpisodeActive) return;
            
            Debug.Log("MISS - Puck out of play");
            
            // Process salary reward (smaller than save)
            salary?.ProcessMiss();
            
            // Show UI feedback (Play scene only)
            if (!IsTrainingMode())
            {
                ShowMissFeedback();
            }
            
            // End episode for miss
            agent?.EndEpisode();
            
            // End current episode and start new one
            OnEpisodeEnd();
            Invoke(nameof(OnEpisodeBegin), 1f); // Brief delay before next episode
        }
        
        /// <summary>
        /// Show goal feedback in Play scene.
        /// </summary>
        private void ShowGoalFeedback()
        {
            string message = $"GOAL -${Mathf.Abs(goalPenalty)}";
            
            // Update event feed
            eventFeed?.Push(message, goalColor);
            
            // Pulse salary HUD
            salaryHUD?.PulseEpisode(goalColor);
            
            // Show popup at goal center
            if (popups != null && goalCenter != null && mainCam != null)
            {
                popups.Spawn(message, goalColor, goalCenter.position, mainCam);
            }
        }
        
        /// <summary>
        /// Show save feedback in Play scene.
        /// </summary>
        private void ShowSaveFeedback()
        {
            string message = $"SAVE +${saveReward}";
            
            // Update event feed
            eventFeed?.Push(message, saveColor);
            
            // Pulse salary HUD
            salaryHUD?.PulseEpisode(saveColor);
            
            // Show popup at goalie position
            if (popups != null && goalieVisualAnchor != null && mainCam != null)
            {
                popups.Spawn(message, saveColor, goalieVisualAnchor.position, mainCam);
            }
        }
        
        /// <summary>
        /// Show miss feedback in Play scene.
        /// </summary>
        private void ShowMissFeedback()
        {
            string message = $"MISS +${missReward}";
            
            // Update event feed
            eventFeed?.Push(message, missColor);
            
            // Pulse salary HUD
            salaryHUD?.PulseEpisode(missColor);
            
            // No popup for miss (puck is out of play)
        }
        
        /// <summary>
        /// Check if currently in training mode.
        /// </summary>
        private bool IsTrainingMode()
        {
            return training != null && training.IsTrainingMode();
        }
        
        /// <summary>
        /// Get current episode active state.
        /// </summary>
        public bool IsEpisodeActive()
        {
            return isEpisodeActive;
        }
        
        /// <summary>
        /// Force end current episode (for testing/debugging).
        /// </summary>
        public void ForceEndEpisode()
        {
            OnEpisodeEnd();
        }
        
        /// <summary>
        /// Set reward values for different events.
        /// </summary>
        public void SetRewardValues(float goalPenalty, float saveReward, float missReward)
        {
            this.goalPenalty = goalPenalty;
            this.saveReward = saveReward;
            this.missReward = missReward;
        }
        
        /// <summary>
        /// Get current reward values.
        /// </summary>
        public (float goalPenalty, float saveReward, float missReward) GetRewardValues()
        {
            return (goalPenalty, saveReward, missReward);
        }
    }
}
