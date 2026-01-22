using System;
using UnityEngine;
using CatRunner.Core;

namespace CatRunner.Environment
{
    [DisallowMultipleComponent]
    public sealed class ParallaxBackgroundController : MonoBehaviour
    {
        [SerializeField] private Layer[] layers = Array.Empty<Layer>();

        private void Awake()
        {
            // Autowire tiler si no se asigno se auto asigna en base a los hijos
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i].root == null) continue;
                if (layers[i].tiler == null)
                    layers[i].tiler = layers[i].root.GetComponent<ParallaxLayerTiler>();
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
    }
}
