using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class ShockCard : MonoBehaviour ,ICardEffect
{
    private PongGame pongGame;

    public float paralysisDuration = 10f;
    
    public float EffectTime => 10f;

    public bool Triggered => throw new System.NotImplementedException();

    Dictionary<PlayerSide,bool> _effectActive = new Dictionary<PlayerSide,bool>(){{PlayerSide.Right, false},{PlayerSide.Left, false}};

    Ball [] rightBalls;
    Ball [] leftBalls;
    Paddle rightPaddle;
    Paddle leftPaddle;

    private void Start()
    {
        pongGame = FindObjectOfType<PongGame>();
        pongGame.cardPlayed += OnCardPlayed;

        var paddles = FindObjectsOfType<Paddle>();
        foreach (var paddle in paddles)
        {
            if(paddle.playerSide!= PlayerSide.Right)
            {
                rightPaddle = paddle;
       
            }
            else
            {
                leftPaddle = paddle;
            }
        }       
    }

    // Update is called once per frame
    void Update()
    {
        if(_effectActive[PlayerSide.Right])
        {
            foreach(var ball in rightBalls)
            {
                if(ball)
                    ball?.GetComponent<Rigidbody>().AddForce(ball.GetComponent<Rigidbody>().transform.position.AsVector2() - 
                new Vector2 (leftPaddle.transform.position.x,rightPaddle.transform.position.y).normalized);
            }
        }
        else if(_effectActive[PlayerSide.Left])
        {
            foreach(var ball in leftBalls)
            {
              ball.GetComponent<Rigidbody>().AddForce(ball.GetComponent<Rigidbody>().transform.position.AsVector2() - 
                new Vector2 (leftPaddle.transform.position.x,rightPaddle.transform.position.y).normalized);
            }
        }

    }

   public void OnCardPlayed(PlayerSide playerSide, CardData card)
    {
        if(card.type != CardType.Shock)
                return;

        if(!_effectActive[playerSide] && card.orientation == CardOrientation.Normal)
        {
            _effectActive[playerSide] = true;
            pongGame.sfxShockAttractStart.Play();
            if(playerSide == PlayerSide.Right)
            {
                  rightBalls = FindObjectsOfType<Ball>();
            }
            else            
            {
                  leftBalls = FindObjectsOfType<Ball>();
            }
            StartCoroutine(DisableEffect(EffectTime, playerSide));
            if (playerSide == PlayerSide.Left)
            {
                leftPaddle.EnableShockAttract();
            }
            else
            {
                rightPaddle.EnableShockAttract();
            }
        }

        if(!_effectActive[playerSide] && card.orientation == CardOrientation.Inverted)
        {
            pongGame.GetPlayer(playerSide.Opposite()).Paddle.EnableParalysis(paralysisDuration);
        }
    }

    IEnumerator DisableEffect( float waitTime, PlayerSide playerSide)
    {
        yield return new WaitForSeconds(waitTime);
        _effectActive[playerSide] = false;
        if (playerSide == PlayerSide.Left)
        {
            leftPaddle.DisableShockAttract();
        }
        else
        {
            rightPaddle.DisableShockAttract();
        }
    }

    public void EnableEffect(bool rightPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void EnableInvertedEffect(bool rightPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void ManualUpdate()
    {
        throw new System.NotImplementedException();
    }

 

    public void OnEffectEnd()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update

}
