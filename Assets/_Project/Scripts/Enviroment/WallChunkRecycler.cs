using CatRunner.Core;
using System.Collections.Generic;
using UnityEngine;

namespace CatRunner.Environment
{
    [DisallowMultipleComponent]
    public sealed class WallChunkRecycler : MonoBehaviour
    {
        [Header("Chunk Settings")]
        [SerializeField] private float chunkWidth = 27f;
        [SerializeField] private float recycleOffset = 0.5f;

        private readonly List<Transform> _chunks = new();
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;

            if (chunkWidth <= 0f)
            {
                Debug.LogError("ChunkWidth must be > 0.");
                enabled = false;
                return;
            }

            for (int i = 0; i < transform.childCount; i++)
                _chunks.Add(transform.GetChild(i));

            if (_chunks.Count == 0)
            {
                Debug.LogError("No chunks found under Layer_Wall_Tiled.");
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing)
                return;

            RecycleIfNeeded();
        }

        private void RecycleIfNeeded()
        {
            float camLeft = GetCameraLeft();

            _chunks.Sort((a, b) => a.position.x.CompareTo(b.position.x));

            Transform leftMost = _chunks[0];

            float rightEdge = leftMost.position.x + chunkWidth;

            if (rightEdge < camLeft - recycleOffset)
            {
                Transform rightMost = _chunks[_chunks.Count - 1];

                leftMost.position = new Vector3(
                    rightMost.position.x + chunkWidth,
                    leftMost.position.y,
                    leftMost.position.z
                );
            }
        }

        private float GetCameraLeft()
        {
            float halfWidth = _camera.orthographicSize * _camera.aspect;
            return _camera.transform.position.x - halfWidth;
        }
    }
}