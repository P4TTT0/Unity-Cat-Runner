using UnityEngine;
using CatRunner.Core;

namespace CatRunner.Environment
{
    [DisallowMultipleComponent]
    public sealed class ParallaxLayerMover : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing)
                return;

            float delta = speed * Time.deltaTime;
            transform.position += Vector3.left * delta;
        }
    }
}