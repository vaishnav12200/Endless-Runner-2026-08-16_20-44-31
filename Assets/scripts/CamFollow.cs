using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 followOffset = new Vector3(0f, 2f, -3f);
    [SerializeField, Min(0f)] float lateralFollowSpeed = 10f;

    void Awake()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        float followAmount = 1f - Mathf.Exp(-lateralFollowSpeed * Time.deltaTime);
        float targetX = target.position.x + followOffset.x;
        float cameraX = Mathf.Lerp(transform.position.x, targetX, followAmount);

        transform.position = new Vector3(
            cameraX,
            target.position.y + followOffset.y,
            target.position.z + followOffset.z);
    }
}
