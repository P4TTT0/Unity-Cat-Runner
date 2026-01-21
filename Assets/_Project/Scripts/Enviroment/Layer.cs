using System;
using UnityEngine;

namespace CatRunner.Environment
{
    [Serializable]
    public sealed class Layer
    {
        public Transform root;

        [Tooltip("0.2 = muy lejos (se mueve poco). 1 = velocidad base. >1 = foreground rápido.")]
        [Range(0f, 2f)]
        public float speedMultiplier = 1f;

        [Tooltip("Opcional. Si existe, recicla tiles para infinito horizontal.")]
        public ParallaxLayerTiler tiler;
    }
}
