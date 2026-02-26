using UnityEngine;
using CatRunner.Interfaces;

namespace CatRunner.Environment
{
    [DisallowMultipleComponent]
    public sealed class DespawnTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var handler = other.GetComponent<IDespawnHandler>();
            if (handler != null)
            {
                handler.HandleDespawn();
            }
        }
    }
}