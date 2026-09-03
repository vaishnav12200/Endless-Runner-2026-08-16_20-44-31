using System.Collections;
using UnityEngine;

public class CoinWork : MonoBehaviour
{
    [SerializeField] bool collectedCoin;
    [SerializeField] AudioSource coinDing;

    Vector3 startingLocalPosition;
    Quaternion startingLocalRotation;

    void Awake()
    {
        startingLocalPosition = transform.localPosition;
        startingLocalRotation = transform.localRotation;
    }

    void Update()
    {
        transform.Rotate(0f, 120f * Time.deltaTime, 0f, Space.World);
        if (collectedCoin)
        {
            transform.Translate(Vector3.up * Time.deltaTime * 6f, Space.World);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (collectedCoin || !other.transform.root.CompareTag("Player"))
        {
            return;
        }

        collectedCoin = true;
        StatControl.coinCount += 1;

        if (coinDing != null)
        {
            coinDing.Play();
        }

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Shrink");
        }

        StartCoroutine(HideCoin());
    }

    IEnumerator HideCoin()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    public void ResetForReuse()
    {
        StopAllCoroutines();
        collectedCoin = false;
        transform.localPosition = startingLocalPosition;
        transform.localRotation = startingLocalRotation;
        gameObject.SetActive(true);

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }
}
