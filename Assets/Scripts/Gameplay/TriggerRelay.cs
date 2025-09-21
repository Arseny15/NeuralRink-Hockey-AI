using UnityEngine;

namespace NeuralRink.Gameplay
{
    /// <summary>
    /// Relay script for trigger events to avoid hard references in prefabs.
    /// Routes trigger events to the GameDirector without coupling prefabs to specific scenes.
    /// </summary>
    public class TriggerRelay : MonoBehaviour
    {
        [Header("Event Configuration")]
        [SerializeField] private GameDirector director;
        [SerializeField] private TriggerType triggerType;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        /// <summary>
        /// Types of trigger events that can be relayed.
        /// </summary>
        public enum TriggerType
        {
            Goal,
            Save,
            Miss
        }
        
        /// <summary>
        /// Initialize trigger relay.
        /// </summary>
        private void Start()
        {
            // Find GameDirector if not assigned
            if (director == null)
            {
                director = FindObjectOfType<GameDirector>();
                
                if (director == null)
                {
                    Debug.LogError($"TriggerRelay on {gameObject.name}: GameDirector not found in scene!");
                }
            }
        }
        
        /// <summary>
        /// Handle trigger enter events.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // Check if the colliding object is the puck
            if (!other.CompareTag("Puck"))
            {
                return;
            }
            
            // Log trigger event
            if (enableDebugLogs)
            {
                Debug.Log($"TriggerRelay: {triggerType} triggered by {other.name}");
            }
            
            // Route to appropriate GameDirector method
            switch (triggerType)
            {
                case TriggerType.Goal:
                    director?.OnGoal();
                    break;
                    
                case TriggerType.Save:
                    director?.OnSave();
                    break;
                    
                case TriggerType.Miss:
                    director?.OnMiss();
                    break;
                    
                default:
                    Debug.LogWarning($"TriggerRelay: Unknown trigger type {triggerType}");
                    break;
            }
        }
        
        /// <summary>
        /// Set the GameDirector reference.
        /// </summary>
        public void SetDirector(GameDirector newDirector)
        {
            director = newDirector;
        }
        
        /// <summary>
        /// Set the trigger type.
        /// </summary>
        public void SetTriggerType(TriggerType type)
        {
            triggerType = type;
        }
        
        /// <summary>
        /// Enable or disable debug logging.
        /// </summary>
        public void SetDebugLogs(bool enabled)
        {
            enableDebugLogs = enabled;
        }
        
        /// <summary>
        /// Get current trigger type.
        /// </summary>
        public TriggerType GetTriggerType()
        {
            return triggerType;
        }
        
        /// <summary>
        /// Check if director is properly assigned.
        /// </summary>
        public bool IsDirectorAssigned()
        {
            return director != null;
        }
        
        /// <summary>
        /// Validate trigger setup.
        /// </summary>
        public bool ValidateSetup()
        {
            bool isValid = true;
            
            if (director == null)
            {
                Debug.LogError($"TriggerRelay on {gameObject.name}: No GameDirector assigned!");
                isValid = false;
            }
            
            if (!GetComponent<Collider>())
            {
                Debug.LogError($"TriggerRelay on {gameObject.name}: No Collider component found!");
                isValid = false;
            }
            else if (!GetComponent<Collider>().isTrigger)
            {
                Debug.LogError($"TriggerRelay on {gameObject.name}: Collider is not set as trigger!");
                isValid = false;
            }
            
            return isValid;
        }
    }
}
