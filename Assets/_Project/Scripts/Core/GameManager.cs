using System;
using System.Collections;
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

        [Header("Speed Multiplier (Crouch / Effects)")]
        [SerializeField] private float multiplierLerpSpeed = 5f;

        [Header("Init animation Settings")]
        [SerializeField] private float cutToTravelDelay = 0.25f;
        [SerializeField] private float arrivalIntroDuration = 0.8f;

        public event Action<GameState> OnGameStateChanged;

        public float CurrentSpeed => GameSpeed * _currentMultiplier;

        private Coroutine _stateRoutine;

        private float _currentMultiplier = 1f;
        private float _targetMultiplier = 1f;

        private void Awake()
        {
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

            _currentMultiplier = Mathf.MoveTowards(
                _currentMultiplier,
                _targetMultiplier,
                multiplierLerpSpeed * Time.deltaTime
            );
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _targetMultiplier = Mathf.Clamp(multiplier, 0.2f, 1f);
        }

        public void ResetSpeedMultiplier()
        {
            _targetMultiplier = 1f;
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
            if (_stateRoutine != null)
            {
                StopCoroutine(_stateRoutine);
                _stateRoutine = null;
            }

            ResetSpeedMultiplier();

            switch (state)
            {
                case GameState.Idle:
                    GameSpeed = 0f;
                    break;

                case GameState.CutYarn:
                    GameSpeed = 0f;
                    _stateRoutine = StartCoroutine(CutToFastTravelRoutine());
                    break;

                case GameState.FastTravel:
                    GameSpeed = maxGameSpeed;
                    break;

                case GameState.SlowDown:
                    GameSpeed = 2f;
                    break;

                case GameState.ArrivalIntro:
                    GameSpeed = 0f;
                    _stateRoutine = StartCoroutine(ArrivalIntroToPlayingRoutine());
                    break;

                case GameState.Playing:
                    ScoreManager.Instance.ResetScore();
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
        private IEnumerator CutToFastTravelRoutine()
        {
            yield return new WaitForSeconds(cutToTravelDelay);

            if (CurrentState != GameState.CutYarn)
                yield break;

            SetState(GameState.FastTravel);
        }

        private IEnumerator ArrivalIntroToPlayingRoutine()
        {
            yield return new WaitForSeconds(arrivalIntroDuration);

            if (CurrentState != GameState.ArrivalIntro)
                yield break;

            SetState(GameState.Playing);
        }
    }
}
