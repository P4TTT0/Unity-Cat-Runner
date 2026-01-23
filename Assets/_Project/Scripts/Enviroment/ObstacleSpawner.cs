using CatRunner.Core;
using UnityEngine;

namespace CatRunner.Environment
{
    public sealed class ObstacleSpawner : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private GameObject[] obstaclePrefabs;
        [SerializeField] private Transform spawnPoint;

        [Header("Hierarchy")]
        [SerializeField] private Transform obstaclesParent;

        [Header("Spawn Distance (world units)")]
        [Tooltip("Minimum distance between obstacles")]
        [SerializeField] private float minSpawnDistance = 6f;

        [Tooltip("Maximum distance between obstacles")]
        [SerializeField] private float maxSpawnDistance = 10f;

        private float _distanceAccumulator;
        private float _nextSpawnDistance;

        private void Start()
        {
            ResetSpawnDistance();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameState;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameState;
            }
        }

        private void HandleGameState(GameState state)
        {
            if (state == GameState.Idle || state == GameState.ReturnToMenu)
            {
                ClearAllObstacles();
                _distanceAccumulator = 0f;
                ResetSpawnDistance();
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
                return;

            float deltaDistance =
                GameManager.Instance.CurrentSpeed * Time.deltaTime;

            _distanceAccumulator += deltaDistance;

            if (_distanceAccumulator >= _nextSpawnDistance)
            {
                SpawnObstacle();
                _distanceAccumulator = 0f;
                ResetSpawnDistance();
            }
        }

        private void SpawnObstacle()
        {
            if (obstaclePrefabs.Length == 0)
                return;

            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            Vector3 spawnPosition = spawnPoint.position;

            if (prefab.TryGetComponent(out ObstacleSpawnConfig config))
            {
                spawnPosition.y += config.GetOffsetY();
            }

            Transform parent = obstaclesParent != null ? obstaclesParent : null;

            Instantiate(prefab, spawnPosition, Quaternion.identity, parent);
        }

        private void ResetSpawnDistance()
        {
            _nextSpawnDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        }

        private void ClearAllObstacles()
        {
            if (obstaclesParent == null)
                return;

            for (int i = obstaclesParent.childCount - 1; i >= 0; i--)
            {
                Destroy(obstaclesParent.GetChild(i).gameObject);
            }
        }
    }
}
