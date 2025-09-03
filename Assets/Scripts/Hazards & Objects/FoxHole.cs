using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxHole : MonoBehaviour
{
    [SerializeField] private GameObject linkedHole;
    [SerializeField] private GameObject player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.transform.position = linkedHole.transform.position;
        }
    }
}
