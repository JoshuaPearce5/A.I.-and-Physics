using UnityEngine;

public class FormUnlocker : MonoBehaviour
{
    [SerializeField] private string formToUnlock;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterStateController controller = other.GetComponentInParent<CharacterStateController>();
            if (controller != null)
            {
                controller.UnlockForm(formToUnlock);
                Destroy(gameObject);
            }
        }
    }
}

