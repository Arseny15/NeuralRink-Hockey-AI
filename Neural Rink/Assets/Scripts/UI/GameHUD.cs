using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NeuralRink.Systems;

namespace NeuralRink.UI
{
    /// <summary>
    /// Game HUD system displaying salary, performance metrics, and game state.
    /// Provides real-time feedback for both training and play modes.
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI salaryText;
        [SerializeField] private TextMeshProUGUI performanceText;
        [SerializeField] private TextMeshProUGUI gameModeText;
        [SerializeField] private TextMeshProUGUI episodeText;
        [SerializeField] private TextMeshProUGUI fpsText;
        
        [Header("Progress Bars")]
        [SerializeField] private Slider savePercentageBar;
        [SerializeField] private Slider episodeProgressBar;
        
        [Header("UI Configuration")]
        [SerializeField] private float updateInterval = 0.1f;
        [SerializeField] private bool showFPS = true;
        [SerializeField] private bool showEpisodeInfo = true;
        
        [Header("Color Configuration")]
        [SerializeField] private Color saveColor = Color.green;
        [SerializeField] private Color goalColor = Color.red;
        [SerializeField] private Color neutralColor = Color.white;
        
        // References
        private SalarySystem salarySystem;
        private TrainingSwitch trainingSwitch;
        private NeuralRink.Gameplay.PlayerController playerController;
        
        // Update timing
        private float lastUpdateTime;
        private float fpsUpdateTime;
        private int frameCount;
        private float currentFPS;
        
        /// <summary>
        /// Initialize HUD system and find required components.
        /// </summary>
        private void Start()
        {
            FindRequiredComponents();
            SetupUI();
            UpdateHUD();
        }
        
        /// <summary>
        /// Find and cache references to required components.
        /// </summary>
        private void FindRequiredComponents()
        {
            salarySystem = FindObjectOfType<SalarySystem>();
            trainingSwitch = FindObjectOfType<TrainingSwitch>();
            playerController = FindObjectOfType<NeuralRink.Gameplay.PlayerController>();
            
            // Subscribe to salary system events
            if (salarySystem != null)
            {
                SalarySystem.OnSalaryChanged += OnSalaryChanged;
                SalarySystem.OnPerformanceUpdated += OnPerformanceUpdated;
            }
        }
        
        /// <summary>
        /// Setup UI elements and initial state.
        /// </summary>
        private void SetupUI()
        {
            // Setup progress bars
            if (savePercentageBar != null)
            {
                savePercentageBar.minValue = 0f;
                savePercentageBar.maxValue = 100f;
                savePercentageBar.value = 0f;
            }
            
            if (episodeProgressBar != null)
            {
                episodeProgressBar.minValue = 0f;
                episodeProgressBar.maxValue = 1f;
                episodeProgressBar.value = 0f;
            }
            
            // Initial UI state
            UpdateGameModeDisplay();
        }
        
