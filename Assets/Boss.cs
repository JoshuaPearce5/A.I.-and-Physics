using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private float fireWaitTime =23.0f;
    private float fireTimer;
    private LineRenderer lr;
    private GameObject player;
    private Vector3 beamDirection = Vector2.up;
    private bool beamStarted = false;
    private float beamTimer = 0.0f;

    private GameObject holdingPlayer; //used to get the proper player tag holding the character controller, need to fix this later.

    void Start()
    {
        holdingPlayer = GameObject.FindGameObjectWithTag("Player");
        //Debug.Log(holdingPlayer);
        if (holdingPlayer.transform.parent != null)
        {
            holdingPlayer = holdingPlayer.transform.parent.gameObject;
        }

        fireTimer = fireWaitTime;

        player = holdingPlayer;
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position);
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0.0f)
        {
            if (!beamStarted)
            {
                beamStarted = !beamStarted;
                beamTimer = 0.0f;
            }
            //beamDirection = Vector3.Slerp(beamDirection - transform.position, Vector3.down - transform.position, 0.001f) + transform.position;
            beamTimer += Time.deltaTime;

            float currentAngle = ((beamTimer / 5.0f) * 180.0f);
            beamDirection = Quaternion.Euler(0, 0, currentAngle) * transform.up;

            FireBeam((Vector2)beamDirection);
            
            if(fireTimer <= -5.0f)
            {
                beamStarted = !beamStarted;
                lr.enabled = false;
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, transform.position);
                fireTimer = fireWaitTime;
                beamDirection = Vector2.up;
            }
        }
    }

    void FireBeam(Vector2 direction)
    {
        //Debug.Log(direction);

        lr.enabled = true;
        Ray2D beam = new Ray2D(transform.position, direction);
        RaycastHit2D hit;
        lr.SetPosition(0, beam.origin);


        hit = Physics2D.Raycast(beam.origin, direction);

        if (hit.collider)
        {
            if(hit.transform.gameObject.tag == "Player")
            {
                PlayerHit();
            }
            //Debug.Log(hit.collider);
            lr.SetPosition(1, hit.point);
        }
    }

    public void PlayerHit()
    {
        player.SendMessage("TakeDamage", (50));
        Debug.Log("Player has been hit");
    }
}
