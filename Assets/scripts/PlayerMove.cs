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
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);
        xPos =gameObject.transform.position.x;
        zPos = gameObject.transform.position.z;
        if (currentMove == true && moveDirection == 1) //movng left
        {
            transform.Translate(Vector3.left * Time.deltaTime * sideSpeed, Space.World);
            if (xPos <=trackNumber)
            {
                currentMove = false;
                moveDirection = 0;
                transform.position = new Vector3(trackNumber, 1, zPos);
            }
        }
        if (currentMove == true && moveDirection == 2) //movng right
        {
            transform.Translate(Vector3.left * Time.deltaTime * sideSpeed * -1, Space.World);
            if (xPos >=trackNumber)
            {
                currentMove = false;
                moveDirection = 0;
                transform.position = new Vector3(trackNumber, 1, zPos);
            }
        }

    }

    public void LeftMove()
    {
        if (trackNumber == 1)
        {
            whoosh.Play();
            currentMove = true;
            moveDirection = 1;
            trackNumber=0;
        }
        if (trackNumber == 2)
        {
             whoosh.Play();
            currentMove = true;
            moveDirection = 1;
            trackNumber=1;
        }
    }
    public void RightMove()
    {
        if (trackNumber == 1)
        {
            whoosh.Play();
            currentMove = true;
            moveDirection = 2;
            trackNumber=2;
        }
        if (trackNumber == 0)
        {
             whoosh.Play();
            currentMove = true;
            moveDirection = 2;
            trackNumber=1;
        }
    }
}
                                                                