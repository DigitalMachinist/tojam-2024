using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Goal goal;
    private Paddle paddle;
    
    public PlayerSide side;

    public event Action<Ball> goalScoredAgainst;

    void Awake()
    {
        goal = GetComponentInChildren<Goal>();
        paddle = GetComponentInChildren<Paddle>();

        goal.goalScored += ball => goalScoredAgainst?.Invoke(ball);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
