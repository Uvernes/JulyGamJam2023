using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMusic : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    // Start is called before the first frame update
    void Awake()
    {
        AudioSource audio = FindObjectOfType<AudioSource>();
        if (audio == null)
            return;
        if (audio == audio.clip) 
            return;
        audio.clip = clip;
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
