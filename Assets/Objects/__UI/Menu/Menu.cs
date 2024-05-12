using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour, IFadeable
{
    private IEnumerator coFade;
    private CanvasGroup canvasGroup;
    private float elaspedSeconds = 0f;
    private float anykeyCooldown;

    public float textBlinkSeconds = .5f;
    public GameObject blinkAnykee;
    
    public event Action play1PSelected; 
    public event Action play2PSelected; 
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // FadeOut(0f);
    }

    void Update()
    {
        anykeyCooldown -= Time.unscaledDeltaTime;
        if (anykeyCooldown < 0f)
        {
            if (Input.anyKeyDown)
            {
                play2PSelected?.Invoke();
            }
        }

        elaspedSeconds += Time.unscaledDeltaTime;
        if (elaspedSeconds >= textBlinkSeconds)
        {
            blinkAnykee.SetActive(!blinkAnykee.activeInHierarchy);
            elaspedSeconds = 0;
        }
    }

    public void FadeIn(float delay, float targetAlpha = 1f, bool useUnscaledTime = true)
    {
        anykeyCooldown = delay;
        Debug.Log("Menu fade in");
        Fade(delay, targetAlpha, useUnscaledTime);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void FadeOut(float delay, float targetAlpha = 0f, bool useUnscaledTime = true)
    {
        Fade(delay, targetAlpha, useUnscaledTime);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
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
}
