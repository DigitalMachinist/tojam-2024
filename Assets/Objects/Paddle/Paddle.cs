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

    public event Action<int, CardOrientation> cardButtonPressed;
    public event Action pausePressed;

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
    }

    public void OnInvert(InputAction.CallbackContext callbackContext)
    {
        isInverting = callbackContext.action.IsPressed();
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

    void Update()
    {
        Vector3 pos = transform.position;
        float yPos = Mathf.Clamp(pos.y + movement.y * speed * Time.deltaTime, yMin, yMax);
        transform.position = new Vector3(pos.x, yPos, pos.z);
    }
}
