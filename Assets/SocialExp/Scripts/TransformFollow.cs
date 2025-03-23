using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollow : MonoBehaviour
{
    bool followTarget = false;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (followTarget)
        {
            transform.position = target.position;
            //transform.rotation = target.rotation;
        }
        
    }
    public void SetTargetAndFollow(Transform t)
    {
        target = t;
        followTarget = true;
    }
    public void StopFollow()
    {
        followTarget = false;
    }
    public void StartFollow()
    {
        followTarget = true;
    }
}
