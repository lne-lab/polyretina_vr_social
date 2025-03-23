using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using System.IO;

public class Task_Locomotion : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _characters;
    [SerializeField]
    private GameObject[] _splineAnimation;
    [SerializeField]
    private Transform[] _calibrationTrialPositions;

    [SerializeField] int currentTrial = 0;


    int nbtrial = 3;

    bool taskStarted = false;
    [SerializeField]
    private GameObject _UICanvas;

    float startTime;

    [SerializeField]
    private TMP_Text _time;

    [SerializeField]
    private Button _buttonValideForcedOk;
    [SerializeField]
    private Button _buttonValideForcedKo;
    private StreamWriter writer;
    bool isRecorderOn = false;

    float _durationTimer = 150.0f;
    bool _isTimerOn = false;
    bool _trialCompleted = false;
    float _limitTimer;

    float lastFrame;
    public float durationOk, durationKo;

    public WallSensor _wallSensor;

    // Start is called before the first frame update
    void Start()
    {
        _UICanvas.SetActive(false);
        _buttonValideForcedOk.onClick.AddListener(delegate { ValidateChoice(true); });
        _buttonValideForcedKo.onClick.AddListener(delegate { ValidateChoice(false); });
  
    }

    void Update()
    {
        if (taskStarted)
        {
            //Update UI
            UpdateUI();

            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                _characters[currentTrial].SetActive(false);

                currentTrial++;
                StartTrial();
            }

        }
    }

    void FixedUpdate()
    {
        if (isRecorderOn)

        {
            if (_wallSensor._isInWall)
            {
                durationKo += Time.time - lastFrame;
            }
            else
            {
                durationOk += Time.time - lastFrame;
            }
            lastFrame = Time.time;
        }
    }

    public void PositioningCompleted()
    {

        Debug.Log("User in position!");
        taskStarted = true;
        startTime = Time.time;
        _UICanvas.SetActive(true);
        UpdateUI();
        StartTrial();
    }

    public void StartTask()
    {
        Debug.Log("Task locomotion start");
        SocialExpUIManager.Instance.SetTest("Task locomotion orientation");
        currentTrial = 0;
        taskStarted = true;
        startTime = Time.time;

        UpdateUI();
        _UICanvas.SetActive(false);

        writer = new StreamWriter(SocialExpManager.Instance.GetUserPath() + SocialExpManager.Instance.GetUserID() + "_TASK" + SocialExpManager.Instance.GetCurrentTask() + "_SUMMARY.csv", false);
        
        writer.WriteLine("sep =;");
        writer.WriteLine("UserID;Trial;Duration;Success;TimeOutWall;TimeInWall;Distance;Angle");
        _isTimerOn = false;
        _trialCompleted = false;

        _wallSensor = SocialExpManager.Instance.GetUserHeadTransform().GetComponent<WallSensor>();
        PositionningUserManager.Instance.NeedRepositioning(this);

    }
    void UpdateUI()
    {
        if (isRecorderOn)
        {
            if (_isTimerOn)
            {
                _time.text = (_limitTimer - Time.time).ToString("00.0");
            }
            else
            {
                _time.text = (_durationTimer).ToString("00.0");
            }
        }

    }
    void ValidateChoice(bool succes)
    {
        //Vector3 pos = SocialExpManager.Instance.GetUserFloorPosition();
        //Vector3 rot = SocialExpManager.Instance.GetUserHeadRotationEuler();
        float distanceD = Vector3.Distance(SocialExpManager.Instance.GetTargetFloorPosition(_characters[currentTrial].transform.position), SocialExpManager.Instance.GetUserFloorPosition());
        //angle = Vector3.Angle(SocialExpManager.Instance.GetUserForward(), _characters[randomCombinationListCharacter[currentTrial]].transform.position - SocialExpManager.Instance.GetUserHeadPosition());
        float angle = SocialExpManager.Instance.GetUserTargetAngle(_characters[currentTrial].transform.position);

        _characters[currentTrial].SetActive(false);
        RecorderPositionEyeTracking.Instance.StopRecording();
        _isTimerOn = false;
        _trialCompleted = true;
        isRecorderOn = false;
        Debug.Log("Task locomotion : "+ succes+"  duration ok: "+ durationOk+" "+ durationKo);
        writer.WriteLine(SocialExpManager.Instance.GetUserID() + ";" + currentTrial + ";" +
                   (Time.time - startTime) + ";"+
                   succes+ ";" +durationOk+";"+durationKo+";"+distanceD+";"+angle               
                   );

      

        _UICanvas.SetActive(false);

        currentTrial++;

        if (currentTrial < nbtrial)
        {

            PositionningUserManager.Instance.NeedRepositioningSpecificLocation(this, _calibrationTrialPositions[currentTrial]);
            //PositionningUserManager.Instance.NeedRepositioning(this);
        }
        else
        {
            FinishTask();
        }
    }
   
        void StartTrial()
    {
        if (currentTrial < nbtrial)
        {
            NoteManager.Instance.WriteNote("Task " + SocialExpManager.Instance.GetCurrentTask() + " Trial" + (currentTrial + 1));
            startTime = Time.time;
          
            _trialCompleted = false;
            _isTimerOn = false;
            isRecorderOn = true;

            RecorderPositionEyeTracking.Instance.SetSingleTarget(_characters[currentTrial].transform);
            RecorderPositionEyeTracking.Instance.StartRecording(SocialExpManager.Instance.GetUserTaskPath(),
                SocialExpManager.Instance.GetUserID() + "_Task" + SocialExpManager.Instance.GetCurrentTask() + "_Trial" + (currentTrial + 1),
                true,true, _wallSensor);
            durationOk = 0f;
            durationKo = 0f;
            lastFrame = Time.time;

            startTime = Time.time;
            StartCoroutine(StartTimer());
            UpdateUI();

            TTS_manager.Instance.StartSound();


            SocialExpUIManager.Instance.SetTrial(currentTrial + 1, nbtrial);
            Debug.Log("Task body orientation, trial: " + currentTrial);
            _characters[currentTrial].SetActive(true);

            //_splineAnimation[currentTrial].SetActive(true)

            StartCoroutine(WaitStartSpline());


        }
        else
        {
            FinishTask();
        }
    }
    IEnumerator WaitStartSpline()
    {
        Debug.Log("wait to move");
        yield return new WaitForSeconds(10f);
        Debug.Log("Start moving");
        _splineAnimation[currentTrial].SetActive(true);
    }

    void OnApplicationQuit()
    {
        if (isRecorderOn)
        {
            writer.WriteLine("stop;exp;;" + System.DateTime.Now.Day + ";" + System.DateTime.Now.Month + ";" + System.DateTime.Now.Hour + ";" + System.DateTime.Now.Minute);
            writer.WriteLine("EXIT;ERROR");
            writer.Close();
        }
    }
    void FinishTask()
    {
        taskStarted = false;
        _UICanvas.SetActive(false);

        Debug.Log("Task locomotion completed");
        writer.Close();
        isRecorderOn = false;
        SocialExpManager.Instance.TaskDone();


    }

    public void ForceStopTask()
    {
        taskStarted = false;
        _UICanvas.SetActive(false);
        writer.WriteLine("EXIT;BACKMENU");
        writer.Close();
        isRecorderOn = false;
    }

    IEnumerator StartTimer()
    {
        _limitTimer = Time.time + _durationTimer;
        _isTimerOn = true;

        bool hasRing = false;
        float limithalfTimer = Time.time + _durationTimer / 2.0f;

        while (Time.time < _limitTimer && _isTimerOn)
        {
            if (!hasRing && Time.time > limithalfTimer)
            {
                hasRing = true;
                TTS_manager.Instance.RingHalfTime();

            }
            yield return new WaitForSeconds(0.5f);
        }
        if (!_trialCompleted)
        {
            _isTimerOn = false;
            ValidateChoice(false);
        }

    }

}
