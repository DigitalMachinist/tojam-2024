using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private bool inPlay = false;

    public float startingYMin = -4f;
    public float startingYMax = 4f;
    public float startingSpeed = 1f;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Go();
    }

    public void Go()
    {
        inPlay = true;
        
        // TODO: The code that spawns this should probably decide this stuff.
        transform.position = new Vector2(0, Random.Range(startingYMin, startingYMax));
        
        Quaternion randomRotation = Quaternion.AngleAxis(360f * Random.value, Vector3.forward);
        Vector2 initialVelocity = randomRotation * Vector2.up * startingSpeed;
        Debug.Log(initialVelocity);
        rigidbody.velocity = initialVelocity;
    }

    public void PrepareToGo(float delay)
    {
        inPlay = false;
        
        // TODO: Play some effect to cover the addition of the ball to play.
    }

    public void PrepareToDie(float delay)
    {
        // Take the ball out of play so it won't accidentally score any further points in some fluke.
        inPlay = false;
        
        // TODO: Play some effect to cover the death/removal of the ball from play.
    }

    public bool IsInPlay()
    {
        return inPlay;
    }
}
