using System;
using UnityEngine;

namespace CatRunner.Environment.Data
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Represents the configuration data for a decorative wall prop
    /// that can be spawned procedurally by the WallPropSpawner.
    /// 
    /// Each entry defines:
    /// - What prefab to spawn
    /// - How often it appears (weight)
    /// - How much spacing it requires
    /// - How much horizontal space it occupies
    /// </summary>
    [Serializable]
    public class WallPropData
    {
        /// <summary>
        /// Prefab that will be instantiated in the scene.
        /// This should contain the visual representation (SpriteRenderer, etc.)
        /// and optionally a Despawn handler if needed.
        /// </summary>
        public GameObject prefab;

        /// <summary>
        /// Minimum horizontal spacing (in world units) before spawning this prop.
        /// This prevents props from appearing too close to each other.
        /// </summary>
        public float minSpacing;

        /// <summary>
        /// Maximum horizontal spacing (in world units) before spawning this prop.
        /// A random value between minSpacing and maxSpacing will be used,
        /// making placement feel more natural and less repetitive.
        /// </summary>
        public float maxSpacing;

        /// <summary>
        /// Relative probability of this prop being selected.
        /// Higher values increase the chance of this prop spawning
        /// compared to other props in the list.
        /// 
        /// Example:
        /// Window weight = 5
        /// Door weight = 1
        /// => Windows will appear roughly 5 times more often than doors.
        /// </summary>
        public float weight;

        /// <summary>
        /// The horizontal width (in world units) that this prop occupies.
        /// Used by the spawner to correctly calculate the next spawn position
        /// and avoid visual overlap.
        /// 
        /// IMPORTANT:
        /// This must match the real visual width of the prefab in world units.
        /// </summary>
        public float width;
    }
}
