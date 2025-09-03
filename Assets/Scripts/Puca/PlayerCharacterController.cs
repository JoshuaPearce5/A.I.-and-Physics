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

    // Player Energy component
    public Energy energy;

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

        // Assign Energy to reference
        energy = GetComponent<Energy>();
    }

    private void Update()
    {
        if (data.controlsDisabled == false)
        {
            currentState.UpdateState();
        }

        HandleKnockbackLock();

        GroundCheck();
        WallCheck();
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

    private void GroundCheck()
    {
        // Check for ground layer
        Collider2D hit = Physics2D.OverlapCircle(data.groundCheck.position, 0.1f, data.groundLayer);
        data.isGrounded = hit != null && hit.CompareTag("Ground");
        data.isAirborne = !data.isGrounded;

        if (data.isGrounded && !Input.GetButton("Jump"))
        {
            data.isJumping = false;
            data.isDoubleJumping = false;
        }
    }

    private void WallCheck()
    {
        Collider2D hit = Physics2D.OverlapCircle(data.wallCheck.position, 0.1f, data.groundLayer);
        data.isTouchingWall = hit != null && hit.CompareTag("Wall");

        // Only apply wall cling logic if airborne AND jump has expired
        if (!data.isGrounded && data.rb.velocity.y <= 0f && data.isTouchingWall)
        {
            data.rb.velocity = new Vector2(0f, 0f);
            data.isJumping = false;
            data.isDoubleJumping = false;
            data.moveDirection = 0f;
            data.controlsDisabled = true;
        }
        else
        {
            data.controlsDisabled = false;
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