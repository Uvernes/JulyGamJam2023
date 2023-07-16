using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] float riseSpeed = 0.5f;
    Vector3 scaleChange;

    // Start is called before the first frame update
    void Start()
    {
        scaleChange = new Vector3(0, riseSpeed * Time.deltaTime, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += scaleChange;
    }
}
