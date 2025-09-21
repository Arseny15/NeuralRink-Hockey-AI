using UnityEngine;
using UnityEngine.UI;
using NeuralRink.Systems;

namespace NeuralRink.Utils
{
    /// <summary>
    /// Specialized HUD component for displaying salary and performance information.
    /// Provides visual feedback for salary changes and episode events.
    /// </summary>
    public class SalaryHUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text salaryText;
        [SerializeField] private Text performanceText;
        [SerializeField] private Text episodeText;
        [SerializeField] private Text trainingModeText;
        
        [Header("Progress Bars")]
        [SerializeField] private Slider savePercentageBar;
        [SerializeField] private Slider episodeProgressBar;
        
        [Header("Animation Settings")]
        [SerializeField] private float pulseDuration = 0.5f;
        [SerializeField] private float pulseScale = 1.2f;
        [SerializeField] private float fadeDuration = 2f;
        
        [Header("System References")]
        [SerializeField] private SalarySystem salarySystem;
        [SerializeField] private TrainingSwitch trainingSwitch;
        
        // Animation state
        private bool isPulsing = false;
        private Vector3 originalScale;
        private Color originalColor;
        
        /// <summary>
        /// Initialize salary HUD.
        /// </summary>
        private void Start()
        {
            InitializeHUD();
            SubscribeToEvents();
            UpdateDisplay();
        }
        
        /// <summary>
        /// Initialize HUD components and state.
        /// </summary>
        private void InitializeHUD()
        {
            // Cache original scale and color
            originalScale = transform.localScale;
            
            if (salaryText != null)
            {
                originalColor = salaryText.color;
            }
            
            // Find system references if not assigned
            if (salarySystem == null)
            {
                salarySystem = FindObjectOfType<SalarySystem>();
            }
            
            if (trainingSwitch == null)
            {
                trainingSwitch = FindObjectOfType<TrainingSwitch>();
            }
            
            // Setup progress bars
            SetupProgressBars();
        }
        
        /// <summary>
        /// Setup progress bar configurations.
        /// </summary>
        private void SetupProgressBars()
        {
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
        }
        
        /// <summary>
        /// Subscribe to salary system events.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (salarySystem != null)
            {
                SalarySystem.OnSalaryChanged += OnSalaryChanged;
                SalarySystem.OnPerformanceUpdated += OnPerformanceUpdated;
                SalarySystem.OnBonusEarned += OnBonusEarned;
                SalarySystem.OnPenaltyApplied += OnPenaltyApplied;
            }
        }
        
        /// <summary>
        /// Update all HUD elements.
        /// </summary>
        public void UpdateDisplay()
        {
            UpdateSalaryDisplay();
            UpdatePerformanceDisplay();
            UpdateEpisodeDisplay();
            UpdateTrainingModeDisplay();
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
                    UpdateSaveBarColor(stats.percentage);
                }
            }
        }
        
        /// <summary>
        /// Update episode information display.
        /// </summary>
        private void UpdateEpisodeDisplay()
        {
            if (trainingSwitch != null && trainingSwitch.IsTrainingMode())
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
        
        /// <summary>
        /// Update training mode display.
        /// </summary>
        private void UpdateTrainingModeDisplay()
        {
            if (trainingModeText != null && trainingSwitch != null)
            {
                if (trainingSwitch.IsTrainingMode())
                {
                    trainingModeText.text = "TRAINING MODE";
                    trainingModeText.color = Color.yellow;
                }
                else
                {
                    trainingModeText.text = "PLAY MODE";
                    trainingModeText.color = Color.cyan;
                }
            }
        }
        
        /// <summary>
        /// Update save percentage bar color based on performance.
        /// </summary>
        private void UpdateSaveBarColor(float percentage)
        {
            if (savePercentageBar == null) return;
            
            var fillImage = savePercentageBar.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                if (percentage >= 80f)
                    fillImage.color = Color.green;
                else if (percentage >= 50f)
                    fillImage.color = Color.yellow;
                else
                    fillImage.color = Color.red;
            }
        }
        
        /// <summary>
        /// Handle salary change event.
        /// </summary>
        private void OnSalaryChanged(float newSalary)
        {
            UpdateSalaryDisplay();
        }
        
        /// <summary>
        /// Handle performance update event.
        /// </summary>
        private void OnPerformanceUpdated(int saves, int goals, float percentage)
        {
            UpdatePerformanceDisplay();
        }
        
        /// <summary>
        /// Handle bonus earned event.
        /// </summary>
        private void OnBonusEarned(float bonusAmount)
        {
            // Could add special bonus animation here
            Debug.Log($"Bonus earned: ${bonusAmount:N0}");
        }
        
        /// <summary>
        /// Handle penalty applied event.
        /// </summary>
        private void OnPenaltyApplied(float penaltyAmount)
        {
            // Could add special penalty animation here
            Debug.Log($"Penalty applied: ${penaltyAmount:N0}");
        }
        
        /// <summary>
        /// Pulse the episode display with a specific color.
        /// </summary>
        public void PulseEpisode(Color pulseColor)
        {
            if (isPulsing) return;
            
            StartCoroutine(PulseCoroutine(pulseColor));
        }
        
        /// <summary>
        /// Coroutine for pulsing animation.
        /// </summary>
        private System.Collections.IEnumerator PulseCoroutine(Color pulseColor)
        {
            isPulsing = true;
            
            // Pulse scale
            float elapsed = 0f;
            while (elapsed < pulseDuration)
            {
                float scale = Mathf.Lerp(1f, pulseScale, Mathf.Sin(elapsed / pulseDuration * Mathf.PI));
                transform.localScale = originalScale * scale;
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Reset scale
            transform.localScale = originalScale;
            
            // Flash color
            if (salaryText != null)
            {
                Color originalTextColor = salaryText.color;
                salaryText.color = pulseColor;
                
                yield return new WaitForSeconds(0.1f);
                
                salaryText.color = originalTextColor;
            }
            
            isPulsing = false;
        }
        
        /// <summary>
        /// Fade out the HUD.
        /// </summary>
        public void FadeOut()
        {
            StartCoroutine(FadeCoroutine(0f));
        }
        
        /// <summary>
        /// Fade in the HUD.
        /// </summary>
        public void FadeIn()
        {
            StartCoroutine(FadeCoroutine(1f));
        }
        
        /// <summary>
        /// Coroutine for fade animation.
        /// </summary>
        private System.Collections.IEnumerator FadeCoroutine(float targetAlpha)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            canvasGroup.alpha = targetAlpha;
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
                SalarySystem.OnBonusEarned -= OnBonusEarned;
                SalarySystem.OnPenaltyApplied -= OnPenaltyApplied;
            }
        }
        
        /// <summary>
        /// Set salary system reference.
        /// </summary>
        public void SetSalarySystem(SalarySystem system)
        {
            salarySystem = system;
            SubscribeToEvents();
        }
        
        /// <summary>
        /// Set training switch reference.
        /// </summary>
        public void SetTrainingSwitch(TrainingSwitch training)
        {
            trainingSwitch = training;
        }
        
        /// <summary>
        /// Enable or disable HUD visibility.
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
