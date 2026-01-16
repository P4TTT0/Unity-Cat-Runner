using CatRunner.Core;
using CatRunner.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CatRunner.Player
{
    [DisallowMultipleComponent]
    public sealed class PlayerInputController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController playerController;

        private InputSystem_Actions _actions;

        private void Awake()
        {
            if (playerController == null)
                playerController = GetComponent<PlayerController>();

            _actions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _actions.Enable();
            _actions.Player.Jump.performed += OnJumpPerformed;
        }

        private void OnDisable()
        {
            _actions.Player.Jump.performed -= OnJumpPerformed;
            _actions.Disable();
        }

        private void OnJumpPerformed(InputAction.CallbackContext ctx)
        {
            // Opcional: filtrar acá, pero mejor que lo decida el PlayerController.
            playerController.TryJump();
        }
    }
}
