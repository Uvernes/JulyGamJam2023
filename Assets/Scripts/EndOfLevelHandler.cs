using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

public class EndOfLevelHandler : MonoBehaviour
{
    // Have the end of level object take up no space (since trigger instead of collider), and it
    // also handles the end of level logic.
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            LoadNextLevel();
    }

    // Use this code if we want the end of level object to take up space (i.e is a collider).
    // This then requires the Player to handle the end of level logic.
    // 
    //void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if (hit.gameObject.tag == "EndOfLevel")
    //        LoadNextLevel();
    //}

    void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
