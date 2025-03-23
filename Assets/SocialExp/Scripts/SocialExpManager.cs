using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LNE.ProstheticVision;
using UnityEngine.SceneManagement; 

public class SocialExpManager : MonoBehaviour
{
    public static SocialExpManager Instance { get; private set; }
    int currentTask = 0;
    int nbTask = 8;
    bool _dominantEye = true;
    string _participantID = "";
    bool isRecorderOn = false;
    [SerializeField]
    private Task_BodyAppearance bodyAppearance;
    public bool linearMode = false;
    /*
    [SerializeField]
    private Task_BodyEmotion bodyEmotion;*/

    [SerializeField]
    private Task_BodyLanguage bodyLanguage;

    [SerializeField]
    private Task_BodyOrientation bodyOrientation;

    [SerializeField]
    private Task_FaceEmotion faceEmotion;

    [SerializeField]
    private Task_FaceEmotion faceEmotionExtraBody;

    [SerializeField]
    private Task_GestureRelationship gestureRelationship;

    [SerializeField]
    private Task_RecognizeRole recognizeRole;

    [SerializeField]
    private Task_Observation observation;

    [SerializeField]
    private Task_SeatSearch seatSearch;

    [SerializeField]
    private Task_Locomotion locomotion;

    [SerializeField]
    private NoteManager noteManager;

    /*
    [SerializeField]
    private GameObject[] _cameraDebug;
    */

   [SerializeField]
    private GameObject userHeadPosition;

    string pathFolder = "C:/DataRecordingVR/";
    private StreamWriter writer;

    string _condition = "control";

    string pathTask;
    public bool isControlGroup = false;
    /*
    [SerializeField]
    private GameObject[] _cameraConditions;
    */

    [SerializeField]
    private ImplantController implantController;


   /* [SerializeField]
    private GameObject _tmpCamera;
    
    [SerializeField]
    private Camera _otherEyeCamera;
   */
    public bool skipFaceEmotion = false;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        if (!Directory.Exists(pathFolder))
        {
            Directory.CreateDirectory(pathFolder);
        }

