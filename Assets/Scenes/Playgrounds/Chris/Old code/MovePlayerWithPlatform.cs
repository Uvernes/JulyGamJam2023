using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerWithPlatform : MonoBehaviour
{
    [SerializeField] GameObject actualPlatform;
    Vector3 playerOriginalScale;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Trigger Entered. Hi!");
            playerOriginalScale = other.transform.localScale;
            // Todo - fix bug where the call below changes the player's scale ratio to match the game object's one!
            other.transform.SetParent(actualPlatform.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Trigger Exited.");
            other.transform.SetParent(null);
            other.transform.localScale = playerOriginalScale;
            
        }
    }
}
