using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] public float chaseSpeed = 2f;
    [SerializeField] public float chargeSpeed = 5f;
    [SerializeField] public float detectionRadius = 5f;
    [SerializeField] public float rayCastBuffer = 1f;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public GameObject enemyOrigin;
    [SerializeField] public float hoverFrequency = 2f;
    [SerializeField] public float hoverAmplitude = 1f;

    public float hoverTimer = 0f;

    [Header("Enemy Booleans")]
    public bool isTouchingGround;
    public bool playerDetected;

}
