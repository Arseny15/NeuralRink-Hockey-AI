using UnityEngine;
using NeuralRink.Systems;
using NeuralRink.Agents;
using NeuralRink.Gameplay;
using NeuralRink.UI;
using NeuralRink.Utils;

namespace NeuralRink.Gameplay
{
    /// <summary>
    /// Central game manager coordinating all systems for Neural Rink.
    /// Handles game state, scene transitions, and system initialization.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("System References")]
        [SerializeField] private TrainingSwitch trainingSwitch;
        [SerializeField] private SalarySystem salarySystem;
        [SerializeField] private RinkSetup rinkSetup;
        [SerializeField] private GameHUD gameHUD;
        [SerializeField] private TelemetryLogger telemetryLogger;
        
        [Header("Game Configuration")]
        [SerializeField] private float gameStartDelay = 1f;
        [SerializeField] private bool autoResetOnGoal = true;
        [SerializeField] private float resetDelay = 2f;
        
        [Header("Audio Configuration")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip goalSound;
        [SerializeField] private AudioClip saveSound;
        [SerializeField] private AudioClip gameStartSound;
        
        // Game state
        private bool isGameActive = false;
        private bool isGamePaused = false;
        private float gameStartTime;
        private int totalRounds = 0;
        private int playerWins = 0;
        private int goalieWins = 0;
        
        // References to game objects
        private GameObject player;
        private GameObject goalie;
        private GameObject puck;
        private GoalieAgent goalieAgent;
        private PlayerController playerController;
        private PuckController puckController;
        
        /// <summary>
        /// Initialize game manager and all systems.
        /// </summary>
        private void Awake()
        {
            InitializeGameManager();
        }
        
        /// <summary>
        /// Initialize all game systems and find references.
        /// </summary>
        private void InitializeGameManager()
        {
            // Find or create required systems
            FindOrCreateSystems();
            
            // Find game objects
            FindGameObjects();
            
            // Setup event subscriptions
            SetupEventSubscriptions();
            
            // Initialize systems
            InitializeSystems();
            
            Debug.Log("Game Manager initialized successfully");
        }
        
        /// <summary>
        /// Find or create required system components.
        /// </summary>
        private void FindOrCreateSystems()
        {
            // Find training switch
            if (trainingSwitch == null)
            {
                trainingSwitch = FindFirstObjectByType<TrainingSwitch>();
                if (trainingSwitch == null)
                {
                    Debug.LogWarning("TrainingSwitch not found. Creating default configuration.");
                    // TODO: Create default TrainingSwitch ScriptableObject
                }
            }
            
            // Find or create salary system
            if (salarySystem == null)
            {
                salarySystem = FindFirstObjectByType<SalarySystem>();
                if (salarySystem == null)
                {
                    GameObject salaryObj = new GameObject("SalarySystem");
                    salarySystem = salaryObj.AddComponent<SalarySystem>();
                }
            }
            
            // Find or create rink setup
            if (rinkSetup == null)
            {
                rinkSetup = FindFirstObjectByType<RinkSetup>();
                if (rinkSetup == null)
                {
                    GameObject rinkObj = new GameObject("RinkSetup");
                    rinkSetup = rinkObj.AddComponent<RinkSetup>();
                }
            }
            
            // Find HUD
            if (gameHUD == null)
            {
                gameHUD = FindFirstObjectByType<GameHUD>();
            }
            
            // Find telemetry logger
            if (telemetryLogger == null)
            {
                telemetryLogger = FindFirstObjectByType<TelemetryLogger>();
            }
        }
        
        /// <summary>
        /// Find and cache references to game objects.
        /// </summary>
        private void FindGameObjects()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            goalie = GameObject.FindGameObjectWithTag("Goalie");
            puck = GameObject.FindGameObjectWithTag("Puck");
            
            if (player != null)
            {
                playerController = player.GetComponent<PlayerController>();
            }
            
            if (goalie != null)
            {
                goalieAgent = goalie.GetComponent<GoalieAgent>();
            }
            
            if (puck != null)
            {
                puckController = puck.GetComponent<PuckController>();
            }
        }
        
        /// <summary>
        /// Setup event subscriptions for game events.
        /// </summary>
        private void SetupEventSubscriptions()
        {
            // Subscribe to salary system events
            if (salarySystem != null)
            {
                SalarySystem.OnBonusEarned += OnSaveEvent;
                SalarySystem.OnPenaltyApplied += OnGoalEvent;
            }
        }
        
        /// <summary>
        /// Initialize all game systems.
        /// </summary>
        private void InitializeSystems()
        {
            // Set initial game state
            isGameActive = false;
            isGamePaused = false;
            
            // Initialize training switch
            if (trainingSwitch != null)
            {
                Time.timeScale = trainingSwitch.GetTimeScale();
            }
            
            // Start game after delay
            Invoke(nameof(StartGame), gameStartDelay);
        }
        
