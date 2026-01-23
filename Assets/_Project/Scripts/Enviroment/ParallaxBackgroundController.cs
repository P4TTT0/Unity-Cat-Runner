using System;
using UnityEngine;
using CatRunner.Core;

namespace CatRunner.Environment
{
    [DisallowMultipleComponent]
    public sealed class ParallaxBackgroundController : MonoBehaviour
    {
        [SerializeField] private Layer[] layers = Array.Empty<Layer>();

        private Vector3[] _initialLayerPositions;

        private void Awake()
        {
            // Autowire tiler si no se asigno se auto asigna en base a los hijos
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i].root == null) continue;
                if (layers[i].tiler == null)
                    layers[i].tiler = layers[i].root.GetComponent<ParallaxLayerTiler>();
            }

            // Guardar posiciones iniciales de cada layer
            _initialLayerPositions = new Vector3[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i].root != null)
                {
                    _initialLayerPositions[i] = layers[i].root.position;
                }
            }
        }

        private void Start()
        {
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
                ResetBackground();
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
                return;

            float baseDelta = GameManager.Instance.CurrentSpeed * Time.deltaTime;

            for (int i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                if (layer.root == null) continue;

                float delta = baseDelta * layer.speedMultiplier;

                layer.root.position += Vector3.left * delta;

                layer.tiler?.RecycleIfNeeded();
            }
        }

        public void ResetBackground()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i].root == null) continue;

                layers[i].root.position = _initialLayerPositions[i];

                layers[i].tiler?.ResetTiles();
            }
        }
    }
}
