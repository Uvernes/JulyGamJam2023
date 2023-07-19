using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{

   // [SerializeField] bool overrideExisting = false;

    void Awake()
    {
        ManageSingleton();
    }

    void ManageSingleton()
    {
        //if (overrideExisting)
        //{
        //    AudioPlayer prevAudioPlayer = FindObjectOfType<AudioPlayer>();
        //    if (prevAudioPlayer != null)
        //    {
        //        prevAudioPlayer.gameObject.SetActive(false);
        //        Destroy(prevAudioPlayer.gameObject);
        //    }
        //    DontDestroyOnLoad(gameObject);
        //}

        int instanceCount = FindObjectsOfType(GetType()).Length;
        if (instanceCount > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

            DontDestroyOnLoad(gameObject);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
