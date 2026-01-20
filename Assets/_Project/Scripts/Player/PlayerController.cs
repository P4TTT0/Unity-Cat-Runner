using CatRunner.Core;
using System;
using UnityEngine;

namespace CatRunner.Player
{
    [DisallowMultipleComponent]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;

        [Header("Intro Settings")]
        [SerializeField] private Transform runnerAnchor;
        [SerializeField] private float introSpeed = 3f;

        [Header("Jump")]
        [SerializeField] private float jumpVelocity = 12f;
        [SerializeField] private float jumpCutMultiplier = 0.5f;

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
        private PlayerMoveMode _moveMode = PlayerMoveMode.Idle;
        private bool _isJumping;

        public static event Action OnReachedRunnerAnchor;
        public bool HasReachedAnchor => _moveMode == PlayerMoveMode.RunningInPlace;

        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int YVelocityHash = Animator.StringToHash("YVelocity");
        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving"); 

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameState;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameState;
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null)
                return;

            // Movimiento hacia el anchor durante la intro
            if (_moveMode == PlayerMoveMode.IntroMove)
            {
                MoveToAnchor();
            }

            UpdateGrounded();
            UpdateTimers();

            animator.SetBool(IsGroundedHash, _isGrounded);
            animator.SetFloat(YVelocityHash, rb.linearVelocity.y);

            bool isPlaying = GameManager.Instance.IsPlaying();
            animator.SetBool(IsMovingHash, isPlaying || _moveMode == PlayerMoveMode.IntroMove);

            if (isPlaying && _moveMode == PlayerMoveMode.RunningInPlace && _jumpBufferTimer > 0f && _coyoteTimer > 0f)
            {
                PerformJump();
            }

            // Reset jumping flag when grounded
            if (_isGrounded && rb.linearVelocity.y <= 0f)
            {
                _isJumping = false;
            }
        }

        private void HandleGameState(GameState state)
        {
            if (state == GameState.Playing && _moveMode == PlayerMoveMode.Idle)
            {
                _moveMode = PlayerMoveMode.IntroMove;
                animator.SetBool(IsMovingHash, true);
            }
        }

        private void MoveToAnchor()
        {
            if (runnerAnchor == null)
            {
                Debug.LogWarning("[PlayerController] RunnerAnchor not assigned. Skipping intro movement.");
                _moveMode = PlayerMoveMode.RunningInPlace;
                OnReachedRunnerAnchor?.Invoke();
                return;
            }

            float targetX = runnerAnchor.position.x;

            transform.position = new Vector3(
                Mathf.MoveTowards(
                    transform.position.x,
                    targetX,
                    introSpeed * Time.deltaTime
                ),
                transform.position.y,
                transform.position.z
            );

            if (Mathf.Abs(transform.position.x - targetX) < 0.01f)
            {
                transform.position = new Vector3(
                    targetX,
                    transform.position.y,
                    transform.position.z
                );

                _moveMode = PlayerMoveMode.RunningInPlace;
                OnReachedRunnerAnchor?.Invoke();
            }
        }

        public void TryJump()
        { 
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
                return;

            if (_moveMode != PlayerMoveMode.RunningInPlace)
                return;

            _jumpBufferTimer = jumpBufferTime;
        }

        public void OnJumpRelease()
        {
            if (_isJumping && rb.linearVelocity.y > 0f)
            {
                Vector2 v = rb.linearVelocity;
                v.y *= jumpCutMultiplier;
                rb.linearVelocity = v;
            }
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
            _isJumping = true;

            Vector2 v = rb.linearVelocity;
            v.y = jumpVelocity;
            rb.linearVelocity = v;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Obstacle"))
            {
                GameManager.Instance.SetState(GameState.GameOver);
            }
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
