using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class Paddle : MonoBehaviour
{
    private IEnumerator coIce;
    private IEnumerator coShock;
    public float yMax = 4f;
    public float yMin = -2.1f;
    public float speed = 5.0f;
    public float regularSpeed;
    private Vector3 movement;
    private bool isInverting = false;
    private bool isIced = false;
    private float iceVelocity = 0f;
    private bool isFire = false;
    private bool isParalyze = false;
    private bool isShockAttract = false;

    public float iceAccelration = 1f;
    public float iceDeceleration = 0.5f;
    public float iceMaxSpeed = 4f;
    public float paralysisThreshold = 0.8f;
    public Sprite spriteNormal;
    public Sprite spriteIced;
    public Sprite spriteFire;
    public Sprite spriteShock;
    public Sprite spriteShockAttract;

    private PlayerInput playerInput;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private PongGame pongGame;

    public PlayerSide playerSide;

    public event Action<int, CardOrientation> cardButtonPressed;
    public event Action cardInvertPressed;
    public event Action cardInvertReleased;
    public event Action pausePressed;
    public event Action escapePressed;

    void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        pongGame = FindObjectOfType<PongGame>();
        regularSpeed = speed;
    }

    public void OnMovement(InputAction.CallbackContext callbackContext)
    {
        Vector2 baseMovement = callbackContext.ReadValue<Vector2>();
        if (baseMovement.y > 0f)
        {
            movement = Vector2.up;
        }
        else if (baseMovement.y < 0f)
        {
            movement = Vector2.down;
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    public void OnInvert(InputAction.CallbackContext callbackContext)
    {
        isInverting = callbackContext.action.IsPressed();
        if (isInverting)
        {
            cardInvertPressed?.Invoke();
        }
        else
        {
            cardInvertReleased?.Invoke();
        }
    }

    public void OnCard1(InputAction.CallbackContext callbackContext)
    {
        cardButtonPressed?.Invoke(0, isInverting ? CardOrientation.Inverted : CardOrientation.Normal);
    }

    public void OnCard2(InputAction.CallbackContext callbackContext)
    {
        cardButtonPressed?.Invoke(1, isInverting ? CardOrientation.Inverted : CardOrientation.Normal);
    }

    public void OnCard3(InputAction.CallbackContext callbackContext)
    {
        cardButtonPressed?.Invoke(2, isInverting ? CardOrientation.Inverted : CardOrientation.Normal);
    }

    public void OnCard4(InputAction.CallbackContext callbackContext)
    {
        cardButtonPressed?.Invoke(3, isInverting ? CardOrientation.Inverted : CardOrientation.Normal);
    }

    public void OnPause(InputAction.CallbackContext callbackContext)
    {
        pausePressed?.Invoke();
    }

    public void OnEscape(InputAction.CallbackContext callbackContext)
    {
        escapePressed?.Invoke();
    }

    void Update()
    {
        Vector3 pos = transform.position;
        float newPos = 0f;

        // Paralysis
        if (movement.y != 0 && isParalyze && Mathf.PerlinNoise1D(Time.time) > paralysisThreshold)
        {
            pongGame.sfxShockParalysis.Play();
            movement = Vector3.zero;
        }
        
        if (isIced)
        {
            if (movement == Vector3.zero)
            {
                float direction = iceVelocity > 0 ? -1f : 1f;
                iceVelocity = iceVelocity - direction * iceDeceleration * Time.deltaTime;
                if (direction > 1f && iceVelocity > 0f)
                {
                    iceVelocity = 0f;
                }
                else if (direction < 1f && iceVelocity < 0f)
                {
                    iceVelocity = 0f;
                }
                newPos = pos.y + iceVelocity * Time.deltaTime;
            }
            else
            {
                iceVelocity = Mathf.Clamp(iceVelocity + movement.y * iceAccelration * Time.deltaTime, -iceMaxSpeed, iceMaxSpeed);
                newPos = pos.y + iceVelocity * Time.deltaTime;
            }
        }
        else
        {
            float motion = movement.y * speed * Time.deltaTime;
            newPos = pos.y + motion;
        }
        
        float clamped = Mathf.Clamp(newPos, yMin, yMax);
        transform.position = new Vector3(pos.x, clamped, pos.z);
    }

    public void EnableParalysis(float duration)
    {
        if (coShock != null)
        {
            StopCoroutine(coShock);
            coShock = null;
        }
        
        coShock = CoParalysis(duration);
        StartCoroutine(coShock);
    }

    public IEnumerator CoParalysis(float duration)
    {
        isParalyze = true;
        SetSprite();
        yield return new WaitForSeconds(duration);
        isParalyze = false;
        SetSprite();

        coShock = null;
    }

    public void EnableIce(float duration)
    {
        if (coIce != null)
        {
            StopCoroutine(coIce);
            coIce = null;
        }
        
        coIce = CoIce(duration);
        StartCoroutine(coIce);
    }

    public IEnumerator CoIce(float duration)
    {
        isIced = true;
        SetSprite();
        pongGame.sfxIcePaddleStart.Play();
        yield return new WaitForSeconds(duration);
        isIced = false;
        SetSprite();
        pongGame.sfxIcePaddleEnd.Play();

        coIce = null;
    }

    public void EnableFire()
    {
        isFire = true;
        SetSprite();
    }

    public void DisableFire()
    {
        isFire = false;
        SetSprite();
    }

    public void EnableShockAttract()
    {
        isShockAttract = true;
        SetSprite();
    }

    public void DisableShockAttract()
    {
        isShockAttract = false;
        SetSprite();
    }

    private void SetSprite()
    {
        if (isIced)
        {
            spriteRenderer.sprite = spriteIced;
        }
        else if (isFire)
        {
            spriteRenderer.sprite = spriteFire;
        }
        else if (isParalyze)
        {
            spriteRenderer.sprite = spriteShock;
        }
        else if (isShockAttract)
        {
            spriteRenderer.sprite = spriteShockAttract;
        }
        else
        {
            spriteRenderer.sprite = spriteNormal;
        }
    }
}
