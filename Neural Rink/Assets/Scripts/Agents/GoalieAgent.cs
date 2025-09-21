using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace NeuralRink.Agents
{
    /// <summary>
    /// ML-Agents 4.0 goalie agent that learns to defend against human-controlled skaters.
    /// Uses PPO training with deterministic physics for consistent learning.
    /// </summary>
    public class GoalieAgent : Agent
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
        [SerializeField] private float timePenalty = -0.001f;
        
        private Vector3 initialPosition;
        private Rigidbody goalieRigidbody;
        private bool hasSaved = false;
        private bool hasConceded = false;
        
        /// <summary>
        /// Initialize the goalie agent with starting position and physics setup.
        /// </summary>
        public override void Initialize()
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
        /// Reset the goalie to initial state for new episode.
        /// </summary>
        public override void OnEpisodeBegin()
        {
            transform.position = initialPosition;
            goalieRigidbody.linearVelocity = Vector3.zero;
            hasSaved = false;
            hasConceded = false;
        }
        
        /// <summary>
        /// Collect observations for the neural network.
        /// </summary>
        public override void CollectObservations(VectorSensor sensor)
        {
            // Goalie position relative to goal center
            Vector3 goalieToGoal = goalTransform.position - transform.position;
            sensor.AddObservation(goalieToGoal.normalized);
            sensor.AddObservation(goalieToGoal.magnitude / maxMoveDistance);
            
            // Puck position and velocity
            Vector3 puckToGoal = goalTransform.position - puckTransform.position;
            sensor.AddObservation(puckToGoal.normalized);
            sensor.AddObservation(puckToGoal.magnitude);
            sensor.AddObservation(puckTransform.GetComponent<Rigidbody>().linearVelocity);
            
            // Player position
            Vector3 playerToGoal = goalTransform.position - playerTransform.position;
            sensor.AddObservation(playerToGoal.normalized);
            sensor.AddObservation(playerToGoal.magnitude);
            
            // Goalie velocity
            sensor.AddObservation(goalieRigidbody.linearVelocity);
        }
        
        /// <summary>
        /// Execute actions from the neural network.
        /// </summary>
        public override void OnActionReceived(ActionBuffers actions)
        {
            // Get continuous actions for movement
            Vector3 moveDirection = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);
            moveDirection = moveDirection.normalized;
            
            // Apply movement with speed limit
            Vector3 targetPosition = transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            
            // Constrain movement within goal area
            Vector3 goalieToGoal = targetPosition - goalTransform.position;
            if (goalieToGoal.magnitude > maxMoveDistance)
            {
                targetPosition = goalTransform.position + goalieToGoal.normalized * maxMoveDistance;
            }
            
            goalieRigidbody.MovePosition(targetPosition);
            
            // Apply time penalty for episode length
            AddReward(timePenalty);
        }
        
        /// <summary>
        /// Handle collision with puck for save/goal detection.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Puck"))
            {
                // Check if puck is moving toward goal
                Vector3 puckToGoal = goalTransform.position - other.transform.position;
                Vector3 puckVelocity = other.GetComponent<Rigidbody>().linearVelocity;
                
                if (Vector3.Dot(puckToGoal.normalized, puckVelocity.normalized) > 0.1f)
                {
                    // Puck is moving toward goal - this is a save
                    if (!hasSaved && !hasConceded)
                    {
                        AddReward(saveReward);
                        hasSaved = true;
                        EndEpisode();
                    }
                }
            }
        }
        
        /// <summary>
        /// Called when puck enters goal area (goal conceded).
        /// </summary>
        public void OnGoalConceded()
        {
            if (!hasConceded && !hasSaved)
            {
                AddReward(goalPenalty);
                hasConceded = true;
                EndEpisode();
            }
        }
        
        /// <summary>
        /// Heuristic behavior for testing without neural network.
        /// </summary>
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;
            
            // Simple heuristic: move toward puck
            Vector3 puckDirection = (puckTransform.position - transform.position).normalized;
            continuousActionsOut[0] = puckDirection.x;
            continuousActionsOut[1] = puckDirection.z;
        }
        
        /// <summary>
        /// Reset goalie position manually (for training scenarios).
        /// </summary>
        public void ResetPosition()
        {
            transform.position = initialPosition;
            goalieRigidbody.linearVelocity = Vector3.zero;
        }
    }
}
