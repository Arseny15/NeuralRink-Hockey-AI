using UnityEngine;
using UnityEngine.InputSystem;

namespace NeuralRink.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Input")]
        public InputActionReference moveAction;   // Vector2
        public InputActionReference aimAction;    // Vector2 (mouse delta or stick)
        public InputActionReference shootAction;  // Button
        public InputActionReference dekeAction;   // Button

        [Header("Movement")]
        public float maxSpeed = 7.5f;
        public float acceleration = 40f;
        public float friction = 8f;

        [Header("Shot")]
        public float minCharge = 0.2f;
        public float maxCharge = 1.0f;
        public float maxShotImpulse = 18f;

        [Header("Deke")]
        public float dekeForce = 8f;
        public float dekeCooldown = 1.0f;

        [Header("Refs")]
        public PuckController puck;

        Rigidbody _rb;
        float _chargeStart = -1f;
        float _dekeReadyAt = 0f;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        void OnEnable()
        {
            moveAction?.action?.Enable();
            aimAction?.action?.Enable();
            shootAction?.action?.Enable();
            dekeAction?.action?.Enable();

            if (shootAction != null)
            {
                shootAction.action.started += OnShootStarted;
                shootAction.action.canceled += OnShootReleased;
            }
            if (dekeAction != null)
            {
                dekeAction.action.performed += OnDeke;
            }
        }

        void OnDisable()
        {
            if (shootAction != null)
            {
                shootAction.action.started -= OnShootStarted;
                shootAction.action.canceled -= OnShootReleased;
            }
            if (dekeAction != null)
            {
                dekeAction.action.performed -= OnDeke;
            }
        }

        void FixedUpdate()
        {
            Vector2 move = moveAction?.action?.ReadValue<Vector2>() ?? Vector2.zero;
            var desiredVel = new Vector3(move.x, 0f, move.y) * maxSpeed;
            Vector3 vel = _rb.linearVelocity;

            // Accelerate toward desired velocity
            Vector3 dv = Vector3.ClampMagnitude(desiredVel - vel, acceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = vel + dv;

            // Simple friction to settle
            _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity, Vector3.zero, friction * Time.fixedDeltaTime);
        }

        void OnShootStarted(InputAction.CallbackContext _)
        {
            _chargeStart = Time.time;
        }

        void OnShootReleased(InputAction.CallbackContext _)
        {
            if (puck == null) return;
            if (_chargeStart < 0f) return;

            float held = Mathf.Clamp(Time.time - _chargeStart, minCharge, maxCharge);
            float power01 = Mathf.InverseLerp(minCharge, maxCharge, held);

            Vector2 aim = aimAction?.action?.ReadValue<Vector2>() ?? Vector2.zero;
            Vector3 dir = (aim.sqrMagnitude > 0.0001f)
                ? new Vector3(aim.x, 0f, aim.y).normalized
                : transform.forward;

            puck.Shoot(dir, power01 * maxShotImpulse);
            _chargeStart = -1f;
        }

        void OnDeke(InputAction.CallbackContext _)
        {
            if (Time.time < _dekeReadyAt) return;
            Vector3 lateral = Vector3.Cross(Vector3.up, transform.forward).normalized * (Random.value < 0.5f ? -1f : 1f);
            _rb.AddForce(lateral * dekeForce, ForceMode.VelocityChange);
            _dekeReadyAt = Time.time + dekeCooldown;
        }
        
        /// <summary>
        /// Check if player can shoot (for compatibility with existing telemetry code)
        /// </summary>
        public bool CanShoot()
        {
            return _chargeStart < 0f && puck != null;
        }
        
        /// <summary>
        /// Reset player to initial state
        /// </summary>
        public void ResetPlayer()
        {
            if (_rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
            
            _chargeStart = -1f;
            _dekeReadyAt = 0f;
        }
    }
}