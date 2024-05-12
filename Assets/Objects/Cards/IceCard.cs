using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCard : MonoBehaviour
{
    private PongGame pongGame;

    public float iceNormalEffectSeconds = 10f;
    Dictionary<PlayerSide,bool> _iceActive = new Dictionary<PlayerSide,bool>(){{PlayerSide.Right, false},{PlayerSide.Left, false}};

    void Awake()
    {
        pongGame = FindObjectOfType<PongGame>();
        pongGame.cardPlayed += OnCardPlayed;
    }

    public void OnCardPlayed(PlayerSide playerSide, CardData card)
    {
        Debug.Log("icecardGOTHERE");
        if (card.type != CardType.Ice)
        {
            return;
        }

        Debug.Log("IceCard");
        if (card.orientation == CardOrientation.Normal)
        {
            pongGame.GetPlayer(playerSide.Opposite()).Paddle.EnableIce(iceNormalEffectSeconds);
        }
        else if(card.orientation == CardOrientation.Inverted)
        {
            
        }
    }

}
