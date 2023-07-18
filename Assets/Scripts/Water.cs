using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] float riseSpeed = 0.5f;
    [SerializeField] float initDelay = 0f;
    float timePassed;
    Vector3 scaleChange;

    // Start is called before the first frame update
    void Start()
    {
        scaleChange = new Vector3(0, riseSpeed, 0);
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (timePassed > initDelay)
        {
            transform.localScale += scaleChange * Time.deltaTime;
        }
    }
}
