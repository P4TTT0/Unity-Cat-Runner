using UnityEngine;

namespace CatRunner.Environment
{
    public sealed class ObstacleSpawnConfig : MonoBehaviour
    {
        [Header("Height Settings")]
        public HeightMode heightMode = HeightMode.Fixed;

        [Tooltip("Usado si HeightMode = Fixed")]
        public float fixedOffsetY = 0f;

        [Tooltip("Usado si HeightMode = RandomRange")]
        public Vector2 randomOffsetYRange = new Vector2(1f, 2.5f);

        public float GetOffsetY()
        {
            return heightMode switch
            {
                HeightMode.Fixed => fixedOffsetY,
                HeightMode.RandomRange => Random.Range(
                    randomOffsetYRange.x,
                    randomOffsetYRange.y
                ),
                _ => 0f
            };
        }
    }
}
