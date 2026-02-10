using System.Collections.Generic;
using UnityEngine;

namespace CatRunner.Menu
{
    [DisallowMultipleComponent]
    public sealed class RopeController : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Transform ropeRoot;
        [SerializeField] private GameObject ropeSegmentPrefab;
        [SerializeField] private int segmentCount = 6;

        [Header("Linking")]
        [SerializeField] private YarnLinker yarnLinker;

        [Header("Joint Settings")]
        [SerializeField] private bool useLimits = true;
        [SerializeField] private float angleLimitMin = -5f;
        [SerializeField] private float angleLimitMax = 5f;

        private readonly List<Rigidbody2D> _segments = new();
        private Transform _hook;

        private void Awake()
        {
            if (ropeRoot.childCount > 0)
                _hook = ropeRoot.GetChild(0);

            BuildRope();
        }

        public void ResetRope()
        {
            ClearRope();
            BuildRope();
        }

        private void BuildRope()
        {
            _segments.Clear();

            var previousRigidbody = _hook?.GetComponent<Rigidbody2D>();

            // Instanciamos temporalmente un segmento de soga para medir su altura y calcular los anchors correctamente
            var tempSegment = Instantiate(ropeSegmentPrefab);
            var tempCol = tempSegment.GetComponent<Collider2D>();
            var segmentHeight = tempCol.bounds.size.y; // Altura del segmento en world space (con escala) => PARA POSICIONAR EN EL MUNDO

            // Obtener el tamaño local para los anchors (sin escala) => PARA CONFIGURAR LOS JOINTS CORRECTAMENTE
            float localSegmentHeight = GetLocalColliderHeight(tempCol);

            // Destruimos el segmento temporal
            Destroy(tempSegment);
            
            var currentPos = ropeRoot.position;
            
            // Si existe un hook entonces empezamos desde su punto inferior
            if (previousRigidbody != null)
            {
                // Obtenemos su altura para posicionar el primer segmento justo debajo del hook
                var hookHeight = previousRigidbody.GetComponent<Collider2D>().bounds.size.y;
                currentPos += Vector3.down * (hookHeight / 2f);
            }

            // Iteramos sobre todos los segmentos de soga disponibles y los posicionamos uno debajo del otro, conectándolos con joints
            for (int i = 0; i < segmentCount; i++)
            {
                // Posicionar el segmento: su punto superior debe estar en currentPos
                var pos = currentPos + Vector3.down * (segmentHeight / 2f);
                var segment = Instantiate(ropeSegmentPrefab, pos, Quaternion.identity, ropeRoot);

                var rb = segment.GetComponent<Rigidbody2D>();
                var joint = segment.GetComponent<HingeJoint2D>();

                if (previousRigidbody != null && joint != null)
                {
                    ConfigureJoint(joint, previousRigidbody, localSegmentHeight, i);
                }

                previousRigidbody = rb;
                
                // Actualizar posición para el siguiente segmento
                currentPos += Vector3.down * segmentHeight;
                
                _segments.Add(rb);
            }

            // Finalmente, unimos el ultimo pedazo de soga al ovillo usando el yarn linker
            if (yarnLinker != null && previousRigidbody != null)
            {
                yarnLinker.LinkToEndOfRope(previousRigidbody, localSegmentHeight);
            }
        }

        private void ConfigureJoint(HingeJoint2D joint, Rigidbody2D connectedBody, float segmentHeight, int segmentIndex)
        {
            joint.autoConfigureConnectedAnchor = false; // Evitamos que Unity autoconfigure los anclajes 
            joint.connectedBody = connectedBody; 
            
            joint.anchor = new Vector2(0, segmentHeight / 2f);

            // Para el primer segmento, si hay un hook, conectamos al punto inferior del hook. Para los siguientes segmentos, conectamos al punto inferior del segmento anterior.
            var previousHeight = segmentIndex == 0 && _hook != null ? _hook.GetComponent<Collider2D>().bounds.size.y : segmentHeight;
            joint.connectedAnchor = new Vector2(0, -previousHeight / 2f);

            // Configurar límites de ángulo para evitar rotación excesiva
            if (useLimits)
            {
                joint.useLimits = true;
                // Asignamos los limites de rotacion dinamicamente en base a la posicion del segmento para crear un efecto de balanceo mas natural,
                // los segmentos mas cercanos al hook tienen limites mas estrictos y los mas cercanos al ovillo tienen limites mas amplios
                var t = (float)segmentIndex / (segmentCount - 1);
                var min = Mathf.Lerp(angleLimitMin * 0.5f, angleLimitMin, t);
                var max = Mathf.Lerp(angleLimitMax * 0.5f, angleLimitMax, t);
                JointAngleLimits2D limits = new JointAngleLimits2D
                {
                    min = angleLimitMin,
                    max = angleLimitMax
                };
                joint.limits = limits;
            }
        }

        private void ClearRope()
        {
            if (yarnLinker != null)
            {
                yarnLinker.ClearLink();
            }

            for (int i = ropeRoot.childCount - 1; i >= 0; i--)
            {
                Transform child = ropeRoot.GetChild(i);

                if (child == _hook)
                    continue;

                Destroy(child.gameObject);
            }

            _segments.Clear();
        }

        private float GetLocalColliderHeight(Collider2D collider)
        {
            if (collider == null) 
                return 0f;

            if (collider is BoxCollider2D box)
            {
                return box.size.y;
            }
            
            return collider.bounds.size.y;
        }
    }
}
