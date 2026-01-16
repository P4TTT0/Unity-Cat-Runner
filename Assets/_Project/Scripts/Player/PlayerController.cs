using CatRunner.Core;
using UnityEngine;

namespace CatRunner.Player
{
    [DisallowMultipleComponent]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;

        [Header("Jump")]
        [SerializeField] private float jumpVelocity = 12f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Game Feel")]
        [SerializeField] private float coyoteTime = 0.1f;
        [SerializeField] private float jumpBufferTime = 0.1f;

        private bool _isGrounded;
        private float _coyoteTimer;
        private float _jumpBufferTimer;

        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int YVelocityHash = Animator.StringToHash("YVelocity");
        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving"); 

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (GameManager.Instance == null)
                return;

            UpdateGrounded();
            UpdateTimers();

            animator.SetBool(IsGroundedHash, _isGrounded);
            animator.SetFloat(YVelocityHash, rb.linearVelocity.y);

            bool isPlaying = GameManager.Instance.IsPlaying();
            animator.SetBool(IsMovingHash, isPlaying);

            if (isPlaying && _jumpBufferTimer > 0f && _coyoteTimer > 0f)
            {
                PerformJump();
            }
        }

        public void TryJump()
        { 
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
                return;

            _jumpBufferTimer = jumpBufferTime;
        }

        private void UpdateGrounded()
        {
            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        }

        private void UpdateTimers()
        {
            if (_isGrounded)
                _coyoteTimer = coyoteTime;
            else
                _coyoteTimer -= Time.deltaTime;

            if (_jumpBufferTimer > 0f)
                _jumpBufferTimer -= Time.deltaTime;
        }

        private void PerformJump()
        {
            _jumpBufferTimer = 0f;
            _coyoteTimer = 0f;

            Vector2 v = rb.linearVelocity;
            v.y = jumpVelocity;
            rb.linearVelocity = v;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
#endif
    }
}
