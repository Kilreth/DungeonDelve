using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpKey : MonoBehaviour
{
    private Text KeyText;
    private int keysFound = 0;

    void Start()
    {
        KeyText = GM.Instance.Canvas.transform.Find("KeyText").gameObject.GetComponent<Text>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Key")
        {
            GameObject key = other.gameObject;
            while (key.transform.parent.gameObject.tag == "Key")
            {
                key = key.transform.parent.gameObject;
            }
            Destroy(key);

            KeyText.text = ++keysFound + "/" + GM.Instance.TotalKeys;
            if (keysFound >= GM.Instance.TotalKeys)
            {
                // broadcast win, end game
            }
        }
    }
}
