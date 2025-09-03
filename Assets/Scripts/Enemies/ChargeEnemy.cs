using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D enemyRB;
    [SerializeField] private GameObject detectionCollider;
    [SerializeField] private GameObject player;

    [Header("Movement")]
    //[SerializeField] private float movementSpeed = 5f;
    private Vector2 moveDirection;
    private bool movementDisabled = false;
    private bool playerDetected = false;

    // Update is called once per frame
    void Update()
    {
        Charge();
    }
    private void Charge()
    {
        if (enemyRB == null || movementDisabled || !playerDetected)
        {
            return;
        }

        enemyRB.velocity = moveDirection;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy"))
        {
            if (playerDetected)
            {
                //collision.gameObject.SendMessage("WallBreak");
                Destroy(gameObject);
                Debug.Log("Enemy crashed");
            }
        }
    }

    public void HitPlayer()
    {
        player.SendMessage("TakeDamage", (50));
        Destroy(gameObject);
        Debug.Log("Player has been hit");
        Debug.Log("Enemy crashed");
    }

    public void PlayerDetected(Vector2 playerDirection)
    {
        Debug.Log("Player Detected");
        moveDirection = playerDirection;
        playerDetected = true;
        Destroy(detectionCollider);
        return;
    }

    public IEnumerator Knockback()
    {
        movementDisabled = true;

        yield return new WaitForSeconds(1f);

        movementDisabled = false;
    }
}