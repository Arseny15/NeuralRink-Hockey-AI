using UnityEngine;
using System;

namespace NeuralRink.Systems
{
    /// <summary>
    /// Manages salary and bonus economy system for the goalie agent.
    /// Tracks performance metrics and calculates rewards based on saves/goals.
    /// </summary>
    public class SalarySystem : MonoBehaviour
    {
        [Header("Salary Configuration")]
        [SerializeField] private float baseSalary = 50000f;
        [SerializeField] private float saveBonus = 5000f;
        [SerializeField] private float goalPenalty = 10000f;
        [SerializeField] private float performanceMultiplier = 1.0f;
        
        [Header("Performance Tracking")]
        [SerializeField] private int totalShots = 0;
        [SerializeField] private int totalSaves = 0;
        [SerializeField] private int totalGoals = 0;
        [SerializeField] private float savePercentage = 0f;
        
        [Header("UI Configuration")]
        [SerializeField] private bool showSalaryHUD = true;
        [SerializeField] private float hudUpdateInterval = 0.5f;
        
        // Runtime state
        private float currentSalary = 0f;
        private float totalEarnings = 0f;
        private float lastHUDUpdate = 0f;
        private bool isInitialized = false;
        
        // Events
        public static event Action<float> OnSalaryChanged;
        public static event Action<int, int, float> OnPerformanceUpdated; // saves, goals, percentage
        public static event Action<float> OnBonusEarned;
        public static event Action<float> OnPenaltyApplied;
        
        /// <summary>
        /// Initialize salary system with base values.
        /// </summary>
        private void Start()
        {
            InitializeSalarySystem();
        }
        
        /// <summary>
        /// Initialize salary system state and UI.
        /// </summary>
        private void InitializeSalarySystem()
        {
            currentSalary = baseSalary;
            totalEarnings = baseSalary;
            isInitialized = true;
            
            // Subscribe to game events
            SubscribeToGameEvents();
            
            // Initialize UI if needed
            if (showSalaryHUD)
            {
                UpdateSalaryHUD();
            }
            
            Debug.Log($"Salary system initialized. Base salary: ${baseSalary:N0}");
        }
        
        /// <summary>
        /// Subscribe to relevant game events for salary calculations.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            // TODO: Subscribe to goalie save/goal events when implemented
            // This would typically be done through the GoalieAgent or game manager
        }
        
        /// <summary>
        /// Update salary system each frame.
        /// </summary>
        private void Update()
        {
            if (!isInitialized) return;
            
            // Update HUD at intervals
            if (showSalaryHUD && Time.time - lastHUDUpdate >= hudUpdateInterval)
            {
                UpdateSalaryHUD();
                lastHUDUpdate = Time.time;
            }
        }
        
        /// <summary>
        /// Process a save event and award bonus.
        /// </summary>
        public void ProcessSave()
        {
            if (!isInitialized) return;
            
            totalShots++;
            totalSaves++;
            
            // Calculate save percentage
            savePercentage = (float)totalSaves / totalShots * 100f;
            
            // Award save bonus
            float bonusAmount = saveBonus * performanceMultiplier;
            currentSalary += bonusAmount;
            totalEarnings += bonusAmount;
            
            // Trigger events
            OnSalaryChanged?.Invoke(currentSalary);
            OnPerformanceUpdated?.Invoke(totalSaves, totalGoals, savePercentage);
            OnBonusEarned?.Invoke(bonusAmount);
            
            if (TrainingSwitch.FindObjectOfType<TrainingSwitch>()?.ShouldEnableDebugLogs() == true)
            {
                Debug.Log($"Save! Bonus: ${bonusAmount:N0}, Total Salary: ${currentSalary:N0}");
            }
        }
        
        /// <summary>
        /// Process a goal conceded event and apply penalty.
        /// </summary>
        public void ProcessGoalConceded()
        {
            if (!isInitialized) return;
            
            totalShots++;
            totalGoals++;
            
            // Calculate save percentage
            savePercentage = (float)totalSaves / totalShots * 100f;
            
            // Apply goal penalty
            float penaltyAmount = goalPenalty * performanceMultiplier;
            currentSalary -= penaltyAmount;
            totalEarnings -= penaltyAmount;
            
            // Ensure salary doesn't go below zero
            currentSalary = Mathf.Max(0f, currentSalary);
            totalEarnings = Mathf.Max(0f, totalEarnings);
            
            // Trigger events
            OnSalaryChanged?.Invoke(currentSalary);
            OnPerformanceUpdated?.Invoke(totalSaves, totalGoals, savePercentage);
            OnPenaltyApplied?.Invoke(penaltyAmount);
            
            if (TrainingSwitch.FindObjectOfType<TrainingSwitch>()?.ShouldEnableDebugLogs() == true)
            {
                Debug.Log($"Goal conceded! Penalty: ${penaltyAmount:N0}, Total Salary: ${currentSalary:N0}");
            }
        }
        
