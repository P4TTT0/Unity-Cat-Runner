using System.Collections.Generic;
using UnityEngine;

namespace CatRunner.Environment
{
    [DisallowMultipleComponent]
    public sealed class ParallaxLayerTiler : MonoBehaviour
    {
        [Header("Tiles")]
        [Tooltip("Si está vacío, usa todos los hijos directos como tiles.")]
        [SerializeField] private List<Transform> tiles = new();

        [Tooltip("Multiplicador del ancho (por si necesitás ajustar solape/gap fino).")]
        [SerializeField] private float tileWidthOffset = 1f;

        [Tooltip("Margen extra antes de reciclar (en unidades locales).")]
        [SerializeField] private float recyclePadding = 0.5f;

        private float _tileWidth;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;

            if (tiles.Count == 0)
            {
                tiles = new List<Transform>(transform.childCount);
                for (int i = 0; i < transform.childCount; i++)
                    tiles.Add(transform.GetChild(i));
            }

            if (tiles.Count == 0)
            {
                Debug.LogError($"[ParallaxLayerTiler] '{name}' has no tiles.");
                enabled = false;
                return;
            }

            // Ordenamos por posición LOCAL
            tiles.Sort((a, b) => a.localPosition.x.CompareTo(b.localPosition.x));

            // Calculamos ancho UNA sola vez
            _tileWidth = CalculateTileWidth(tiles[0]) * tileWidthOffset;

            if (_tileWidth <= 0f)
            {
                Debug.LogError($"[ParallaxLayerTiler] '{name}' has invalid tile width.");
                enabled = false;
            }
        }

        public void RecycleIfNeeded()
        {
            if (_camera == null || tiles.Count == 0)
                return;

            // Cámara izquierda en WORLD
            float camLeftWorld = _camera.ViewportToWorldPoint(Vector3.zero).x;

            // Convertimos a LOCAL SPACE del layer
            float camLeftLocal = transform.InverseTransformPoint(new Vector3(camLeftWorld, 0f, 0f)).x;

            Transform leftMost = tiles[0];

            float leftMostRightEdge = leftMost.localPosition.x + _tileWidth;

            if (leftMostRightEdge < camLeftLocal - recyclePadding)
            {
                Transform rightMost = tiles[tiles.Count - 1];

                float newX = rightMost.localPosition.x + _tileWidth;

                leftMost.localPosition = new Vector3(
                    newX,
                    leftMost.localPosition.y,
                    leftMost.localPosition.z
                );

                tiles.RemoveAt(0);
                tiles.Add(leftMost);
            }
        }

        private float CalculateTileWidth(Transform tile)
        {
            var renderers = tile.GetComponentsInChildren<SpriteRenderer>();

            if (renderers.Length == 0)
            {
                Debug.LogError($"[ParallaxLayerTiler] Tile '{tile.name}' in '{name}' has no SpriteRenderers.");
                return 0f;
            }

            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            return Mathf.Max(0.01f, bounds.size.x);
        }
    }
}
