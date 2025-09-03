using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    // State Machine References
    public EnemyData data;
    public EnemyState currentState;
    public IdleState idleState;
    public PreChargeState preChargeState;
    public ChargingState chargingState;
    public StunnedState stunnedState;
    public ReturningState returningState;

    public CapsuleCollider2D capsuleCollider;

    public SpriteRenderer sprite;

    public Vector3 playerLocation;

    public Vector3 groundLocation;

    public Rigidbody2D rb;

    private void Awake()
    {
        data = GetComponent<EnemyData>();

        capsuleCollider = GetComponent<CapsuleCollider2D>();

        sprite = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();

        idleState = new IdleState(this);
        preChargeState = new PreChargeState(this);
        chargingState = new ChargingState(this);
        stunnedState = new StunnedState(this);
        returningState = new ReturningState(this);

        currentState = idleState;
        currentState.EnterState();
    }

    void Update()
    {
        currentState.UpdateState();

        //UpdateEnvironmentSensors();
        UpdatePlayerDetection();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void SwitchState(EnemyState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    //public void UpdateEnvironmentSensors()
    //{
    //    Bounds bounds = capsuleCollider.bounds;
    //    Vector2 origin = bounds.center;
    //    float buffer = data.rayCastBuffer;
    //    Vector2 size = new Vector2(bounds.size.x + buffer, bounds.size.y + buffer);

    //    // Ground Check
    //    Collider2D groundHit = Physics2D.OverlapCapsule(origin, size, CapsuleDirection2D.Vertical, 0f, data.groundLayer);

    //    if (groundHit != null)
    //    {
    //        data.isTouchingGround = true;

    //        groundLocation = groundHit.transform.position;
    //    }
    //    else
    //    {
    //        data.isTouchingGround = false;

    //    }
    //}

    public void UpdatePlayerDetection()
    {
        Bounds bounds = capsuleCollider.bounds;
        Vector2 origin = bounds.center;

        // Player detection check
        Collider2D playerDetected = Physics2D.OverlapCircle(origin, data.detectionRadius, data.playerLayer);

        if (playerDetected != null)
        {
            data.playerDetected = playerDetected;
            playerLocation = playerDetected.transform.position;
        }
        else
        {
            data.playerDetected = false; 
        }
    }

    private void OnDrawGizmos()
    {
        if (capsuleCollider == null || data == null)
            return;

        Bounds bounds = capsuleCollider.bounds;
        Vector2 origin = bounds.center;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, data.detectionRadius);
    }
}
