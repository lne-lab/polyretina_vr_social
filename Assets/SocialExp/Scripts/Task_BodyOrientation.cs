using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using System.IO;
using static UnityEditor.Recorder.OutputPath;


public class Task_BodyOrientation : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _characters;

    [SerializeField]
    private GameObject _props;

    [SerializeField] int currentTrial = 0;

    private List<int> combinationList = new List<int>();
    public  List<int> randomCombinationListAction = new List<int>();
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



    float _durationChoice = 0.0f;

    [SerializeField]
    private TMP_Dropdown _orientationOptions;

    // Start is called before the first frame update
    void Start()
    {
        _props.SetActive(false);


        combinationList = new List<int>() { 0, 1, 2, 3 };
        randomCombinationListAction = UtilityRandom.ShuffleList(combinationList);

        combinationList = new List<int>() { 3, 2, 1, 0 };
        randomCombinationListCharacter = UtilityRandom.ShuffleList(combinationList);
      
        
        _UICanvas.SetActive(false);
        _buttonValideForced.onClick.AddListener(ValidateChoice);



        List<string> options = new List<string> {
            "Pas de réponse",
            "Devant",
            "Droite",
            "Gauche",
            "Arrière"};
        _orientationOptions.AddOptions(options);
        _orientationOptions.onValueChanged.AddListener(delegate { ValidateChoiceTime(); });


        //   StartTask();

    }
    float distance, angle;
    void ValidateChoiceTime()
    {
         distance = Vector3.Distance(SocialExpManager.Instance.GetTargetFloorPosition(_characters[randomCombinationListCharacter[currentTrial]].transform.position), SocialExpManager.Instance.GetUserFloorPosition());
        angle = SocialExpManager.Instance.GetUserTargetAngle(_characters[randomCombinationListCharacter[currentTrial]].transform.position);//Vector3.Angle(SocialExpManager.Instance.GetUserForward(), _characters[randomCombinationListCharacter[currentTrial]].transform.position - SocialExpManager.Instance.GetUserHeadPosition());

        if (isRecorderOn)
        {
            _durationChoice = Time.time - startTime;
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
 *          if (Input.GetKeyDown(KeyCode.Keypad2))
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

    public void StartTask()
    {
        Debug.Log("Task body orientation start");
        _props.SetActive(false);
        SocialExpUIManager.Instance.SetTest("Task body orientation");
        currentTrial = 0;
        taskStarted = true;
        startTime = Time.time;
        _durationChoice = 0.0f;
        _orientationOptions.value = 0;
        
        UpdateUI();
        _UICanvas.SetActive(false);

        writer = new StreamWriter(SocialExpManager.Instance.GetUserPath() + SocialExpManager.Instance.GetUserID() + "_TASK" + SocialExpManager.Instance.GetCurrentTask() + "_SUMMARY.csv", false);
       
        writer.WriteLine("sep =;");
        writer.WriteLine("UserID;Trial;Value;Duration;Choice;DurationChoice;Confidence;Distance;Angle");
        _isTimerOn = false;
        _trialCompleted = false;

        PositionningUserManager.Instance.NeedRepositioning(this, DirectionPositioning.Center);

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


    


    void ValidateChoice()
    {
       

        //Vector3 pos = SocialExpManager.Instance.GetUserFloorPosition();
        //Vector3 rot = SocialExpManager.Instance.GetUserHeadRotationEuler();


        _characters[randomCombinationListCharacter[currentTrial]].SetActive(false);
 RecorderPositionEyeTracking.Instance.StopRecording();

        isRecorderOn = false;

        Debug.Log("Real: " + (randomCombinationListAction[currentTrial] + 1) + " Choice: " + _orientationOptions.value);

        writer.WriteLine(SocialExpManager.Instance.GetUserID() + ";" + currentTrial + ";"
          + (randomCombinationListAction[currentTrial] + 1) + ";"
          + (Time.time - startTime) + ";" + _orientationOptions.value + ";" + _durationChoice + ";" + confidenceSlider.value + ";" + distance + ";" + angle);

      
       

        confidenceSlider.value = 0;
        _orientationOptions.value = 0;
        _durationChoice = 0.0f;
        _UICanvas.SetActive(false);

        currentTrial++;

        if (currentTrial < nbtrial)
        {
            
            PositionningUserManager.Instance.NeedRepositioning(this, DirectionPositioning.Center);
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

    void StartTrial()
    {
        if (currentTrial < nbtrial)
        {
            NoteManager.Instance.WriteNote("Task " + SocialExpManager.Instance.GetCurrentTask() + " Trial" + (currentTrial + 1));
            startTime = Time.time;
            _props.SetActive(true);
            _orientationOptions.value = 0;
            confidenceSlider.value = 0;
            _durationChoice = 0.0f;
            _trialCompleted = false;
            _isTimerOn = false;
            RecorderPositionEyeTracking.Instance.SetSingleTarget(_characters[randomCombinationListCharacter[currentTrial]].transform);
            RecorderPositionEyeTracking.Instance.StartRecording(SocialExpManager.Instance.GetUserTaskPath(),
                SocialExpManager.Instance.GetUserID() + "_Task" + SocialExpManager.Instance.GetCurrentTask() + "_Trial" + (currentTrial+1),
                true);
            isRecorderOn = true;
            startTime = Time.time;
            StartCoroutine(StartTimer());
            TTS_manager.Instance.StartSound();
            UpdateUI();


            SocialExpUIManager.Instance.SetTrial(currentTrial + 1, nbtrial);
            Debug.Log("Task body orientation, trial: " + currentTrial+" index: "+ randomCombinationListCharacter[currentTrial]);
            _characters[randomCombinationListCharacter[currentTrial]].SetActive(true);
            _characters[randomCombinationListCharacter[currentTrial]].GetComponent<Animator>().SetInteger("ActionTurn", randomCombinationListAction[currentTrial]);

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

        Debug.Log("Task body orientation completed");
        writer.Close();
        isRecorderOn = false;
        SocialExpManager.Instance.TaskDone();

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
