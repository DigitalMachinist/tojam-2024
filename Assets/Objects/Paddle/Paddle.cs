using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Paddle : MonoBehaviour
{

    public float speed = 3.0f;
    private Vector3 movement;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void OnMove( InputAction.CallbackContext callbackContext)
    {
        movement = callbackContext.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + movement * speed * Time.deltaTime;
        // if (Input.GetButton("Fire1"))
        // {
        //     transform.position = transform.position + new Vector3(0,1) *speed * Time.deltaTime;
        // }
        // if (Input.GetButton("Fire2"))
        // {
        //     transform.position = transform.position + new Vector3(0,-1) *speed * Time.deltaTime;
        // }
    }
}
