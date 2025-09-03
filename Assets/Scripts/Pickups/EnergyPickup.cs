using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class EnergyPickup : MonoBehaviour
{
    [SerializeField] private GameObject player;

    public GameObject pickup;

    bool pickupSpawned = true;

    //[Header("Audio")]
    //[SerializeField] private AudioClip energySoundClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //SoundFXManager.instance.PlayRandomPitchSoundFXClip(energySoundClip, transform, 1f);

            Energy playerEnergy = other.GetComponentInParent<Energy>();

            playerEnergy.SendMessage("PickupEnergy", (90));

            pickupSpawned = false;
        }
    }

    private void Update()
    {
        if (pickupSpawned == false)
        {
            pickup.SetActive(false);
            CoroutineHandler.Instance.StartCoroutine(PickupCoroutine());
        }
    }

    IEnumerator PickupCoroutine()
    {
            Debug.Log("Started Coroutine at timestamp : " + Time.time);

            yield return new WaitForSeconds(3);

            Debug.Log("Finished Coroutine at timestamp : " + Time.time);

            pickupSpawned = true;

            pickup.SetActive(true);
    }

}
