using UnityEngine;
using System;

namespace CatRunner.Core
{
    public sealed class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public event Action<int> OnScoreChanged;

        public int CurrentScore { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void ResetScore()
        {
            CurrentScore = 0;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        public void AddPoints(int amount)
        {
            if (amount <= 0) return;

            CurrentScore += amount;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}
