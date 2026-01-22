using UnityEngine;
using CatRunner.Core;

namespace CatRunner.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class ScoreBySpeed : MonoBehaviour
    {
        [Header("Score Settings")]
        [Tooltip("Multiplicador base del score")]
        [SerializeField] private float scoreMultiplier = 1f;

        [Tooltip("Cuántos puntos se suman por unidad de velocidad")]
        [SerializeField] private float pointsPerSpeedUnit = 1f;

        private float _scoreAccumulator;

        private void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
                return;

            float speed = GameManager.Instance.CurrentSpeed;

            float scoreDelta = speed * pointsPerSpeedUnit * Time.deltaTime * scoreMultiplier;

            _scoreAccumulator += scoreDelta;

            if (_scoreAccumulator >= 1f)
            {
                int pointsToAdd = Mathf.FloorToInt(_scoreAccumulator);
                _scoreAccumulator -= pointsToAdd;

                ScoreManager.Instance.AddPoints(pointsToAdd);
            }
        }
    }
}
