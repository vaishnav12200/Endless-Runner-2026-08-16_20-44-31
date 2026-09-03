using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed =2;
    [SerializeField] float xPos;
    [SerializeField] float zPos;
    [SerializeField] int trackNumber=1;
    [SerializeField] int sideSpeed=9;
    [SerializeField] bool currentMove;
    [SerializeField] int moveDirection;
    [SerializeField] AudioSource whoosh; // 1=left ,2 = right
    [SerializeField] Vector3 laneCheckHalfExtents = new Vector3(0.48f, 0.48f, 0.48f);

    readonly Collider[] laneCheckResults = new Collider[16];
    int previousTrackNumber = 1;

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);

        xPos = transform.position.x;
        zPos = transform.position.z;

        if (!currentMove)
        {
            return;
        }

        if (IsLaneBlocked(trackNumber))
        {
            CancelLaneChange();
            return;
        }

        float nextX = Mathf.MoveTowards(transform.position.x, trackNumber, sideSpeed * Time.deltaTime);
        transform.position = new Vector3(nextX, transform.position.y, transform.position.z);

        if (Mathf.Approximately(nextX, trackNumber))
        {
            currentMove = false;
            moveDirection = 0;
        }
    }

    public void LeftMove()
    {
        if (currentMove)
        {
            return;
        }

        if (trackNumber == 1)
        {
            BeginLaneChange(0, 1);
        }

        if (trackNumber == 2)
        {
            BeginLaneChange(1, 1);
        }
    }

    public void RightMove()
    {
        if (currentMove)
        {
            return;
        }

        if (trackNumber == 1)
        {
            BeginLaneChange(2, 2);
        }

        if (trackNumber == 0)
        {
            BeginLaneChange(1, 2);
        }
    }

    void BeginLaneChange(int targetTrack, int direction)
    {
        if (IsLaneBlocked(targetTrack))
        {
            return;
        }

        previousTrackNumber = trackNumber;
        trackNumber = targetTrack;
        currentMove = true;
        moveDirection = direction;

        if (whoosh != null)
        {
            whoosh.Play();
        }
    }

    void CancelLaneChange()
    {
        currentMove = false;
        moveDirection = 0;
        trackNumber = previousTrackNumber;
        transform.position = new Vector3(previousTrackNumber, transform.position.y, transform.position.z);
        xPos = transform.position.x;
        zPos = transform.position.z;
    }

    bool IsLaneBlocked(int targetTrack)
    {
        Vector3 checkCenter = new Vector3(targetTrack, transform.position.y, transform.position.z);
        int hitCount = Physics.OverlapBoxNonAlloc(
            checkCenter,
            laneCheckHalfExtents,
            laneCheckResults,
            Quaternion.identity,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = laneCheckResults[i];
            laneCheckResults[i] = null;

            if (hit == null || hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                continue;
            }

            if (hit.GetComponentInParent<CollisionReset>() != null)
            {
                return true;
            }
        }

        return false;
    }
}