        /// <summary>
        /// Start the game session.
        /// </summary>
        public void StartGame()
        {
            isGameActive = true;
            gameStartTime = Time.time;
            totalRounds++;
            
            // Play start sound
            PlaySound(gameStartSound);
            
            // Start training episode if in training mode
            if (trainingSwitch != null && trainingSwitch.IsTrainingMode())
            {
                trainingSwitch.StartEpisode();
            }
            
            Debug.Log($"Game started - Round {totalRounds}");
        }
        
        /// <summary>
        /// Pause or unpause the game.
        /// </summary>
        public void TogglePause()
        {
            isGamePaused = !isGamePaused;
            Time.timeScale = isGamePaused ? 0f : (trainingSwitch?.GetTimeScale() ?? 1f);
            
            Debug.Log($"Game {(isGamePaused ? "paused" : "resumed")}");
        }
        
        /// <summary>
        /// Reset the current round.
        /// </summary>
        public void ResetRound()
        {
            if (!isGameActive) return;
            
            isGameActive = false;
            
            // Reset rink setup
            if (rinkSetup != null)
            {
                rinkSetup.ResetRink();
            }
            
            // Reset salary system
            if (salarySystem != null)
            {
                // Don't reset salary system completely, just continue tracking
            }
            
            // End current training episode
            if (trainingSwitch != null && trainingSwitch.IsTrainingMode())
            {
                trainingSwitch.EndEpisode();
            }
            
            Debug.Log("Round reset");
            
            // Start new round after delay
            if (autoResetOnGoal)
            {
                Invoke(nameof(StartGame), resetDelay);
            }
        }
        
        /// <summary>
        /// Handle save event (goalie successfully defended).
        /// </summary>
        private void OnSaveEvent(float bonusAmount)
        {
            goalieWins++;
            PlaySound(saveSound);
            
            Debug.Log($"SAVE! Goalie wins this round. Bonus: ${bonusAmount:N0}");
            
            if (autoResetOnGoal)
            {
                Invoke(nameof(ResetRound), resetDelay);
            }
        }
        
        /// <summary>
        /// Handle goal event (player scored).
        /// </summary>
        private void OnGoalEvent(float penaltyAmount)
        {
            playerWins++;
            PlaySound(goalSound);
            
            Debug.Log($"GOAL! Player wins this round. Penalty: ${penaltyAmount:N0}");
            
            if (autoResetOnGoal)
            {
                Invoke(nameof(ResetRound), resetDelay);
            }
        }
        
        /// <summary>
        /// Play audio clip if audio source is available.
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null && 
                trainingSwitch != null && trainingSwitch.ShouldEnableSound())
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        /// <summary>
        /// Switch between training and play modes.
        /// </summary>
        public void SwitchMode(bool trainingMode)
        {
            if (trainingSwitch != null)
            {
                trainingSwitch.SetTrainingMode(trainingMode);
                
                // Update UI visibility
                if (gameHUD != null)
                {
                    gameHUD.ToggleHUD(!trainingMode);
                }
                
                Debug.Log($"Switched to {(trainingMode ? "Training" : "Play")} mode");
            }
        }
        
        /// <summary>
        /// Get current game statistics.
        /// </summary>
        public (int rounds, int playerWins, int goalieWins, float gameTime) GetGameStats()
        {
            float gameTime = isGameActive ? Time.time - gameStartTime : 0f;
            return (totalRounds, playerWins, goalieWins, gameTime);
        }
        
        /// <summary>
        /// Check if game is currently active.
        /// </summary>
        public bool IsGameActive()
        {
            return isGameActive && !isGamePaused;
        }
        
        /// <summary>
        /// Check if game is paused.
        /// </summary>
        public bool IsGamePaused()
        {
            return isGamePaused;
        }
        
        /// <summary>
        /// Get win percentage for player.
        /// </summary>
        public float GetPlayerWinPercentage()
        {
            if (totalRounds == 0) return 0f;
            return (float)playerWins / totalRounds * 100f;
        }
        
        /// <summary>
        /// Get win percentage for goalie.
        /// </summary>
        public float GetGoalieWinPercentage()
        {
            if (totalRounds == 0) return 0f;
            return (float)goalieWins / totalRounds * 100f;
        }
        
        /// <summary>
        /// Force end current game session.
        /// </summary>
        public void EndGame()
        {
            isGameActive = false;
            
            if (trainingSwitch != null && trainingSwitch.IsTrainingMode())
            {
                trainingSwitch.EndEpisode();
            }
            
            Debug.Log("Game ended");
        }
        
        /// <summary>
        /// Cleanup event subscriptions.
        /// </summary>
        private void OnDestroy()
        {
            if (salarySystem != null)
            {
                SalarySystem.OnBonusEarned -= OnSaveEvent;
                SalarySystem.OnPenaltyApplied -= OnGoalEvent;
            }
        }
        
        /// <summary>
        /// Handle application focus changes (pause on focus loss).
        /// </summary>
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && isGameActive && !isGamePaused)
            {
                TogglePause();
            }
        }
        
        /// <summary>
        /// Handle application pause (mobile platforms).
        /// </summary>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && isGameActive && !isGamePaused)
            {
                TogglePause();
            }
        }
    }
}
