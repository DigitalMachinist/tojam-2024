using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IceCard : MonoBehaviour
{
    private PongGame pongGame;

    public float iceNormalEffectSeconds = 10f;
    public int numberOfIceballs = 3;
    public float minSpeedFactorIceball = 0.6f;
    public float maxSpeedFactorIceball = 1f;
    public float coneDegreesIceballs = 60f;
    public Snowball prefabIceball;

    void Awake()
    {
        pongGame = FindObjectOfType<PongGame>();
        pongGame.cardPlayed += OnCardPlayed;
    }

    public void OnCardPlayed(PlayerSide playerSide, CardData card)
    {
        if (card.type != CardType.Ice)
        {
            return;
        }

        if (card.orientation == CardOrientation.Normal)
        {
            pongGame.GetPlayer(playerSide.Opposite()).Paddle.EnableIce(iceNormalEffectSeconds);
        }
        else if(card.orientation == CardOrientation.Inverted)
        {
            Ball ball = pongGame.Balls.FirstOrDefault();
            if (ball == null)
            {
                return;
            }

            Vector3 position = ball.transform.position;
            Vector3 velocity = ball.Rigidbody.velocity;
            float directionAdjustment = -coneDegreesIceballs / 2;
            for (var i = 0; i < numberOfIceballs; i++)
            {
                Snowball snowball = Instantiate<Snowball>(prefabIceball, position, Quaternion.identity, pongGame.ballsParent);
                Vector3 snowballVelocity = velocity * Random.Range(minSpeedFactorIceball, maxSpeedFactorIceball);
                snowballVelocity = Quaternion.Euler(0f, 0f, directionAdjustment) * snowballVelocity;
                snowball.Rigidbody.velocity = snowballVelocity;
                snowball.gameObject.layer = ball.gameObject.layer;
                directionAdjustment += coneDegreesIceballs / numberOfIceballs;
            }
            pongGame.RemoveBall(ball);
        }
    }

}
