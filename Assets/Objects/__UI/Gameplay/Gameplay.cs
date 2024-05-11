using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public Text leftScoreText;
    public Text rightScoreText;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        SetScore(PlayerSide.Left, 0);
        SetScore(PlayerSide.Right, 0);
    }

    public void SetScore(PlayerSide side, int score)
    {
        if (side == PlayerSide.Left)
        {
            leftScoreText.text = score.ToString();
        }
        else
        {
            rightScoreText.text = score.ToString();
        }
    }
}
