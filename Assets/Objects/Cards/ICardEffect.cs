using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardEffect 
{
    public float EffectTime{ get;}
    public bool Triggered { get;}

    public void OnCardPlayed(PlayerSide playerSide, CardData card);

    public void EnableEffect(bool rightPlayer);

    public void EnableInvertedEffect(bool rightPlayer);

    public void OnEffectEnd();

    public void ManualUpdate();
}
