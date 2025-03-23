using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSensor : MonoBehaviour
{
    public bool _isInWall;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        /*
        if (_isInWall)
        {
            Debug.Log("Found in box!");
        }
        else
        {
            Debug.Log("Not in box!");
        }*/
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Wall")){
            _isInWall = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall")){
            _isInWall = false;
        }
    }
}
