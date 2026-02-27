using CatRunner.Core;
using CatRunner.Environment.Data;
using System.Collections.Generic;
using UnityEngine;

namespace CatRunner.Environment
{
    [DisallowMultipleComponent]
    public sealed class WallPropSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform spawnRoot;
        [SerializeField] private List<WallPropData> props;

        [Header("Config")]
        [SerializeField] private float baseY = 0f;
        [SerializeField] private int maxActive = 5;
        [SerializeField][Range(0f, 1f)] private float repeatPenalty = 0.5f;

        private int _activeCount;
        private WallPropData _lastSpawnedProp;
        private GameObject _lastPropInstance;

        private void Start()
        {
            if (spawnRoot == null)
                spawnRoot = transform;

            if (spawnPoint == null)
            {
                Debug.LogError("SpawnPoint not assigned.");
                enabled = false;
                return;
            }

            if (props == null || props.Count == 0)
            {
                Debug.LogError("No props configured.");
                enabled = false;
                return;
            }

            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Playing)
            {
                _lastSpawnedProp = null;
                _lastPropInstance = null;
                
                for (int i = 0; i < maxActive; i++)
                    SpawnProp();
            }

            if (state == GameState.Idle)
            {
                ClearAll();
            }
        }

        private void SpawnProp()
        {
            WallPropData data = GetWeightedRandom();

            float spawnX;
            if (_lastPropInstance != null && _lastSpawnedProp != null)
            {
                float spacing = Random.Range(data.minSpacing, data.maxSpacing);
                spawnX = _lastPropInstance.transform.position.x + _lastSpawnedProp.width + spacing;
            }
            else
            {
                spawnX = spawnPoint.position.x;
            }

            var instance = Instantiate(
                data.prefab,
                new Vector3(spawnX, baseY, 0f),
                Quaternion.identity,
                spawnRoot
            );

            var despawn = instance.GetComponent<DestroyDespawn>();
            if (despawn != null)
                despawn.OnDespawned += HandleDespawn;

            _lastPropInstance = instance;
            _lastSpawnedProp = data;
            _activeCount++;
        }

        private void HandleDespawn()
        {
            _activeCount--;

            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
                SpawnProp();
        }

        private void ClearAll()
        {
            _activeCount = 0;
            _lastSpawnedProp = null;
            _lastPropInstance = null;

            for (int i = spawnRoot.childCount - 1; i >= 0; i--)
                Destroy(spawnRoot.GetChild(i).gameObject);
        }

        private WallPropData GetWeightedRandom()
        {
            float total = 0f;
            
            foreach (var p in props)
            {
                float weight = Mathf.Max(0f, p.weight);
                
                if (_lastSpawnedProp != null && p == _lastSpawnedProp)
                    weight *= repeatPenalty;
                
                total += weight;
            }

            float r = Random.Range(0f, total);
            float cumulative = 0f;

            foreach (var p in props)
            {
                float weight = Mathf.Max(0f, p.weight);
                
                if (_lastSpawnedProp != null && p == _lastSpawnedProp)
                    weight *= repeatPenalty;
                
                cumulative += weight;
                if (r <= cumulative)
                    return p;
            }

            return props[0];
        }
    }
}