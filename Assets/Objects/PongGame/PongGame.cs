using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PongGame : MonoBehaviour
{
    private Player[] players;
    private List<Ball> balls;
    private Dictionary<PlayerSide, int> scores;
    
    public int winningScore = 10;
    public float ballAddDelaySeconds = 0.3f;
    public float ballRemoveDelaySeconds = 0.3f;
    public Ball prefabDefaultBall;

    // Args: Winner, winner score, loser score
    public event Action<PlayerSide, int, int> gameEnded;

    void Awake()
    {
        scores = new Dictionary<PlayerSide, int>()
        {
            { PlayerSide.Left, 0 },
            { PlayerSide.Right, 0 }
        };
    }
    
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
    }

    private void OnGoalScoredAgainst(Ball ball, PlayerSide side)
    {
        PlayerSide sideScored = side.Opposite();
        int newScore = scores[sideScored] + 1;
        scores[sideScored] = newScore;
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
        ball.Go();
    }

    public void RemoveBall(Ball ball)
    {
        balls.Remove(ball);
        Destroy(ball);

        if (!balls.Any())
        {
            StartCoroutine(CoAddBall(prefabDefaultBall, ballAddDelaySeconds));
        }
    }

    private IEnumerator CoAddBall(Ball ball, float delay)
    {
        AddBall(Instantiate(ball));
        ball.PrepareToGo(delay);
        yield return new WaitForSeconds(delay);
        ball.Go();
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
}
