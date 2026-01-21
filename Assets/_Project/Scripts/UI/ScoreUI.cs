using CatRunner.Core;
using TMPro;
using UnityEngine;

namespace CatRunner.UI
{
    public sealed class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        private void Start()
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.OnScoreChanged += UpdateScore;
        }

        private void OnDisable()
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.OnScoreChanged -= UpdateScore;
        }

        private void UpdateScore(int score)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}
