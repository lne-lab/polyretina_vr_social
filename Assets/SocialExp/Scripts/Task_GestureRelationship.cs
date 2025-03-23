using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using System.IO;


public class Task_GestureRelationship : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _charactersGroup;
    
    [SerializeField]
    private GameObject _props;

    [SerializeField] int currentTrial = 0;

    private List<int> combinationList = new List<int>();
    public List<int> randomCombinationListCharacter = new List<int>();
    int nbtrial = 2;


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
 

    float _durationChoice = 0.0f;
    [SerializeField]
    private TMP_Dropdown _relationOptions;

    // Start is called before the first frame update
    void Start()
    {
        combinationList = new List<int>() { 0, 1, 2};
        //0: Pro, 1 Familly, 2 Stranger
        randomCombinationListCharacter = UtilityRandom.ShuffleList(combinationList);

        nbtrial = _charactersGroup.Length;

        _UICanvas.SetActive(false);
        _buttonValideForced.onClick.AddListener(ValidateChoice);

       
        List<string> options = new List<string> {
            "Pas de réponse",
            "Professionelle",
            "Amicale",
            "Inconnu"};
        _relationOptions.AddOptions(options);

        _relationOptions.value = 0;
        _relationOptions.onValueChanged.AddListener(delegate { ValidateChoiceTime(); });

        //_buttonConfidence.onClick.AddListener(ReadConfidence);

        _props.SetActive(false);

        //StartTask();
    }
    float distance, angle;
    void ValidateChoiceTime()
    {
        if (isRecorderOn)
        {
            _durationChoice = Time.time - startTime;
            distance = Vector3.Distance(SocialExpManager.Instance.GetTargetFloorPosition(_charactersGroup[randomCombinationListCharacter[currentTrial]].transform.position), SocialExpManager.Instance.GetUserFloorPosition());
            //angle = Vector3.Angle(SocialExpManager.Instance.GetUserForward(), _charactersGroup[randomCombinationListCharacter[currentTrial]].transform.position - SocialExpManager.Instance.GetUserHeadPosition());
            angle = SocialExpManager.Instance.GetUserTargetAngle(_charactersGroup[randomCombinationListCharacter[currentTrial]].transform.position);
            TTS_manager.Instance.ReadConfidenceShort();
            _isTimerOn = false;
            _trialCompleted = true;
            // 
        }
    }
    /*
    void ReadConfidence()
    {
        TTS_manager.Instance.ReadConfidenceShort();
    }*/
    // Update is called once per frame
    void Update()
    {
        if (taskStarted)
        {
            //Update UI
            UpdateUI();
            /*
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                _charactersGroup[randomCombinationListCharacter[currentTrial]].SetActive(false);
                currentTrial++;
                StartTrial();
            }*/
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
        Debug.Log("Task gesture-relationship start");
        _props.SetActive(false);
        SocialExpUIManager.Instance.SetTest("Task gesture-relationship");
        currentTrial = 0;

        taskStarted = true;
        startTime = Time.time;

        _durationChoice = 0.0f;
        _relationOptions.value = 0;

        UpdateUI();
        _UICanvas.SetActive(false);

        writer = new StreamWriter(SocialExpManager.Instance.GetUserPath() + SocialExpManager.Instance.GetUserID() + "_TASK" + SocialExpManager.Instance.GetCurrentTask() + "_SUMMARY.csv", false);
        isRecorderOn = true;
        writer.WriteLine("sep =;");
        writer.WriteLine("UserID;Trial;Value;Duration;Choice;DurationChoice;Confidence;Distance;Angle");
        _isTimerOn = false;
        _trialCompleted = false;

        PositionningUserManager.Instance.NeedRepositioning(this);

    }

    void StartTrial()
    {
        if (currentTrial < nbtrial)
        {
            NoteManager.Instance.WriteNote("Task " + SocialExpManager.Instance.GetCurrentTask() + " Trial" + (currentTrial + 1));
            SocialExpUIManager.Instance.SetTrial(currentTrial + 1, nbtrial);
            _props.SetActive(true);
            Debug.Log("Task gesture-relationship, trial: " + currentTrial);
            Debug.Log("Task randomCombinationListCharacter[currentTrial]: " + randomCombinationListCharacter[currentTrial]);
            _charactersGroup[randomCombinationListCharacter[currentTrial]].SetActive(true);

            startTime = Time.time;

            _durationChoice = 0.0f;
            _relationOptions.value = 0;



            confidenceSlider.value = 0;

            _trialCompleted = false;
            _isTimerOn = false;
            Animator[] anim = _charactersGroup[randomCombinationListCharacter[currentTrial]].GetComponentsInChildren<Animator>();
            //Transform[] tran = new Transform[anim.Length];
            //for (int i = 0; i < tran.Length; i++)
            //{
            //  tran[i] = anim[i].gameObject.transform;
            //}
            //RecorderPositionEyeTracking.Instance.SetMultiTarget(tran);
            Transform[] targets = new Transform[2];
            targets[0] = _charactersGroup[randomCombinationListCharacter[currentTrial]].transform.GetChild(0);
            targets[1] = _charactersGroup[randomCombinationListCharacter[currentTrial]].transform.GetChild(1);
            RecorderPositionEyeTracking.Instance.SetMultiTarget(targets, true);
            RecorderPositionEyeTracking.Instance.StartRecording(SocialExpManager.Instance.GetUserTaskPath(),
                SocialExpManager.Instance.GetUserID() + "_Task" + SocialExpManager.Instance.GetCurrentTask() + "_Trial" + (currentTrial + 1),
                true);
            isRecorderOn = true;
            startTime = Time.time;
            StartCoroutine(StartTimer());
            TTS_manager.Instance.StartSound();

        }
        else
        {
            Debug.Log("Task gesture-relationship completed");
            FinishTask();
        }
    }
 

    void ValidateChoice()
    {
        //Vector3 pos = SocialExpManager.Instance.GetUserFloorPosition();
        //Vector3 rot = SocialExpManager.Instance.GetUserHeadRotationEuler();

        _props.SetActive(false);
        _charactersGroup[randomCombinationListCharacter[currentTrial]].SetActive(false);
        RecorderPositionEyeTracking.Instance.StopRecording();
        _isTimerOn = false;
        _trialCompleted = true;
        isRecorderOn = false;

        Debug.Log("Real: " + (randomCombinationListCharacter[currentTrial] + 1) + " Choice: " + _relationOptions.value);

        writer.WriteLine(SocialExpManager.Instance.GetUserID() + ";" + currentTrial + ";"
       + (randomCombinationListCharacter[currentTrial] + 1) + ";"
       + (Time.time - startTime) + ";" + _relationOptions.value + ";" + _durationChoice + ";" + confidenceSlider.value + ";" + distance + ";" +angle);

     


        confidenceSlider.value = 0;
       

        _durationChoice = 0.0f;
        _relationOptions.value = 0;

        _UICanvas.SetActive(false);

        currentTrial++;

        if (currentTrial < nbtrial)
        {

            PositionningUserManager.Instance.NeedRepositioning(this);
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

        Debug.Log("Task gesture-relationship completed");
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
