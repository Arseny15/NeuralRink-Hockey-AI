using UnityEngine;
using NeuralRink.Systems;

namespace NeuralRink.Training
{
    /// <summary>
    /// Bootstrap script for training scene setup.
    /// Handles training-specific initialization and configuration.
    /// </summary>
    public class BootstrapTraining : MonoBehaviour
    {
        [Header("Training Configuration")]
        [SerializeField] private TrainingSwitch trainingSwitch;
        [SerializeField] private bool autoStartTraining = true;
        [SerializeField] private bool enableDebugLogs = true;
        
        [Header("Performance Optimization")]
        [SerializeField] private GameObject[] stripWhenTraining;
        [SerializeField] private bool disableAudio = true;
        [SerializeField] private bool disableParticles = true;
        [SerializeField] private bool disableAnimations = true;
        
        [Header("Scene References")]
        [SerializeField] private NeuralRink.Gameplay.GameDirector gameDirector;
        [SerializeField] private NeuralRink.Gameplay.RinkSetup rinkSetup;
        [SerializeField] private NeuralRink.Agents.GoalieAgent goalieAgent;
        
        /// <summary>
        /// Initialize training bootstrap.
        /// </summary>
        private void Start()
        {
            InitializeTrainingEnvironment();
        }
        
        /// <summary>
        /// Initialize training environment and optimize for ML training.
        /// </summary>
        private void InitializeTrainingEnvironment()
        {
            Debug.Log("Initializing Neural Rink Training Environment...");
            
            // Find required components
            FindRequiredComponents();
            
            // Configure training switch
            ConfigureTrainingSwitch();
            
            // Optimize scene for training
            OptimizeSceneForTraining();
            
            // Start training if enabled
            if (autoStartTraining)
            {
                StartTraining();
            }
            
            Debug.Log("Training environment initialization complete");
        }
        
        /// <summary>
        /// Find and cache required components.
        /// </summary>
        private void FindRequiredComponents()
        {
            // Find training switch
            if (trainingSwitch == null)
            {
                trainingSwitch = FindObjectOfType<TrainingSwitch>();
                if (trainingSwitch == null)
                {
                    Debug.LogError("BootstrapTraining: TrainingSwitch not found! Please assign or create one.");
                }
            }
            
            // Find game director
            if (gameDirector == null)
            {
                gameDirector = FindObjectOfType<NeuralRink.Gameplay.GameDirector>();
                if (gameDirector == null)
                {
                    Debug.LogError("BootstrapTraining: GameDirector not found! Please assign or create one.");
                }
            }
            
            // Find rink setup
            if (rinkSetup == null)
            {
                rinkSetup = FindObjectOfType<NeuralRink.Gameplay.RinkSetup>();
                if (rinkSetup == null)
                {
                    Debug.LogError("BootstrapTraining: RinkSetup not found! Please assign or create one.");
                }
            }
            
            // Find goalie agent
            if (goalieAgent == null)
            {
                goalieAgent = FindObjectOfType<NeuralRink.Agents.GoalieAgent>();
                if (goalieAgent == null)
                {
                    Debug.LogError("BootstrapTraining: GoalieAgent not found! Please assign or create one.");
                }
            }
        }
        
        /// <summary>
        /// Configure training switch for training mode.
        /// </summary>
        private void ConfigureTrainingSwitch()
        {
            if (trainingSwitch == null) return;
            
            // Ensure training mode is enabled
            trainingSwitch.SetTrainingMode(true);
            
            // Configure training parameters
            if (enableDebugLogs)
            {
                // Enable debug logs for training
                Debug.Log("Training mode enabled with debug logging");
            }
            
            // Set deterministic physics
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f; // 50 FPS for consistent physics
            
            Debug.Log("Training switch configured for training mode");
        }
        
        /// <summary>
        /// Optimize scene for training performance.
        /// </summary>
        private void OptimizeSceneForTraining()
        {
            // Disable audio for training
            if (disableAudio)
            {
                AudioListener.volume = 0f;
                Debug.Log("Audio disabled for training");
            }
            
            // Strip unnecessary objects
            if (stripWhenTraining != null)
            {
                foreach (GameObject obj in stripWhenTraining)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }
                Debug.Log($"Stripped {stripWhenTraining.Length} objects for training");
            }
            
            // Disable particles
            if (disableParticles)
            {
                ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
                foreach (ParticleSystem ps in particles)
                {
                    ps.gameObject.SetActive(false);
                }
                Debug.Log($"Disabled {particles.Length} particle systems for training");
            }
            
            // Disable animations
            if (disableAnimations)
            {
                Animator[] animators = FindObjectsOfType<Animator>();
                foreach (Animator anim in animators)
                {
                    anim.enabled = false;
                }
                Debug.Log($"Disabled {animators.Length} animators for training");
            }
            
            // Optimize camera for training
            OptimizeCameraForTraining();
            
            // Optimize lighting for training
            OptimizeLightingForTraining();
        }
        
