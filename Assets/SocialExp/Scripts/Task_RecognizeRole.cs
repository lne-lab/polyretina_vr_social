using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using RootMotion.FinalIK;

public class Task_RecognizeRole : MonoBehaviour
{
    [SerializeField]
    private Transform[] _characters;

    [SerializeField]
    private Transform[] _positions;

    [SerializeField]
    private GameObject _props;

    [SerializeField] int currentTrial = 0;

    private List<int> combinationList = new List<int>();
    public List<int> randomCombinationListCharacter = new List<int>();
    int nbtrial = 3;

    bool taskStarted = false;
    [SerializeField]
    private GameObject _UICanvas;

    float startTime;

    [SerializeField]
    private TMP_Text _time;

    [SerializeField]
    private Button _buttonValideForced;

    private StreamWriter writer;
    bool isRecorderOn = false;
    float _durationTimer = 150.0f;
    bool _isTimerOn = false;
    bool _trialCompleted = false;
    float _limitTimer;

    
    [SerializeField]
    private Slider confidenceSlider;

    float _durationChoice = 0.0f;

    [SerializeField]
    private TMP_Dropdown _roleOptions;

    [SerializeField]
    private float timeToWaitForController = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        //0: doctor front
        //1: doctor right
        //2: doctor left


        //0: front
        //1: right
        //2: left
        _props.SetActive(false);

        combinationList = new List<int>() { 0, 1, 2};
        randomCombinationListCharacter = UtilityRandom.ShuffleList(combinationList);

        _UICanvas.SetActive(false);
        _buttonValideForced.onClick.AddListener(ValidateChoice);

        List<string> options = new List<string> {
            "Pas de réponse",
            "Droite",
            "Gauche",
         "Devant" };
        _roleOptions.AddOptions(options);
        _roleOptions.onValueChanged.AddListener(delegate { ValidateChoiceTime(); });


