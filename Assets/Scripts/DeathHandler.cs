using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathHandler : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // A trigger on trigger collision will only be detected if atleast of the triggers
        // has a RigidBody. Hence, a RigidBody was added to the Water.
        if (other.gameObject.tag == "Water")
        {
            HandleDeath();
        }
    }
    void HandleDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
