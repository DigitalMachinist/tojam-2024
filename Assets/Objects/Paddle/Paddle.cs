using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class Paddle : MonoBehaviour
{
    public int gamepadIndex;
    public float speed = 3.0f;
    private Vector3 movement;
    private bool inverted = false;

    private PlayerInput playerInput;
    private Rigidbody2D rigidbody;

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
    
    public void OnMove( InputAction.CallbackContext callbackContext)
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

    public void OnInvert( InputAction.CallbackContext callbackContext)
    {
        inverted = callbackContext.action.IsPressed();
    }



    // Update is called once per frame
    void Update()
    {
        rigidbody.velocity = movement * speed;
        // transform.position = transform.position + movement * speed * Time.deltaTime;

        if(playerInput.actions["Card1"].triggered)
        {
            Debug.Log("Card1 pressed");
        }

    }
}
