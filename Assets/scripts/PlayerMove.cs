using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerMove : MonoBehaviour
{
    [Header("Forward Speed")]
    [SerializeField] float startingSpeed = 6f;
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float maximumSpeed = 14f;
    [SerializeField] float speedGainPer100Meters = 0.35f;

    [Header("Lane Movement")]
    [SerializeField] float xPos;
    [SerializeField] float zPos;
    [SerializeField] int trackNumber=1;
    [SerializeField] int sideSpeed=9;
    [SerializeField] bool currentMove;
    [SerializeField] int moveDirection;
    [SerializeField] AudioSource whoosh; // 1=left ,2 = right
    [SerializeField] Vector3 laneCheckHalfExtents = new Vector3(0.48f, 0.48f, 0.48f);

    [Header("Mobile Input")]
    [SerializeField] float minimumSwipeDistance = 60f;

    readonly Collider[] laneCheckResults = new Collider[16];
    int previousTrackNumber = 1;
    Vector2 mouseSwipeStart;
    bool mouseSwipeInProgress;
    float distanceTravelled;

    public float DistanceTravelled => distanceTravelled;
    public float CurrentSpeed => moveSpeed;

    void Awake()
    {
        moveSpeed = startingSpeed;
        previousTrackNumber = trackNumber;
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        if (EnhancedTouchSupport.enabled)
        {
            EnhancedTouchSupport.Disable();
        }
    }

    void Update()
    {
        if (!CanMove())
        {
            return;
        }

        HandleSwipeInput();
        HandleDesktopInput();

        float distanceBonus = (distanceTravelled / 100f) * speedGainPer100Meters;
        moveSpeed = Mathf.Min(maximumSpeed, startingSpeed + distanceBonus);

        float forwardStep = Time.deltaTime * moveSpeed;
        transform.Translate(Vector3.forward * forwardStep, Space.World);
        distanceTravelled += forwardStep;

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
        if (!CanMove() || currentMove)
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
        if (!CanMove() || currentMove)
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

    public void BeginRun()
    {
        distanceTravelled = 0f;
        moveSpeed = startingSpeed;
    }

    bool CanMove()
    {
        return GameFlowController.Instance == null || GameFlowController.Instance.IsPlaying;
    }

    void HandleSwipeInput()
    {
        if (Touch.activeTouches.Count == 0)
        {
            return;
        }

        Touch touch = Touch.activeTouches[0];
        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            TryMoveFromSwipe(touch.screenPosition - touch.startScreenPosition);
        }
    }

    void HandleDesktopInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame)
            {
                LeftMove();
            }
            else if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
            {
                RightMove();
            }
        }

        Mouse mouse = Mouse.current;
        if (mouse == null)
        {
            return;
        }

        if (mouse.leftButton.wasPressedThisFrame)
        {
            mouseSwipeStart = mouse.position.ReadValue();
            mouseSwipeInProgress = true;
        }
        else if (mouseSwipeInProgress && mouse.leftButton.wasReleasedThisFrame)
        {
            mouseSwipeInProgress = false;
            TryMoveFromSwipe(mouse.position.ReadValue() - mouseSwipeStart);
        }
    }

    void TryMoveFromSwipe(Vector2 swipeDelta)
    {
        if (Mathf.Abs(swipeDelta.x) < minimumSwipeDistance ||
            Mathf.Abs(swipeDelta.x) < Mathf.Abs(swipeDelta.y))
        {
            return;
        }

        if (swipeDelta.x < 0f)
        {
            LeftMove();
        }
        else
        {
            RightMove();
        }
    }
}
