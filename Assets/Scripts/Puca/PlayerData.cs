using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("Default Movement Settings")]
    [SerializeField] public float moveSpeed = 7f;
    [SerializeField] public float slowSpeed = 0.2f;
    [SerializeField] public float maxRunSpeed = 10f; // Force applied when moving
    [SerializeField] public float runDeceleration = 10f; // Deceleration when not moving
    [SerializeField] public float runAcceleration = 10f; // Acceleration when moving
    [SerializeField] public float wallRunJumpForce = 0.2f; // The force the player jumps from the ground when facing a wall
    [SerializeField] public float jumpForce = 10f; // Force applied when jumping
    [SerializeField] public float maxJumpTime = 0.2f; // Maximum time jump force is applied
    [SerializeField] public float wallJumpForceX = 7f; // Force applied on X axis when jumping off wall
    [SerializeField] public float wallJumpForceY = 7f; // Force applied on Y axis when jumping off wall
    [SerializeField] public float jumpGravityMult = 0.2f; // Multiplier for jump gravity
    [SerializeField] public float fallGravityMult = 4f; // Multiplier for fall gravity
    [SerializeField] public float maxFallSpeed = -10f; // Maximum fall speed clamp 
    [SerializeField] public float wallJumpTime = 0.3f; // How long movement input is ignored after wall jump
    [SerializeField] public float airControlMultiplier = 0.7f; // Multiplier for movement in air
    [SerializeField] public float doubleJumpControlMultiplier = 10f; // Multiplier for increased air control after double jump
    [SerializeField] public float doubleJumpControlDuration = 0.2f; // How long the player gains increased air control after double jump
    [SerializeField] public float knockbackTime = 0.05f; // Knockback timer for input lock
    [SerializeField] public float knockbackCounter = 0f;
    [SerializeField] public float jumpBufferTime = 0.5f; // Buffer for jumping after leaving platform
    [SerializeField] public float jumpBufferCounter = 0f; // Timer for jump buffer

    [SerializeField] public Vector2 currentVelocity; // Current velocity of the player
    [SerializeField] public float gravityScale = -2f; // Base gravity applied to the player

    [Header("Eagle Movement Settings")]
    [SerializeField] public float flySpeed = 7f; // Force applied when flying
    [SerializeField] public float maxFlySpeed = 7f; // Maximum flight speed
    [SerializeField] public float upwardFlySpeed = 7f; // Multiplier for decreased speed when flying upwards
    [SerializeField] public float downwardFlySpeed = -7f; // Multiplier for increased speed when flying downwards

    [Header("Environment Checks")]
    [SerializeField] public LayerMask groundLayer; // Layer mask for ground check
    [SerializeField] public Transform groundCheck; // Ground check child object at player's feet
    [SerializeField] public Transform wallCheck; // Wall check child object to detect wall in front of player
    [SerializeField] public Transform backWallCheck; // Wall check child object to detect wall behind player
    [SerializeField] public Transform ceilingCheck; // Ceiling check child object to detect ceiling above player
    [SerializeField] public float wallCheckDistance = 0.2f; // Distance for wall check RayCast
    [SerializeField] public float groundCheckDistance = 0.1f; // Distance for ground check RayCast
    [SerializeField] public float ceilingCheckDistance = 0.2f; // Distance for ceiling check RayCast
    [SerializeField] public float rayCastBuffer = 0.1f;
    [SerializeField] public LayerMask hazardLayer; // Layer mask for hazard check

    [Header("Enemy Checks")]
    [SerializeField] public LayerMask enemyLayer; // Layer mask for enemy check
    [SerializeField] public float enemyCheckDistance = 0.01f; // Distance for enemy check RayCast

    [Header("Animator Controllers")]
    [SerializeField] public RuntimeAnimatorController defaultAnimator; // Animator for Puca form
    [SerializeField] public RuntimeAnimatorController eagleAnimator; // Animator for Eagle form
    [SerializeField] public RuntimeAnimatorController foxAnimator; // Animator for Fox form
    [SerializeField] public RuntimeAnimatorController boarAnimator; // Animator for Boar form

    [Header("Transformation Rigs")]
    [SerializeField] public GameObject defaultRig; // Rigged Puca GameObject
    [SerializeField] public GameObject eagleRig; // Rigged Eagle GameObject
    [SerializeField] public GameObject foxRig; // Rigged Fox GameObject
    [SerializeField] public GameObject boarRig; // Rigged Boar GameObject

    [Header("ParticleEffect")]
    [SerializeField] public ParticleSystem smokeEffect;

    [Header("Audio")]
    [SerializeField] public AudioClip jumpSoundClip;
    [SerializeField] public AudioClip doubleJumpSoundClip;
    [SerializeField] public AudioClip transformSoundClip;
    [SerializeField] public AudioClip[] flightSoundClips;
    [SerializeField] public AudioClip flightSoundClip;

    public Rigidbody2D rb; // Reference to player RigidBody2D

    public float moveDirection = 0f; // Which direction player is moving horizontally
    public float verticalMoveDirection = 0f; // Which direction player is moving vertically

    public bool isGrounded; // If player is touching the ground
    public bool isFlipped = false; // If player is flipped horizontally
    public bool isTouchingWall; // If player is colliding with wall
    public bool isFacingWall; // If play is facing a wall
    public bool isTouchingCeiling; // If player is colliding with ceiling

    public bool isAirborne; // If player is in the air
    public bool isJumping; // If player is jumping
    public bool isDoubleJumping; // If player is double jumping
    public bool jumpPressed; // If jump input was pressed
    public bool jumpHeld; // If jump input is held down
    public bool hasReleasedJump; // If jump input was released
    public bool isKnockedBack; // If the player is knocked back
    public bool isStunned; // If the player is stunned

    public float jumpHeldCounter; // How long jump has been held down
    public float wallJumpLockCounter; // Timer for wall jump input lock
    public float knockbackLockCounter; // Timer for knockback input lock
    public float doubleJumpControlCounter; // Timer for air control after double jumping

    public bool controlsDisabled = false; // Disable controls for dialogue

    public bool eagleFormUnlocked = false;
    public bool foxFormUnlocked = false;
    public bool boarFormUnlocked = false;

    public bool eagleFormActive = false;
    public bool boarFormActive = false;
    public bool foxFormActive = false;
}