        /// <summary>
        /// Update performance multiplier based on recent performance.
        /// </summary>
        public void UpdatePerformanceMultiplier(float multiplier)
        {
            performanceMultiplier = Mathf.Clamp(multiplier, 0.1f, 3.0f);
            
            if (TrainingSwitch.FindObjectOfType<TrainingSwitch>()?.ShouldEnableDebugLogs() == true)
            {
                Debug.Log($"Performance multiplier updated to: {performanceMultiplier:F2}");
            }
        }
        
        /// <summary>
        /// Get current salary amount.
        /// </summary>
        public float GetCurrentSalary()
        {
            return currentSalary;
        }
        
        /// <summary>
        /// Get total earnings.
        /// </summary>
        public float GetTotalEarnings()
        {
            return totalEarnings;
        }
        
        /// <summary>
        /// Get save percentage.
        /// </summary>
        public float GetSavePercentage()
        {
            return savePercentage;
        }
        
        /// <summary>
        /// Get performance statistics.
        /// </summary>
        public (int saves, int goals, int shots, float percentage) GetPerformanceStats()
        {
            return (totalSaves, totalGoals, totalShots, savePercentage);
        }
        
        /// <summary>
        /// Reset salary system to initial state.
        /// </summary>
        public void ResetSalarySystem()
        {
            currentSalary = baseSalary;
            totalEarnings = baseSalary;
            totalShots = 0;
            totalSaves = 0;
            totalGoals = 0;
            savePercentage = 0f;
            performanceMultiplier = 1.0f;
            
            // Trigger events
            OnSalaryChanged?.Invoke(currentSalary);
            OnPerformanceUpdated?.Invoke(totalSaves, totalGoals, savePercentage);
            
            Debug.Log("Salary system reset to initial state");
        }
        
        /// <summary>
        /// Update salary HUD display.
        /// </summary>
        private void UpdateSalaryHUD()
        {
            // TODO: Implement HUD update logic
            // This would typically update UI elements showing current salary and performance
        }
        
        /// <summary>
        /// Get formatted salary string for display.
        /// </summary>
        public string GetFormattedSalary()
        {
            return $"${currentSalary:N0}";
        }
        
        /// <summary>
        /// Get formatted performance string for display.
        /// </summary>
        public string GetFormattedPerformance()
        {
            return $"{savePercentage:F1}% ({totalSaves}/{totalShots})";
        }
        
        /// <summary>
        /// Set base salary amount.
        /// </summary>
        public void SetBaseSalary(float newBaseSalary)
        {
            baseSalary = Mathf.Max(0f, newBaseSalary);
            currentSalary = baseSalary;
            
            OnSalaryChanged?.Invoke(currentSalary);
        }
        
        /// <summary>
        /// Set save bonus amount.
        /// </summary>
        public void SetSaveBonus(float newSaveBonus)
        {
            saveBonus = Mathf.Max(0f, newSaveBonus);
        }
        
        /// <summary>
        /// Set goal penalty amount.
        /// </summary>
        public void SetGoalPenalty(float newGoalPenalty)
        {
            goalPenalty = Mathf.Max(0f, newGoalPenalty);
        }
        
        /// <summary>
        /// Enable or disable salary HUD display.
        /// </summary>
        public void SetShowSalaryHUD(bool show)
        {
            showSalaryHUD = show;
        }
        
        /// <summary>
        /// Get salary system configuration for debugging.
        /// </summary>
        public string GetDebugInfo()
        {
            return $"Salary: ${currentSalary:N0}, " +
                   $"Saves: {totalSaves}, " +
                   $"Goals: {totalGoals}, " +
                   $"Save%: {savePercentage:F1}%, " +
                   $"Multiplier: {performanceMultiplier:F2}";
        }
    }
}
