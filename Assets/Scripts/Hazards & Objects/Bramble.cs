using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bramble: MonoBehaviour
{
    [SerializeField] private GameObject player;

    [Header("Audio")]
    [SerializeField] private AudioClip impactSoundClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoundFXManager.instance.PlayRandomPitchSoundFXClip(impactSoundClip, transform, 1f);

            // Declare player components
            Health playerHealth = other.GetComponentInParent<Health>();
            playerHealth.SendMessage("TakeDamage", (20));
        }
        else if (other.CompareTag("Enemy"))
        {
            other.gameObject.SendMessage("Stomped");
        }
    }
}