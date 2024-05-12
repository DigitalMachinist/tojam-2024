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
    public bool Triggered { get; private set; }

    private void Awake() {
        Triggered = false;
    }

    private void Start()
    {
        PongGame pg = FindObjectOfType<PongGame>();

        pg.cardPlayed += OnCardPlayed;
    }
    private void OnDestroy() 
    {
        PongGame pg = FindObjectOfType<PongGame>();
        pg.cardPlayed -= OnCardPlayed;
    }


    public void OnCardPlayed(PlayerSide playerSide, CardData card)
    {
        if(card.type != CardType.Earth)
            return;

        timer = 0f;

        //need to do physic layer thing, but I'll do that fresh brains since it'll take me 405% less time then
        //BUG? Are the players actualy set correcly 
        if(playerSide == PlayerSide.Right && card.orientation == CardOrientation.Normal)
        {
            EnableEffect(true);
        }    
        else if(playerSide == PlayerSide.Right && card.orientation == CardOrientation.Inverted)
        {
            EnableEffect(false);
        }
        else if(playerSide == PlayerSide.Left && card.orientation == CardOrientation.Normal)
        {
            EnableEffect(false);
        }
        else if(playerSide == PlayerSide.Left && card.orientation == CardOrientation.Inverted)
        {
            EnableEffect(true);
        }
    }

    public void EnableEffect(bool rightPlayer)
    {
        GameObject barrier = Instantiate(barrierPrefab, new Vector3(Random.Range(xRange.x, xRange.y * (!rightPlayer? 1:-1)), Random.Range(yRange.x,yRange.y)), Quaternion.identity);
        Destroy(barrier, EffectTime);
    }

    public void EnableInvertedEffect(bool rightPlayer)
    {
        GameObject barrier = Instantiate(barrierPrefab, new Vector3(Random.Range(xRange.x, xRange.y * (rightPlayer? 1:-1)), Random.Range(yRange.x,yRange.y)), Quaternion.identity);
        Destroy(barrier, EffectTime);

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
