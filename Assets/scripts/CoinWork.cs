using System.Collections;
using UnityEngine;

public class CoinWork : MonoBehaviour
{
    [SerializeField] bool collectedCoin;
    [SerializeField] AudioSource coinDing;
        // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 2, 0,Space.World);
        if (collectedCoin == true)
        {
            transform.Translate(Vector3.up * Time.deltaTime * 6, Space.World);
        }
    }
    void OnTriggerEnter(Collider other)
   
        {
            collectedCoin = true;
            StatControl.coinCount += 1;
            coinDing.Play();
            this.gameObject.GetComponent<Animator>().Play("Shrink"); 
            StartCoroutine(DeleteCoin());
        }
        IEnumerator DeleteCoin()
        {
            yield return new WaitForSeconds(0.5f);
            this.gameObject.SetActive(false);
        }
    
}
