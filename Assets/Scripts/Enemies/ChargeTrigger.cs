using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeTrigger : MonoBehaviour
{
    [SerializeField] private ChargeEnemy parentEnemy;
    [SerializeField] private PlayerCharacterController player;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private string triggerType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerType == "Detect" && other.CompareTag("Player"))
        {
                parentEnemy.PlayerDetected(player.transform.position - transform.position);
        }

        if (other.CompareTag("Player") && triggerType != "Detect")
        {

            // Declare Rigidbodies
            Rigidbody2D playerRb = other.GetComponentInParent<Rigidbody2D>();
            Rigidbody2D enemyRb = GetComponentInParent<Rigidbody2D>();

            // Knock back the player
            ApplyKnockback(playerRb, parentEnemy.transform.position, knockbackForce);
            player.Knockback();

            // Knock back the enemy
            ApplyKnockback(enemyRb, playerRb.transform.position, knockbackForce, true);
            parentEnemy.Knockback();
        }
        // Send HitPlayer to parent enemy
        if (triggerType == "Edge" && other.CompareTag("Player"))
        {
            parentEnemy.HitPlayer();
        }
    }

    private void ApplyKnockback(Rigidbody2D rb, Vector3 sourcePosition, float force, bool reverse = false)
    {
        if (rb == null) return;

        Vector2 direction = (rb.transform.position - sourcePosition).normalized;

        if (reverse)
            direction *= -1;

        direction.y = Mathf.Abs(direction.y);
        Vector2 knockbackDirection = (new Vector2(direction.x, direction.y + 0.5f)).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * force, ForceMode2D.Impulse);

        Debug.DrawLine(sourcePosition, rb.transform.position, Color.red, 1f);
        Debug.Log($"{rb.name} Knockback dir: {knockbackDirection}");
    }

}
