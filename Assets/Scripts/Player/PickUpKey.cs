using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpKey : MonoBehaviour
{
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
        }
    }
}
