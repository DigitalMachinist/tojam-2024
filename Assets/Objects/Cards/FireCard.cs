using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCard : MonoBehaviour ,ICardEffect
{
    public float EffectTime => throw new System.NotImplementedException();

    public bool Triggered => throw new System.NotImplementedException();

    Dictionary<PlayerSide,bool> _fireActive = new Dictionary<PlayerSide,bool>(){{PlayerSide.Right, false},{PlayerSide.Left, false}};

    private void Start()
    {
        PongGame pg = FindObjectOfType<PongGame>();

        pg.cardPlayed += OnCardPlayed;
    }


    public void OnCardPlayed(PlayerSide playerSide, CardData card)
    {
        if(card.type != CardType.Fire)
                return;

       
        if(!_fireActive[playerSide] && card.orientation == CardOrientation.Normal)
        {
            //TODO: Add event for ball bouncing?
        }
        else if(!_fireActive[playerSide] && card.orientation == CardOrientation.Inverted)
        {
            Debug.Log("Got here1");
            var paddles = FindObjectsOfType<Paddle>();
            foreach (var paddle in paddles)
            {
                if(paddle.playerSide!= playerSide)
                {
                    Debug.Log("Got here11");
                    _fireActive[playerSide] = true;
                    StartCoroutine(ReturnToNormalSpeed(playerSide, 10, paddle, paddle.speed));
                    paddle.speed *= 8f;
                }
            }
        }
    }

    IEnumerator ReturnToNormalSpeed(PlayerSide playerSide, float waitTime, Paddle paddle, float originalSpeed)
    {
        yield return new WaitForSeconds(waitTime);
        paddle.speed = originalSpeed;
        _fireActive[playerSide] = false;
        Debug.Log("return to normal speed");

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
