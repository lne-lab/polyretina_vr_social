﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalagaan;
using Kalagaan.BlendShapesPresetTool;

using TMPro;
using UnityEngine.UI;
using System.IO;

public class Task_FaceEmotion : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _characters;

    [SerializeField]
    private GameObject _props;

    [SerializeField] int currentTrial = 0;

    private List<int> combinationList = new List<int>();
    public List<int> randomCombinationListAction = new List<int>();
    public List<int> randomCombinationListCharacter = new List<int>();
    int nbtrial = 4;


    bool taskStarted = false;
    [SerializeField]
    private GameObject _UICanvas;

    float startTime;

    [SerializeField]
    private TMP_Text _time;



    [SerializeField]
    private Slider confidenceSlider;

    [SerializeField]
    private Button _buttonValideForced;

    private StreamWriter writer;
    bool isRecorderOn = false;
    float _durationTimer = 150.0f;
    bool _isTimerOn = false;
    bool _trialCompleted = false;
    float _limitTimer;

    [SerializeField]
    private TMP_Dropdown _faceEmotionOptions;

    public bool _hasBodyEmotion = false;

    float _durationChoice = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //0: neutral, 1 : happy, 2: sad, 3: surprise, 4 : anger
        _props.SetActive(false);

        combinationList = new List<int>() { 0, 1, 2 };
        randomCombinationListAction = UtilityRandom.ShuffleList(combinationList);

        nbtrial = randomCombinationListAction.Count;

        combinationList = new List<int>() { 0, 1, 2 };
        //combinationList = new List<int>() { 3, 2, 1, 0 };
        randomCombinationListCharacter = UtilityRandom.ShuffleList(combinationList);

        _UICanvas.SetActive(false);
        _buttonValideForced.onClick.AddListener(ValidateChoice);


        // StartTask();



        List<string> options;
        options = new List<string> {
            "Pas de réponse",
            "Neutre",
            "Positive",
            "Negative"};

        _faceEmotionOptions.AddOptions(options);
        _faceEmotionOptions.value = 0;
        _faceEmotionOptions.onValueChanged.AddListener(delegate { ValidateChoiceTime(); });

        //   StartTask();

    }
    float distance, angle;
    void ValidateChoiceTime()
    {
        if (isRecorderOn)
        {
            _durationChoice = Time.time - startTime;
            distance = Vector3.Distance(SocialExpManager.Instance.GetTargetFloorPosition(_characters[randomCombinationListCharacter[currentTrial]].transform.position), SocialExpManager.Instance.GetUserFloorPosition());
            //angle = Vector3.Angle(SocialExpManager.Instance.GetUserForward(), _characters[randomCombinationListCharacter[currentTrial]].transform.position - SocialExpManager.Instance.GetUserHeadPosition());
            angle = SocialExpManager.Instance.GetUserTargetAngle(_characters[randomCombinationListCharacter[currentTrial]].transform.position);
            TTS_manager.Instance.ReadConfidenceShort();
            _isTimerOn = false;
            _trialCompleted = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (taskStarted)
        {
            //Update UI
            UpdateUI();
            /*
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                _characters[randomCombinationListCharacter[currentTrial]].SetActive(false);
                currentTrial++;
                StartTrial();
            }
            */
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
    }
    public void StartTask()
    {
        Debug.Log("Task face emotion start");
        _props.SetActive(false);
        SocialExpUIManager.Instance.SetTest("Task face emotion");
        currentTrial = 0;

        taskStarted = true;
        startTime = Time.time;

        UpdateUI();
        _UICanvas.SetActive(false);

        writer = new StreamWriter(SocialExpManager.Instance.GetUserPath() + SocialExpManager.Instance.GetUserID() + "_TASK" + SocialExpManager.Instance.GetCurrentTask() + "_SUMMARY.csv", false);
        isRecorderOn = true;
        writer.WriteLine("sep =;");
        writer.WriteLine("UserID;Trial;Value;Duration;Choice;DurationChoice;Confidence;Distance;Angle");
        _isTimerOn = false;
        _trialCompleted = false;

        if (_hasBodyEmotion)
        {
            PositionningUserManager.Instance.NeedRepositioning(this);
        }
        else
        {
            PositionningUserManager.Instance.NeedRepositioning(this, DirectionPositioning.Center);

        }



    }

    void StartTrial()
    {
        if (currentTrial < nbtrial)
        {
            NoteManager.Instance.WriteNote("Task " + SocialExpManager.Instance.GetCurrentTask() + " Trial" + (currentTrial + 1));
            _props.SetActive(true);
            SocialExpUIManager.Instance.SetTrial(currentTrial + 1, nbtrial);
            Debug.Log("Task face emotion, trial: " + currentTrial);
            _characters[randomCombinationListCharacter[currentTrial]].SetActive(true);
            BlendShapesPresetAnimator faceAnimator = _characters[randomCombinationListCharacter[currentTrial]].GetComponent<BlendShapesPresetAnimator>();

            //faceAnimator.SetWeight(randomCombinationListAction[currentTrial], 1);

            faceAnimator.m_weight0 = 0f;
            faceAnimator.m_weight1 = 0f;
            faceAnimator.m_weight2 = 0f;
            faceAnimator.m_weight3 = 0f;
            _durationChoice = 0f;

            startTime = Time.time;

            _faceEmotionOptions.value = 0;

            confidenceSlider.value = 0;

            _trialCompleted = false;
            _isTimerOn = false;
            RecorderPositionEyeTracking.Instance.SetSingleTarget(_characters[randomCombinationListCharacter[currentTrial]].transform);
            RecorderPositionEyeTracking.Instance.StartRecording(SocialExpManager.Instance.GetUserTaskPath(),
                SocialExpManager.Instance.GetUserID() + "_Task" + SocialExpManager.Instance.GetCurrentTask() + "_Trial" + (currentTrial + 1),
                true);
            isRecorderOn = true;
            startTime = Time.time;
            StartCoroutine(StartTimer());

            TTS_manager.Instance.StartSound();

            switch (randomCombinationListAction[currentTrial])
            {
                case 0:
                    //neutral

                    break;
                case 1:
                    //happy
                    faceAnimator.m_weight0 = .9f;
                    break;
                case 2:
                    //sad
                    faceAnimator.m_weight1 = .9f;
                    break;
                case 3:
                    //surprise
                    faceAnimator.m_weight2 = .9f;
                    break;


            }
            if (_hasBodyEmotion)
            {
                _characters[randomCombinationListCharacter[currentTrial]].GetComponent<Animator>().SetInteger("anim", randomCombinationListAction[currentTrial]);
            }

            //    _characters[randomCombinationListCharacter[currentTrial]].GetComponent<Animator>().SetInteger("ActionEmotion", randomCombinationListAction[currentTrial]);

        }
        else
        {

            Debug.Log("Task face emotion completed");
            FinishTask();
        }
    }

    void ValidateChoice()
    {
        //Vector3 pos = SocialExpManager.Instance.GetUserFloorPosition();
        //Vector3 rot = SocialExpManager.Instance.GetUserHeadRotationEuler();

        _props.SetActive(false);
        _characters[randomCombinationListCharacter[currentTrial]].SetActive(false);
        RecorderPositionEyeTracking.Instance.StopRecording();
        isRecorderOn = false;
        _isTimerOn = false;
        _trialCompleted = true;

        Debug.Log("Real: " + (randomCombinationListAction[currentTrial] + 1) + " Choice: " + _faceEmotionOptions.value);


        writer.WriteLine(SocialExpManager.Instance.GetUserID() + ";" + currentTrial + ";"
            + (randomCombinationListAction[currentTrial] + 1) + ";"
            + (Time.time - startTime) + ";" + _faceEmotionOptions.value + ";" + _durationChoice + ";" + confidenceSlider.value + ";" + distance + ";" + angle);



        confidenceSlider.value = 0;
        _faceEmotionOptions.value = 0;
        _durationChoice = 0f;
        _UICanvas.SetActive(false);

        currentTrial++;

        if (currentTrial < nbtrial)
        {
            if (_hasBodyEmotion)
            {
                PositionningUserManager.Instance.NeedRepositioning(this);
            }
            else
            {
                PositionningUserManager.Instance.NeedRepositioning(this, DirectionPositioning.Center);

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
        _props.SetActive(false);
        taskStarted = false;
        _UICanvas.SetActive(false);

        Debug.Log("Task face emotion completed");
        writer.Close();
        isRecorderOn = false;
        SocialExpManager.Instance.TaskDone();

    }
    public void ForceStopTask()
    {
        _props.SetActive(false);
        taskStarted = false;
        _UICanvas.SetActive(false);
        Debug.Log("Task face emotion completed");
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
            ValidateChoice();
        }

    }

   

   
}
