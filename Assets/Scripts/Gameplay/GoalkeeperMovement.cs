using UnityEngine;

namespace NeuralRink.Gameplay
{
    /// <summary>
    /// Physics-based goalkeeper movement system adapted from proven air hockey mechanics.
    /// Uses velocity-based movement with speed limiting for responsive goalkeeper control.
    /// Slightly slower than player movement for balanced gameplay.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class GoalkeeperMovement : MonoBehaviour
    {
        [Header("Movement Configuration")]
        [SerializeField] private float goalkeeperSpeed = 12f; // Slightly slower than players
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private bool freezeYPosition = true;
        [SerializeField] private bool freezeRotation = true;
        
        [Header("Movement Bounds (Optional)")]
        [SerializeField] private bool useBounds = true;
        [SerializeField] private Vector3 boundsCenter = Vector3.zero;
        [SerializeField] private Vector3 boundsSize = new Vector3(6f, 0f, 8f);
        
        [Header("Input Configuration")]
        [SerializeField] private KeyCode forwardKey = KeyCode.UpArrow;
        [SerializeField] private KeyCode backwardKey = KeyCode.DownArrow;
        [SerializeField] private KeyCode leftKey = KeyCode.LeftArrow;
        [SerializeField] private KeyCode rightKey = KeyCode.RightArrow;
        
        private Rigidbody rigidBody;
        private Vector3 maxSpeed;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        
        /// <summary>
        /// Initialize goalkeeper movement system.
        /// </summary>
        void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            maxSpeed = new Vector3(goalkeeperSpeed, 0, goalkeeperSpeed);
            
            // Store initial transform for reset functionality
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            
            // Configure rigidbody constraints
            SetupRigidbodyConstraints();
        }
        
        /// <summary>
        /// Setup rigidbody constraints for goalkeeper movement.
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
        /// Handle goalkeeper input and apply movement forces.
        /// </summary>
        void Update()
        {
            HandleMovementInput();
            EnforceBounds();
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
        /// Enforce movement bounds to keep goalkeeper in goal area.
        /// </summary>
        private void EnforceBounds()
        {
            if (!useBounds) return;
            
            Vector3 position = transform.position;
            Vector3 min = boundsCenter - boundsSize * 0.5f;
            Vector3 max = boundsCenter + boundsSize * 0.5f;
            
            // Clamp position to bounds
            Vector3 clampedPosition = new Vector3(
                Mathf.Clamp(position.x, min.x, max.x),
                position.y,
                Mathf.Clamp(position.z, min.z, max.z)
            );
            
            // If position was clamped, update transform and stop velocity in that direction
            if (Vector3.Distance(position, clampedPosition) > 0.01f)
            {
                transform.position = clampedPosition;
                
                // Stop velocity in the direction that hit the bounds
                Vector3 velocity = rigidBody.linearVelocity;
                if (position.x != clampedPosition.x) velocity.x = 0f;
                if (position.z != clampedPosition.z) velocity.z = 0f;
                rigidBody.linearVelocity = velocity;
            }
        }
        
        /// <summary>
        /// Reset goalkeeper to initial position and stop movement.
        /// </summary>
        public void ResetGoalkeeper()
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
        
        /// <summary>
        /// Set goalkeeper speed dynamically.
        /// </summary>
        public void SetGoalkeeperSpeed(float newSpeed)
        {
            goalkeeperSpeed = newSpeed;
            maxSpeed = new Vector3(goalkeeperSpeed, 0, goalkeeperSpeed);
        }
        
        /// <summary>
        /// Set acceleration force dynamically.
        /// </summary>
        public void SetAcceleration(float newAcceleration)
        {
            acceleration = newAcceleration;
        }
        
        /// <summary>
        /// Set movement bounds for goalkeeper area.
        /// </summary>
        public void SetBounds(Vector3 center, Vector3 size)
        {
            boundsCenter = center;
            boundsSize = size;
            useBounds = true;
        }
        
        /// <summary>
        /// Enable or disable movement bounds.
        /// </summary>
        public void SetBoundsEnabled(bool enabled)
        {
            useBounds = enabled;
        }
        
        /// <summary>
        /// Get current speed for telemetry/UI.
        /// </summary>
        public float GetCurrentSpeed()
        {
            return rigidBody.linearVelocity.magnitude;
        }
        
        /// <summary>
        /// Check if goalkeeper is moving.
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
        
        /// <summary>
        /// Draw bounds gizmo in scene view.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!useBounds) return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(boundsCenter, boundsSize);
        }
    }
}
