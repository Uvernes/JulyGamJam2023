using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerWithPlatform : MonoBehaviour
{
    [SerializeField] GameObject actualPlatform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Trigger Entered.");
            other.transform.SetParent(actualPlatform.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Trigger Exited.");
            other.transform.SetParent(null);
        }
    }
}
