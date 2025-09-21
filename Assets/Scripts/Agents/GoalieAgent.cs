using UnityEngine;

namespace NeuralRink.Agents
{
    /// <summary>
    /// Smart goalie agent that automatically detects and uses ML-Agents if available,
    /// or falls back to simple AI if ML-Agents is not installed.
    /// This prevents compilation errors while providing the best available AI.
    /// </summary>
    public class GoalieAgent : MonoBehaviour
    {
        [Header("Goalie Configuration")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float maxMoveDistance = 2f;
        [SerializeField] private Transform goalTransform;
        [SerializeField] private Transform puckTransform;
        [SerializeField] private Transform playerTransform;
        
        [Header("Reward Configuration")]
        [SerializeField] private float saveReward = 1f;
        [SerializeField] private float goalPenalty = -1f;
        [SerializeField] private float distancePenalty = -0.01f;
        
        private Vector3 initialPosition;
        private Rigidbody goalieRigidbody;
        private bool hasSaved = false;
        private bool hasConceded = false;
        
        // ML-Agents detection
        private bool mlAgentsAvailable = false;
        private object mlAgent = null;
        
        /// <summary>
        /// Initialize the goalie agent with ML-Agents detection.
        /// </summary>
        private void Start()
        {
            initialPosition = transform.position;
            goalieRigidbody = GetComponent<Rigidbody>();
            
            if (goalieRigidbody == null)
            {
                goalieRigidbody = gameObject.AddComponent<Rigidbody>();
            }
            
            goalieRigidbody.freezeRotation = true;
            goalieRigidbody.useGravity = false;
            
            // Try to detect ML-Agents availability
            DetectMLAgents();
            
            if (mlAgentsAvailable)
            {
                Debug.Log("🤖 ML-Agents detected - Using advanced AI goalie");
            }
            else
            {
                Debug.Log("🎮 ML-Agents not available - Using simple AI goalie");
            }
        }
        
        /// <summary>
        /// Detect if ML-Agents is available without causing compilation errors.
        /// </summary>
        private void DetectMLAgents()
        {
            try
            {
                // Try to find ML-Agents components without importing
                var agentComponents = GetComponents<MonoBehaviour>();
                foreach (var component in agentComponents)
                {
                    string componentName = component.GetType().Name;
                    if (componentName.Contains("Agent") && componentName != "GoalieAgent")
                    {
                        mlAgentsAvailable = true;
                        mlAgent = component;
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
        /// Update goalie behavior based on available AI system.
        /// </summary>
        private void Update()
        {
            if (puckTransform != null && goalTransform != null)
            {
                if (mlAgentsAvailable)
                {
                    // Let ML-Agents handle the behavior
                    UpdateMLBehavior();
                }
                else
                {
                    // Use simple AI behavior
                    UpdateSimpleBehavior();
                }
            }
        }
        
        /// <summary>
        /// ML-Agents behavior (when available).
        /// </summary>
        private void UpdateMLBehavior()
        {
            // ML-Agents will handle movement through its own system
            // This method is called but ML-Agents takes control
        }
        
        /// <summary>
        /// Simple AI behavior (fallback when ML-Agents not available).
        /// </summary>
        private void UpdateSimpleBehavior()
        {
            MoveTowardsPuck();
        }
        
        /// <summary>
        /// Move goalie towards puck position (simple AI).
        /// </summary>
        private void MoveTowardsPuck()
        {
            Vector3 puckPosition = puckTransform.position;
            Vector3 goalPosition = goalTransform.position;
            
            // Calculate direction to puck
            Vector3 directionToPuck = (puckPosition - transform.position).normalized;
            
            // Limit movement to max distance from goal
            Vector3 targetPosition = goalPosition + Vector3.ClampMagnitude(
                directionToPuck * maxMoveDistance, maxMoveDistance);
            
            // Move towards target position
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            goalieRigidbody.linearVelocity = moveDirection * moveSpeed;
        }
        
        /// <summary>
        /// Reset goalie to initial position.
        /// </summary>
        public void ResetGoalie()
        {
            transform.position = initialPosition;
            goalieRigidbody.linearVelocity = Vector3.zero;
            hasSaved = false;
            hasConceded = false;
        }
        
        /// <summary>
        /// Handle goal event.
        /// </summary>
        public void OnGoal()
        {
            hasConceded = true;
            Debug.Log("Goal conceded! Penalty: " + goalPenalty);
            ResetGoalie();
        }
        
        /// <summary>
        /// Handle save event.
        /// </summary>
        public void OnSave()
        {
            hasSaved = true;
            Debug.Log("Save made! Reward: " + saveReward);
        }
        
        /// <summary>
        /// End current episode.
        /// </summary>
        public void EndEpisode()
        {
            ResetGoalie();
        }
        
        /// <summary>
        /// Notify goal event (ML-Agents compatible).
        /// </summary>
        public void NotifyGoal()
        {
            OnGoal();
        }
        
        /// <summary>
        /// Notify save event (ML-Agents compatible).
        /// </summary>
        public void NotifySave()
        {
            OnSave();
        }
        
        /// <summary>
        /// Check if ML-Agents is available.
        /// </summary>
        public bool IsMLAgentsAvailable()
        {
            return mlAgentsAvailable;
        }
        
        /// <summary>
        /// Get current AI type being used.
        /// </summary>
        public string GetAIType()
        {
            return mlAgentsAvailable ? "ML-Agents (Advanced)" : "Simple AI (Fallback)";
        }
    }
}