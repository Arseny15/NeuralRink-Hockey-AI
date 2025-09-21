using UnityEngine;

namespace NeuralRink.Agents
{
    /// <summary>
    /// Simple goalie agent that moves towards the puck without ML-Agents.
    /// This is a fallback for when ML-Agents is not available.
    /// </summary>
    public class SimpleGoalieAgent : MonoBehaviour
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
        
        /// <summary>
        /// Initialize the goalie agent with starting position and physics setup.
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
        }
        
        /// <summary>
        /// Update goalie position based on puck location.
        /// </summary>
        private void Update()
        {
            if (puckTransform != null && goalTransform != null)
            {
                MoveTowardsPuck();
            }
        }
        
        /// <summary>
        /// Move goalie towards puck position.
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
        /// Notify goal event.
        /// </summary>
        public void NotifyGoal()
        {
            OnGoal();
        }
        
        /// <summary>
        /// Notify save event.
        /// </summary>
        public void NotifySave()
        {
            OnSave();
        }
    }
}