        //   StartTask();

    }
    float distance, angle;
    void ValidateChoiceTime()
    {
        if (isRecorderOn)
        {
            _durationChoice = Time.time - startTime;
            distance = Vector3.Distance(SocialExpManager.Instance.GetTargetFloorPosition(_characters[2].transform.position), SocialExpManager.Instance.GetUserFloorPosition());
            angle = SocialExpManager.Instance.GetUserTargetAngle(_characters[2].transform.position);
            //angle = Vector3.Angle(SocialExpManager.Instance.GetUserForward(), _characters[2].transform.position - SocialExpManager.Instance.GetUserHeadPosition());


            TTS_manager.Instance.ReadConfidenceShort();
            _isTimerOn = false;
            _trialCompleted = true;

        }
    }

    void Update()
    {
        if (taskStarted)
        {
            UpdateUI();
            /*
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                _characters[randomCombinationListCharacter[currentTrial]].gameObject.SetActive(false);
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

    public void StartTask()
    {
        Debug.Log("Task role start");
        _props.SetActive(false);
        SocialExpUIManager.Instance.SetTest("Task role");
        currentTrial = 0;

        startTime = Time.time;
        _durationChoice = 0.0f;

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
    void FinishTask()
    {
        _props.SetActive(false);
        taskStarted = false;
        _UICanvas.SetActive(false);

        Debug.Log("Task body orientation completed");
        writer.Close();
        isRecorderOn = false;
        SocialExpManager.Instance.TaskDone();

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
    void StartTrial()
    {
        if (currentTrial < nbtrial)
        {
            NoteManager.Instance.WriteNote("Task " + SocialExpManager.Instance.GetCurrentTask() + " Trial" + (currentTrial + 1));
            SocialExpUIManager.Instance.SetTrial(currentTrial + 1, nbtrial);

            startTime = Time.time;
            
            confidenceSlider.value = 0;
            _durationChoice = 0.0f;
            _props.SetActive(true);
            _trialCompleted = false;
            _isTimerOn = false;
            RecorderPositionEyeTracking.Instance.SetMultiTarget(_characters);
            RecorderPositionEyeTracking.Instance.StartRecording(SocialExpManager.Instance.GetUserTaskPath(),
                SocialExpManager.Instance.GetUserID() + "_Task" + SocialExpManager.Instance.GetCurrentTask() + "_Trial" + (currentTrial + 1),
                true);
            isRecorderOn = true;
            startTime = Time.time;

            StartCoroutine(StartTimer());

            TTS_manager.Instance.StartSound();

            UpdateUI();

            Debug.Log("Task body appearance, trial: " + currentTrial);
            switch (randomCombinationListCharacter[currentTrial])
            {
                case 0:
                    //doctor front, patient left, secretary right
                    _characters[0].position = _positions[0].position;
                    _characters[0].rotation = _positions[0].rotation;

                    _characters[1].position = _positions[2].position;
                    _characters[1].rotation = _positions[2].rotation;

                    _characters[2].position = _positions[1].position;
                    _characters[2].rotation = _positions[1].rotation;
                   
                    break;
                case 1:
                    //doctor right, patient front, secretary left
                    _characters[0].position = _positions[1].position;
                    _characters[0].rotation = _positions[1].rotation;

                    _characters[1].position = _positions[0].position;
                    _characters[1].rotation = _positions[0].rotation;

                    _characters[2].position = _positions[2].position;
                    _characters[2].rotation = _positions[2].rotation;
                    break;
                case 2:
                    //doctor left, patient right, secretary left
                    _characters[0].position = _positions[2].position;
                    _characters[0].rotation = _positions[2].rotation;

                    _characters[1].position = _positions[1].position;
                    _characters[1].rotation = _positions[1].rotation;

                    _characters[2].position = _positions[0].position;
                    _characters[2].rotation = _positions[0].rotation;

                    break;
               


            }
            _characters[0].gameObject.SetActive(true);
            _characters[1].gameObject.SetActive(true);
            _characters[2].gameObject.SetActive(true);

            _characters[0].GetComponent<Animator>().SetInteger("ActionGesture", currentTrial);
            _characters[1].GetComponent<Animator>().SetInteger("ActionGesture", currentTrial);
            _characters[2].GetComponent<Animator>().SetInteger("ActionGesture", currentTrial);
            StartCoroutine(WaitForControllerActivation());
            //_characters[randomCombinationListCharacter[currentTrial]].SetActive(true);
            //_characters[randomCombinationListCharacter[currentTrial]].GetComponent<Animator>().SetInteger("ActionGesture", currentTrial);
          
        }
        else
        {
            Debug.Log("Task body appearance completed");
            SocialExpManager.Instance.TaskDone();
        }
    }

    IEnumerator WaitForControllerActivation()
    {
        yield return new WaitForSeconds(timeToWaitForController);

        _characters[0].GetComponent<LookAtController>().enabled = true;
        _characters[1].GetComponent<LookAtController>().enabled = true;
        _characters[2].GetComponent<LookAtController>().enabled = true;


    }

    void ValidateChoice()
    {
        //Vector3 pos = SocialExpManager.Instance.GetUserFloorPosition();
        //Vector3 rot = SocialExpManager.Instance.GetUserHeadRotationEuler();
        _props.SetActive(false);
        _characters[0].GetComponent<LookAtController>().enabled = false;
        _characters[1].GetComponent<LookAtController>().enabled = false;
        _characters[2].GetComponent<LookAtController>().enabled = false;


        _characters[0].gameObject.SetActive(false);
        _characters[1].gameObject.SetActive(false);
        _characters[2].gameObject.SetActive(false);

       
        // _characters[randomCombinationListCharacter[currentTrial]].SetActive(false);
        RecorderPositionEyeTracking.Instance.StopRecording();
        _isTimerOn = false;
        isRecorderOn = false;
        _trialCompleted = true;
        Debug.Log("Real: " + (randomCombinationListCharacter[currentTrial] + 1) + " Choice: " + _roleOptions.value);


        writer.WriteLine(SocialExpManager.Instance.GetUserID() + ";" + currentTrial + ";" +
            (randomCombinationListCharacter[currentTrial] + 1) + ";" +
            (Time.time - startTime) + ";" + _roleOptions.value + ";" + _durationChoice + ";" + confidenceSlider.value+ ";" + distance + ";" + angle);


        confidenceSlider.value = 0;

        _roleOptions.value = 0;
        _durationChoice = 0.0f;

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

    public void ForceStopTask()
    {
        _props.SetActive(false);
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
    void OnApplicationQuit()
    {
        if (isRecorderOn)
        {
            writer.WriteLine("stop;exp;;" + System.DateTime.Now.Day + ";" + System.DateTime.Now.Month + ";" + System.DateTime.Now.Hour + ";" + System.DateTime.Now.Minute);
            writer.WriteLine("EXIT;ERROR");
            writer.Close();
        }
    }

}
