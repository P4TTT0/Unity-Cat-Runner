using CatRunner.Core;
using UnityEngine;

namespace CatRunner.Environment
{
    [DisallowMultipleComponent]
    public sealed class ObstacleMover : MonoBehaviour
    {
        private void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
                return;

            float speed = GameManager.Instance.CurrentSpeed;
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
    }
}
