using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float smoothingTime = 1f; // Controls how long the camera should take to catch up to player
    [SerializeField] public Vector3 offset = new Vector3(0f, 0f, -10f); // Offset camera from player position on Z axis

    private Vector3 velocity = Vector3.zero; // Updated in the SmoothDamp function


    public void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        // Tells the camera where to go
        Vector3 targetPosition = player.transform.position + offset;

        // Calculation for smoothing the camera movement to give rubber band effect
        // Change smoothingTime to adjust
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothingTime);
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}
