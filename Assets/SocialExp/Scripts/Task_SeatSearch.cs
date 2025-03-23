using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class Task_SeatSearch : MonoBehaviour
{

    [SerializeField]
    private GameObject[] _seats; //First (index 09 is the empty chair
    [SerializeField]
    private Transform[] _seatsPosition;
    public int[] trial1_Pos, trial2_Pos, trial3_Pos, trial4_Pos, trial5_Pos;
    public int[][] trials_Pos;
    [SerializeField] int currentTrial = 0;
    int nbSeats;
    int nbtrial = 6;
    bool taskStarted = false;
    [SerializeField]
    private GameObject _UICanvas;

    float startTime;
    [SerializeField]
    private TMP_Text _distance, _angle, _time;
    [SerializeField]
    private Button[] _buttonValideSeat;

    [SerializeField]
    private Button _buttonValideForced;

    [SerializeField]
    private Slider confidenceSlider;

    private StreamWriter writer;
    bool isRecorderOn = false;
    float _distanceLimit = 1.5f;
    float _angleLimit = 45.0f;
    float _durationTimer = 150.0f;
    bool _isTimerOn = false;
    bool _trialCompleted = false;
    bool userChoose = false;
    float _limitTimer;
    float _timeChoice;
    int _choice = 0;

  
    public List<int> randomCombinationList = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        //Sequence 2  5  3  4  1  
        nbSeats = _seats.Length;
        trials_Pos = new int[5][];

        trials_Pos[0] = trial1_Pos;
        trials_Pos[1] = trial2_Pos;
        trials_Pos[2] = trial3_Pos;
        trials_Pos[3] = trial4_Pos;
        trials_Pos[4] = trial5_Pos;

        nbtrial = trials_Pos.Length;

        List<int> combinationList = new List<int>() { 0, 1, 2,3,4};
        randomCombinationList = UtilityRandom.ShuffleList(combinationList);

        for (int i = 0; i < nbSeats; i++)
        {
            _seats[i].SetActive(false);
        }

        if (nbSeats != _seatsPosition.Length)
        {
            Debug.LogError("Seat number incorrect");

        }
        _UICanvas.SetActive(false);
        
        for(int i = 0; i < _buttonValideSeat.Length; i++)
        {
            int tmp = i;
            _buttonValideSeat[i].onClick.AddListener(delegate { SelectChoice(tmp); });
        }
        


       
        _buttonValideForced.onClick.AddListener(ValidateChoice);
    }

    // Update is called once per frame
    void Update()
    {
        if (taskStarted)
        {


            //Update UI
            UpdateUI();

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                currentTrial++;
                StartTrial();
            }



        }
    }

    public void PositioningCompleted()
    {

        Debug.Log("User in position!");
        taskStarted = true;
        startTime = Time.time;
        UpdateUI();
        StartTrial();
    }
    public void StartTask()
    {
        Debug.Log("Task seats start");
        SocialExpUIManager.Instance.SetTest("Task seat search");
        currentTrial = 0;
        writer = new StreamWriter(SocialExpManager.Instance.GetUserPath() + SocialExpManager.Instance.GetUserID() + "_TASK" + SocialExpManager.Instance.GetCurrentTask() + "_SUMMARY.csv", false);
        isRecorderOn = true;
        writer.WriteLine("sep =;");
        writer.WriteLine("UserID;Trial;Value;Duration;Choice;DurationChoice;Confidence;Distance;Angle");
        _isTimerOn = false;
        _trialCompleted = false;
        userChoose = false;
        PositionningUserManager.Instance.NeedRepositioning(this);

    }


    void UpdateUI()
    {
        if (_isTimerOn)
        {
            _time.text = (_limitTimer - Time.time).ToString("00.0");
        }
        else
        {
            _time.text = (_durationTimer).ToString("00.0");
        }
        _distance.text = Vector3.Distance(SocialExpManager.Instance.GetTargetFloorPosition(_seats[0].transform.position), SocialExpManager.Instance.GetUserFloorPosition()).ToString("00.0");
        //        currentAngle = Vector3.Angle(user.forward, target.position - user.position);
        _angle.text = SocialExpManager.Instance.GetUserTargetAngle(_seats[0].transform.position).ToString("00.0");//Vector3.Angle(SocialExpManager.Instance.GetUserForward(), _seats[0].transform.position - SocialExpManager.Instance.GetUserHeadPosition()).ToString("00.0");
        //_angle.text = Vector3.Angle(_seats[0].transform.position, SocialExpManager.Instance.GetUserHeadPosition()).ToString();
    }
    void SelectChoice(int val)
    {
        _choice = val;
        Debug.Log("choice: " + _choice);
        _timeChoice = Time.time - startTime;
        
        distance = Vector3.Distance(SocialExpManager.Instance.GetTargetFloorPosition(_seats[0].transform.position), SocialExpManager.Instance.GetUserFloorPosition());
        angle = SocialExpManager.Instance.GetUserTargetAngle(_seats[0].transform.position);

        _isTimerOn = false;
        userChoose = true;
        if (val != 0) {
            TTS_manager.Instance.ReadConfidence();
        }
        StopAllCoroutines();

    }


    float distance, angle;
    void ValidateChoice()
    {
        _UICanvas.SetActive(false);
        _isTimerOn = false;
        _trialCompleted = true;

        Vector3 pos = SocialExpManager.Instance.GetUserFloorPosition();
        Vector3 rot = SocialExpManager.Instance.GetUserHeadRotationEuler();
        
        /*
         int success = 0;
         if (distance < _distanceLimit && angle < _angleLimit)
         {
             success = 1;
         }*/
        Debug.Log("choice before line: " + _choice);
        Debug.Log(SocialExpManager.Instance.GetUserID() + ";" + currentTrial + ";" + (trials_Pos[randomCombinationList[currentTrial]][0] + 1) + ";" + _timeChoice + ";" + _choice + ";" + confidenceSlider.value + ";" + distance + ";" + angle);
        writer.WriteLine(SocialExpManager.Instance.GetUserID() + ";" + currentTrial + ";" + (trials_Pos[randomCombinationList[currentTrial]][0]+1) + ";" + (Time.time - startTime) + ";" + _choice + ";" + _timeChoice + ";" + confidenceSlider.value + ";" + distance + ";" + angle );
        RecorderPositionEyeTracking.Instance.StopRecording();
      

        currentTrial++;
        if (currentTrial < nbtrial)
        {
            for (int i = 0; i < nbSeats; i++)
            {
                _seats[i].SetActive(false);
            }
            PositionningUserManager.Instance.NeedRepositioning(this);
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
            // RecorderPositionEyeTracking.Instance.StartRecording("r1","r2");
            _trialCompleted = false;
            _isTimerOn = false;
            confidenceSlider.value = 0;
            //RecorderPositionEyeTracking.Instance.SetSingleTarget(_seats[0].transform, false);
            RecorderPositionEyeTracking.Instance.SetMultiTarget(_seats, true);

            RecorderPositionEyeTracking.Instance.StartRecording(SocialExpManager.Instance.GetUserTaskPath(),
                SocialExpManager.Instance.GetUserID() + "_Task" + SocialExpManager.Instance.GetCurrentTask() + "_Trial" + (currentTrial+1),
                true);
            isRecorderOn = true;
            startTime = Time.time;
            userChoose = false;
            _timeChoice = 0;
            _UICanvas.SetActive(true);
            StartCoroutine(StartTimer());
            UpdateUI();
            _choice = 0;

            TTS_manager.Instance.StartSound();

            SocialExpUIManager.Instance.SetTrial(currentTrial + 1, nbtrial);
            //Debug.Log("Task seats, trial: " + currentTrial);
            Debug.Log("FIND seat: " + (trials_Pos[randomCombinationList[currentTrial]][0]+1));
            for (int i = 0; i < nbSeats; i++)
            {
                _seats[i].SetActive(true);             
                _seats[i].transform.position = _seatsPosition[trials_Pos[randomCombinationList[currentTrial]][i]].position;
                _seats[i].transform.rotation = _seatsPosition[trials_Pos[randomCombinationList[currentTrial]][i]].rotation;

            }
        }
        else
        {
            FinishTask();
        }
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
        Debug.Log("Task completed");
        taskStarted = false;
        _UICanvas.SetActive(false);


        for (int i = 0; i < nbSeats; i++)
        {
            _seats[i].SetActive(false);
        }

        SocialExpManager.Instance.TaskDone();
        StartCoroutine(WaitAndCloseWriter());
    }

    public void ForceStopTask()
    {
        for (int i = 0; i < nbSeats; i++)
        {
            _seats[i].SetActive(false);
        }

        taskStarted = false;
        _UICanvas.SetActive(false);

        if (isRecorderOn)
        {
            writer.WriteLine("EXIT;BACKMENU");
            writer.Close();
            isRecorderOn = false;
        }
    }

    IEnumerator StartTimer()
    {
        _limitTimer = Time.time + _durationTimer;
        _isTimerOn = true;

        bool hasRing = false;
        float limithalfTimer = Time.time + _durationTimer / 2.0f;
       // Debug.Log("Start timer");
        while (Time.time < _limitTimer && _isTimerOn)
        {
            if (!hasRing && Time.time > limithalfTimer)
            {
                hasRing = true;
                TTS_manager.Instance.RingHalfTime();
           //     Debug.Log("HALF TIME timer");
            }
            yield return new WaitForSeconds(0.5f);
        }
       // Debug.Log("END timer");
        if (!userChoose)
        {
            _isTimerOn = false;
            Debug.Log("Trial not completed");
            ValidateChoice();

        }

    }

    IEnumerator WaitAndCloseWriter()
    {
        yield return new WaitForSeconds(0.2f);
        writer.Close();
        isRecorderOn = false;
    }

}
