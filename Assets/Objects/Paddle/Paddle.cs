using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class Paddle : MonoBehaviour
{
    public float yMax = 4f;
    public float yMin = -3f;
    public float speed = 3.0f;
    private Vector3 movement;
    private bool isInverting = false;

    private PlayerInput playerInput;
    private Rigidbody2D rigidbody;

    public PlayerSide playerSide;

    public event Action<int, CardOrientation> cardButtonPressed;
    public event Action cardInvertPressed;
    public event Action cardInvertReleased;
    public event Action pausePressed;
    public event Action escapePressed;

    void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // void Start()
    // {
    //     playerInput.user.UnpairDevices();
    //     InputUser.PerformPairingWithDevice(Keyboard.current, playerInput.user);
    //     if (Gamepad.all.Count >= gamepadIndex + 1)
    //     {
    //         InputUser.PerformPairingWithDevice(Gamepad.all[gamepadIndex], playerInput.user);
    //     }
    // }
    
    public void OnMovement(InputAction.CallbackContext callbackContext)
    {
        // movement = callbackContext.ReadValue<Vector2>();
        // movement.x = 0f;
        Vector2 baseMovement = callbackContext.ReadValue<Vector2>();
        if (baseMovement.y > 0f)
        {
            movement = Vector2.up;
        }
        else if (baseMovement.y < 0f)
        {
            movement = Vector2.down;
        }
        else
        {
            movement = Vector2.zero;
        }
        // Debug.Log(movement);
    }

    public void OnInvert(InputAction.CallbackContext callbackContext)
    {
        isInverting = callbackContext.action.IsPressed();
        if (isInverting)
        {
            cardInvertPressed?.Invoke();
        }
        else
        {
            cardInvertReleased?.Invoke();
        }
    }

    public void OnCard1(InputAction.CallbackContext callbackContext)
    {
        cardButtonPressed?.Invoke(0, isInverting ? CardOrientation.Inverted : CardOrientation.Normal);
    }

    public void OnCard2(InputAction.CallbackContext callbackContext)
    {
        cardButtonPressed?.Invoke(1, isInverting ? CardOrientation.Inverted : CardOrientation.Normal);
    }

    public void OnCard3(InputAction.CallbackContext callbackContext)
    {
        cardButtonPressed?.Invoke(2, isInverting ? CardOrientation.Inverted : CardOrientation.Normal);
    }

    public void OnCard4(InputAction.CallbackContext callbackContext)
    {
        cardButtonPressed?.Invoke(3, isInverting ? CardOrientation.Inverted : CardOrientation.Normal);
    }

    public void OnPause(InputAction.CallbackContext callbackContext)
    {
        pausePressed?.Invoke();
    }

    public void OnEscape(InputAction.CallbackContext callbackContext)
    {
        escapePressed?.Invoke();
    }

    void Update()
    {
        Vector3 pos = transform.position;
        float motion = movement.y * speed * Time.deltaTime;
        float result = pos.y + motion;
        float clamped = Mathf.Clamp(result, yMin, yMax);
        // Debug.Log($"POS: {pos.y}, MOTION: {motion}, RESULT: {result}, CLAMPED: {clamped}, MIN: {yMin}, MAX: {yMax}");
        transform.position = new Vector3(pos.x, clamped, pos.z);
    }
}
