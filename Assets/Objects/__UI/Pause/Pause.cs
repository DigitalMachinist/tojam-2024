using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class Pause : MonoBehaviour, IFadeable
{
    private IEnumerator coFade;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
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
            timeElapsed += useUnscaledTime ? Time.unscaledTime : Time.deltaTime;
            float fraction = timeElapsed / delay;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, fraction);
        }

        canvasGroup.alpha = targetAlpha;
        coFade = null;
    }
}
