using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour, IOrientable, IFlippable
{
    private Card card;

    void Awake()
    {
        card = GetComponentInChildren<Card>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
