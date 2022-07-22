#define USE_NEW_INPUT_SYSTEM
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }


    public bool IsLeftMouseButtonDownClick()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.MouseButtonLeft.WasPressedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif

    }

    public float GetCameraZoomFloat()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else

        float zoomAmount = 0f;
        if (Input.mouseScrollDelta.y > 0f)
        {
            zoomAmount = -1f;
        }
        if (Input.mouseScrollDelta.y < 0f)
        {
            zoomAmount = 1f;
        }
        return zoomAmount;
#endif

    }

    public Vector2 GetCameraMoveVector2()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else

        Vector2 inputMoveDir = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.y = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.y = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = +1f;
        }

        return inputMoveDir;
#endif

    }


    public float GetCameraRotateFloat()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else

    float rotationAmount = 0;
    if (Input.GetKey(KeyCode.Q))
    {
        rotationAmount = +1f;
    }
    if (Input.GetKey(KeyCode.E))
    {
        rotationAmount = -1f;
    }

    return rotationAmount;
#endif

    }
}