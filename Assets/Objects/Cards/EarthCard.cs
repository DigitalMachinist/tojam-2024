using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthCard : MonoBehaviour ,ICardEffect
{

    [SerializeField]
    GameObject barrierPrefab;
    [SerializeField]
    public float EffectTime => 10f;
    public Vector2 xRange = new Vector2(0, 8);
    public Vector2 yRange = new Vector2(-2.8f, 4.8f);
    private float timer = -1f;
    private Dictionary<PlayerSide,bool> _effectActive = new Dictionary<PlayerSide,bool>(){{PlayerSide.Right, false},{PlayerSide.Left, false}};
    public bool Triggered { get; private set; }


    private void Awake() {
        Triggered = false;
    }

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

        if(_effectActive[PlayerSide.Right] && paddle.playerSide == PlayerSide.Right)   
        {
            ChangeBallCollisionLayer(true, ball);
        }
        else if(_effectActive[PlayerSide.Right] && paddle.playerSide == PlayerSide.Left)   
        {
             ball.gameObject.layer =  LayerMask.NameToLayer("Ball");
        }
        else if(_effectActive[PlayerSide.Left] && paddle.playerSide == PlayerSide.Left)
        {
            ChangeBallCollisionLayer(false, ball);
        }
        else if(_effectActive[PlayerSide.Left] && paddle.playerSide == PlayerSide.Right)
        {
             ball.gameObject.layer =  LayerMask.NameToLayer("Ball");
        }
    }

    public void OnCardPlayed(PlayerSide playerSide, CardData card)
    {
        if(card.type != CardType.Earth)
            return;

        timer = 0f;

        Debug.Log("Earth card played");
        //need to do physic layer thing, but I'll do that fresh brains since it'll take me 405% less time then
        //BUG? Are the players actualy set correcly 
        if(playerSide == PlayerSide.Right && !_effectActive[playerSide] && card.orientation == CardOrientation.Normal)
        {
            EnableEffect(true);
            EnableEffect(true);
            EnableEffect(true);
            _effectActive[playerSide] = true;
            StartCoroutine(ReturnBallToNormal(EffectTime, playerSide));
        }    
        else if(playerSide == PlayerSide.Right&& !_effectActive[playerSide]  && card.orientation == CardOrientation.Inverted)
        {
            EnableEffect(false);
            EnableEffect(false);
            EnableEffect(false);
            _effectActive[playerSide] = true;
            StartCoroutine(ReturnBallToNormal(EffectTime, playerSide));
        }
        else if(playerSide == PlayerSide.Left && !_effectActive[playerSide] && card.orientation == CardOrientation.Normal)
        {
            EnableEffect(false);
            EnableEffect(false);
            EnableEffect(false);
            _effectActive[playerSide] = true;
            StartCoroutine(ReturnBallToNormal(EffectTime, playerSide));
        }
        else if(playerSide == PlayerSide.Left && !_effectActive[playerSide] && card.orientation == CardOrientation.Inverted)
        {
            EnableEffect(true);
            EnableEffect(true);
            EnableEffect(true);
            _effectActive[playerSide] = true;
            StartCoroutine(ReturnBallToNormal(EffectTime, playerSide));
        }
    }

private void ChangeBallCollisionLayer(bool rightPlayer, Ball ball)
{
        if(rightPlayer)
        {
            ball.gameObject.layer =  LayerMask.NameToLayer("Ball Right");
        }
        else
        {
  
            ball.gameObject.layer =  LayerMask.NameToLayer("Ball Left");
        }
}



    public void EnableEffect(bool rightPlayer)
    {
        
        GameObject barrier = Instantiate(barrierPrefab, new Vector3(Random.Range(xRange.x, xRange.y * (!rightPlayer? 1:-1)), Random.Range(yRange.x,yRange.y)), Quaternion.identity);
        if(rightPlayer)
        {
            barrier.layer =  LayerMask.NameToLayer("Obstacle Right");
        }
        else
        {
            barrier.layer =  LayerMask.NameToLayer("Obstacle Left");
        }

        Destroy(barrier, EffectTime);
    }

    IEnumerator ReturnBallToNormal(float waitTime, PlayerSide playerSide)
    {
        yield return new WaitForSeconds(waitTime);
        var balls = FindObjectsOfType<Ball>();
        foreach (var ball in balls)
        {
            ball.gameObject.layer =  LayerMask.NameToLayer("Ball");
        } 
        _effectActive[playerSide] = false;
    }

    public void EnableInvertedEffect(bool rightPlayer)
    {
        GameObject barrier = Instantiate(barrierPrefab, new Vector3(Random.Range(xRange.x, xRange.y * (rightPlayer? 1:-1)), Random.Range(yRange.x,yRange.y)), Quaternion.identity);
        Destroy(barrier, EffectTime);

        if(rightPlayer)
        {
            barrier.layer =  LayerMask.NameToLayer("Obstacle Right");

            var balls = FindObjectsOfType<Ball>();
            foreach (var ball in balls)
            {
            ball.gameObject.layer =  LayerMask.NameToLayer("Ball Right");
            } 
        }
        else
        {
            barrier.layer =  LayerMask.NameToLayer("Obstacle Left");

            var balls = FindObjectsOfType<Ball>();
            foreach (var ball in balls)
            {
                ball.gameObject.layer =  LayerMask.NameToLayer("Ball Left");
            } 
        }
    }

    public void ManualUpdate()
    {
        
    }

    public void OnEffectEnd()
    {
        var barriers = FindObjectsOfType<Barrier>();
        foreach (var barrier in barriers)
        {
            Destroy(barrier);
        }
    }
}
