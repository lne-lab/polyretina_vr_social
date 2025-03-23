using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
public class FollowerArrived : MonoBehaviour
{
    SplineFollower follower;

    // Start is called before the first frame update
    void Start()
    {
        follower = GetComponent<SplineFollower>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TargetArrived()
    {
        follower.enabled = false;
    }
}
