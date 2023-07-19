using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FallingPlatform : MonoBehaviour
{
    [SerializeField] float delay;
    float timePassed;

    bool isFalling = false;
    float downSpeed = 10;
    //public GameObject Player;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isFalling = true;
            Debug.Log("falling");
            //Invoke("Update", 2);
        }
    }

    void Update()
    {
        timePassed += Time.deltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFalling==true & timePassed > delay)
        {
            downSpeed += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y - downSpeed, transform.position.z);
            Debug.Log("fell");
        }
        
    }
}
