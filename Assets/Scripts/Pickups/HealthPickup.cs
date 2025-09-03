using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private GameObject player;

    //[Header("Audio")]
    //[SerializeField] private AudioClip healSoundClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //SoundFXManager.instance.PlayRandomPitchSoundFXClip(healSoundClip, transform, 1f);

            Health playerHealth = other.GetComponentInParent<Health>();

            playerHealth.SendMessage("Heal", (25));

            Destroy(gameObject);
        }
    }
}
