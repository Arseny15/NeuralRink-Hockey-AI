using UnityEngine;

namespace NeuralRink.Gameplay
{
    /// <summary>
    /// Physics-based player movement system adapted from proven air hockey mechanics.
    /// Uses velocity-based movement with speed limiting for responsive, realistic hockey player control.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Configuration")]
        [SerializeField] private float playerSpeed = 14f;
        [SerializeField] private float acceleration = 60f;
        [SerializeField] private bool freezeYPosition = true;
        [SerializeField] private bool freezeRotation = true;
        
        [Header("Input Configuration")]
        [SerializeField] private KeyCode forwardKey = KeyCode.W;
        [SerializeField] private KeyCode backwardKey = KeyCode.S;
        [SerializeField] private KeyCode leftKey = KeyCode.A;
        [SerializeField] private KeyCode rightKey = KeyCode.D;
        
        private Rigidbody rigidBody;
        private Vector3 maxSpeed;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        
        /// <summary>
        /// Initialize player movement system.
        /// </summary>
        void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            maxSpeed = new Vector3(playerSpeed, 0, playerSpeed);
            
            // Store initial transform for reset functionality
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            
            // Configure rigidbody constraints
            SetupRigidbodyConstraints();
        }
        
        /// <summary>
        /// Setup rigidbody constraints for hockey player movement.
        /// </summary>
        private void SetupRigidbodyConstraints()
        {
            if (freezeYPosition)
            {
                rigidBody.constraints |= RigidbodyConstraints.FreezePositionY;
            }
            
            if (freezeRotation)
            {
                rigidBody.constraints |= RigidbodyConstraints.FreezeRotationX | 
                                        RigidbodyConstraints.FreezeRotationZ;
            }
        }
        
        /// <summary>
        /// Handle player input and apply movement forces.
        /// </summary>
        void Update()
        {
            HandleMovementInput();
        }
        
        /// <summary>
        /// Process movement input and apply forces to rigidbody.
        /// </summary>
        private void HandleMovementInput()
        {
            // Forward/Backward movement (X-axis)
            if (Input.GetKey(forwardKey))
            {
                if (Mathf.Abs(rigidBody.linearVelocity.x) < maxSpeed.x)
                {
                    rigidBody.AddForce(-acceleration, 0, 0, ForceMode.Acceleration);
                }
            }
            
            if (Input.GetKey(backwardKey))
            {
                if (Mathf.Abs(rigidBody.linearVelocity.x) < maxSpeed.x)
                {
                    rigidBody.AddForce(acceleration, 0, 0, ForceMode.Acceleration);
                }
            }
            
            // Left/Right movement (Z-axis)
            if (Input.GetKey(leftKey))
            {
                if (Mathf.Abs(rigidBody.linearVelocity.z) < maxSpeed.z)
                {
                    rigidBody.AddForce(0, 0, -acceleration, ForceMode.Acceleration);
                }
            }
            
            if (Input.GetKey(rightKey))
            {
                if (Mathf.Abs(rigidBody.linearVelocity.z) < maxSpeed.z)
                {
                    rigidBody.AddForce(0, 0, acceleration, ForceMode.Acceleration);
                }
            }
        }
        
        /// <summary>
        /// Reset player to initial position and stop movement.
        /// </summary>
        public void ResetPlayer()
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
        
        /// <summary>
        /// Set player speed dynamically.
        /// </summary>
        public void SetPlayerSpeed(float newSpeed)
        {
            playerSpeed = newSpeed;
            maxSpeed = new Vector3(playerSpeed, 0, playerSpeed);
        }
        
        /// <summary>
        /// Set acceleration force dynamically.
        /// </summary>
        public void SetAcceleration(float newAcceleration)
        {
            acceleration = newAcceleration;
        }
        
        /// <summary>
        /// Get current speed for telemetry/UI.
        /// </summary>
        public float GetCurrentSpeed()
        {
            return rigidBody.linearVelocity.magnitude;
        }
        
        /// <summary>
        /// Check if player is moving.
        /// </summary>
        public bool IsMoving()
        {
            return rigidBody.linearVelocity.magnitude > 0.1f;
        }
        
        /// <summary>
        /// Set custom input keys.
        /// </summary>
        public void SetInputKeys(KeyCode forward, KeyCode backward, KeyCode left, KeyCode right)
        {
            forwardKey = forward;
            backwardKey = backward;
            leftKey = left;
            rightKey = right;
        }
        
        /// <summary>
        /// Enable or disable movement input.
        /// </summary>
        public void SetMovementEnabled(bool enabled)
        {
            this.enabled = enabled;
            
            if (!enabled)
            {
                // Stop movement when disabled
                rigidBody.linearVelocity = Vector3.zero;
            }
        }
    }
}
