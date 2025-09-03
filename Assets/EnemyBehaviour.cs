using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] Enemy enemy;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private CapsuleCollider2D col;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();

        sr.sprite = enemy.enemySprite;
        rb.gravityScale = enemy.gravityScale;
        col.size = new Vector2(enemy.colliderSizeX, enemy.colliderSizeY);
    }

    void Update()
    {
        
    }
}
