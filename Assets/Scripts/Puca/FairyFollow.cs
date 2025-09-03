using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FairyFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 offset = new Vector3(-3, 5, 0); // Offset from player
    [SerializeField] private new Light2D light; // Point light attached to follower
    [SerializeField] private float smoothingTime = 0.5f; // How long to ideally catch up to target position 
    [SerializeField] private float bobbingAmplitude = 0.2f; // How much the fairy will bob
    [SerializeField] private float bobbingFrequency = 2f; // How often the fairy will bob
    [SerializeField] private float bobbingDistanceThreshold = 0.5f; // How far the fairy will stray

    private float bobbingTimer = 0f;

    private Vector3 velocity = Vector3.zero; // Updated in the SmoothDamp function

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player = player.transform.parent.gameObject;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Flip offset based on player direction
        if (player.transform.localScale.x > 0)
        {
            offset = new Vector3(-3, 5, 0);
        }
        else
        {
            offset = new Vector3(1, 5, 0);
        }

        Vector3 targetPosition = player.transform.position + offset;

        // Smoothly follow the player
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothingTime);

        // Bobbing logic when close enough
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < bobbingDistanceThreshold)
        {
            bobbingTimer += Time.deltaTime * bobbingFrequency;

            // Circular bobbing: sine and cosine for soft X and Y offsets
            float bobX = Mathf.Sin(bobbingTimer) * bobbingAmplitude;
            float bobY = Mathf.Cos(bobbingTimer) * bobbingAmplitude;

            transform.position += new Vector3(bobX, bobY, 0) * Time.deltaTime;
        }
    }
}
