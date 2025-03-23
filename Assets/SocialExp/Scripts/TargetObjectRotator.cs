using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObjectRotator : MonoBehaviour
{
    [SerializeField, Range(0, 10)]
    private float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1), speed);
        //transform.Rotate(0,0, speed, Space.Self);
    }
}
