using UnityEngine;
using CatRunner.Interfaces;
using System;

namespace CatRunner.Environment
{
    [DisallowMultipleComponent]
    public sealed class WallPropDespawn : MonoBehaviour, IDespawnHandler
    {
        public Action OnDespawned;

        public void HandleDespawn()
        {
            OnDespawned?.Invoke();
            Destroy(gameObject);
        }
    }
}