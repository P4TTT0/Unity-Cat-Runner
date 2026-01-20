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

        [Header("Timing")]
        [SerializeField] private float minSpawnDelay = 1.2f;
        [SerializeField] private float maxSpawnDelay = 2.2f;

        private float _spawnTimer;

        private void Start()
        {
            ResetTimer();
        }

        private void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
                return;

            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0f)
            {
                SpawnObstacle();
                ResetTimer();
            }
        }

        private void SpawnObstacle()
        {
            if (obstaclePrefabs.Length == 0)
                return;

            GameObject prefab =
                obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            Instantiate(
                prefab,
                spawnPoint.position,
                Quaternion.identity,
                obstaclesParent
            );
        }

        private void ResetTimer()
        {
            _spawnTimer = Random.Range(minSpawnDelay, maxSpawnDelay);
        }
    }
}
