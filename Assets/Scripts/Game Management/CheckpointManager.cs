using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private GameObject checkpoint;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemySpawn1;
    [SerializeField] private GameObject enemySpawn2;
    [SerializeField] private GameObject enemyPrefab;
    private GameObject[] enemyArray;

    public void respawn()
    {
        player.transform.position = checkpoint.transform.position;

        enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject i in enemyArray)
        {
            Destroy(i);
        }

        Instantiate(enemyPrefab, enemySpawn1.transform);
        Instantiate(enemyPrefab, enemySpawn2.transform);
    }

    public void setCheckpoint(GameObject passedCheckpoint)
    {
        checkpoint = passedCheckpoint;
    }
}