        //  StartCoroutine(WaitAndStartExperiment());
    }
    /*
    private IEnumerator WaitAndStartExperiment()
    {
        yield return new WaitForSeconds(0.5f);
        StartExperiment();
    }*/
    void StartExperiment()
    {
        currentTask = 0;
        StartTask();
    }
    public void SetSkipAnimationFaceTask(bool val)
    {
        skipFaceEmotion = val;
    }
    void StartTask()
    {
        Debug.Log("Task: " + currentTask + 1 + " [START]");
        // _cameraDebug[currentTask].SetActive(true);
        if(currentTask == 3)
        {
            currentTask++;
            StartTask();
            return;
        }
        if (currentTask < 10)
        {
            if (!Directory.Exists(pathFolder + _participantID + "/Task_" + (currentTask + 1)))
            {

                Directory.CreateDirectory(pathFolder + _participantID + "/Task_" + (currentTask + 1));
                pathTask = pathFolder + _participantID + "/Task_" + (currentTask + 1);
            }
         
            writer.WriteLine("start;task;" + (currentTask + 1) + ";" + System.DateTime.Now.Day + ";" + System.DateTime.Now.Month + ";" + System.DateTime.Now.Hour + ";" + System.DateTime.Now.Minute + ";" + System.DateTime.Now.Second  );
            PositionningUserManager.Instance.StartNewTask(currentTask);
            //TTS_manager.Instance.ReadInstructionTask();

            switch (currentTask)
            {
                case 0:
                    noteManager.StartTask(currentTask + 1, "Seat Search");
                    seatSearch.StartTask();
                    break;
                case 1:
                    noteManager.StartTask(currentTask + 1, "Body Orientation");
                    bodyOrientation.StartTask();
                    break;
                case 2:

                    noteManager.StartTask(currentTask + 1, "Body Appearance");
                    bodyAppearance.StartTask();
                    break;
                case 3:
                    noteManager.StartTask(currentTask + 1, "Face Emotion Extra Body");
                    faceEmotionExtraBody.StartTask();
                    break;
                case 4:
                    noteManager.StartTask(currentTask + 1, "Face Emotion");
                    faceEmotion.StartTask();
                    break;
                case 5:
                    noteManager.StartTask(currentTask + 1, "Gesture Relationship");
                    gestureRelationship.StartTask();
                    break;
                case 6:
                    noteManager.StartTask(currentTask + 1, "Recognize Role");
                    recognizeRole.StartTask();
                    break;
                case 7:
                    noteManager.StartTask(currentTask + 1, "Locomotion");
                    locomotion.StartTask();
                    break;
                case 8:
                    noteManager.StartTask(currentTask + 1, "Body Language");
                    bodyLanguage.StartTask();
                    break;
                case 9:
                    noteManager.StartTask(currentTask + 1, "Observation");
                    observation.StartTask();
                    break;


            }
        }
        else
        {
            EndOfExperiment();
        }
       
    }
    public void StopTaskBackMenu()
    {
        
        if (isRecorderOn)
        {
            writer.WriteLine("stop;exp;;" + System.DateTime.Now.Day + ";" + System.DateTime.Now.Month + ";" + System.DateTime.Now.Hour + ";" + System.DateTime.Now.Minute + ";" + System.DateTime.Now.Second);
            writer.WriteLine("EXIT;BACKMENU");
            writer.Close();
            
            isRecorderOn = false;
        }
        switch (currentTask)
        {
            case 0:
                seatSearch.ForceStopTask();
                break;
            case 1:
                bodyOrientation.ForceStopTask();
                break;
            case 2:
                bodyAppearance.ForceStopTask();
                break;
            case 3:
                faceEmotionExtraBody.ForceStopTask();
                break;
            case 4:
                faceEmotion.ForceStopTask();
                break;
            case 5:
                gestureRelationship.ForceStopTask();
                break;
            case 6:
                recognizeRole.ForceStopTask();
                break;
            case 7:
                locomotion.ForceStopTask();
                break;
            case 8:
                bodyLanguage.ForceStopTask();
                break;
            case 9:
                observation.ForceStopTask();
                break;



        }
    }
 string genderParticipant = "";
    string age = "";
        string expVR ="";
    string language = "";
    public void SetPreferencesParticipant(string gender, string ag, string exp, string lang)
    {
        genderParticipant = gender;
        age = ag;
        expVR = exp;
        language = lang;
        Debug.Log("Info participant: " + genderParticipant + " " + age + " " + expVR + " "+lang );
            
    }
    public void PrepareStartParticipant(string id, int startTask)
    {
        _participantID = id;
        currentTask = startTask;
        if (implantController.isImplantActive)
        {
            _condition = "Artificial" + "_" + implantController.GetCurrentFOV();
        }
        else
        {
            _condition = "Control" + "_" + implantController.GetCurrentFOV();
        }
       
        _dominantEye = implantController.IsRightEye();

        if (!Directory.Exists(pathFolder + _participantID))
        {
            Directory.CreateDirectory(pathFolder + _participantID);
        }

        string fileInfoPath = pathFolder + _participantID + "/" + _participantID + "_INFO.csv";
        writer = new StreamWriter(fileInfoPath, false);
        writer.WriteLine("sep =;");
        writer.WriteLine("user id;" + _participantID + ";Tache;Jour;Mois;Heure;Minute;Second");
        writer.WriteLine("user condition;" + _condition + ";DominantEyeRight;" + _dominantEye + ";Language;" + language);
        writer.WriteLine("Gender;" + genderParticipant + ";Age;" + age + ";Experience;"+ expVR);
        writer.WriteLine("start;exp;;" + System.DateTime.Now.Day + ";" + System.DateTime.Now.Month + ";" + System.DateTime.Now.Hour + ";" + System.DateTime.Now.Minute + ";" + System.DateTime.Now.Second);

        noteManager.InitNote(pathFolder + _participantID + "/" + _participantID + "_NOTES.txt");
        isRecorderOn = true;
        Debug.Log("Recorder initated : " + fileInfoPath);




        //_tmpCamera.SetActive(false);
        //StartCoroutine(SetupCameraEyes());
        /*
        int tmpCondition = _condition;
        if (!_dominantEye)
        {
            tmpCondition = 2 * _condition;
        }
        //userHeadPosition = _cameraConditions[tmpCondition];
        */

        RecorderPositionEyeTracking.Instance.user = userHeadPosition.transform;
        RecorderPositionEyeTracking.Instance.SetDominantEye(_dominantEye);
        StartTask();
    }

    IEnumerator SetupCameraEyes()
    {
        yield return new WaitForSeconds(0.1f);
        /*
        int tmpCondition = _condition;
        if (!_dominantEye)
        {
            tmpCondition = 2 * _condition;
        }
           _cameraConditions[tmpCondition].SetActive(true);

        _cameraConditions[tmpCondition].GetComponent<Camera>().cullingMask = 0;
        _cameraConditions[tmpCondition].GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
        yield return new WaitForSeconds(0.2f);
        
        _cameraConditions[tmpCondition].GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Right;

        Debug.Log("Condition : " + _condition + "  tmp condition: "+ tmpCondition);
        if (_condition != 0)
        {
            _cameraConditions[tmpCondition].GetComponent<LNE.ProstheticVision.Prosthesis>().enabled = true;
            
        }
        else
        {
            _cameraConditions[tmpCondition].GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        }
        _cameraConditions[tmpCondition].GetComponent<Camera>().cullingMask = -1;


        yield return new WaitForSeconds(0.2f);

        if (_dominantEye)
        {
            _cameraConditions[tmpCondition].GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Right;
            _otherEyeCamera.stereoTargetEye = StereoTargetEyeMask.Left;
            UnityEngine.XR.XRSettings.gameViewRenderMode = UnityEngine.XR.GameViewRenderMode.RightEye;
        }
        else
        {
            _cameraConditions[tmpCondition].GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Left;
            _otherEyeCamera.stereoTargetEye = StereoTargetEyeMask.Right;
            UnityEngine.XR.XRSettings.gameViewRenderMode = UnityEngine.XR.GameViewRenderMode.LeftEye;

        }

        _otherEyeCamera.gameObject.SetActive(true);
        */

    }
    public void ForceQuitExp()
    {
       
        if (isRecorderOn)
        {
            writer.WriteLine("stop;exp;;" + System.DateTime.Now.Day + ";" + System.DateTime.Now.Month + ";" + System.DateTime.Now.Hour + ";" + System.DateTime.Now.Minute + ";" + System.DateTime.Now.Second);
            writer.WriteLine("EXIT;FORCEQUIT");
            writer.Close();

        }
        noteManager.CloseNote();
        Application.Quit();
    }
    void OnApplicationQuit()
    {
       
        if (isRecorderOn)
        {
            writer.WriteLine("stop;exp;;" + System.DateTime.Now.Day + ";" + System.DateTime.Now.Month + ";" + System.DateTime.Now.Hour + ";" + System.DateTime.Now.Minute + ";" + System.DateTime.Now.Second);
            writer.WriteLine("EXIT;ERROR");
            writer.Close();
        }
        noteManager.CloseNote();
    }

    public Vector3 GetUserHeadRotationEuler()
    {
        return userHeadPosition.transform.eulerAngles;
    }
    public Vector3 GetUserFloorPosition()
    {
        return new Vector3(userHeadPosition.transform.position.x, 0f, userHeadPosition.transform.position.z);
    }
    public Vector3 GetTargetFloorPosition(Vector3 target)
    {
        return new Vector3(target.x, 0f, target.z);
    }
    public float GetUserTargetAngle(Vector3 target)
    {
        target = new Vector3(target.x, 0f, target.z);
       return  Vector3.Angle(new Vector3(userHeadPosition.transform.forward.x, 0f, userHeadPosition.transform.forward.z), target- new Vector3(userHeadPosition.transform.position.x, 0f, userHeadPosition.transform.position.z));
    }
    public Vector3 GetUserHeadPosition()
    {
        return userHeadPosition.transform.position;
    }
    public Transform GetUserHeadTransform()
    {
        return userHeadPosition.transform;
    }
    public Vector3 GetUserForward()
    {
        return userHeadPosition.transform.forward;
    }
    public Quaternion GetUserHeadRotationQuaternion()
    {
        return userHeadPosition.transform.rotation;
    }
    public string GetUserPath()
    {
        return pathFolder + _participantID + "/";
    }
    public string GetUserTaskPath()
    {
        return pathTask;
    }
    public string GetUserID()
    {
        return _participantID;
    }
    public int GetCurrentTask()
    {
        return currentTask + 1;
    }
    public void TaskDone()
    {
        writer.WriteLine("stop;task;" + (currentTask + 1) + ";" + System.DateTime.Now.Day + ";" + System.DateTime.Now.Month + ";" + System.DateTime.Now.Hour + ";" + System.DateTime.Now.Minute + ";" + System.DateTime.Now.Second);
        //_cameraDebug[currentTask].SetActive(false);
        Debug.Log("Task: " + (currentTask + 1) + " [DONE]");
        currentTask++;
        StartTask();

    }

    // Update is called once per frame
    void Update()
    {/*
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            TaskDone();
        }
        */
    }
    void EndOfExperiment()
    {
        Debug.Log("End of experiment");
        writer.WriteLine("stop;exp;;" + System.DateTime.Now.Day + ";" + System.DateTime.Now.Month + ";" + System.DateTime.Now.Hour + ";" + System.DateTime.Now.Minute + ";" + System.DateTime.Now.Second);
        writer.WriteLine("EXIT;OK");
        writer.Close();
        isRecorderOn = false;
        TTS_manager.Instance.ReadEndExperimentInstruction();
        ImplantController.Instance.SetImplantActiveCalibration(false);

        //TODO: Inform participant to remove headset before closing
        // Application.Quit();
    }
}
