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
    private IEnumerator coAddBall;
    private Player[] players;
    private List<Ball> balls;
    private Dictionary<PlayerSide, int> scores;
    private PlayerSide playedCardSide;
    private CardData playedCardData;
    
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

    [Header("Audio")]
    public AudioSource musicTitle;
    public AudioSource[] musicsGame;
    public AudioSource sfxStart;
    public AudioSource sfxPause;
    public AudioSource sfxWin;
    public AudioSource sfxGoal;
    public AudioSource sfxBallSpawn;
    public AudioSource sfxBallHitPaddle;
    public AudioSource sfxBallHitOther;
    public AudioSource sfxActivation;
    public AudioSource sfxActivationEarth;
    public AudioSource sfxActivationFire;
    public AudioSource sfxActivationIce;
    public AudioSource sfxActivationShock1;
    public AudioSource sfxActivationShock2;
    public AudioSource sfxActivationSpaaace;
    public AudioSource sfxActivationJoker;
    public AudioSource sfxEarthBarrierSpawn;
    public AudioSource sfxEarthBarrierHit;
    public AudioSource sfxEarthBarrierBreak;

    // Args: Winner, winner score, loser score
    public event Action<PlayerSide, int, int> gameEnded;
    public event Action<PlayerSide, CardData> cardPlayed;
    public event Action<Ball> ballSpawned;
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
        uiPlayCard.ending += OnPlayCardEnded;
        uiPlayCard.playActivationSound += OnPlayCardActivationSound;
        
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
        if (coAddBall != null)
        {
            StopCoroutine(coAddBall);
            coAddBall = null;
        }
        
        Debug.Log("Started a game");
        ResetBalls();
        ResetScore();
        coAddBall = CoAddBall(CreateBall(), 0);
        StartCoroutine(coAddBall);
        
        musicTitle.Stop();
        musicsGame[Random.Range(0, musicsGame.Length - 1)].Play();
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
                musicTitle.Play();
                foreach (var audioSource in musicsGame)
                {
                    audioSource.Stop();
                }
                break;
            
            case GameState.Gameplay:
                uiMenu.FadeOut(menuFadeSeconds);
                uiPlayCard.FadeOut(menuFadeSeconds);
                uiPause.FadeOut(menuFadeSeconds);
                uiWin.FadeOut(menuFadeSeconds);
                Unpause();
                foreach (var audioSource in musicsGame)
                {
                    audioSource.volume = .2f;
                }
                sfxStart.Play();
                break;
            
            case GameState.PlayCard:
                uiMenu.FadeOut(menuFadeSeconds);
                uiPlayCard.FadeIn(menuFadeSeconds);
                uiPause.FadeOut(menuFadeSeconds);
                uiWin.FadeOut(menuFadeSeconds);
                Pause();
                foreach (var audioSource in musicsGame)
                {
                    audioSource.volume = 0.1f;
                }
                sfxActivation.Play();
                break;
            
            case GameState.Pause:
                uiMenu.FadeOut(menuFadeSeconds);
                uiPlayCard.FadeOut(menuFadeSeconds);
                uiPause.FadeIn(menuFadeSeconds);
                uiWin.FadeOut(menuFadeSeconds);
                Pause();
                foreach (var audioSource in musicsGame)
                {
                    audioSource.volume = 0.05f;
                }
                sfxPause.Play();
                break;
            
            case GameState.Win:
                uiMenu.FadeOut(menuFadeSeconds);
                uiPlayCard.FadeOut(menuFadeSeconds);
                uiPause.FadeOut(menuFadeSeconds);
                uiWin.FadeIn(menuFadeSeconds);
                ResetBalls();
                ResetPlayers();
                Pause();
                foreach (var audioSource in musicsGame)
                {
                    audioSource.volume = 0.05f;
                }
                sfxWin.Play();
                break;
        }

        this.state = state;
    }

    private void OnPlayCardActivationSound(Card card)
    {
        Debug.Log($"Play activation sound for {card.type}");
        switch (card.type)
        {
            case CardType.Earth:
                if(card.orientation == CardOrientation.Normal)
                {
                  PlayAudio(sfxActivationEarth);  
                }
                else
                {
                    PlayReverseAudio(sfxActivationEarth);
                }
                break;
            
            case CardType.Fire:
                if(card.orientation == CardOrientation.Normal)
                {
                  PlayAudio(sfxActivationFire);  
                }
                else
                {
                    PlayReverseAudio(sfxActivationFire);
                }
                break;
            
            case CardType.Ice:
                if(card.orientation == CardOrientation.Normal)
                {
                  PlayAudio(sfxActivationIce);  
                }
                else
                {
                    PlayReverseAudio(sfxActivationIce);
                }
                break;
            
            case CardType.Shock:
                if(card.orientation == CardOrientation.Normal)
                {
                  PlayAudio(sfxActivationShock1);  
                  PlayAudio(sfxActivationShock2);  
                }
                else
                {
                    PlayReverseAudio(sfxActivationShock1);
                    PlayReverseAudio(sfxActivationShock2);
                }
                break;
            
            case CardType.Spaaace:
                if(card.orientation == CardOrientation.Normal)
                {
                  PlayAudio(sfxActivationSpaaace);  
                }
                else
                {
                    PlayReverseAudio(sfxActivationSpaaace);
                }
                break;
            
            case CardType.Joker:
                if(card.orientation == CardOrientation.Normal)
                {
                  PlayAudio(sfxActivationJoker);  
                }
                else
                {
                    PlayReverseAudio(sfxActivationJoker);
                }
                break;
            
        }
    }

    public void PlayAudio(AudioSource audioSource)
    {
        audioSource.pitch = 1;
        audioSource.timeSamples =0;
        audioSource.Play();
    }

    public void PlayReverseAudio(AudioSource audioSource)
    {
        audioSource.pitch = -1;
        audioSource.timeSamples = audioSource.clip.samples - 1;
        audioSource.Play();
        
    }

    private void OnCardPlayed(CardData cardData, PlayerSide side)
    {
        if (state != GameState.Gameplay)
        {
            return;
        }
        
        Debug.Log($"{side} used {cardData.type} card in {cardData.orientation} orientation.");
        playedCardData = cardData;
        playedCardSide = side;
        SetState(GameState.PlayCard);
        uiPlayCard.Go(cardData.type, cardData.orientation);
    }

    private void OnPlayCardEnded()
    {
        Debug.Log("Play card animation ending");
        SetState(GameState.Gameplay);
        cardPlayed?.Invoke(playedCardSide, playedCardData);
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
        if (state != GameState.Pause && state != GameState.Win)
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
        
        StartGame();
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
        sfxGoal.Play();
        // Debug.Log($"{sideScored} scored ({newScore})!");

        // Detect that a player won.
        if (newScore >= winningScore)
        {
            var loserScore = scores[side];
            Debug.Log($"Winner winner, chicken dinner! {sideScored} wins ({newScore} to {loserScore})!");
            uiWin.SetWinningSide(sideScored);
            SetState(GameState.Win);
            sfxWin.Play();
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
        
        Ball newBall = Instantiate(prefabDefaultBall, ballsParent);
        ballSpawned?.Invoke(newBall);
        return newBall;
    }

    public void RemoveBall(Ball ball)
    {
        balls.Remove(ball);
        Destroy(ball.gameObject);

        // If this is the last important ball on the field, create a new one.
        if (!IsGameEnded() && !balls.Any(ball => ball.isImportant))
        {
            coAddBall = CoAddBall(CreateBall(), ballAddDelaySeconds);
            StartCoroutine(coAddBall);
        }
    }

    private IEnumerator CoAddBall(Ball ball, float delay)
    {
        if (ball == null)
        {
            coAddBall = null;
            yield break;
        }
        
        AddBall(ball);
        ball.PrepareToGo(delay);
        
        yield return new WaitForSeconds(delay);
        
        if (ball == null)
        {
            coAddBall = null;
            yield break;
        }
        
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
        sfxBallSpawn.Play();
        coAddBall = null;
    }

    private IEnumerator CoRemoveBall(Ball ball, float delay)
    {
        if (ball == null)
        {
            yield break;
        }
        
        ball.PrepareToDie(delay);
        
        yield return new WaitForSeconds(delay);
        
        if (ball == null)
        {
            yield break;
        }
        
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
