using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayCard : MonoBehaviour
{
    private IEnumerator coFade;
    private CanvasGroup canvasGroup;
    private Animator animator;
    private Card card;
    private int frame = 0;
    private int frames = 2;
    private float elapsedTime = 0f;

    public float cardFlipSeconds = 0.25f;
    public float frameTime = 0.1f;
    public Sprite speedFrame1;
    public Sprite speedFrame2;
    public Image background;

    public event Action ending;
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        animator = GetComponent<Animator>();
        card = GetComponentInChildren<Card>();
    }

    void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;
        if (elapsedTime >= frameTime)
        {
            frame = ++frame % frames;
            background.sprite = frame == 0 ? speedFrame1 : speedFrame2;
            elapsedTime -= frameTime;
        }
    }

    public void Go(CardType type, CardOrientation orientation)
    {
        card.SetType(type);
        card.SetOrientation(orientation);
        card.SetFacing(CardFacing.Back);
        animator.SetTrigger("Play");
    }
    
    public void FadeIn(float delay, float targetAlpha = 1f, bool useUnscaledTime = true)
    {
        Fade(delay, targetAlpha, useUnscaledTime);
    }

    public void FadeOut(float delay, float targetAlpha = 0f, bool useUnscaledTime = true)
    {
        Fade(delay, targetAlpha, useUnscaledTime);
    }

    private void Fade(float delay, float targetAlpha, bool useUnscaledTime = true)
    {
        if (coFade != null)
        {
            StopCoroutine(coFade);
        }

        if (delay == 0f)
        {
            canvasGroup.alpha = targetAlpha;
        }

        coFade = CoFade(delay, targetAlpha, useUnscaledTime);
        StartCoroutine(coFade);
    }
    
    private IEnumerator CoFade(float delay, float targetAlpha, bool useUnscaledTime)
    {
        float timeElapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        while (timeElapsed < delay)
        {
            yield return null;
            timeElapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float fraction = timeElapsed / delay;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, fraction);
        }

        canvasGroup.alpha = targetAlpha;
        coFade = null;
    }

    public void Flip()
    {
        card.SetFacing(CardFacing.Face, cardFlipSeconds, true);
    }

    public void PlaySound()
    {
        Debug.Log("BLEAT sound goes here");
    }

    public void EndAnimation()
    {
        ending?.Invoke();
    }
}
