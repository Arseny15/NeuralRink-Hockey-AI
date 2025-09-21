using UnityEngine;
using UnityEngine.InputSystem;

namespace NeuralRink.Gameplay
{
    /// <summary>
    /// Human-controlled skater player controller using Unity Input System.
    /// Handles movement, shooting, and interaction with the puck.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Configuration")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float maxSpeed = 12f;
        
        [Header("Shooting Configuration")]
        [SerializeField] private float shotPower = 15f;
        [SerializeField] private float shotCooldown = 1f;
        [SerializeField] private Transform shotPoint;
        [SerializeField] private LayerMask puckLayer = 1;
        
        [Header("Input Configuration")]
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private string moveActionMap = "Player";
        [SerializeField] private string moveAction = "Move";
        [SerializeField] private string shootAction = "Shoot";
        [SerializeField] private string aimAction = "Aim";
        
        private InputAction moveInput;
        private InputAction shootInput;
        private InputAction aimInput;
        
        private Rigidbody playerRigidbody;
        private Camera playerCamera;
        private Vector3 moveInputVector;
        private Vector2 aimInputVector;
        private bool canShoot = true;
        private float lastShotTime;
        
        /// <summary>
        /// Initialize player controller with input system and physics setup.
        /// </summary>
        private void Awake()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            if (playerRigidbody == null)
            {
                playerRigidbody = gameObject.AddComponent<Rigidbody>();
            }
            
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
            
            SetupInputActions();
        }
        
        /// <summary>
        /// Set up Unity Input System actions for player control.
        /// </summary>
        private void SetupInputActions()
        {
            if (inputActions != null)
            {
                moveInput = inputActions[moveActionMap][moveAction];
                shootInput = inputActions[moveActionMap][shootAction];
                aimInput = inputActions[moveActionMap][aimAction];
                
                shootInput.performed += OnShootInput;
            }
        }
        
        /// <summary>
        /// Enable input actions when object becomes active.
        /// </summary>
        private void OnEnable()
        {
            moveInput?.Enable();
            shootInput?.Enable();
            aimInput?.Enable();
        }
        
        /// <summary>
        /// Disable input actions when object becomes inactive.
        /// </summary>
        private void OnDisable()
        {
            moveInput?.Disable();
            shootInput?.Disable();
            aimInput?.Disable();
        }
        
        /// <summary>
        /// Handle player input and movement in FixedUpdate for physics consistency.
        /// </summary>
        private void FixedUpdate()
        {
            HandleMovement();
            HandleRotation();
        }
        
        /// <summary>
        /// Process movement input and apply physics-based movement.
        /// </summary>
        private void HandleMovement()
        {
            if (moveInput != null)
            {
                Vector2 moveInput2D = moveInput.ReadValue<Vector2>();
                moveInputVector = new Vector3(moveInput2D.x, 0, moveInput2D.y);
            }
            
            // Apply movement force
            Vector3 moveForce = moveInputVector * moveSpeed;
            playerRigidbody.AddForce(moveForce, ForceMode.Acceleration);
            
            // Limit maximum speed
            if (playerRigidbody.linearVelocity.magnitude > maxSpeed)
            {
                playerRigidbody.linearVelocity = playerRigidbody.linearVelocity.normalized * maxSpeed;
            }
        }
        
        /// <summary>
        /// Handle player rotation based on movement direction and aim input.
        /// </summary>
        private void HandleRotation()
        {
            if (aimInput != null)
            {
                aimInputVector = aimInput.ReadValue<Vector2>();
            }
            
            Vector3 targetDirection;
            
            // Prioritize aim input over movement direction
            if (aimInputVector.magnitude > 0.1f && playerCamera != null)
            {
                // Convert screen space aim to world space
                Vector3 screenAim = new Vector3(aimInputVector.x, 0, aimInputVector.y);
                targetDirection = playerCamera.transform.TransformDirection(screenAim);
                targetDirection.y = 0;
            }
            else if (moveInputVector.magnitude > 0.1f)
            {
                // Use movement direction for rotation
                targetDirection = moveInputVector;
            }
            else
            {
                return; // No rotation needed
            }
            
            // Apply rotation
            if (targetDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection.normalized);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.fixedDeltaTime
                );
            }
        }
        
        /// <summary>
        /// Handle shoot input action.
        /// </summary>
        private void OnShootInput(InputAction.CallbackContext context)
        {
            if (canShoot && Time.time - lastShotTime >= shotCooldown)
            {
                AttemptShot();
            }
        }
        
        /// <summary>
        /// Attempt to shoot the puck with physics-based force.
        /// </summary>
        private void AttemptShot()
        {
            // Find nearby puck
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, 2f, puckLayer);
            
            foreach (Collider obj in nearbyObjects)
            {
                if (obj.CompareTag("Puck"))
                {
                    Rigidbody puckRigidbody = obj.GetComponent<Rigidbody>();
                    if (puckRigidbody != null)
                    {
                        // Calculate shot direction
                        Vector3 shotDirection = transform.forward;
                        
                        // Apply shot force to puck
                        puckRigidbody.AddForce(shotDirection * shotPower, ForceMode.Impulse);
                        
                        // Update shot timing
                        lastShotTime = Time.time;
                        
                        // Notify systems about shot
                        OnShotFired();
                        
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Called when player successfully shoots the puck.
        /// </summary>
        private void OnShotFired()
        {
            // TODO: Add shot effects, sound, telemetry
            Debug.Log("Player shot fired!");
        }
        
        /// <summary>
        /// Get current player speed for UI display.
        /// </summary>
        public float GetCurrentSpeed()
        {
            return playerRigidbody.linearVelocity.magnitude;
        }
        
        /// <summary>
        /// Check if player can shoot (cooldown check).
        /// </summary>
        public bool CanShoot()
        {
            return canShoot && Time.time - lastShotTime >= shotCooldown;
        }
        
        /// <summary>
        /// Reset player position and state.
        /// </summary>
        public void ResetPlayer()
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            lastShotTime = 0f;
        }
        
        /// <summary>
        /// Set shot power multiplier for training scenarios.
        /// </summary>
        public void SetShotPowerMultiplier(float multiplier)
        {
            shotPower = 15f * multiplier;
        }
    }
}
