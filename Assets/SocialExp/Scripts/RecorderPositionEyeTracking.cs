using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ViveSR.anipal.Eye;
using LNE.VectorExts;

public class RecorderPositionEyeTracking : MonoBehaviour
{
    public static RecorderPositionEyeTracking Instance { get; private set; }

    private StreamWriter writer;
    bool isRecording = false;
    float startRecordingTime;
    public Transform user;
    public Transform[] _targets;
    bool _hasTarget = false;
    bool _hasWallSensor = false;
    bool _isMultiTarget = false;

    [SerializeField]
    private SRanipal_Eye_Framework eyeTrackingData;
    [SerializeField]
    private bool dominantEye = true;

    WallSensor wallSensor;
    
    private static EyeData_v2 eyeData = new EyeData_v2();

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRecording)
        {

            WritePositions();

        }
    }
  
    public void SetDominantEye(bool eye)
    {
        dominantEye = eye;

    }

    public void SetSingleTarget(Transform t, bool trackHead = true)
    {
        _hasTarget = true;
        if (trackHead)
        {
            _targets = new Transform[2];
            _targets[0] = t;
            _targets[1] = FindHeadInChildren(t);
        }
        else
        {
            _targets = new Transform[1];
            _targets[0] = t;

        }


    }
    public void SetMultiTarget(Transform[] t, bool trackHead = true)
    {
        _hasTarget = true;
        if (trackHead)
        {
            Transform[] expandedArray = new Transform[t.Length * 2];

            for (int i = 0; i < t.Length; i++)
            {
                // Add the original object to the new array
                expandedArray[i * 2] = t[i];

                // Find the "Head" object in the children of the current object
                Transform headTransform = FindHeadInChildren(t[i]);

                // Add the found "Head" transform or null if not found
                expandedArray[i * 2 + 1] = headTransform;
                _targets = expandedArray;
            }
        }
        else
        {
            _targets = t;
        }

    }

    public void SetMultiTarget(GameObject[] gameObjects, bool trackHead = true)
    {
        _hasTarget = true;

        // Check if trackHead is true to add "Head" targets as well
        if (trackHead)
        {
            // Create an expanded array double the size to fit original objects and "Head" objects
            Transform[] expandedArray = new Transform[gameObjects.Length * 2];

            for (int i = 0; i < gameObjects.Length; i++)
            {
                // Convert GameObject to Transform and add it to the new array
                Transform originalTransform = gameObjects[i].transform;
                expandedArray[i * 2] = originalTransform;

                // Find the "Head" object in the children of the current GameObject's Transform
                Transform headTransform = FindHeadInChildren(originalTransform);

                // Add the found "Head" transform or null if not found
                expandedArray[i * 2 + 1] = headTransform;

                // Update _targets with the expanded array containing both objects and "Head" objects
                _targets = expandedArray;
            }

        }
        else
        {
            // Convert GameObject[] to Transform[] without adding "Head" objects
            _targets = new Transform[gameObjects.Length];
            for (int i = 0; i < gameObjects.Length; i++)
            {
                _targets[i] = gameObjects[i].transform;
            }
        }
    }

    public void AddExtraTarget(GameObject[] extraGameObjects)
    {
        // Create a new array that can hold both the existing _targets and the new GameObject Transforms
        Transform[] expandedArray = new Transform[_targets.Length + extraGameObjects.Length];

        // Copy the existing _targets elements into the new array
        for (int i = 0; i < _targets.Length; i++)
        {
            expandedArray[i] = _targets[i];
        }

        // Add each GameObject's Transform into the new array
        for (int j = 0; j < extraGameObjects.Length; j++)
        {
            expandedArray[_targets.Length + j] = extraGameObjects[j].transform;
        }

        // Update _targets to the new expanded array
        _targets = expandedArray;
    }

    public void StartRecording(string path, string name, bool hasTarget = false, bool hasWallSens = false, WallSensor sensor = null)
    {
        Debug.Log("Start recording: "+path+" "+name);
        startRecordingTime = Time.time;
        _hasTarget = hasTarget;

        _hasWallSensor = hasWallSens;
        wallSensor = sensor;
        
        writer = new StreamWriter(path + "/" + name + "_POSITION.csv", false);
        writer.WriteLine("sep =;");
        writer.Write("{0};{1};{2};{3};{4};{5};{6};{7};",
        "Time",
          "UserID",
        "User Pos X",
        "User Pos Y",
        "User Pos Z",
        "User Rot X",
        "User Rot Y",
        "User Rot Z"
        );
        writer.Write("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};",
        "Eye X",
        "Eye Y",
        "EyePositionX",
        "EyePositionY",
        "EyePositionZ",
        "EyeDirectionX",
        "EyeDirectionY",
        "EyeDirectionZ",
        "PupilDilation",
        "TargetEye",
        "TagTargetEye"
        );
        if (_hasTarget)
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                writer.Write("{0};{1};{2};{3};{4};{5};",
                   "Target" + (i + 1) + " Pos X "+ _targets[i].name,
                "Target" + (i + 1) + " Pos Y " + _targets[i].name,
                "Target" + (i + 1) + " Pos Z " + _targets[i].name,
                "Target" + (i + 1) + " Rot X "+ _targets[i].name,
                "Target" + (i + 1) + " Rot Y " + _targets[i].name,
                "Target" + (i + 1) + " Rot Z " + _targets[i].name
           );

            }
        }
        if (_hasWallSensor)
        {
            writer.Write("{0};",
       "IsInnWall"  );
        }

        writer.Write("\n");
        isRecording = true;

    }
    string target = "nothing";
    string targetTag = "default";
    private RaycastHit hit;
    public void WritePositions()
    {
        writer.Write("{0};{1};{2};{3};{4};{5};{6};{7};",
             Time.time - startRecordingTime,
             SocialExpManager.Instance.GetUserID(),
             user.transform.position.x,
             user.transform.position.y,
             user.transform.position.z,
             user.transform.rotation.eulerAngles.x,
             user.transform.rotation.eulerAngles.y,
             user.transform.rotation.eulerAngles.z
             );

        target = "nothing";
        targetTag = "nothing";
        
        var camera = LNE.ProstheticVision.Prosthesis.Instance.Camera;
        /*
        // looking plane (placed infront of camera)
        var plane = new Plane(-camera.transform.forward, camera.transform.forward);
        // looking direction (start from camera with camera normal as vec3(0, 0, 1))
        var eyeGaze = camera.stereoTargetEye == StereoTargetEyeMask.Right ?
                        eyeData.verbose_data.right.gaze_direction_normalized :
                        eyeData.verbose_data.left.gaze_direction_normalized;

        eyeGaze.x *= -1;

        var directionVector = camera.transform.rotation * eyeGaze;
        var direction = new Ray(camera.transform.position, directionVector);

        // looking position
        var position = LNE.AuxMath.Intersection(plane, direction);

        // position in screen space
        var screenPoint = camera.WorldToScreenPoint(position)
                                .DivideXY(camera.pixelWidth, camera.pixelHeight)
                                .SubtractXY(0.5f)
                                .SubtractXY(LNE.ProstheticVision.EyeGaze.Offset);

        if (Physics.Raycast(direction, out hit, 50))
        {
            target = hit.transform.name;
            tag = hit.transform.gameObject.tag;
        }
        Debug.DrawRay(camera.transform.position, directionVector * 50, Color.red, 2.0f);
        */
        Ray newRay = LNE.ProstheticVision.EyeGaze.GetViveProRay();
        /*
        var plane = new Plane(-camera.transform.forward, camera.transform.forward);
        // looking position
        var position = AuxMath.Intersection(plane, newRay.direction);
        
        // position in screen space
        var screenPoint = camera.WorldToScreenPoint(position)
                                .DivideXY(camera.pixelWidth, camera.pixelHeight)
                                .SubtractXY(0.5f)
                                .SubtractXY(LNE.ProstheticVision.EyeGaze.Offset);
        */
        if (Physics.Raycast(newRay.origin, newRay.direction, out hit, 50))
        {
            target = hit.transform.name;
            targetTag = hit.transform.gameObject.tag;
        }
        //Debug.Log("Target: " + target);

        //Debug.DrawRay(newRay.origin, newRay.direction * 50, Color.blue, 2.0f);

        var eye = LNE.ProstheticVision.EyeGaze.VivePro;
        writer.Write("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};",
        eye.x,
         eye.y,
         newRay.origin.x,
         newRay.origin.y,
         newRay.origin.z,
         newRay.direction.x,
         newRay.direction.y,
         newRay.direction.z,
         LNE.ProstheticVision.EyeGaze.GetPupilDilation(),
         target,
         targetTag
       );


        if (_hasTarget)
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                writer.Write("{0};{1};{2};{3};{4};{5};",
            _targets[i].transform.position.x,
           _targets[i].transform.position.y,
            _targets[i].transform.position.z,
            _targets[i].transform.rotation.eulerAngles.x,
            _targets[i].transform.rotation.eulerAngles.y,
            _targets[i].transform.rotation.eulerAngles.z
           );

            }
        }
        if (_hasWallSensor)
        {
            writer.Write("{0};",
            wallSensor._isInWall);
        }

        writer.Write("\n");
    }
 
    public void StopRecording()
    {
        Debug.Log("Stop recording positions");
        _hasTarget = true;
        isRecording = false;
        writer.Close();

    }
    void OnApplicationQuit()
    {
        if (isRecording)
        {
            writer.WriteLine("EXIT ERROR");
            writer.Close();
        }
    }
    public Transform FindHeadInChildren(Transform parent)
    {
        // Check if the current parent object contains "Head" in its name
        if (parent.name.Contains("Head"))
        {
            return parent;
        }

        // Recursively check each child
        foreach (Transform child in parent)
        {
            Transform found = FindHeadInChildren(child);
            if (found != null)
            {
                return found;
            }
        }

        // Return null if no object with "Head" in its name was found
        return null;
    }
}
