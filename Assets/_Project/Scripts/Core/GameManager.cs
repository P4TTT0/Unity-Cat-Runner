using System;
using UnityEngine;

namespace CatRunner.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }

        public float GameSpeed { get; private set; }

        [Header("Game Speed Settings")]
        [SerializeField] private float initialGameSpeed = 5f;
        [SerializeField] private float maxGameSpeed = 15f;
        [SerializeField] private float speedIncreaseRate = 0.1f;

        public event Action<GameState> OnGameStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            SetState(GameState.Idle);
        }

        private void Update()
        {
            if (CurrentState == GameState.Playing)
            {
                IncreaseGameSpeed();
            }
        }

        private void IncreaseGameSpeed()
        {
            GameSpeed = Mathf.Min(
                GameSpeed + speedIncreaseRate * Time.deltaTime,
                maxGameSpeed
            );
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;

            HandleStateChange(newState);

            OnGameStateChanged?.Invoke(newState);
        }

        private void HandleStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.Idle:
                    GameSpeed = 0f;
                    break;

                case GameState.CutYarn:
                    GameSpeed = 0f;
                    break;

                case GameState.FastTravel:
                    GameSpeed = maxGameSpeed;
                    break;

                case GameState.SlowDown:
                    GameSpeed = 2f;
                    break;

                case GameState.ArrivalIntro:
                    GameSpeed = 0f;
                    break;

                case GameState.Playing:
                    GameSpeed = initialGameSpeed;
                    break;

                case GameState.GameOver:
                    GameSpeed = 0f;
                    break;
            }
        }

        public bool IsPlaying()
        {
            return CurrentState == GameState.Playing;
        }

        public void StartRun()
        {
            SetState(GameState.Playing);
        }

        public void TriggerGameOver()
        {
            if (CurrentState == GameState.GameOver)
                return;

            SetState(GameState.GameOver);
        }

        public void RestartGame()
        {
            SetState(GameState.Idle);
        }
    }
}