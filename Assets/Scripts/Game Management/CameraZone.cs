using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public Vector3 cameraOffset = new Vector3(0f, 0f, -15f);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Camera.main.GetComponent<SmoothCameraFollow>().SetOffset(cameraOffset);
        }
    }
}
