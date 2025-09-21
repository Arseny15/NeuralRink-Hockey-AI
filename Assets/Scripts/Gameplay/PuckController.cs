using UnityEngine;

namespace NeuralRink.Gameplay
{
    /// <summary>
    /// Physics-based puck controller with realistic ice friction and goal detection.
    /// Handles puck movement, collisions, and scoring events.
    /// </summary>
    public class PuckController : MonoBehaviour
    {
        [Header("Physics Configuration")]
        [SerializeField] private float iceFriction = 0.98f;
        [SerializeField] private float airResistance = 0.99f;
        [SerializeField] private float maxVelocity = 25f;
        [SerializeField] private float bounceDamping = 0.7f;
        
        [Header("Goal Detection")]
        [SerializeField] private LayerMask goalLayer = 1;
        [SerializeField] private Transform goalTransform;
        [SerializeField] private float goalTriggerRadius = 1f;
        
        [Header("Audio Configuration")]
        [SerializeField] private AudioClip[] hitSounds;
        [SerializeField] private AudioClip goalSound;
        [SerializeField] private float minHitVolume = 0.1f;
        [SerializeField] private float maxHitVolume = 1f;
        
        private Rigidbody puckRigidbody;
        private AudioSource audioSource;
        private bool isInGoal = false;
        private Vector3 lastPosition;
        private float lastCollisionTime;
        
        /// <summary>
        /// Initialize puck physics and audio components.
        /// </summary>
        private void Awake()
        {
            puckRigidbody = GetComponent<Rigidbody>();
            if (puckRigidbody == null)
            {
                puckRigidbody = gameObject.AddComponent<Rigidbody>();
            }
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            SetupPuckPhysics();
        }
        
        /// <summary>
        /// Configure puck physics properties for realistic ice behavior.
        /// </summary>
        private void SetupPuckPhysics()
        {
            puckRigidbody.mass = 0.17f; // Standard hockey puck mass
            puckRigidbody.linearDamping = 0f; // We'll handle friction manually
            puckRigidbody.angularDamping = 0.1f;
            puckRigidbody.freezeRotation = false;
            puckRigidbody.useGravity = false; // Ice surface assumption
            
            // Make puck a trigger for goal detection
            GetComponent<Collider>().isTrigger = true;
        }
        
        /// <summary>
        /// Apply ice friction and velocity limits in FixedUpdate for consistent physics.
        /// </summary>
        private void FixedUpdate()
        {
            ApplyIceFriction();
            LimitVelocity();
            CheckGoalProximity();
        }
        
        /// <summary>
        /// Apply realistic ice friction to puck movement.
        /// </summary>
        private void ApplyIceFriction()
        {
            // Apply ice friction (exponential decay)
            puckRigidbody.linearVelocity *= iceFriction;
            puckRigidbody.angularVelocity *= iceFriction;
            
            // Stop very slow movement to prevent infinite sliding
            if (puckRigidbody.linearVelocity.magnitude < 0.1f)
            {
                puckRigidbody.linearVelocity = Vector3.zero;
            }
            
            if (puckRigidbody.angularVelocity.magnitude < 0.1f)
            {
                puckRigidbody.angularVelocity = Vector3.zero;
            }
        }
        
        /// <summary>
        /// Limit maximum puck velocity for realistic physics.
        /// </summary>
        private void LimitVelocity()
        {
            if (puckRigidbody.linearVelocity.magnitude > maxVelocity)
            {
                puckRigidbody.linearVelocity = puckRigidbody.linearVelocity.normalized * maxVelocity;
            }
        }
        
        /// <summary>
        /// Check if puck is near goal for scoring detection.
        /// </summary>
        private void CheckGoalProximity()
        {
            if (goalTransform != null && !isInGoal)
            {
                float distanceToGoal = Vector3.Distance(transform.position, goalTransform.position);
                if (distanceToGoal <= goalTriggerRadius)
                {
                    // Check if puck is moving toward goal
                    Vector3 goalDirection = (goalTransform.position - transform.position).normalized;
                    if (Vector3.Dot(puckRigidbody.linearVelocity.normalized, goalDirection) > 0.5f)
                    {
                        OnGoalScored();
                    }
                }
            }
        }
        
        /// <summary>
        /// Handle collision with other objects (walls, players, goalie).
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // Handle collision with solid objects
            if (!other.isTrigger && other.gameObject != gameObject)
            {
                OnCollision(other);
            }
            
