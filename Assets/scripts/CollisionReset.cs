using UnityEngine;

public class CollisionReset : MonoBehaviour
{
    [SerializeField] AudioSource crashFX;

    bool hasTriggered;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
        {
            return;
        }

        PlayerMove playerMove = other.GetComponentInParent<PlayerMove>();
        if (playerMove == null)
        {
            return;
        }

        hasTriggered = true;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CamFollow cameraFollow = mainCamera.GetComponent<CamFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.enabled = false;
            }
        }

        playerMove.enabled = false;

        Animator animator = playerMove.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.Play("Falling Back Death", 0, 0f);
        }

        if (crashFX != null)
        {
            crashFX.Play();
        }
    }

    public void ResetForReuse()
    {
        hasTriggered = false;
    }
}
