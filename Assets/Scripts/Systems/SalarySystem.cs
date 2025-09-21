using UnityEngine;
using System;

namespace NeuralRink.Systems
{
    public sealed class SalarySystem : MonoBehaviour
    {
        public static SalarySystem Instance { get; private set; }

        [Header("Economy")]
        public int Salary = 100;
        public int Goals = 0;
        public int Saves = 0;
        public int Streak = 0;

        public static event Action<int> OnSalaryChanged;
        public static event Action<int, int, float> OnPerformanceUpdated;
        public static event Action<float> OnBonusEarned;
        public static event Action<float> OnPenaltyApplied;
        public event Action<int> OnStreakChanged;
        public event Action OnSessionEnd;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void ApplyGoal()
        {
            Goals++; Streak++; Salary += 10;
            OnSalaryChanged?.Invoke(Salary);
            OnStreakChanged?.Invoke(Streak);
        }

        public void ApplySave()
        {
            Saves++; Streak = 0; Salary -= 5;
            OnSalaryChanged?.Invoke(Salary);
            OnStreakChanged?.Invoke(Streak);
        }

        public void EndSession()
        {
            if (Goals >= 5) Salary += 50;
            else if (Goals < 3) Salary -= 20;
            OnSalaryChanged?.Invoke(Salary);
            OnSessionEnd?.Invoke();
        }

        // Wrappers expected by other scripts
        public void OnEpisodeBegin() { Goals = 0; Saves = 0; Streak = 0; }
        public void OnEpisodeEnd()   { EndSession(); }
        public void ProcessGoal()    { ApplyGoal(); }
        public void ProcessMiss()    { ApplySave(); }
        public void ProcessSave()    { ApplySave(); }
        public void ProcessGoalConceded() { ApplyGoal(); }
        
        /// <summary>
        /// Get current salary amount (for compatibility)
        /// </summary>
        public float GetCurrentSalary()
        {
            return Salary;
        }
        
        /// <summary>
        /// Get performance statistics (for compatibility)
        /// </summary>
        public (int saves, int goals, int shots, float percentage) GetPerformanceStats()
        {
            int shots = Goals + Saves;
            float percentage = shots > 0 ? (float)Saves / shots * 100f : 0f;
            return (Saves, Goals, shots, percentage);
        }
        
        /// <summary>
        /// Get formatted salary string
        /// </summary>
        public string GetFormattedSalary()
        {
            return $"${Salary:N0}";
        }
    }
}