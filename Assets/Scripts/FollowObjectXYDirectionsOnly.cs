using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObjectXYDirectionsOnly : MonoBehaviour
{
    [SerializeField] GameObject followObject;
    Vector2 initialOffsetToObject;

    // Start is called before the first frame update
    void Start()
    {
        initialOffsetToObject = new Vector2(followObject.transform.position.x - transform.position.x,
                                            followObject.transform.position.y - transform.position.y);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(followObject.transform.position.x - initialOffsetToObject.x,
                                         followObject.transform.position.y - initialOffsetToObject.y,
                                         transform.position.z);
    }
}
