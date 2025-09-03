using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private GameObject checkpoint;
    [SerializeField] private GameObject player;

    public void respawn()
    {
        player.transform.position = checkpoint.transform.position;
    }

    public void setCheckpoint(GameObject passedCheckpoint)
    {
        checkpoint = passedCheckpoint;
    }
}
