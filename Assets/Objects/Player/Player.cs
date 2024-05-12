using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Goal goal;
    private Paddle paddle;
    private CardType[] cards;

    public PlayerSide side;
    public float cardFlipSeconds = 0.25f;
    public float cardCooldownSeconds = 5f;
    public float cardRevealStepSeconds = 0.25f;
    public CardSlot[] cardSlots;

    public event Action<Ball> goalScoredAgainst;
    public event Action<CardData> cardPlayed;
    public event Action<CardOrientation> cardsOriented;
    public event Action paused;

    void Awake()
    {
        goal = GetComponentInChildren<Goal>();
        paddle = GetComponentInChildren<Paddle>();

        goal.goalScored += ball => goalScoredAgainst?.Invoke(ball);
        paddle.cardButtonPressed += PlayCard;
        paddle.cardInvertPressed += () => OnCardsOriented(CardOrientation.Inverted);
        paddle.cardInvertReleased += () => OnCardsOriented(CardOrientation.Normal);
        paddle.pausePressed += () => paused?.Invoke();
    }

    private void OnCardsOriented(CardOrientation orientation)
    {
        foreach (var cardSlot in cardSlots)
        {
            cardSlot.SetOrientation(orientation, cardFlipSeconds);
        }
        
        cardsOriented?.Invoke(orientation);
    }

    public void Reset()
    {
        cards = new CardType[4];
        for (var i = 0; i < 4; i++)
        {
            cards[i] = SelectRandomValidCardType();
            
            // Show the back faces of the player's cards for now.
            cardSlots[i].SetType(cards[i]); 
            cardSlots[i].SetOrientation(CardOrientation.Normal, 0f); 
            cardSlots[i].SetFacing(CardFacing.Back, 0f);
        }
    }

    public void PlayCard(int index, CardOrientation orientation)
    {
        if (cards[index] == CardType.None)
        {
            // Debug.Log("Can't play because this card is on cooldown.");
            return;
        }
        
        cardPlayed?.Invoke(new CardData()
        {
            type = cards[index],
            orientation = orientation,
            index = index
        });
        
        CardType newType = SelectRandomValidCardType();
        StartCoroutine(CoCooldown(index, newType));
    }

    private IEnumerator CoCooldown(int index, CardType newType)
    {
        cards[index] = CardType.None;
        cardSlots[index].SetFacing(CardFacing.Back, cardFlipSeconds);
        yield return new WaitForSeconds(cardCooldownSeconds);
        cards[index] = newType;
        cardSlots[index].SetType(cards[index]); 
        cardSlots[index].SetFacing(CardFacing.Face, cardFlipSeconds);
    }

    public void Reveal()
    {
        StartCoroutine(CoReveal());
    }

    private IEnumerator CoReveal()
    {
        foreach (var cardSlot in cardSlots)
        {
            yield return new WaitForSeconds(cardRevealStepSeconds);
            cardSlot.SetFacing(CardFacing.Face, cardFlipSeconds);
        }
    }

    private CardType SelectRandomValidCardType()
    {
        CardType type;
        
        do
        {
            type = RandomUtils.GetRandomEnumValue<CardType>();
        }
        while (type == CardType.None);

        return type;
    }
}
