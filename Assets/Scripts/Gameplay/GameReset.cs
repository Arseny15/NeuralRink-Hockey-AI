using UnityEngine;

namespace NeuralRink.Gameplay
{
    /// <summary>
    /// Game reset system adapted from proven air hockey mechanics.
    /// Allows resetting any game object to its initial state with R key.
    /// Can be attached to players, goalkeeper, puck, or any game object.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class GameReset : MonoBehaviour
    {
        [Header("Reset Configuration")]
        [SerializeField] private KeyCode resetKey = KeyCode.R;
        [SerializeField] private bool resetOnKeyPress = true;
        [SerializeField] private bool resetVelocity = true;
        [SerializeField] private bool resetRotation = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugMessages = false;
        
        private Vector3 initPosition;
        private Vector3 initVelocity;
        private Quaternion initRotation;
        private Rigidbody rigidBody;
        
        /// <summary>
        /// Store initial state for reset functionality.
        /// </summary>
        void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            
            // Store initial state
            initPosition = rigidBody.position;
            initRotation = rigidBody.rotation;
            initVelocity = rigidBody.linearVelocity;
            
            if (showDebugMessages)
            {
                Debug.Log($"GameReset initialized for {gameObject.name} at position {initPosition}");
            }
        }
        
        /// <summary>
        /// Handle reset input.
        /// </summary>
        void Update()
        {
            if (resetOnKeyPress && Input.GetKey(resetKey))
            {
                ResetToInitialState();
            }
        }
        
        /// <summary>
        /// Reset object to its initial state.
        /// </summary>
        public void ResetToInitialState()
        {
            // Reset position
            transform.position = initPosition;
            
            // Reset rotation if enabled
            if (resetRotation)
            {
                transform.rotation = initRotation;
            }
            
            // Reset velocity if enabled
            if (resetVelocity)
            {
                rigidBody.linearVelocity = initVelocity;
                rigidBody.angularVelocity = Vector3.zero;
            }
            
            if (showDebugMessages)
            {
                Debug.Log($"Reset {gameObject.name} to initial state");
            }
        }
        
        /// <summary>
        /// Reset to a specific position.
        /// </summary>
        public void ResetToPosition(Vector3 position)
        {
            transform.position = position;
            
            if (resetVelocity)
            {
                rigidBody.linearVelocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
            }
            
            if (showDebugMessages)
            {
                Debug.Log($"Reset {gameObject.name} to position {position}");
            }
        }
        
        /// <summary>
        /// Update the initial state (useful for checkpoints).
        /// </summary>
        public void UpdateInitialState()
        {
            initPosition = rigidBody.position;
            initRotation = rigidBody.rotation;
            initVelocity = rigidBody.linearVelocity;
            
            if (showDebugMessages)
            {
                Debug.Log($"Updated initial state for {gameObject.name}");
            }
        }
        
        /// <summary>
        /// Get initial position.
        /// </summary>
        public Vector3 GetInitialPosition()
        {
            return initPosition;
        }
        
        /// <summary>
        /// Get initial rotation.
        /// </summary>
        public Quaternion GetInitialRotation()
        {
            return initRotation;
        }
        
        /// <summary>
        /// Set whether to reset on key press.
        /// </summary>
        public void SetResetOnKeyPress(bool enabled)
        {
            resetOnKeyPress = enabled;
        }
        
        /// <summary>
        /// Set the reset key.
        /// </summary>
        public void SetResetKey(KeyCode newKey)
        {
            resetKey = newKey;
        }
        
        /// <summary>
        /// Enable or disable debug messages.
        /// </summary>
        public void SetDebugMessages(bool enabled)
        {
            showDebugMessages = enabled;
        }
    }
}
