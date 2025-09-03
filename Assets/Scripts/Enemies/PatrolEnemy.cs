using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] CapsuleCollider2D capsuleCollider;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] bool movingRight;
    [SerializeField] bool movementDisabled = false;
    [SerializeField] float moveDirection;

    private bool isFacingWall;
    private bool isKnocked = false;


    [Header("Audio")]
    [SerializeField] private AudioClip attackSoundClip;
    [SerializeField] private AudioClip[] stompedSoundClips;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        groundCheck = transform.Find("GroundCheck");
        wallCheck = transform.Find("WallCheck");
    }

    void Update()
    {
        if (!movementDisabled)
        {
            HandleFacingDirection();
        }
    }

    void FixedUpdate()
    {
        UpdateEnvironmentSensors();
        Patrol();
    }

    private void Patrol()
    {
        if (rb == null || movementDisabled || isKnocked)
        {
            return;
        }
        else if (!isKnocked && !movementDisabled)
        {
            if (isFacingWall)
            {
                FlipDirection();
            }

            moveDirection = movingRight ? 1f : -1f;

            rb.velocity = new Vector2(movementSpeed * moveDirection, rb.velocity.y);

        }
    }

    private void FlipDirection()
    {
        movingRight = !movingRight;
    }

    public void HitPlayer()
    {
        SoundFXManager.instance.PlayRandomPitchSoundFXClip(attackSoundClip, transform, 0.25f);
    }

    public IEnumerator Knockback()
    {
        Debug.Log("Enemy Knocked Back");
        movementDisabled = true;
        float knockbackDir = movingRight ? -1f : 1f;

        rb.velocity = new Vector2(5f * knockbackDir, 5f);

        yield return new WaitForSeconds(2f);

        movementDisabled = false;

    }

    public void Stomped()
    {
        if (SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlayRandomSoundFXClip(stompedSoundClips, transform, 0.3f);
        }

        Destroy(gameObject);
    }

    public void UpdateEnvironmentSensors()
    {
        isFacingWall = false;

        Vector2 wallCheckDir = movingRight ? Vector2.right : Vector2.left;

        // Cast a ray in the moving direction
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, wallCheckDir, 0.5f, groundLayer);

        Debug.DrawRay(wallCheck.position, wallCheckDir * 0.5f, Color.red);


        if (wallHit.collider != null)
        {
            isFacingWall = true;
        }
    }

    public void HandleFacingDirection()
    {
        sprite.flipX = movingRight;
    }
}
