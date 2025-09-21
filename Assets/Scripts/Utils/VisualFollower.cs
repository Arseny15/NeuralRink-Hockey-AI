using UnityEngine;
using NeuralRink.Systems;

namespace NeuralRink.Utils
{
    /// <summary>
    /// Visual follower component that can be disabled in training mode for performance.
    /// Follows a target transform and can be toggled based on training switch state.
    /// </summary>
    public class VisualFollower : MonoBehaviour
    {
        [Header("Target Configuration")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private bool followPosition = true;
        [SerializeField] private bool followRotation = false;
        [SerializeField] private bool followScale = false;
        
        [Header("Training Mode Settings")]
        [SerializeField] private TrainingSwitch trainingSwitch;
        [SerializeField] private bool disableInTraining = true;
        [SerializeField] private bool disableChildrenInTraining = true;
        
        [Header("Smoothing Settings")]
        [SerializeField] private bool useSmoothing = false;
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private AnimationCurve smoothCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Bounds Checking")]
        [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector3 boundsCenter = Vector3.zero;
        [SerializeField] private Vector3 boundsSize = Vector3.one * 10f;
        
        // State tracking
        private bool isActive = true;
        private bool wasInTrainingMode = false;
        private Vector3 lastTargetPosition;
        private Quaternion lastTargetRotation;
        private Vector3 lastTargetScale;
        
        /// <summary>
        /// Initialize visual follower.
        /// </summary>
        private void Start()
        {
            InitializeVisualFollower();
        }
        
        /// <summary>
        /// Initialize visual follower components and state.
        /// </summary>
        private void InitializeVisualFollower()
        {
            // Find training switch if not assigned
            if (trainingSwitch == null)
            {
                trainingSwitch = FindFirstObjectByType<TrainingSwitch>();
            }
            
            // Cache initial state
            if (target != null)
            {
                lastTargetPosition = target.position;
                lastTargetRotation = target.rotation;
                lastTargetScale = target.localScale;
            }
            
            // Check initial training mode state
            UpdateTrainingModeState();
        }
        
        /// <summary>
        /// Update visual follower each frame.
        /// </summary>
        private void Update()
        {
            // Check for training mode changes
            UpdateTrainingModeState();
            
            // Update following if active
            if (isActive && target != null)
            {
                UpdateFollowing();
            }
        }
        
        /// <summary>
        /// Update training mode state and visibility.
        /// </summary>
        private void UpdateTrainingModeState()
        {
            if (trainingSwitch == null) return;
            
            bool isInTrainingMode = trainingSwitch.IsTrainingMode();
            
            // Check if training mode changed
            if (isInTrainingMode != wasInTrainingMode)
            {
                wasInTrainingMode = isInTrainingMode;
                
                if (disableInTraining)
                {
                    SetActive(!isInTrainingMode);
                }
            }
        }
        
        /// <summary>
        /// Update following behavior.
        /// </summary>
        private void UpdateFollowing()
        {
            // Follow position
            if (followPosition)
            {
                Vector3 targetPosition = target.position + offset;
                
                // Apply bounds checking
                if (useBounds)
                {
                    targetPosition = ClampToBounds(targetPosition);
                }
                
                if (useSmoothing)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, 
                        smoothSpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = targetPosition;
                }
                
                lastTargetPosition = target.position;
            }
            
            // Follow rotation
            if (followRotation)
            {
                if (useSmoothing)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, 
                        smoothSpeed * Time.deltaTime);
                }
                else
                {
                    transform.rotation = target.rotation;
                }
                
                lastTargetRotation = target.rotation;
            }
            
            // Follow scale
            if (followScale)
            {
                if (useSmoothing)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, target.localScale, 
                        smoothSpeed * Time.deltaTime);
                }
                else
                {
                    transform.localScale = target.localScale;
                }
                
                lastTargetScale = target.localScale;
            }
        }
        
        /// <summary>
        /// Clamp position to bounds if bounds checking is enabled.
        /// </summary>
        private Vector3 ClampToBounds(Vector3 position)
        {
            Vector3 min = boundsCenter - boundsSize * 0.5f;
            Vector3 max = boundsCenter + boundsSize * 0.5f;
            
            return new Vector3(
                Mathf.Clamp(position.x, min.x, max.x),
                Mathf.Clamp(position.y, min.y, max.y),
                Mathf.Clamp(position.z, min.z, max.z)
            );
        }
        
        /// <summary>
        /// Set active state of the visual follower.
        /// </summary>
        public void SetActive(bool active)
        {
            isActive = active;
            
            // Disable/enable children if specified
            if (disableChildrenInTraining)
            {
                SetChildrenActive(active);
            }
            
            // Disable/enable this object
            gameObject.SetActive(active);
        }
        
        /// <summary>
        /// Set active state of child objects.
        /// </summary>
        private void SetChildrenActive(bool active)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(active);
            }
        }
        
        /// <summary>
        /// Set target transform to follow.
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            
            if (target != null)
            {
                lastTargetPosition = target.position;
                lastTargetRotation = target.rotation;
                lastTargetScale = target.localScale;
            }
        }
        
        /// <summary>
        /// Set offset from target position.
        /// </summary>
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }
        
        /// <summary>
        /// Set following behavior options.
        /// </summary>
        public void SetFollowingOptions(bool position, bool rotation, bool scale)
        {
            followPosition = position;
            followRotation = rotation;
            followScale = scale;
        }
        
        /// <summary>
        /// Set smoothing options.
        /// </summary>
        public void SetSmoothing(bool enabled, float speed, AnimationCurve curve = null)
        {
            useSmoothing = enabled;
            smoothSpeed = speed;
            
            if (curve != null)
            {
                smoothCurve = curve;
            }
        }
        
        /// <summary>
        /// Set bounds checking options.
        /// </summary>
        public void SetBounds(bool enabled, Vector3 center, Vector3 size)
        {
            useBounds = enabled;
            boundsCenter = center;
            boundsSize = size;
        }
        
        /// <summary>
        /// Set training mode behavior.
        /// </summary>
        public void SetTrainingModeBehavior(bool disableInTraining, bool disableChildren)
        {
            this.disableInTraining = disableInTraining;
            this.disableChildrenInTraining = disableChildren;
        }
        
        /// <summary>
        /// Get current target transform.
        /// </summary>
        public Transform GetTarget()
        {
            return target;
        }
        
        /// <summary>
        /// Get current offset.
        /// </summary>
        public Vector3 GetOffset()
        {
            return offset;
        }
        
        /// <summary>
        /// Check if visual follower is currently active.
        /// </summary>
        public bool IsActive()
        {
            return isActive && gameObject.activeInHierarchy;
        }
        
        /// <summary>
        /// Check if currently in training mode.
        /// </summary>
        public bool IsInTrainingMode()
        {
            return trainingSwitch != null && trainingSwitch.IsTrainingMode();
        }
        
        /// <summary>
        /// Force update following behavior (useful for teleporting targets).
        /// </summary>
        public void ForceUpdate()
        {
            if (target != null)
            {
                UpdateFollowing();
            }
        }
        
        /// <summary>
        /// Reset to target position immediately (no smoothing).
        /// </summary>
        public void SnapToTarget()
        {
            if (target == null) return;
            
            bool wasSmoothing = useSmoothing;
            useSmoothing = false;
            
            UpdateFollowing();
            
            useSmoothing = wasSmoothing;
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
