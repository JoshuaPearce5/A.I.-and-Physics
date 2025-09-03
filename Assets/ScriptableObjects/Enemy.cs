using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu]
public class Enemy : ScriptableObject
{
    public Sprite enemySprite;
    public float gravityScale;
    public float colliderSizeX;
    public float colliderSizeY;

    public float speed;
    public AudioClip attackSound;
    public AudioClip[] hurtSound;
}
