using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    protected Rigidbody2D rigidbody;
    protected bool inPlay = false;
    protected PongGame pongGame;

    public Rigidbody2D Rigidbody => rigidbody;

    public bool isImportant = true;

    public event Action<Ball, Collision2D> ballCollision;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        pongGame = FindObjectOfType<PongGame>();
    }

    public void Go(Vector2 startPosition, Vector2 startVelocity)
    {
        inPlay = true;
        transform.position = startPosition;
        rigidbody.velocity = startVelocity;
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

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        ballCollision?.Invoke(this, other);
        
        Paddle paddle = other.gameObject.GetComponent<Paddle>();
        if (paddle != null)
        {
            if (paddle.playerSide == PlayerSide.Left)
            {
                gameObject.layer = LayerMask.NameToLayer("Ball Left");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Ball Right");
            }
            pongGame.sfxBallHitPaddle.Play();
            return;
        }

        Barrier barrier = other.gameObject.GetComponent<Barrier>();
        if (barrier != null)
        {
            pongGame.sfxEarthBarrierHit.Play();
            return;
        }
        
        pongGame.sfxBallHitOther.Play();
    }
}
