using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioSource audio = FindObjectOfType<AudioSource>();
        if (audio == null)
            return;
        audio.gameObject.SetActive(false);
        Destroy(audio.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
