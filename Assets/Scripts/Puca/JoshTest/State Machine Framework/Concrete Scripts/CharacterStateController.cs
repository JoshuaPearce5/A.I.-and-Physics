using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CharacterStateController : MonoBehaviour
{
    public StateLayer movementLayer = new StateLayer();
    public StateLayer formLayer = new StateLayer();
    public StateLayer statusLayer = new StateLayer();

    public PlayerData data;

    public Animator animator;

    public Health health;

    public Energy energy;

    public CapsuleCollider2D capsuleCollider;

    // Form States
    public FormDefaultState formDefaultState;
    public FormEagleState formEagleState;
    public FormFoxState formFoxState;
    public FormBoarState formBoarState;

    // Movement States
    public MovementIdleState movementIdleState;
    public MovementRunState movementRunState;
    public MovementJumpState movementJumpState;
    public MovementAirborneState movementAirborneState;
    public MovementWallHangState movementWallHangState;
    public MovementWallJumpState movementWallJumpState;

    // Status States
    public StatusDefaultState statusDefaultState;
    public StatusKnockbackState statusKnockbackState;
    public StatusStunnedState statusStunnedState;
    public StatusSlowedState statusSlowedState;
    public StatusKilledState statusKilledState;

    public void Awake()
    {
        data = GetComponent<PlayerData>();

        animator = GetComponent<Animator>();

        health = GetComponent<Health>();

        energy = GetComponent<Energy>();

        capsuleCollider = GetComponent<CapsuleCollider2D>();

        // Initialize Form States
        formDefaultState = new FormDefaultState(this);
        formEagleState = new FormEagleState(this);
        formFoxState = new FormFoxState(this);
        formBoarState = new FormBoarState(this);

        // Initialize Movement States
        movementIdleState = new MovementIdleState(this);
        movementRunState = new MovementRunState(this);
        movementJumpState = new MovementJumpState(this);
        movementAirborneState = new MovementAirborneState(this);
        movementWallHangState = new MovementWallHangState(this);
        movementWallJumpState = new MovementWallJumpState(this);

        // Initialize Status States
        statusDefaultState = new StatusDefaultState(this);
        statusKnockbackState = new StatusKnockbackState(this);
        statusStunnedState = new StatusStunnedState(this);
        statusSlowedState = new StatusSlowedState(this);
        statusKilledState = new StatusKilledState(this);

        // Initialize State Layers
        movementLayer.SetState(movementIdleState);
        formLayer.SetState(formDefaultState);
        statusLayer.SetState(statusDefaultState);

        Debug.Log(movementLayer.CurrentState.GetType().Name + " initialized.");
        Debug.Log(formLayer.CurrentState.GetType().Name + " initialized.");
        Debug.Log(statusLayer.CurrentState.GetType().Name + " initialized.");


    }

    public void Update()
    {

        if (!data.isKnockedBack && !data.isStunned)
        {
            HandleFacingDirection();
            HandleMovementInput();
            HandleJumpInput();
        }

        movementLayer.Update();
        formLayer.Update();
        statusLayer.Update();

    }
    public void FixedUpdate()
    {
        if (!data.isStunned)
        {
            movementLayer.FixedUpdate();
            statusLayer.FixedUpdate();

            UpdateEnvironmentSensors();
            UpdateEnemySensors();
            UpdateHazardSensors();
            RegenerateEnergy();

            MovePlayer();
        }
    }
    public void UpdateEnvironmentSensors()
    {
        Bounds bounds = capsuleCollider.bounds;
        Vector2 origin = bounds.center;
        float buffer = data.rayCastBuffer;
        Vector2 size = new Vector2 (bounds.size.x/2, buffer*2);

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

        if (wallHit != null)
        {
            data.isTouchingWall = true;
            if(!data.isFlipped)
            {
                data.isFacingWall = true;
            }
            else
            {
                data.isFacingWall = false;
            }
        }
        else if(backWallHit != null)
        {
            data.isTouchingWall = true;
            if(data.isFlipped)
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


            enemyTopHit.gameObject.SendMessage("Stomped");

            movementLayer.SetState(new MovementAirborneState(this));
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

            enemyRightSideHit.SendMessage("HitPlayer");
            enemyRightSideHit.SendMessage("Knockback");

            statusLayer.SetState(new StatusKnockbackState(this));
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

            enemyLeftSideHit.SendMessage("HitPlayer");
            enemyLeftSideHit.SendMessage("Knockback");

            statusLayer.SetState(new StatusKnockbackState(this));
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

    private void OnDrawGizmos()
    {
        if (capsuleCollider == null || data == null)
            return;

        Bounds bounds = capsuleCollider.bounds;
        Vector2 origin = bounds.center;
        float buffer = data.rayCastBuffer;
        Vector2 size = new Vector2(bounds.size.x / 2, buffer * 2);
        Vector2 hazardSize = bounds.size;

        // Set colors for each BoxCast visualization
        Color groundColor = Color.green;
        Color ceilingColor = Color.red;
        Color wallColor = Color.blue;
        Color backWallColor = Color.yellow;

        // Draw Ground BoxCast
        DrawBoxCast(data.groundCheck.position, size, Vector2.down, buffer, groundColor);

        // Draw Ceiling BoxCast
        //DrawBoxCast(data.ceilingCheck.position, size, Vector2.up, buffer, ceilingColor);

        DrawBoxCast(origin, hazardSize, Vector2.zero, 0f, ceilingColor);

        Gizmos.color = wallColor;
        Gizmos.DrawWireSphere(data.wallCheck.position, 0.2f);

        Gizmos.color = backWallColor;
        Gizmos.DrawWireSphere(data.backWallCheck.position, 0.2f);
    }

    private void DrawBoxCast(Vector2 origin, Vector2 size, Vector2 direction, float distance, Color color)
    {
        Gizmos.color = color;

        Vector2 castCenter = origin + direction * distance;

        Vector2 topLeft = new Vector2(castCenter.x - size.x / 2, castCenter.y + size.y / 2);
        Vector2 topRight = new Vector2(castCenter.x + size.x / 2, castCenter.y + size.y / 2);
        Vector2 bottomLeft = new Vector2(castCenter.x - size.x / 2, castCenter.y - size.y / 2);
        Vector2 bottomRight = new Vector2(castCenter.x + size.x / 2, castCenter.y - size.y / 2);

        // Draw rectangle edges
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
    public void HandleFacingDirection()
    {
        if (!data.controlsDisabled && data.wallJumpLockCounter < 0.2f)
        {
            // Change direction player is facing
            if (data.moveDirection > 0f)
            {
                data.defaultRig.GetComponent<SpriteRenderer>().flipX = false;
                data.eagleRig.GetComponent<SpriteRenderer>().flipX = false;
                data.foxRig.GetComponent<SpriteRenderer>().flipX = false;
                data.boarRig.GetComponent<SpriteRenderer>().flipX = false;

                data.isFlipped = false;
            }
            else if (data.moveDirection < 0f)
            {
                data.defaultRig.GetComponent<SpriteRenderer>().flipX = true;
                data.eagleRig.GetComponent<SpriteRenderer>().flipX = true;
                data.foxRig.GetComponent<SpriteRenderer>().flipX = true;
                data.boarRig.GetComponent<SpriteRenderer>().flipX = true;

                data.isFlipped = true;
            }
        }
    }
    public void RegenerateEnergy()
    {
        if (!Input.GetButton("Fly") && !Input.GetButton("Sneak") && !Input.GetButton("Charge"))
        {
            energy.RegainEnergy(5f);
        }
    }
    public void HandleMovementInput()
    {
        // Controls player movement input
        data.moveDirection = Input.GetAxisRaw("Horizontal");
        data.verticalMoveDirection = Input.GetAxisRaw("Vertical");
        data.jumpHeld = Input.GetButton("Jump");
    }
    public void HandleJumpInput()
    {
        if ((data.isJumping || data.isDoubleJumping) && data.jumpHeld && data.jumpHeldCounter > 0f)
        {
            data.jumpHeldCounter -= Time.deltaTime;

            if (data.doubleJumpControlCounter > 0f)
            {
                data.doubleJumpControlCounter -= Time.fixedDeltaTime;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            data.jumpPressed = true;
            data.jumpBufferCounter = data.jumpBufferTime;
        }
        else
        {
            data.jumpBufferCounter -= Time.deltaTime;
        }
    }
    public void MovePlayer()
    {
        Vector2 moveAmount = data.currentVelocity * Time.fixedDeltaTime;
        Vector2 moveDir = moveAmount.normalized;
        float moveDistance = moveAmount.magnitude;

        RaycastHit2D hit = Physics2D.BoxCast(
            capsuleCollider.bounds.center,
            capsuleCollider.bounds.size,
            0f,
            moveDir,
            moveDistance + data.rayCastBuffer,
            data.groundLayer);

        if (data.controlsDisabled)
        {
            Vector2 velocity = data.currentVelocity;

            velocity.x = 0f;

            data.currentVelocity = velocity;
        }   

        if (hit.collider != null)
        {
            // Move up to collision point minus buffer
            float adjustedDistance = hit.distance - data.rayCastBuffer;
            transform.Translate(moveDir * Mathf.Max(adjustedDistance, 0));

            // Nullify velocity component into wall/floor
            if (Mathf.Abs(moveDir.x) > 0.01f)
                data.currentVelocity.x = 0f;
            if (Mathf.Abs(moveDir.y) > 0.01f)
                data.currentVelocity.y = 0f;
        }
        else 
        {
            // No collision, move freely
            transform.Translate(moveAmount);
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
