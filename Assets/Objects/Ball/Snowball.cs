using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : Ball
{
    protected override void OnCollisionEnter2D(Collision2D other)
    {
        pongGame.sfxIceBallBreak.Play();
        Destroy(gameObject, 0.1f);
    }
}
