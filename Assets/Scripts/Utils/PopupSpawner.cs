using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace NeuralRink.Utils
{
    /// <summary>
    /// Spawner system for creating and managing popup text in 3D space.
    /// Handles object pooling and coordinates popup creation with camera positioning.
    /// </summary>
    public class PopupSpawner : MonoBehaviour
    {
        [Header("Popup Configuration")]
        [SerializeField] private GameObject popupPrefab;
        [SerializeField] private int poolSize = 20;
        [SerializeField] private float spawnOffset = 1f;
        [SerializeField] private Vector3 defaultMoveDirection = Vector3.up;
        
        [Header("Camera Settings")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private bool faceCamera = true;
        [SerializeField] private float cameraDistance = 10f;
        
        [Header("Auto-Setup")]
        [SerializeField] private bool autoCreatePrefab = true;
        [SerializeField] private bool autoFindCamera = true;
        
        // Object pooling
        private Queue<PopupText> popupPool = new Queue<PopupText>();
        private List<PopupText> activePopups = new List<PopupText>();
        
        /// <summary>
        /// Initialize popup spawner system.
        /// </summary>
        private void Start()
        {
            InitializePopupSpawner();
        }
        
        /// <summary>
        /// Initialize popup spawner components and setup.
        /// </summary>
        private void InitializePopupSpawner()
        {
            // Find camera if not assigned
            if (autoFindCamera && targetCamera == null)
            {
                targetCamera = Camera.main;
                if (targetCamera == null)
                {
                    targetCamera = FindObjectOfType<Camera>();
                }
            }
            
            // Create popup prefab if needed
            if (autoCreatePrefab && popupPrefab == null)
            {
                CreatePopupPrefab();
            }
            
            // Initialize object pool
            InitializeObjectPool();
        }
        
        /// <summary>
        /// Create popup prefab automatically.
        /// </summary>
        private void CreatePopupPrefab()
        {
            GameObject prefab = new GameObject("PopupText");
            
            // Add PopupText component
            PopupText popupText = prefab.AddComponent<PopupText>();
            
            // Add Text component
            Text textComponent = prefab.AddComponent<Text>();
            textComponent.fontSize = 24;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;
            
            // Add CanvasGroup for alpha control
            CanvasGroup canvasGroup = prefab.AddComponent<CanvasGroup>();
            
            // Set default properties
            popupText.SetMoveDirection(defaultMoveDirection);
            popupText.SetMoveSpeed(2f);
            popupText.SetLifetime(2f);
            
            popupPrefab = prefab;
        }
        
        /// <summary>
        /// Initialize object pool for popup text.
        /// </summary>
        private void InitializeObjectPool()
        {
            if (popupPrefab == null)
            {
                Debug.LogError("PopupSpawner: No popup prefab available for object pool!");
                return;
            }
            
            // Create pool of popup objects
            for (int i = 0; i < poolSize; i++)
            {
                GameObject popupObj = Instantiate(popupPrefab, transform);
                popupObj.SetActive(false);
                
                PopupText popupText = popupObj.GetComponent<PopupText>();
                if (popupText != null)
                {
                    popupPool.Enqueue(popupText);
                }
            }
            
            Debug.Log($"PopupSpawner: Initialized object pool with {poolSize} popups");
        }
        
        /// <summary>
        /// Spawn a popup at the specified world position.
        /// </summary>
        public PopupText Spawn(string text, Color color, Vector3 worldPosition, Camera camera = null)
        {
            if (popupPrefab == null)
            {
                Debug.LogWarning("PopupSpawner: Cannot spawn popup - no prefab available");
                return null;
            }
            
            // Get popup from pool or create new one
            PopupText popup = GetPopupFromPool();
            if (popup == null)
            {
                popup = CreateNewPopup();
            }
            
            if (popup == null)
            {
                Debug.LogWarning("PopupSpawner: Failed to create popup");
                return null;
            }
            
            // Position popup
            Vector3 spawnPosition = worldPosition + (defaultMoveDirection.normalized * spawnOffset);
            popup.transform.position = spawnPosition;
            popup.gameObject.SetActive(true);
            
            // Use provided camera or default
            Camera popupCamera = camera != null ? camera : targetCamera;
            
            // Start popup animation
            popup.StartPopup(text, color, popupCamera);
            
            // Add to active list
            activePopups.Add(popup);
            
            return popup;
        }
        
        /// <summary>
        /// Spawn a popup with custom movement direction.
        /// </summary>
        public PopupText Spawn(string text, Color color, Vector3 worldPosition, Vector3 moveDirection, Camera camera = null)
        {
            PopupText popup = Spawn(text, color, worldPosition, camera);
            
            if (popup != null)
            {
                popup.SetMoveDirection(moveDirection);
            }
            
            return popup;
        }
        
        /// <summary>
        /// Get popup from object pool.
        /// </summary>
        private PopupText GetPopupFromPool()
        {
            if (popupPool.Count > 0)
            {
                return popupPool.Dequeue();
            }
            
            return null;
        }
        
        /// <summary>
        /// Create new popup if pool is exhausted.
        /// </summary>
        private PopupText CreateNewPopup()
        {
            GameObject popupObj = Instantiate(popupPrefab, transform);
            PopupText popupText = popupObj.GetComponent<PopupText>();
            
            if (popupText != null)
            {
                Debug.Log("PopupSpawner: Created additional popup beyond pool size");
                return popupText;
            }
            
            return null;
        }
        
        /// <summary>
        /// Return popup to object pool.
        /// </summary>
        public void ReturnPopupToPool(PopupText popup)
        {
            if (popup == null) return;
            
            // Remove from active list
            activePopups.Remove(popup);
            
            // Reset popup state
            popup.gameObject.SetActive(false);
            popup.transform.SetParent(transform);
            popup.transform.position = Vector3.zero;
            popup.transform.rotation = Quaternion.identity;
            popup.transform.localScale = Vector3.one;
            
            // Return to pool
            popupPool.Enqueue(popup);
        }
        
        /// <summary>
        /// Update popup spawner each frame.
        /// </summary>
        private void Update()
        {
            UpdateActivePopups();
        }
        
        /// <summary>
        /// Update active popups and return completed ones to pool.
        /// </summary>
        private void UpdateActivePopups()
        {
            for (int i = activePopups.Count - 1; i >= 0; i--)
            {
                PopupText popup = activePopups[i];
                
                if (popup == null || !popup.gameObject.activeInHierarchy)
                {
                    activePopups.RemoveAt(i);
                    continue;
                }
                
                // Check if popup animation is complete
                if (!popup.IsAnimating())
                {
                    ReturnPopupToPool(popup);
                }
            }
        }
        
        /// <summary>
        /// Clear all active popups.
        /// </summary>
        public void ClearAllPopups()
        {
            for (int i = activePopups.Count - 1; i >= 0; i--)
            {
                PopupText popup = activePopups[i];
                if (popup != null)
                {
                    popup.StopPopup();
                    ReturnPopupToPool(popup);
                }
            }
            
            activePopups.Clear();
        }
        
        /// <summary>
        /// Set target camera for popup positioning.
        /// </summary>
        public void SetTargetCamera(Camera camera)
        {
            targetCamera = camera;
        }
        
        /// <summary>
        /// Set default move direction for popups.
        /// </summary>
        public void SetDefaultMoveDirection(Vector3 direction)
        {
            defaultMoveDirection = direction;
        }
        
        /// <summary>
        /// Set spawn offset distance.
        /// </summary>
        public void SetSpawnOffset(float offset)
        {
            spawnOffset = offset;
        }
        
        /// <summary>
        /// Set object pool size.
        /// </summary>
        public void SetPoolSize(int size)
        {
            poolSize = Mathf.Max(1, size);
        }
        
        /// <summary>
        /// Get current number of active popups.
        /// </summary>
        public int GetActivePopupCount()
        {
            return activePopups.Count;
        }
        
        /// <summary>
        /// Get current pool size.
        /// </summary>
        public int GetPoolSize()
        {
            return popupPool.Count;
        }
        
        /// <summary>
        /// Check if popup spawner is properly initialized.
        /// </summary>
        public bool IsInitialized()
        {
            return popupPrefab != null && popupPool.Count > 0;
        }
        
        /// <summary>
        /// Spawn a goal popup with predefined settings.
        /// </summary>
        public PopupText SpawnGoalPopup(Vector3 position, Camera camera = null)
        {
            return Spawn("GOAL!", Color.red, position, camera);
        }
        
        /// <summary>
        /// Spawn a save popup with predefined settings.
        /// </summary>
        public PopupText SpawnSavePopup(Vector3 position, Camera camera = null)
        {
            return Spawn("SAVE!", Color.green, position, camera);
        }
        
        /// <summary>
        /// Spawn a miss popup with predefined settings.
        /// </summary>
        public PopupText SpawnMissPopup(Vector3 position, Camera camera = null)
        {
            return Spawn("MISS!", new Color(1f, 0.75f, 0f), position, camera);
        }
    }
}
