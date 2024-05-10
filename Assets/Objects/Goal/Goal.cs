using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public event Action<Ball> goalScored;
    
    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (!ball.IsInPlay())
        {
            return;
        }
        
        goalScored?.Invoke(ball);
    }
}
