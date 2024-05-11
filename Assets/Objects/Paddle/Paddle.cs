using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Paddle : MonoBehaviour
{

    public float speed = 3.0f;
    private Vector3 movement;
    private bool inverted = false;

    private PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
    }
    
    public void OnMove( InputAction.CallbackContext callbackContext)
    {
        movement = callbackContext.ReadValue<Vector2>();
    }

    public void OnInvert( InputAction.CallbackContext callbackContext)
    {
        inverted = callbackContext.action.IsPressed();
    }



    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + movement * speed * Time.deltaTime;

        if(playerInput.actions["Card1"].triggered)
        {
            Debug.Log("Card1 pressed");
        }

    }
}
