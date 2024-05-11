using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour, IOrientable, IFlippable
{
    private Card card;

    public event Action cardOrientationComplete;
    public event Action cardFlipComplete;

    void Awake()
    {
        card = GetComponentInChildren<Card>();

        card.orientationComplete += cardOrientationComplete;
        card.flipComplete += cardFlipComplete;
    }

    public void SetType(CardType type)
    {
        card.SetType(type);
    }

    public void SetOrientation(CardOrientation orientation, float delay, bool useUnscaledtime = true)
    {
        card.SetOrientation(orientation, delay, useUnscaledtime);
    }

    public void SetFacing(CardFacing facing, float delay, bool useUnscaledtime = true)
    {
        card.SetFacing(facing, delay, useUnscaledtime);
    }
}