        /// <summary>
        /// Optimize camera settings for training.
        /// </summary>
        private void OptimizeCameraForTraining()
        {
            Camera[] cameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
            {
                // Disable post-processing effects
                if (cam.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>() != null)
                {
                    cam.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().enabled = false;
                }
                
                // Set simple rendering path
                cam.renderingPath = RenderingPath.Forward;
                
                // Disable HDR for performance
                cam.allowHDR = false;
                cam.allowMSAA = false;
            }
            
            Debug.Log($"Optimized {cameras.Length} cameras for training");
        }
        
        /// <summary>
        /// Optimize lighting for training.
        /// </summary>
        private void OptimizeLightingForTraining()
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                // Disable shadows for performance
                light.shadows = LightShadows.None;
                
                // Disable cookies and flares
                light.cookie = null;
                light.flare = null;
            }
            
            Debug.Log($"Optimized {lights.Length} lights for training");
        }
        
        /// <summary>
        /// Start training session.
        /// </summary>
        public void StartTraining()
        {
            if (trainingSwitch == null)
            {
                Debug.LogError("Cannot start training - TrainingSwitch not found!");
                return;
            }
            
            Debug.Log("Starting ML-Agents training session...");
            
            // Start training episode
            if (gameDirector != null)
            {
                gameDirector.OnEpisodeBegin();
            }
            
            // Log training configuration
            LogTrainingConfiguration();
        }
        
        /// <summary>
        /// Log current training configuration.
        /// </summary>
        private void LogTrainingConfiguration()
        {
            Debug.Log("=== Training Configuration ===");
            Debug.Log($"Training Mode: {trainingSwitch.IsTrainingMode()}");
            Debug.Log($"Episode Length: {trainingSwitch.GetEpisodeLength()}s");
            Debug.Log($"Max Episodes: {trainingSwitch.GetMaxEpisodes()}");
            Debug.Log($"Physics Randomization: {trainingSwitch.ShouldRandomizePhysics()}");
            Debug.Log($"Debug Logs: {trainingSwitch.ShouldEnableDebugLogs()}");
            Debug.Log($"Time Scale: {Time.timeScale}");
            Debug.Log($"Fixed Delta Time: {Time.fixedDeltaTime}");
            Debug.Log("=============================");
        }
        
        /// <summary>
        /// Stop training session.
        /// </summary>
        public void StopTraining()
        {
            Debug.Log("Stopping training session...");
            
            if (gameDirector != null)
            {
                gameDirector.ForceEndEpisode();
            }
            
            if (trainingSwitch != null)
            {
                trainingSwitch.EndEpisode();
            }
        }
        
        /// <summary>
        /// Reset training environment.
        /// </summary>
        public void ResetTrainingEnvironment()
        {
            Debug.Log("Resetting training environment...");
            
            // Reset rink setup
            if (rinkSetup != null)
            {
                rinkSetup.ResetRink();
            }
            
            // Reset goalie agent
            if (goalieAgent != null)
            {
                goalieAgent.ResetPosition();
            }
            
            // Start new episode
            if (gameDirector != null)
            {
                gameDirector.OnEpisodeBegin();
            }
        }
        
        /// <summary>
        /// Validate training setup.
        /// </summary>
        public bool ValidateTrainingSetup()
        {
            bool isValid = true;
            
            if (trainingSwitch == null)
            {
                Debug.LogError("Validation failed: TrainingSwitch not found");
                isValid = false;
            }
            
            if (gameDirector == null)
            {
                Debug.LogError("Validation failed: GameDirector not found");
                isValid = false;
            }
            
            if (rinkSetup == null)
            {
                Debug.LogError("Validation failed: RinkSetup not found");
                isValid = false;
            }
            
            if (goalieAgent == null)
            {
                Debug.LogError("Validation failed: GoalieAgent not found");
                isValid = false;
            }
            
            if (isValid)
            {
                Debug.Log("Training setup validation passed!");
            }
            else
            {
                Debug.LogError("Training setup validation failed!");
            }
            
            return isValid;
        }
        
        /// <summary>
        /// Get training statistics.
        /// </summary>
        public string GetTrainingStats()
        {
            if (trainingSwitch == null) return "TrainingSwitch not available";
            
            return $"Training Stats:\n" +
                   $"Episodes: {trainingSwitch.GetCurrentEpisode()}/{trainingSwitch.GetMaxEpisodes()}\n" +
                   $"Episode Time: {trainingSwitch.GetCurrentEpisodeTime():F2}s\n" +
                   $"Training Mode: {trainingSwitch.IsTrainingMode()}\n" +
                   $"Episode Active: {trainingSwitch.IsEpisodeActive()}";
        }
        
        /// <summary>
        /// Handle application focus changes.
        /// </summary>
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && trainingSwitch != null && trainingSwitch.IsTrainingMode())
            {
                Debug.Log("Application lost focus - training may be affected");
            }
        }
        
        /// <summary>
        /// Handle application pause.
        /// </summary>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && trainingSwitch != null && trainingSwitch.IsTrainingMode())
            {
                Debug.Log("Application paused - training may be affected");
            }
        }
    }
}
