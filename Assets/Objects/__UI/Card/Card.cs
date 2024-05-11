using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour, IFlippable, IOrientable
{
    private IEnumerator coRotate;
    private IEnumerator coFlip;
    
    public CardType type = CardType.Joker;
    public CardOrientation orientation = CardOrientation.Normal;
    public CardFacing facing = CardFacing.Face;
    public Sprite spriteEarth;
    public Sprite spriteFire;
    public Sprite spriteIce;
    public Sprite spriteShock;
    public Sprite spriteSpaaace;
    public Sprite spriteJoker;
    public Image face;
    public Image back;

    public event Action rotateComplete;
    public event Action flipComplete;

    void Awake()
    {
        SetType(type);
        SetOrientation(orientation);
        SetFacing(facing);
    }

    public void SetType(CardType type)
    {
        this.type = type;
        switch (type)
        {
            case CardType.Earth:
                face.sprite = spriteEarth;
                break;
            
            case CardType.Fire:
                face.sprite = spriteFire;
                break;
            
            case CardType.Ice:
                face.sprite = spriteIce;
                break;
            
            case CardType.Shock:
                face.sprite = spriteShock;
                break;
            
            case CardType.Spaaace:
                face.sprite = spriteSpaaace;
                break;
            
            case CardType.Joker:
                face.sprite = spriteJoker;
                break;
        }
    }

    public void SetOrientation(CardOrientation orientation, float delay = 0f, bool useUnscaledTime = true)
    {
        this.orientation = orientation;
        Rotate(orientation, delay, useUnscaledTime);
    }

    private void Rotate(CardOrientation orientation, float delay, bool useUnscaledTime = true)
    {
        Quaternion targetRotation = orientation == CardOrientation.Normal 
            ? Quaternion.identity 
            : Quaternion.Euler(0f, 180f, 0f);
        
        if (delay == 0f)
        {
            face.rectTransform.localRotation = targetRotation;
            back.rectTransform.localRotation = targetRotation;
            rotateComplete?.Invoke();
            return;
        }

        if (coRotate != null)
        {
            StopCoroutine(coRotate);
        }

        coRotate = CoRotate(targetRotation, delay, useUnscaledTime);
        StartCoroutine(coRotate);
    }

    private IEnumerator CoRotate(Quaternion targetRotation, float delay, bool useUnscaledTime)
    {
        float timeElapsed = 0f;
        Quaternion startRotation = face.rectTransform.localRotation;
        while (timeElapsed < delay)
        {
            yield return null;
            timeElapsed += useUnscaledTime ? Time.unscaledTime : Time.deltaTime;
            float fraction = timeElapsed / delay;
            Quaternion currentRotation = Quaternion.Slerp(startRotation, targetRotation, fraction);
            face.rectTransform.localRotation = currentRotation;
            back.rectTransform.localRotation = currentRotation;
        } 
        
        face.rectTransform.localRotation = targetRotation;
        back.rectTransform.localRotation = targetRotation;
        coRotate = null;
        rotateComplete?.Invoke();
    }

    public void SetFacing(CardFacing facing, float delay = 0f, bool useUnscaledTime = true)
    {
        this.facing = facing;

        Flip(facing, delay, useUnscaledTime);
    }

    private void Flip(CardFacing facing, float delay, bool useUnscaledTime = true)
    {
        float targetFaceScale = facing == CardFacing.Face ? 1f : 0f;
        float targetBackScale = GetBackScale(targetFaceScale);
        
        if (delay == 0f)
        {
            face.rectTransform.localScale = new Vector3(targetFaceScale, 1f, 1f);
            back.rectTransform.localScale = new Vector3(targetBackScale, 1f, 1f);
            flipComplete?.Invoke();

            return;
        }

        if (coFlip != null)
        {
            StopCoroutine(coFlip);
        }

        coFlip = CoFlip(targetFaceScale, delay, useUnscaledTime);
        StartCoroutine(coFlip);
    }

    private IEnumerator CoFlip(float targetFaceScale, float delay, bool useUnscaledTime)
    {
        float timeElapsed = 0f;
        float targetBackScale = GetBackScale(targetFaceScale);
        float startFaceScale = face.rectTransform.localScale.x;
        float startBackScale = back.rectTransform.localScale.x;
        while (timeElapsed < delay)
        {
            yield return null;
            timeElapsed += useUnscaledTime ? Time.unscaledTime : Time.deltaTime;
            float fraction = timeElapsed / delay;
            face.rectTransform.localScale = new Vector3(Mathf.Lerp(startFaceScale, targetFaceScale, fraction), 1f, 1f);
            back.rectTransform.localScale = new Vector3(Mathf.Lerp(startBackScale, targetBackScale, fraction), 1f, 1f);
        } 
        
        face.rectTransform.localScale = new Vector3(targetFaceScale, 1f, 1f);
        back.rectTransform.localScale = new Vector3(targetBackScale, 1f, 1f);
        coFlip = null;
        flipComplete?.Invoke();
    }

    private float GetBackScale(float faceScale)
    {
        return 1f - faceScale;
    }
}
