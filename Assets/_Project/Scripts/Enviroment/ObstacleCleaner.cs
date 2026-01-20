using UnityEngine;

namespace CatRunner.Environment
{
    public sealed class ObstacleCleaner : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}
