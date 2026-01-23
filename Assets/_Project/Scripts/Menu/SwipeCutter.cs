using CatRunner.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CatRunner.Menu
{
    public sealed class SwipeCutter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _mainCamera;

        [Header("Trail Settings")]
        [SerializeField] private float _trailLifetime = 0.2f;
        [SerializeField] private float _trailWidth = 0.1f;
        [SerializeField] private Color _trailStartColor = Color.white;
        [SerializeField] private Color _trailEndColor = new Color(1f, 1f, 1f, 0f);
        [SerializeField] private Material _trailMaterial;

        private GameObject _trailObject;
        private TrailRenderer _trail;
        private bool _isSwiping;
        private Vector2 _previousWorldPos;
        private bool _hasCut;

        private void Awake()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;

            CreateTrailObject();
            SetTrailActive(false);
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Idle)
            {
                if (_isSwiping)
                {
                    _isSwiping = false;
                    SetTrailActive(false);
                }
                return;
            }

            // Detectar inicio del swipe
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _isSwiping = true;
                _previousWorldPos = GetMouseWorldPosition();
                
                // Mover primero a la nueva posición
                MoveTrailTo(_previousWorldPos);
                // Limpiar el trail
                _trail.Clear();
                // Activar después de mover y limpiar
                SetTrailActive(true);
                return;
            }

            // Detectar fin del swipe
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _isSwiping = false;
                SetTrailActive(false);
                return;
            }

            if (!_isSwiping)
                return;

            Vector2 currentWorldPos = GetMouseWorldPosition();
            MoveTrailTo(currentWorldPos);

            CheckForRopeCut(_previousWorldPos, currentWorldPos);

            _previousWorldPos = currentWorldPos;
        }

        private void CheckForRopeCut(Vector2 from, Vector2 to)
        {
            // Realizar un linecast entre la posición anterior y actual del mouse
            RaycastHit2D[] hits = Physics2D.LinecastAll(from, to);

            if (hits == null || hits.Length == 0)
                return;

            foreach (var hit in hits)
            {
                if (hit.collider == null)
                    continue;

                if (!hit.collider.CompareTag("Rope"))
                    continue;

                // Destruir el segmento de la cuerda
                CutRopeSegment(hit.collider.gameObject);
            }
        }

        private void CutRopeSegment(GameObject segment)
        {
            if (_hasCut)
                return;

            _hasCut = true;

            var joint = segment.GetComponent<HingeJoint2D>();
            if (joint != null)
                Destroy(joint);

            Destroy(segment);

            GameManager.Instance.SetState(GameState.CutYarn);
        }

        private Vector2 GetMouseWorldPosition()
        {
            Vector3 screenPos = Mouse.current.position.ReadValue();
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            return new Vector2(worldPos.x, worldPos.y);
        }

        private void CreateTrailObject()
        {
            _trailObject = new GameObject("SwipeTrail");
            _trailObject.transform.SetParent(transform);

            _trail = _trailObject.AddComponent<TrailRenderer>();
            _trail.time = _trailLifetime;
            _trail.startWidth = _trailWidth;
            _trail.endWidth = 0f;
            _trail.minVertexDistance = 0.01f;

            _trail.startColor = _trailStartColor;
            _trail.endColor = _trailEndColor;

            if (_trailMaterial != null)
            {
                _trail.material = _trailMaterial;
            }
            else
            {
                _trail.material = new Material(Shader.Find("Sprites/Default"));
            }

            _trail.numCornerVertices = 5;
            _trail.numCapVertices = 5;
        }

        private void SetTrailActive(bool active)
        {
            _trailObject.SetActive(active);
        }

        private void MoveTrailTo(Vector2 worldPos)
        {
            _trailObject.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
        }

        public void ResetCutter()
        {
            _hasCut = false;
            _isSwiping = false;
            SetTrailActive(false);
        }
    }
}
