using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PongGame : MonoBehaviour
{
    private Player[] players;
    private List<Ball> balls;
    private Dictionary<PlayerSide, int> scores;
    
    public GameState state = GameState.Menu;
    public int winningScore = 10;
    public float ballAddDelaySeconds = 0.3f;
    public float ballRemoveDelaySeconds = 0.3f;
    [FormerlySerializedAs("ballCrossProductThreshold")] public float ballDotProductThreshold = 0.25f;
    public float startingBallYMin = -4f;
    public float startingBallYMax = 4f;
    public float startingBallSpeed = 10f;
    public float menuFadeSeconds = 0.2f;
    public Menu uiMenu;
    public Gameplay uiGameplay;
    public PlayCard uiPlayCard;
    public Pause uiPause;
    public Win uiWin;
    public Transform ballsParent;
    public Ball prefabDefaultBall;

    // Args: Winner, winner score, loser score
    public event Action<PlayerSide, int, int> gameEnded;
    public event Action<PlayerSide, CardData> cardPlayed;
    public event Action<PlayerSide, CardOrientation> cardsOriented;
    
    // Start is called before the first frame update
    void Start()
    {
        Pause();
        players = FindObjectsByType<Player>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        foreach (var player in players)
        {
            if (player.side == PlayerSide.Left)
            {
                player.goalScoredAgainst += ball => OnGoalScoredAgainst(ball, PlayerSide.Left);
                player.cardPlayed += cardData => OnCardPlayed(cardData, PlayerSide.Left);
                player.cardsOriented += orientation => OnCardsOriented(orientation, PlayerSide.Left);
                player.paused += OnPaused;
                player.escaped += OnEscaped;
            }
            else
            {
                player.goalScoredAgainst += ball => OnGoalScoredAgainst(ball, PlayerSide.Right);
                player.cardPlayed += cardData => OnCardPlayed(cardData, PlayerSide.Right);
                player.cardsOriented += orientation => OnCardsOriented(orientation, PlayerSide.Right);
                player.paused += OnPaused;
                player.escaped += OnEscaped;
            }
        }
        
        // Hook up state change listeners.
        uiMenu.play2PSelected += OnMenu2PSelected;
        // uiPause.ended += OnPauseEnded;
        // TODO: Return to menu from pause
        
        // Configure the initial state for the state set in the inspector.
        SetState(state);
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        Time.timeScale = 1;
    }

    public void StartGame()
    {
        Debug.Log("Started a game");
        ResetBalls();
        ResetScore();
        // SetState(GameState.Gameplay);
        StartCoroutine(CoAddBall(CreateBall(), 0));
        Unpause();
    }

    public void ResetPlayers()
    {
        foreach (var player in players)
        {
            player.Reset();
            player.Reveal();
        }
    }

    private void ResetBalls()
    {
        if (balls == null)
        {
            balls = new List<Ball>();
        }

        var tempBalls = new List<Ball>(balls);
        foreach (var ball in tempBalls)
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
        Debug.Log("Setting state to " + state);
        // TODO: Logic for control map state and such
        switch (state)
        {
            case GameState.Menu:
                uiMenu.FadeIn(menuFadeSeconds);
                uiPlayCard.FadeOut(menuFadeSeconds);
                uiPause.FadeOut(menuFadeSeconds);
                uiWin.FadeOut(menuFadeSeconds);
                ResetBalls();
                ResetPlayers();
                Pause();
                break;
            
            case GameState.Gameplay:
                uiMenu.FadeOut(menuFadeSeconds);
                uiPlayCard.FadeOut(menuFadeSeconds);
                uiPause.FadeOut(menuFadeSeconds);
                uiWin.FadeOut(menuFadeSeconds);
                StartGame();
                break;
            
            case GameState.PlayCard:
                uiMenu.FadeOut(menuFadeSeconds);
                uiPlayCard.FadeIn(menuFadeSeconds);
                uiPause.FadeOut(menuFadeSeconds);
                uiWin.FadeOut(menuFadeSeconds);
                Pause();
                uiPlayCard.Go(CardType.Joker, CardOrientation.Inverted);
                break;
            
            case GameState.Pause:
                uiMenu.FadeOut(menuFadeSeconds);
                uiPlayCard.FadeOut(menuFadeSeconds);
                uiPause.FadeIn(menuFadeSeconds);
                uiWin.FadeOut(menuFadeSeconds);
                Pause();
                break;
            
            case GameState.Win:
                uiMenu.FadeOut(menuFadeSeconds);
                uiPlayCard.FadeOut(menuFadeSeconds);
                uiPause.FadeOut(menuFadeSeconds);
                uiWin.FadeIn(menuFadeSeconds);
                ResetBalls();
                ResetPlayers();
                Pause();
                break;
        }

        this.state = state;
    }

    private void OnCardPlayed(CardData cardData, PlayerSide side)
    {
        if (state != GameState.Gameplay)
        {
            return;
        }
        
        Debug.Log($"{side} used {cardData.type} card in {cardData.orientation} orientation.");
        cardPlayed?.Invoke(side, cardData); 
    }

    private void OnCardsOriented(CardOrientation orientation, PlayerSide side)
    {
        if (state != GameState.Gameplay)
        {
            return;
        }

        cardsOriented?.Invoke(side, orientation);
    }

    private void OnPaused()
    {
        if (state == GameState.Pause)
        {
            SetState(GameState.Gameplay);
        }
        else
        {
            SetState(GameState.Pause);
        }
    }

    private void OnEscaped()
    {
        if (state != GameState.Pause)
        {
            return;
        }
        
        SetState(GameState.Menu);
    }

    private void OnMenu2PSelected()
    {
        if (state != GameState.Menu)
        {
            return;
        }
        
        SetState(GameState.Gameplay);
    }

    private void OnGoalScoredAgainst(Ball ball, PlayerSide side)
    {
        if (state != GameState.Gameplay)
        {
            RemoveBall(ball);
            return;
        }
        
        PlayerSide sideScored = side.Opposite();
        int newScore = scores[sideScored] + 1;
        scores[sideScored] = newScore;
        uiGameplay.SetScore(sideScored, newScore);
        // Debug.Log($"{sideScored} scored ({newScore})!");

        // Detect that a player won.
        if (newScore >= winningScore)
        {
            var loserScore = scores[side];
            Debug.Log($"Winner winner, chicken dinner! {sideScored} wins ({newScore} to {loserScore})!");
            uiWin.SetWinningSide(sideScored);
            SetState(GameState.Win);
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
        Vector2 ballStartVelocity = Vector2.zero;
        do
        {
            // We threshold the dot product with Right/Left to ensure the ball never starts with a velocity that is
            // so predominantly up/down that it takes many bounces to reach a player's paddle.
            ballStartVelocity = Quaternion.AngleAxis(360f * Random.value, Vector3.forward) * Vector2.up * startingBallSpeed;
        }
        while (Mathf.Abs(Vector3.Dot(ballStartVelocity, Vector2.right)) < ballDotProductThreshold);
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
