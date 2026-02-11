using UnityEngine;

namespace CatRunner.Menu
{
    public class YarnLinker : MonoBehaviour
    {
        [Header("Initial Swing")]
        [SerializeField] private bool randomizeInitialPosition = true;
        [SerializeField] private Vector2 horizontalOffsetRange = new Vector2(-0.3f, 0.3f);
        [SerializeField] private Vector2 verticalOffsetRange = new Vector2(-0.1f, 0.2f);
        
        private HingeJoint2D _currentJoint;

        public void LinkToEndOfRope(Rigidbody2D endOfRope, float ropeSegmentLocalHeight)
        {
            ClearLink();

            if (endOfRope == null) return;

            // Obtener la altura del yarn en world space para posicionamiento
            var yarnCollider = GetComponent<Collider2D>();
            var yarnWorldHeight = yarnCollider != null ? yarnCollider.bounds.size.y : 0f;
            var yarnLocalHeight = GetLocalColliderHeight(yarnCollider);
            
            // Calcular la altura world del segmento de cuerda
            var ropeSegmentWorldHeight = endOfRope.GetComponent<Collider2D>().bounds.size.y;

            // Posicionar el yarn justo debajo del último segmento
            var endOfRopePos = endOfRope.transform.position;
            var yarnPosition = endOfRopePos + Vector3.down * (ropeSegmentWorldHeight / 2f + yarnWorldHeight / 2f);
            
            // Aplicar offset aleatorio para crear balanceo inicial
            if (randomizeInitialPosition)
            {
                float horizontalOffset = Random.Range(horizontalOffsetRange.x, horizontalOffsetRange.y);
                float verticalOffset = Random.Range(verticalOffsetRange.x, verticalOffsetRange.y);
                yarnPosition += new Vector3(horizontalOffset, verticalOffset, 0f);   
            }
           
            transform.position = yarnPosition;

            // Crear y configurar el joint usando tamaños locales
            var joint = gameObject.AddComponent<HingeJoint2D>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = endOfRope;
            joint.anchor = new Vector2(0, yarnLocalHeight / 2f); // Punto superior del yarn (local)
            joint.connectedAnchor = new Vector2(0, -ropeSegmentLocalHeight / 2f); // Punto inferior del último segmento (local)

            // Configurar límites similares a los de la cuerda
            joint.useLimits = true;
            JointAngleLimits2D limits = new JointAngleLimits2D
            {
                min = -5f,
                max = 5f
            };
            joint.limits = limits;

            _currentJoint = joint;
        }

        public void ClearLink()
        {
            if (_currentJoint != null)
            {
                Destroy(_currentJoint);
                _currentJoint = null;
            }
        }

        private float GetLocalColliderHeight(Collider2D collider)
        {
            if (collider == null) 
            {
                Debug.LogWarning("YarnLinker: No collider found!");
                return 0f;
            }

            if (collider is CircleCollider2D circle)
            {
                return circle.radius * 2f;
            }
            
            return collider.bounds.size.y;
        }
    }
}
