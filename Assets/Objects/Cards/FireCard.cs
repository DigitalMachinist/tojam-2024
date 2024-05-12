using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCard : MonoBehaviour ,ICardEffect
{
    public float EffectTime => 10;

    public bool Triggered => throw new System.NotImplementedException();

    Dictionary<PlayerSide,bool> _effectActive = new Dictionary<PlayerSide,bool>(){{PlayerSide.Right, false},{PlayerSide.Left, false}};

    private void Start()
    {
        PongGame pg = FindObjectOfType<PongGame>();

        pg.cardPlayed += OnCardPlayed;
        
        pg.ballSpawned += OnBallSpawned;
    }

    private void OnBallSpawned(Ball ball)
    {
       ball.ballCollision += OnBallCollision;
    }

    void OnBallCollision(Ball ball, Collision2D col)
    {  
       if(!col.gameObject.TryGetComponent<Paddle>(out Paddle paddle))
            return;

        if((_effectActive[PlayerSide.Right] && paddle.playerSide == PlayerSide.Right)
        ||(_effectActive[PlayerSide.Left] && paddle.playerSide == PlayerSide.Left))   
        {
            ball.GetComponent<Rigidbody2D>().velocity *= 1.4f;
        }

    }

    public void OnCardPlayed(PlayerSide playerSide, CardData card)
    {
        if(card.type != CardType.Fire)
                return;

       
        if(!_effectActive[playerSide] && card.orientation == CardOrientation.Normal)
        {
            _effectActive[playerSide] = true;
            StartCoroutine(DisableEffect(EffectTime, playerSide));
        }
        else if(!_effectActive[playerSide] && card.orientation == CardOrientation.Inverted)
        {
            Debug.Log("Got here2");
            var paddles = FindObjectsOfType<Paddle>();
            foreach (var paddle in paddles)
            {
                if(paddle.playerSide!= playerSide)
                {
                    Debug.Log("Got here22");
                    _effectActive[playerSide] = true;
                    StartCoroutine(ReturnToNormalSpeed(playerSide, EffectTime, paddle));
                    paddle.speed *= 8f;
                }
            }
        }
    }

    IEnumerator DisableEffect( float waitTime, PlayerSide playerSide)
    {
        yield return new WaitForSeconds(waitTime);
        _effectActive[playerSide] = false;

        // Debug.Log("return to normal speed");

    }

    IEnumerator ReturnToNormalSpeed(PlayerSide playerSide, float waitTime, Paddle paddle)
    {
        yield return new WaitForSeconds(waitTime);
        paddle.speed = paddle.regularSpeed;
        _effectActive[playerSide] = false;
        // Debug.Log("return to normal speed");

    }

    public void EnableEffect(bool rightPlayer)
    {

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
}
