using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCharacterController : MonoBehaviour
{
    // State Machine references
    public PlayerData data;
    public PlayerState currentState;
    public DefaultState defaultState;
    public EagleState eagleState;
    public FoxState foxState;
    public BoarState boarState;

    // Player Animator component
    public Animator animator;

    public Health health;

    // Player Energy component
    public Energy energy;

    public CapsuleCollider2D capsuleCollider;


    private void Awake()
    {
        // Assign Player Data component
        data = GetComponent<PlayerData>();

        // Instantiate all player States
        defaultState = new DefaultState(this);
        eagleState = new EagleState(this);
        foxState = new FoxState(this);
        boarState = new BoarState(this);

        // Sets the current State to default and enters that State
        currentState = defaultState;
        currentState.EnterState();

        // Assign Rigidbody2D to reference
        data.rb = GetComponent<Rigidbody2D>();

        // Assign Animator to reference
        animator = GetComponent<Animator>();

        health = GetComponent<Health>();

        // Assign Energy to reference
        energy = GetComponent<Energy>();

        capsuleCollider = GetComponent<CapsuleCollider2D>();

    }

    private void Update()
    {
        if (data.controlsDisabled == false)
        {
            currentState.UpdateState();
        }

        HandleKnockbackLock();

    }

    public void SwitchState(PlayerState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    private void FixedUpdate()
    {
        if (data.controlsDisabled == false)
        {
            currentState.FixedUpdateState();
        }

        UpdateEnvironmentSensors();
        UpdateEnemySensors();
        UpdateHazardSensors();

    }

    public void Knockback()
    {
        // Handles initiating the Knockback effect
        data.knockbackLockCounter = data.knockbackTime;
        data.isJumping = true;
        data.isDoubleJumping = false;
    }

    private void HandleKnockbackLock()
    {
        // Countdown for lockup of controls after knockback
        if (data.knockbackLockCounter > 0)
        {
            data.knockbackLockCounter -= Time.deltaTime;
            data.controlsDisabled = true;
        }
        else
        {
            data.controlsDisabled = false;
        }
    }

    public void UpdateEnvironmentSensors()
    {
        Bounds bounds = capsuleCollider.bounds;
        Vector2 origin = bounds.center;
        float buffer = data.rayCastBuffer;
        Vector2 size = new Vector2(bounds.size.x / 2, buffer * 2);

        // Ground Check (BoxCast down)
        RaycastHit2D groundHit = Physics2D.BoxCast(data.groundCheck.position, size, 0f, Vector2.down, buffer, data.groundLayer);

        // Ceiling Check (BoxCast up)
        RaycastHit2D ceilingHit = Physics2D.BoxCast(data.ceilingCheck.position, size, 0f, Vector2.up, buffer, data.groundLayer);

        // Wall Check (OverlapCircle right)
        Collider2D wallHit = Physics2D.OverlapCircle(data.wallCheck.position, 0.2f, data.groundLayer);

        // Back Wall Check (OverlapCircle left)
        Collider2D backWallHit = Physics2D.OverlapCircle(data.backWallCheck.position, 0.2f, data.groundLayer);

        // Update boolean flags in PlayerData
        data.isGrounded = groundHit.collider != null;
        data.isTouchingCeiling = ceilingHit.collider != null;
        data.isAirborne = !data.isGrounded;
        
        if (data.isGrounded && data.rb.velocity.y <= 0f)
        {
            data.isJumping = false;
            data.isDoubleJumping = false;
        }
        if (wallHit != null)
        {
            data.isTouchingWall = true;
            if (!data.isFlipped)
            {
                data.isFacingWall = true;
            }
            else
            {
                data.isFacingWall = false;
            }
        }
        else if (backWallHit != null)
        {
            data.isTouchingWall = true;
            if (data.isFlipped)
            {
                data.isFacingWall = true;
            }
            else
            {
                data.isFacingWall = false;
            }
        }

        else
        {
            data.isTouchingWall = false;
            data.isFacingWall = false;
        }

    }

    public void UpdateEnemySensors()
    {
        // Enemy Top hit check
        Collider2D enemyTopHit = Physics2D.OverlapCircle(data.groundCheck.position, data.enemyCheckDistance, data.enemyLayer);

        if (enemyTopHit != null)
        {
            // Handle enemy hit logic here
            Debug.Log("Enemy hit from above: " + enemyTopHit.name);

            Vector2 velocity = data.currentVelocity;

            // Knockback in the opposite direction of the enemy hit
            Vector2 knockbackDir = Vector2.up.normalized;

            // Apply knockback force
            float knockbackStrength = 10f;
            velocity = knockbackDir * knockbackStrength;

            data.currentVelocity = velocity;

        }

        // Enemy Right Side hit check
        Collider2D enemyRightSideHit = Physics2D.OverlapCircle(data.wallCheck.position, data.enemyCheckDistance, data.enemyLayer);

        if (enemyRightSideHit != null)
        {
            Debug.Log("Enemy hit from the right side: " + enemyRightSideHit.name);

            Vector2 velocity = data.currentVelocity;

            // Knockback in the opposite direction of the enemy hit
            Vector2 knockbackDir = new Vector2(-3f, 3f);

            // Apply knockback force
            float knockbackStrength = 3f;
            velocity = knockbackDir * knockbackStrength;

            data.currentVelocity = velocity;

            health.TakeDamage(30);

        }

        // Enemy Left Side hit check
        Collider2D enemyLeftSideHit = Physics2D.OverlapCircle(data.backWallCheck.position, data.enemyCheckDistance, data.enemyLayer);

        if (enemyLeftSideHit != null)
        {
            Debug.Log("Enemy hit from the left side: " + enemyLeftSideHit.name);

            Vector2 velocity = data.currentVelocity;

            // Knockback in the opposite direction of the enemy hit
            Vector2 knockbackDir = new Vector2(3f, 3f);

            // Apply knockback force
            float knockbackStrength = 3f;
            velocity = knockbackDir * knockbackStrength;

            data.currentVelocity = velocity;

            health.TakeDamage(30);

        }

    }

    public void UpdateHazardSensors()
    {
        Bounds bounds = capsuleCollider.bounds;
        Vector2 origin = bounds.center;
        Vector2 size = bounds.size;
        float buffer = data.rayCastBuffer;

        Vector2[] directions = { Vector2.down, Vector2.up, Vector2.right, Vector2.left };

        foreach (Vector2 dir in directions)
        {
            RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, dir, buffer, data.hazardLayer);

            if (hit.collider != null)
            {
                Debug.Log("Hit Hazard from:" + dir);
                Vector2 velocity = data.currentVelocity;

                // Knockback in the opposite direction of the hazard hit
                Vector2 knockbackDir = -dir.normalized;

                // Apply knockback force
                float knockbackStrength = 10f;
                velocity = knockbackDir * knockbackStrength;

                data.currentVelocity = velocity;

                health.TakeDamage(20);

                break;
            }
        }
    }

    public void UnlockForm(string formName)
    {
        if (formName == "Eagle")
        {
            data.eagleFormUnlocked = true;
            Debug.Log("Eagle form unlocked!");
        }
        else if (formName == "Fox") data.foxFormUnlocked = true;
        else if (formName == "Boar") data.boarFormUnlocked = true;
    }

}