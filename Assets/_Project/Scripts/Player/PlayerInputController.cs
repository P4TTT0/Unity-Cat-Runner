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

        private void Start()
        {
            _actions.Enable();

            _actions.Player.Jump.performed += OnJumpPerformed;
            _actions.Player.Jump.canceled += OnJumpCanceled;

            _actions.Player.Crouch.started += OnCrouchStarted;
            _actions.Player.Crouch.canceled += OnCrouchCanceled;
        }

        private void OnDisable()
        {
            _actions.Player.Jump.performed -= OnJumpPerformed;
            _actions.Player.Jump.canceled -= OnJumpCanceled;

            _actions.Player.Crouch.started -= OnCrouchStarted;
            _actions.Player.Crouch.canceled -= OnCrouchCanceled;

            _actions.Disable();
        }

        private void OnJumpPerformed(InputAction.CallbackContext ctx) => playerController.TryJump();
        private void OnJumpCanceled(InputAction.CallbackContext ctx) => playerController.OnJumpRelease();

        private void OnCrouchStarted(InputAction.CallbackContext ctx) => playerController.SetCrouch(true);
        private void OnCrouchCanceled(InputAction.CallbackContext ctx) => playerController.SetCrouch(false);
    }
}
