using CatRunner.Core;
using System.Collections.Generic;
using UnityEngine;

namespace CatRunner.Enviroment
{
    public class BackgroundScroller : MonoBehaviour
    {
        [Header("Setup")]
        [Tooltip("Optional. If empty, all direct children will be used as tiles.")]
        [SerializeField] private List<Transform> _tiles = new();
        [SerializeField] private float _tileWidthOffset = 1f;

        private float _tileWidth;
        private int _tileCount;

        private void Awake()
        {
            if (_tiles.Count == 0)
            {
                _tiles = new List<Transform>(transform.childCount);
                for (int i = 0; i < transform.childCount; i++)
                    _tiles.Add(transform.GetChild(i));
            }

            _tileCount = _tiles.Count;
            if (_tileCount == 0)
            {
                Debug.LogError("[BackgroundScroller] No tiles assigned or found as children.");
                enabled = false;
                return;
            }

            _tileWidth = CalculateTileWidth(_tiles[0]);
            if (_tileWidth <= 0f)
            {
                Debug.LogError("[BackgroundScroller] Invalid tile width.");
                enabled = false;
                return;
            }

            _tiles.Sort((a, b) => a.position.x.CompareTo(b.position.x));
        }

        private float CalculateTileWidth(Transform tile)
        {
            var renderers = tile.GetComponentsInChildren<SpriteRenderer>();

            if (renderers.Length == 0)
            {
                Debug.LogError($"[BackgroundScroller] Tile '{tile.name}' has no SpriteRenderers in children.");
                return 0f;
            }

            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds.size.x;
        }

        private void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
                return;

            float delta = GameManager.Instance.GameSpeed * Time.deltaTime;

            for (int i = 0; i < _tiles.Count; i++)
            {
                _tiles[i].position += Vector3.left * delta;
            }

            RecycleTilesIfNeeded();
        }

        private void RecycleTilesIfNeeded()
        {
            var leftMost = _tiles[0];
            float leftMostRightEdge = leftMost.position.x + (_tileWidth * _tileWidthOffset);

            if (leftMostRightEdge < Camera.main.transform.position.x - _tileWidth)
            {
                var rightMost = _tiles[_tileCount - 1];
                float newX = rightMost.position.x + _tileWidth;

                leftMost.position = new Vector3(
                    newX,
                    leftMost.position.y,
                    leftMost.position.z
                );

                _tiles.RemoveAt(0);
                _tiles.Add(leftMost);
            }
        }
    }
}

