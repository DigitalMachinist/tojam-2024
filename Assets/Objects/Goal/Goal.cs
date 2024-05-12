using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public event Action<Ball> goalScored;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
        Ball ball = other.GetComponent<Ball>();
        if (ball == null || !ball.IsInPlay())
        {
            return;
        }

        Debug.Log("Score");
        
        goalScored?.Invoke(ball);
    }
}
