using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    InputActionsAsset PlayerInputAction;
    private void Awake()
    {
        PlayerInputAction = new InputActionsAsset();
        PlayerInputAction.PlayerControls.Move.performed += OnMove;
        PlayerInputAction.PlayerControls.Move.canceled += OnMoveCanceled;
        PlayerInputAction.PlayerControls.Shoot.performed += OnShoot;
        PlayerInputAction.PlayerControls.Pass.performed += OnPass;
        PlayerInputAction.PlayerControls.Receive.performed += OnReceive;
    }

    private void OnReceive(InputAction.CallbackContext context)
    {
        //Debug.Log($"按下接球键J" + context);
        EventManager<PlayerInputEvent>.instance.TriggerEvent(PlayerInputEvent.Rececive);
    }

    private void OnPass(InputAction.CallbackContext context)
    {
        //Debug.Log($"按下传球键P" + context);
        EventManager<PlayerInputEvent>.instance.TriggerEvent(PlayerInputEvent.Pass);
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        //Debug.Log($"按下投篮键Space" + context);
        EventManager<PlayerInputEvent>.instance.TriggerEvent(PlayerInputEvent.Shoot);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        //Debug.Log($"<color=red>取消按下方向键</color>" + context);
        EventManager<PlayerInputEvent>.instance.TriggerEvent(PlayerInputEvent.MoveCancled);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log($"按下方向键" + context);
        Vector2 moveDir = context.ReadValue<Vector2>();
        EventManager<PlayerInputEvent>.instance.TriggerEvent(PlayerInputEvent.Move, moveDir);
    }

    private void OnEnable()
    {
        PlayerInputAction.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        PlayerInputAction.PlayerControls.Disable();
    }
}
