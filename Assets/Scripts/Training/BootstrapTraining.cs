#define DISABLE_PP

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NeuralRink.Training
{
    public sealed class BootstrapTraining : MonoBehaviour
    {
        [Header("Bootstrap Configuration")]
        [SerializeField] private bool enableTrainingMode = true;
        [SerializeField] private bool setupCameras = true;
        [SerializeField] private bool configurePhysics = true;

        private NeuralRink.Systems.TrainingSwitch trainingSwitch;
        private NeuralRink.Gameplay.GameDirector gameDirector;
        private NeuralRink.Gameplay.RinkSetup rinkSetup;
        private NeuralRink.Agents.GoalieAgent goalieAgent;

        void Start()
        {
            InitializeBootstrap();
        }

        void InitializeBootstrap()
        {
            Debug.Log("🚀 Bootstrap Training - Initializing...");
            
            FindReferences();
            ConfigureTrainingMode();
            SetupEnvironment();
            
            Debug.Log("✅ Bootstrap Training - Complete!");
        }

        void FindReferences()
        {
            if (trainingSwitch == null)
            {
                trainingSwitch = FindFirstObjectByType<NeuralRink.Systems.TrainingSwitch>();
            }
            
            if (gameDirector == null)
            {
                gameDirector = FindFirstObjectByType<NeuralRink.Gameplay.GameDirector>();
            }
            
            if (rinkSetup == null)
            {
                rinkSetup = FindFirstObjectByType<NeuralRink.Gameplay.RinkSetup>();
            }
            
            if (goalieAgent == null)
            {
                goalieAgent = FindFirstObjectByType<NeuralRink.Agents.GoalieAgent>();
            }
        }

        void ConfigureTrainingMode()
        {
            if (trainingSwitch != null && enableTrainingMode)
            {
                Time.timeScale = trainingSwitch.TrainingTimeScale;
                Debug.Log($"Training mode enabled - Time scale: {trainingSwitch.TrainingTimeScale}x");
            }
        }

        void SetupEnvironment()
        {
            if (setupCameras)
            {
                SetupCameras();
            }
            
            if (configurePhysics)
            {
                ConfigurePhysics();
            }
        }

        void SetupCameras()
        {
            var cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (var cam in cameras)
            {
                // Disable post-processing for training performance
#if !DISABLE_PP
                // Post-processing setup would go here when package is available
#endif
                
                // Optimize camera settings for training
                cam.renderingPath = RenderingPath.Forward;
            }
        }

        void ConfigurePhysics()
        {
            // Optimize physics for training
            Physics.defaultSolverIterations = 6;
            Physics.defaultSolverVelocityIterations = 1;
        }

        void OnDestroy()
        {
            // Reset time scale when bootstrap is destroyed
            Time.timeScale = 1f;
        }
    }
}