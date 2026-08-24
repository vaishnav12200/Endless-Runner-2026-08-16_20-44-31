using UnityEngine;

public class StatControl : MonoBehaviour
{
   public static int coinCount;
   [SerializeField] GameObject textBox;
   [SerializeField] int internalCoinCount;
    void Update()
    {
        internalCoinCount = coinCount;
        textBox.GetComponent<TMPro.TMP_Text>().text = "" + coinCount;;
    }
}
