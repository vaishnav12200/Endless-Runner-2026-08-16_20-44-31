using UnityEngine;

public class CollisionReset : MonoBehaviour
{
    [SerializeField] GameObject theCamera;
    [SerializeField] GameObject PlayerCube;
    [SerializeField] GameObject charAnim;
    [SerializeField] AudioSource crashFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            theCamera.GetComponent<CamFollow>().enabled = false;
            PlayerCube.GetComponent<PlayerMove>().enabled = false;
            charAnim.GetComponent<Animator>().Play("Falling Back Death");
            crashFX.Play();
        }
    }
}
