using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace NeuralRink.Utils
{
    /// <summary>
    /// Event feed system for displaying game events in real-time.
    /// Shows a scrolling list of recent events with color coding.
    /// </summary>
    public class EventFeed : MonoBehaviour
    {
        [Header("UI Configuration")]
        [SerializeField] private Transform feedContainer;
        [SerializeField] private GameObject eventTextPrefab;
        [SerializeField] private int maxEvents = 10;
        [SerializeField] private float eventLifetime = 5f;
        [SerializeField] private float fadeDuration = 1f;
        
        [Header("Animation Settings")]
        [SerializeField] private float slideSpeed = 100f;
        [SerializeField] private float spacing = 30f;
        [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Auto-Setup")]
        [SerializeField] private bool autoCreateContainer = true;
        [SerializeField] private bool autoCreatePrefab = true;
        
        // Event management
        private Queue<EventItem> eventQueue = new Queue<EventItem>();
        private List<EventItem> activeEvents = new List<EventItem>();
        
        /// <summary>
        /// Event item structure for managing individual events.
        /// </summary>
        private class EventItem
        {
            public GameObject gameObject;
            public Text textComponent;
            public float createTime;
            public float lifetime;
            public bool isAnimating;
            
            public EventItem(GameObject obj, Text text, float time, float life)
            {
                gameObject = obj;
                textComponent = text;
                createTime = time;
                lifetime = life;
                isAnimating = false;
            }
        }
        
        /// <summary>
        /// Initialize event feed system.
        /// </summary>
        private void Start()
        {
            InitializeEventFeed();
        }
        
        /// <summary>
        /// Initialize event feed components and setup.
        /// </summary>
        private void InitializeEventFeed()
        {
            // Create container if needed
            if (autoCreateContainer && feedContainer == null)
            {
                CreateFeedContainer();
            }
            
            // Create event prefab if needed
            if (autoCreatePrefab && eventTextPrefab == null)
            {
                CreateEventTextPrefab();
            }
            
            // Validate setup
            if (feedContainer == null)
            {
                Debug.LogError("EventFeed: No feed container assigned and auto-create failed!");
                return;
            }
            
            if (eventTextPrefab == null)
            {
                Debug.LogError("EventFeed: No event text prefab assigned and auto-create failed!");
                return;
            }
        }
        
        /// <summary>
        /// Create feed container automatically.
        /// </summary>
        private void CreateFeedContainer()
        {
            GameObject container = new GameObject("EventFeedContainer");
            container.transform.SetParent(transform);
            container.transform.localPosition = Vector3.zero;
            container.transform.localRotation = Quaternion.identity;
            container.transform.localScale = Vector3.one;
            
            // Add RectTransform for UI
            RectTransform rectTransform = container.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // Add Vertical Layout Group
            VerticalLayoutGroup layoutGroup = container.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = spacing;
            layoutGroup.childAlignment = TextAnchor.LowerLeft;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;
            
            // Add Content Size Fitter
            ContentSizeFitter sizeFitter = container.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            feedContainer = container.transform;
        }
        
        /// <summary>
        /// Create event text prefab automatically.
        /// </summary>
        private void CreateEventTextPrefab()
        {
            GameObject prefab = new GameObject("EventText");
            
            // Add RectTransform
            RectTransform rectTransform = prefab.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 30);
            
            // Add Text component
            Text textComponent = prefab.AddComponent<Text>();
            textComponent.text = "Event Text";
            textComponent.fontSize = 14;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleLeft;
            
            // Add Layout Element
            LayoutElement layoutElement = prefab.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 30;
            layoutElement.minHeight = 30;
            
            eventTextPrefab = prefab;
        }
        
        /// <summary>
        /// Add a new event to the feed.
        /// </summary>
        public void Push(string message, Color color)
        {
            if (feedContainer == null || eventTextPrefab == null)
            {
                Debug.LogWarning("EventFeed: Cannot push event - container or prefab not available");
                return;
            }
            
            // Create new event item
            GameObject eventObj = Instantiate(eventTextPrefab, feedContainer);
            Text textComponent = eventObj.GetComponent<Text>();
            
            if (textComponent != null)
            {
                textComponent.text = message;
                textComponent.color = color;
            }
            
            EventItem eventItem = new EventItem(eventObj, textComponent, Time.time, eventLifetime);
            activeEvents.Add(eventItem);
            
            // Remove oldest events if we exceed max
            while (activeEvents.Count > maxEvents)
            {
                RemoveOldestEvent();
            }
            
            // Animate new event
            StartCoroutine(AnimateEventIn(eventItem));
        }
        
        /// <summary>
        /// Animate event sliding in from the right.
        /// </summary>
        private System.Collections.IEnumerator AnimateEventIn(EventItem eventItem)
        {
            if (eventItem.gameObject == null) yield break;
            
            eventItem.isAnimating = true;
            
            RectTransform rectTransform = eventItem.gameObject.GetComponent<RectTransform>();
            if (rectTransform == null) yield break;
            
            Vector3 startPos = rectTransform.anchoredPosition + new Vector2(200, 0);
            Vector3 endPos = rectTransform.anchoredPosition;
            
            float elapsed = 0f;
            float duration = 0.3f;
            
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                t = slideCurve.Evaluate(t);
                
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            rectTransform.anchoredPosition = endPos;
            eventItem.isAnimating = false;
        }
        
        /// <summary>
        /// Update event feed each frame.
        /// </summary>
        private void Update()
        {
            UpdateEventLifetimes();
        }
        
        /// <summary>
        /// Update event lifetimes and remove expired events.
        /// </summary>
        private void UpdateEventLifetimes()
        {
            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                EventItem eventItem = activeEvents[i];
                
                if (eventItem.gameObject == null)
                {
                    activeEvents.RemoveAt(i);
                    continue;
                }
                
                float age = Time.time - eventItem.createTime;
                
                // Start fade out when approaching lifetime
                if (age >= eventItem.lifetime - fadeDuration)
                {
                    StartCoroutine(FadeOutEvent(eventItem));
                }
                
                // Remove when lifetime exceeded
                if (age >= eventItem.lifetime)
                {
                    RemoveEvent(eventItem);
                    activeEvents.RemoveAt(i);
                }
            }
        }
        
        /// <summary>
        /// Fade out an event before removing it.
        /// </summary>
        private System.Collections.IEnumerator FadeOutEvent(EventItem eventItem)
        {
            if (eventItem.gameObject == null || eventItem.textComponent == null) yield break;
            
            Color originalColor = eventItem.textComponent.color;
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                if (eventItem.gameObject == null) yield break;
                
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                Color newColor = originalColor;
                newColor.a = alpha;
                eventItem.textComponent.color = newColor;
                
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        /// <summary>
        /// Remove an event from the feed.
        /// </summary>
        private void RemoveEvent(EventItem eventItem)
        {
            if (eventItem.gameObject != null)
            {
                Destroy(eventItem.gameObject);
            }
        }
        
        /// <summary>
        /// Remove the oldest event from the feed.
        /// </summary>
        private void RemoveOldestEvent()
        {
            if (activeEvents.Count > 0)
            {
                EventItem oldestEvent = activeEvents[0];
                RemoveEvent(oldestEvent);
                activeEvents.RemoveAt(0);
            }
        }
        
        /// <summary>
        /// Clear all events from the feed.
        /// </summary>
        public void Clear()
        {
            foreach (EventItem eventItem in activeEvents)
            {
                RemoveEvent(eventItem);
            }
            activeEvents.Clear();
        }
        
        /// <summary>
        /// Set maximum number of events to display.
        /// </summary>
        public void SetMaxEvents(int max)
        {
            maxEvents = Mathf.Max(1, max);
            
            // Remove excess events
            while (activeEvents.Count > maxEvents)
            {
                RemoveOldestEvent();
            }
        }
        
        /// <summary>
        /// Set event lifetime duration.
        /// </summary>
        public void SetEventLifetime(float lifetime)
        {
            eventLifetime = Mathf.Max(0.1f, lifetime);
        }
        
        /// <summary>
        /// Set fade duration for events.
        /// </summary>
        public void SetFadeDuration(float duration)
        {
            fadeDuration = Mathf.Max(0.1f, duration);
        }
        
        /// <summary>
        /// Get current number of active events.
        /// </summary>
        public int GetActiveEventCount()
        {
            return activeEvents.Count;
        }
        
        /// <summary>
        /// Check if event feed is properly initialized.
        /// </summary>
        public bool IsInitialized()
        {
            return feedContainer != null && eventTextPrefab != null;
        }
    }
}
