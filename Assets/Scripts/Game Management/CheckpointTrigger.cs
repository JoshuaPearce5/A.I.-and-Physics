using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] CheckpointManager checkpointManager;
    private GameObject checkpointPassed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        checkpointManager.setCheckpoint(gameObject);
    }
}
