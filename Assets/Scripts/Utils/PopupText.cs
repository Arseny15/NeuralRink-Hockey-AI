using UnityEngine;
using UnityEngine.UI;

namespace NeuralRink.Utils
{
    /// <summary>
    /// Individual popup text component for displaying temporary messages in 3D space.
    /// Handles animation, movement, and lifecycle of popup text elements.
    /// </summary>
    public class PopupText : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float fadeSpeed = 1f;
        [SerializeField] private Vector3 moveDirection = Vector3.up;
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Visual Settings")]
        [SerializeField] private float startScale = 0.5f;
        [SerializeField] private float endScale = 1f;
        [SerializeField] private float maxScale = 1.5f;
        
        // Components
        private Text textComponent;
        private CanvasGroup canvasGroup;
        private Camera targetCamera;
        
        // Animation state
        private float startTime;
        private Vector3 startPosition;
        private Color startColor;
        private bool isAnimating = false;
        
        /// <summary>
        /// Initialize popup text component.
        /// </summary>
        private void Awake()
        {
            // Get or create required components
            textComponent = GetComponent<Text>();
            if (textComponent == null)
            {
                textComponent = gameObject.AddComponent<Text>();
            }
            
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            // Set default text properties
            textComponent.fontSize = 24;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;
        }
        
        /// <summary>
        /// Start popup animation.
        /// </summary>
        public void StartPopup(string text, Color color, Camera camera = null)
        {
            // Set text and color
            textComponent.text = text;
            textComponent.color = color;
            startColor = color;
            
            // Set camera reference
            targetCamera = camera;
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
            
            // Initialize animation state
            startTime = Time.time;
            startPosition = transform.position;
            isAnimating = true;
            
            // Set initial scale and alpha
            transform.localScale = Vector3.one * startScale;
            canvasGroup.alpha = 1f;
            
            // Start animation coroutine
            StartCoroutine(AnimatePopup());
        }
        
        /// <summary>
        /// Coroutine for popup animation.
        /// </summary>
        private System.Collections.IEnumerator AnimatePopup()
        {
            while (isAnimating && Time.time - startTime < lifetime)
            {
                float elapsed = Time.time - startTime;
                float normalizedTime = elapsed / lifetime;
                
                // Update position
                UpdatePosition(elapsed);
                
                // Update scale
                UpdateScale(normalizedTime);
                
                // Update alpha
                UpdateAlpha(normalizedTime);
                
                // Update color (optional fade to white)
                UpdateColor(normalizedTime);
                
                yield return null;
            }
            
            // Animation complete - destroy popup
            DestroyPopup();
        }
        
        /// <summary>
        /// Update popup position based on movement direction and speed.
        /// </summary>
        private void UpdatePosition(float elapsed)
        {
            Vector3 movement = moveDirection.normalized * moveSpeed * elapsed;
            transform.position = startPosition + movement;
            
            // Face camera if available
            if (targetCamera != null)
            {
                transform.LookAt(targetCamera.transform);
                transform.Rotate(0, 180, 0); // Flip to face camera correctly
            }
        }
        
        /// <summary>
        /// Update popup scale based on animation curve.
        /// </summary>
        private void UpdateScale(float normalizedTime)
        {
            float scaleValue = scaleCurve.Evaluate(normalizedTime);
            float currentScale = Mathf.Lerp(startScale, endScale, scaleValue);
            
            // Apply scale burst effect
            if (normalizedTime < 0.2f)
            {
                float burstScale = Mathf.Lerp(endScale, maxScale, normalizedTime / 0.2f);
                currentScale = burstScale;
            }
            
            transform.localScale = Vector3.one * currentScale;
        }
        
        /// <summary>
        /// Update popup alpha for fade effect.
        /// </summary>
        private void UpdateAlpha(float normalizedTime)
        {
            // Start fading out in the last 30% of lifetime
            if (normalizedTime > 0.7f)
            {
                float fadeTime = (normalizedTime - 0.7f) / 0.3f;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeTime);
            }
            else
            {
                canvasGroup.alpha = 1f;
            }
        }
        
        /// <summary>
        /// Update popup color (optional effect).
        /// </summary>
        private void UpdateColor(float normalizedTime)
        {
            // Optional: Fade color to white over time
            if (normalizedTime > 0.5f)
            {
                float colorFadeTime = (normalizedTime - 0.5f) / 0.5f;
                Color currentColor = Color.Lerp(startColor, Color.white, colorFadeTime);
                textComponent.color = currentColor;
            }
        }
        
        /// <summary>
        /// Destroy popup with cleanup.
        /// </summary>
        private void DestroyPopup()
        {
            isAnimating = false;
            Destroy(gameObject);
        }
        
        /// <summary>
        /// Stop popup animation and destroy immediately.
        /// </summary>
        public void StopPopup()
        {
            isAnimating = false;
            DestroyPopup();
        }
        
        /// <summary>
        /// Set popup movement direction.
        /// </summary>
        public void SetMoveDirection(Vector3 direction)
        {
            moveDirection = direction;
        }
        
        /// <summary>
        /// Set popup movement speed.
        /// </summary>
        public void SetMoveSpeed(float speed)
        {
            moveSpeed = speed;
        }
        
        /// <summary>
        /// Set popup lifetime.
        /// </summary>
        public void SetLifetime(float time)
        {
            lifetime = time;
        }
        
        /// <summary>
        /// Set popup scale values.
        /// </summary>
        public void SetScaleValues(float start, float end, float max)
        {
            startScale = start;
            endScale = end;
            maxScale = max;
        }
        
        /// <summary>
        /// Check if popup is currently animating.
        /// </summary>
        public bool IsAnimating()
        {
            return isAnimating;
        }
        
        /// <summary>
        /// Get popup remaining lifetime.
        /// </summary>
        public float GetRemainingLifetime()
        {
            if (!isAnimating) return 0f;
            return Mathf.Max(0f, lifetime - (Time.time - startTime));
        }
    }
}
