using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PongGame : MonoBehaviour
{
    private Player[] players;
    private List<Ball> balls;
    private Dictionary<PlayerSide, int> scores;
    private GameState state = GameState.Menu;
    
    public int winningScore = 10;
    public float ballAddDelaySeconds = 0.3f;
    public float ballRemoveDelaySeconds = 0.3f;
    public float startingBallYMin = -4f;
    public float startingBallYMax = 4f;
    public float startingBallSpeed = 10f;
    public float menuFadeSeconds = 0.2f;
    // public UIDocument hud;
    public Menu uiMenu;
    public Gameplay uiGameplay;
    public PlayCard uiPlayCard;
    public Pause uiPause;
    public Transform ballsParent;
    public Ball prefabDefaultBall;

    // Args: Winner, winner score, loser score
    public event Action<PlayerSide, int, int> gameEnded;
    
    // Start is called before the first frame update
    void Start()
    {
        players = FindObjectsByType<Player>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        // Hook up event listeners for goal scoring.
        foreach (var player in players)
        {
            if (player.side == PlayerSide.Left)
            {
                player.goalScoredAgainst += ball => OnGoalScoredAgainst(ball, PlayerSide.Left);
            }
            else
            {
                player.goalScoredAgainst += ball => OnGoalScoredAgainst(ball, PlayerSide.Right);
            }
        }
        
        // Start a ball going!
        StartGame();
    }

    public void StartGame()
    {
        ResetBalls();
        ResetScore();
        SetState(GameState.Menu);
        StartCoroutine(CoAddBall(CreateBall(), 0));
    }

    private void ResetBalls()
    {
        if (balls == null)
        {
            balls = new List<Ball>();
        }
        
        foreach (var ball in balls)
        {
            RemoveBall(ball);
        }
    }

    private void ResetScore()
    {
        scores = new Dictionary<PlayerSide, int>()
        {
            { PlayerSide.Left, 0 },
            { PlayerSide.Right, 0 }
        };
        
        foreach (var pair in scores)
        {
            uiGameplay.SetScore(pair.Key, pair.Value);
        }
    }

    private void SetState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                uiMenu.FadeIn(menuFadeSeconds);
                uiPlayCard.FadeOut(menuFadeSeconds);
                uiPause.FadeOut(menuFadeSeconds);
                break;
            
            case GameState.Gameplay:
                uiMenu.FadeOut(menuFadeSeconds);
                uiPlayCard.FadeOut(menuFadeSeconds);
                uiPause.FadeOut(menuFadeSeconds);
                break;
            
            case GameState.PlayCard:
                uiMenu.FadeOut(menuFadeSeconds);
                uiPlayCard.FadeIn(menuFadeSeconds);
                uiPause.FadeOut(menuFadeSeconds);
                break;
            
            case GameState.Pause:
                uiMenu.FadeOut(menuFadeSeconds);
                uiPlayCard.FadeOut(menuFadeSeconds);
                uiPause.FadeIn(menuFadeSeconds);
                break;
        }
        
    }

    private void OnGoalScoredAgainst(Ball ball, PlayerSide side)
    {
        PlayerSide sideScored = side.Opposite();
        int newScore = scores[sideScored] + 1;
        scores[sideScored] = newScore;
        uiGameplay.SetScore(sideScored, newScore);
        Debug.Log($"{sideScored} scored ({newScore})!");

        // Detect that a player won.
        if (newScore >= winningScore)
        {
            var loserScore = scores[side];
            Debug.Log($"Winner winner, chicken dinner! {sideScored} wins ({newScore} to {loserScore})!");
            gameEnded?.Invoke(sideScored, newScore, loserScore);
        }

        // Begin the delay to delete this ball from play.
        StartCoroutine(CoRemoveBall(ball, ballRemoveDelaySeconds));
    }

    public void AddBall(Ball ball)
    {
        balls.Add(ball);
    }

    public Ball CreateBall()
    {
        return Instantiate(prefabDefaultBall, ballsParent);
    }

    public void RemoveBall(Ball ball)
    {
        balls.Remove(ball);
        Destroy(ball.gameObject);

        // If this is the last important ball on the field, create a new one.
        if (!IsGameEnded() && !balls.Any(ball => ball.isImportant))
        {
            StartCoroutine(CoAddBall(CreateBall(), ballAddDelaySeconds));
        }
    }

    private IEnumerator CoAddBall(Ball ball, float delay)
    {
        AddBall(ball);
        ball.PrepareToGo(delay);
        yield return new WaitForSeconds(delay);
        Vector2 ballStartPosition = new Vector2(0, Random.Range(startingBallYMin, startingBallYMax));
        Vector2 ballStartVelocity = Quaternion.AngleAxis(360f * Random.value, Vector3.forward) * Vector2.up * startingBallSpeed;
        ball.Go(ballStartPosition, ballStartVelocity);
    }

    private IEnumerator CoRemoveBall(Ball ball, float delay)
    {
        ball.PrepareToDie(delay);
        yield return new WaitForSeconds(delay);
        RemoveBall(ball);
    }

    public Player GetPlayer(PlayerSide side)
    {
        // If this throws an exception, a player is missing or player sides probably aren't set correctly.
        return players.First(x => x.side == side);
    }

    public bool IsGameEnded()
    {
        foreach (var pair in scores)
        {
            if (pair.Value >= winningScore)
            {
                return true;
            }
        }

        return false;
    }
}