        /// <summary>
        /// Update HUD display at regular intervals.
        /// </summary>
        private void Update()
        {
            // Update FPS counter
            if (showFPS)
            {
                UpdateFPSCounter();
            }
            
            // Update HUD at intervals
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                UpdateHUD();
                lastUpdateTime = Time.time;
            }
        }
        
        /// <summary>
        /// Update FPS counter display.
        /// </summary>
        private void UpdateFPSCounter()
        {
            frameCount++;
            
            if (Time.time - fpsUpdateTime >= 1f)
            {
                currentFPS = frameCount / (Time.time - fpsUpdateTime);
                frameCount = 0;
                fpsUpdateTime = Time.time;
                
                if (fpsText != null)
                {
                    fpsText.text = $"FPS: {currentFPS:F1}";
                }
            }
        }
        
        /// <summary>
        /// Update all HUD elements.
        /// </summary>
        private void UpdateHUD()
        {
            UpdateSalaryDisplay();
            UpdatePerformanceDisplay();
            UpdateEpisodeDisplay();
        }
        
        /// <summary>
        /// Update salary display.
        /// </summary>
        private void UpdateSalaryDisplay()
        {
            if (salaryText != null && salarySystem != null)
            {
                salaryText.text = $"Salary: {salarySystem.GetFormattedSalary()}";
            }
        }
        
        /// <summary>
        /// Update performance metrics display.
        /// </summary>
        private void UpdatePerformanceDisplay()
        {
            if (salarySystem != null)
            {
                var stats = salarySystem.GetPerformanceStats();
                
                if (performanceText != null)
                {
                    performanceText.text = $"Saves: {stats.saves}/{stats.shots} ({stats.percentage:F1}%)";
                }
                
                if (savePercentageBar != null)
                {
                    savePercentageBar.value = stats.percentage;
                    
                    // Update bar color based on performance
                    var fillImage = savePercentageBar.fillRect.GetComponent<Image>();
                    if (fillImage != null)
                    {
                        if (stats.percentage >= 80f)
                            fillImage.color = saveColor;
                        else if (stats.percentage >= 50f)
                            fillImage.color = neutralColor;
                        else
                            fillImage.color = goalColor;
                    }
                }
            }
        }
        
        /// <summary>
        /// Update episode information display.
        /// </summary>
        private void UpdateEpisodeDisplay()
        {
            if (trainingSwitch != null && showEpisodeInfo)
            {
                if (trainingSwitch.IsTrainingMode())
                {
                    if (episodeText != null)
                    {
                        episodeText.text = $"Episode: {trainingSwitch.GetCurrentEpisode()}/{trainingSwitch.GetMaxEpisodes()}";
                    }
                    
                    if (episodeProgressBar != null)
                    {
                        float progress = (float)trainingSwitch.GetCurrentEpisode() / trainingSwitch.GetMaxEpisodes();
                        episodeProgressBar.value = progress;
                    }
                }
                else
                {
                    if (episodeText != null)
                    {
                        episodeText.text = "Play Mode";
                    }
                    
                    if (episodeProgressBar != null)
                    {
                        episodeProgressBar.value = 0f;
                    }
                }
            }
        }
        
        /// <summary>
        /// Update game mode display.
        /// </summary>
        private void UpdateGameModeDisplay()
        {
            if (gameModeText != null && trainingSwitch != null)
            {
                string modeText = trainingSwitch.IsTrainingMode() ? "TRAINING MODE" : "PLAY MODE";
                gameModeText.text = modeText;
                gameModeText.color = trainingSwitch.IsTrainingMode() ? Color.yellow : Color.cyan;
            }
        }
        
        /// <summary>
        /// Handle salary change event.
        /// </summary>
        private void OnSalaryChanged(float newSalary)
        {
            if (salaryText != null)
            {
                // Animate salary change
                StartCoroutine(AnimateTextChange(salaryText, $"Salary: ${newSalary:N0}"));
            }
        }
        
        /// <summary>
        /// Handle performance update event.
        /// </summary>
        private void OnPerformanceUpdated(int saves, int goals, float percentage)
        {
            if (performanceText != null)
            {
                string performanceString = $"Saves: {saves}/{saves + goals} ({percentage:F1}%)";
                StartCoroutine(AnimateTextChange(performanceText, performanceString));
            }
            
            if (savePercentageBar != null)
            {
                StartCoroutine(AnimateSliderChange(savePercentageBar, percentage));
            }
        }
        
        /// <summary>
        /// Animate text change with scaling effect.
        /// </summary>
        private System.Collections.IEnumerator AnimateTextChange(TextMeshProUGUI textComponent, string newText)
        {
            if (textComponent == null) yield break;
            
            // Scale down
            float duration = 0.1f;
            float elapsed = 0f;
            Vector3 originalScale = textComponent.transform.localScale;
            
            while (elapsed < duration)
            {
                float scale = Mathf.Lerp(1f, 0.8f, elapsed / duration);
                textComponent.transform.localScale = originalScale * scale;
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Update text
            textComponent.text = newText;
            
            // Scale back up
            elapsed = 0f;
            while (elapsed < duration)
            {
                float scale = Mathf.Lerp(0.8f, 1f, elapsed / duration);
                textComponent.transform.localScale = originalScale * scale;
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            textComponent.transform.localScale = originalScale;
        }
        
        /// <summary>
        /// Animate slider value change.
        /// </summary>
        private System.Collections.IEnumerator AnimateSliderChange(Slider slider, float targetValue)
        {
            if (slider == null) yield break;
            
            float startValue = slider.value;
            float duration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                slider.value = Mathf.Lerp(startValue, targetValue, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            slider.value = targetValue;
        }
        
        /// <summary>
        /// Cleanup event subscriptions.
        /// </summary>
        private void OnDestroy()
        {
            if (salarySystem != null)
            {
                SalarySystem.OnSalaryChanged -= OnSalaryChanged;
                SalarySystem.OnPerformanceUpdated -= OnPerformanceUpdated;
            }
        }
        
        /// <summary>
        /// Toggle HUD visibility.
        /// </summary>
        public void ToggleHUD(bool visible)
        {
            gameObject.SetActive(visible);
        }
        
        /// <summary>
        /// Set update interval for HUD refresh.
        /// </summary>
        public void SetUpdateInterval(float interval)
        {
            updateInterval = Mathf.Max(0.01f, interval);
        }
        
        /// <summary>
        /// Show or hide FPS counter.
        /// </summary>
        public void SetShowFPS(bool show)
        {
            showFPS = show;
            if (fpsText != null)
            {
                fpsText.gameObject.SetActive(show);
            }
        }
        
        /// <summary>
        /// Show or hide episode information.
        /// </summary>
        public void SetShowEpisodeInfo(bool show)
        {
            showEpisodeInfo = show;
            if (episodeText != null)
            {
                episodeText.gameObject.SetActive(show);
            }
            if (episodeProgressBar != null)
            {
                episodeProgressBar.gameObject.SetActive(show);
            }
        }
    }
}