            // Handle goal collision
            if (other.CompareTag("Goal"))
            {
                OnGoalScored();
            }
        }
        
        /// <summary>
        /// Process collision with solid objects and apply physics response.
        /// </summary>
        private void OnCollision(Collider other)
        {
            // Calculate collision response
            Vector3 collisionNormal = (transform.position - other.ClosestPoint(transform.position)).normalized;
            
            // Reflect velocity with damping
            Vector3 reflectedVelocity = Vector3.Reflect(puckRigidbody.linearVelocity, collisionNormal);
            puckRigidbody.linearVelocity = reflectedVelocity * bounceDamping;
            
            // Play hit sound based on impact velocity
            PlayHitSound(puckRigidbody.linearVelocity.magnitude);
            
            // Update collision timing
            lastCollisionTime = Time.time;
            
            // Notify collision event
            OnPuckHit(other);
        }
        
        /// <summary>
        /// Handle goal scoring event.
        /// </summary>
        private void OnGoalScored()
        {
            if (!isInGoal)
            {
                isInGoal = true;
                
                // Play goal sound
                if (goalSound != null)
                {
                    audioSource.PlayOneShot(goalSound);
                }
                
                // Stop puck movement
                puckRigidbody.linearVelocity = Vector3.zero;
                puckRigidbody.angularVelocity = Vector3.zero;
                
                // Notify goal event
                OnGoalEvent();
                
                Debug.Log("GOAL SCORED!");
            }
        }
        
        /// <summary>
        /// Play hit sound based on impact velocity.
        /// </summary>
        private void PlayHitSound(float impactVelocity)
        {
            if (hitSounds != null && hitSounds.Length > 0 && Time.time - lastCollisionTime > 0.1f)
            {
                // Select random hit sound
                AudioClip hitSound = hitSounds[Random.Range(0, hitSounds.Length)];
                
                // Scale volume based on impact velocity
                float volume = Mathf.Lerp(minHitVolume, maxHitVolume, 
                    Mathf.Clamp01(impactVelocity / 10f));
                
                audioSource.PlayOneShot(hitSound, volume);
            }
        }
        
        /// <summary>
        /// Reset puck to initial position and state.
        /// </summary>
        public void ResetPuck(Vector3 startPosition)
        {
            transform.position = startPosition;
            puckRigidbody.linearVelocity = Vector3.zero;
            puckRigidbody.angularVelocity = Vector3.zero;
            isInGoal = false;
            lastCollisionTime = 0f;
        }
        
        /// <summary>
        /// Apply external force to puck (for shooting).
        /// </summary>
        public void ApplyForce(Vector3 force, ForceMode forceMode = ForceMode.Impulse)
        {
            puckRigidbody.AddForce(force, forceMode);
        }
        
        /// <summary>
        /// Get current puck velocity for UI display.
        /// </summary>
        public Vector3 GetVelocity()
        {
            return puckRigidbody.linearVelocity;
        }
        
        /// <summary>
        /// Check if puck is stationary.
        /// </summary>
        public bool IsStationary()
        {
            return puckRigidbody.linearVelocity.magnitude < 0.1f;
        }
        
        /// <summary>
        /// Called when puck hits another object.
        /// </summary>
        private void OnPuckHit(Collider hitObject)
        {
            // TODO: Add collision effects, telemetry logging
            Debug.Log($"Puck hit: {hitObject.name}");
        }
        
        /// <summary>
        /// Called when goal is scored.
        /// </summary>
        private void OnGoalEvent()
        {
            // TODO: Notify game systems, update score, trigger goalie penalty
            var goalieAgent = FindFirstObjectByType<NeuralRink.Agents.GoalieAgent>();
            goalieAgent?.OnGoalConceded();
        }
        
        /// <summary>
        /// Shoot the puck in a specified direction with given impulse.
        /// </summary>
        public void Shoot(Vector3 direction, float impulse)
        {
            if (puckRigidbody == null) return;
            
            // Apply the shooting force
            puckRigidbody.AddForce(direction.normalized * impulse, ForceMode.Impulse);
            
            // Clamp velocity to maximum
            if (puckRigidbody.linearVelocity.magnitude > maxVelocity)
            {
                puckRigidbody.linearVelocity = puckRigidbody.linearVelocity.normalized * maxVelocity;
            }
            
            Debug.Log($"Puck shot with impulse: {impulse}, direction: {direction}");
        }
        
    }
}
