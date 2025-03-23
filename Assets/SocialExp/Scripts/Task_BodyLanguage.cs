using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using System.IO;


public class Task_BodyLanguage : MonoBehaviour
{
    [SerializeField]
    private Transform[] _characters;

    [SerializeField]
    private GameObject _props;

    [SerializeField] int currentTrial = 0;

    private List<int> combinationList = new List<int>();
    public List<List<int>> listCharacterLookAt = new List<List<int>>();
    public List<int> randomCombinationList = new List<int>();

    int nbtrial = 3;



    [SerializeField]
    private TransformFollow[] _charactersHeadTargets;


    [SerializeField]
    private Transform[] _charactersHeadPos;

    [SerializeField]
    private Transform[] _charactersHeadTargetAway;

    [SerializeField]
    private Transform _userHead;


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
    private TMP_Dropdown _roleOptions;


    // Start is called before the first frame update
    void Start()
    {
       // combinationList = new List<int>() { 0, 1, 2, 3 };
        combinationList = new List<int>() { 0, 1, 2 };     
        randomCombinationList = UtilityRandom.ShuffleList(combinationList);

        //Look at values: 5 -> look away, 4 look at user
        listCharacterLookAt.Add(new List<int>() { 1, 0, 4 }); //Look at doctor/secretary
        listCharacterLookAt.Add(new List<int>() { 2, 2, 1 }); //Look at injured
        listCharacterLookAt.Add(new List<int>() { 3, 3, 4}); //Look at User
        
        
      //  listCharacterLookAt.Add(new List<int>() { 3, 3, 5, 1 }); //Look at women

        _props.SetActive(false);
        for (int i = 0; i < _characters.Length; i++)
        {
            _characters[i].gameObject.SetActive(false);
        }
        _props.SetActive(false);
        // StartTask();

        _UICanvas.SetActive(false);
        _buttonValideForced.onClick.AddListener(ValidateChoice);

        List<string> options = new List<string> {
            "Pas de réponse",
            "Secrétaire",
            "Patient",
        "Utilisateur"};
        _roleOptions.AddOptions(options);
        _roleOptions.onValueChanged.AddListener(delegate { ValidateChoiceTime(); });


    }
    float distance, angle;

    void ValidateChoiceTime()
    {
        if (isRecorderOn)
        {
            _durationChoice = Time.time - startTime;
            distance = Vector3.Distance(SocialExpManager.Instance.GetTargetFloorPosition(_characters[1].transform.position), SocialExpManager.Instance.GetUserFloorPosition());
           // angle = Vector3.Angle(SocialExpManager.Instance.GetUserForward(), _characters[1].transform.position - SocialExpManager.Instance.GetUserHeadPosition());
            angle = SocialExpManager.Instance.GetUserTargetAngle(_characters[1].transform.position);
            TTS_manager.Instance.ReadConfidenceShort();
            _isTimerOn = false;
            _trialCompleted = true;
        }
    }

    
    void Update()
    {
        if (taskStarted)
        {
            //Update UI
            UpdateUI();
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                //_characters[listCharacterLookAt[currentTrial]].SetActive(false);
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
        _UICanvas.SetActive(true);
        
        UpdateUI();
        StartTrial();
    }

    public void StartTask()
    {
        Debug.Log("Task body language start");
        _userHead = SocialExpManager.Instance.GetUserHeadTransform();
        SocialExpUIManager.Instance.SetTest("Task body language");
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
    }
    void ValidateChoice()
    {
        //Vector3 pos = SocialExpManager.Instance.GetUserFloorPosition();
        //Vector3 rot = SocialExpManager.Instance.GetUserHeadRotationEuler();


       // _characters[randomCombinationListCharacter[currentTrial]].SetActive(false);
        RecorderPositionEyeTracking.Instance.StopRecording();
        _isTimerOn = false;
        _trialCompleted = true;
        isRecorderOn = false;

        Debug.Log("Real: " + (randomCombinationList[currentTrial] + 1) + " Choice: " + _roleOptions.value);

        writer.WriteLine(SocialExpManager.Instance.GetUserID() + ";" + currentTrial + ";" + (randomCombinationList[currentTrial]+1) + ";"+ 
            (Time.time - startTime) + ";" + _roleOptions.value + ";" + _durationChoice + ";" + confidenceSlider.value + ";" + distance + ";" +angle);


        


        confidenceSlider.value = 0;
        _roleOptions.value = 0;
        _durationChoice = 0.0f;

        _UICanvas.SetActive(false);

        _props.SetActive(false);
        for (int i = 0; i < _characters.Length; i++)
        {
            _characters[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < _charactersHeadTargets.Length; i++)
        {
            _charactersHeadTargets[i].StopFollow();
        }


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
    void StartTrial()
    {
        if (currentTrial < nbtrial)
        {
            NoteManager.Instance.WriteNote("Task " + SocialExpManager.Instance.GetCurrentTask() + " Trial" + (currentTrial + 1));
            _props.SetActive(true);
            for(int i = 0; i < _characters.Length; i++)
            {
                _characters[i].gameObject.SetActive(true);
            }

            startTime = Time.time;

            confidenceSlider.value = 0;

            _trialCompleted = false;
            _isTimerOn = false;
           
            RecorderPositionEyeTracking.Instance.SetMultiTarget(_characters);

            RecorderPositionEyeTracking.Instance.StartRecording(SocialExpManager.Instance.GetUserTaskPath(),
            SocialExpManager.Instance.GetUserID() + "_Task" + SocialExpManager.Instance.GetCurrentTask() + "_Trial" + (currentTrial + 1),
                true);
            startTime = Time.time;
            isRecorderOn = true;
            StartCoroutine(StartTimer());
            UpdateUI();


            TTS_manager.Instance.StartSound();


            Debug.Log("Task body language, trial: " + currentTrial);
            SocialExpUIManager.Instance.SetTrial(currentTrial+1, nbtrial);
            for (int i = 0; i < _characters.Length; i++)
            {
                Debug.Log("Character: " + i +" : "+ _characters[i].name + " look at : " + listCharacterLookAt[randomCombinationList[currentTrial]][i]);
                if (listCharacterLookAt[randomCombinationList[currentTrial]][i] == 3)
                {
                    //Look at user
                    _charactersHeadTargets[i].SetTargetAndFollow(_userHead);
                }
                else if (listCharacterLookAt[randomCombinationList[currentTrial]][i] == 4)
                {
                    //Look away
                    _charactersHeadTargets[i].SetTargetAndFollow(_charactersHeadTargetAway[i]);
                }
                else
                {
                    //General case
                    _charactersHeadTargets[i].SetTargetAndFollow(_charactersHeadPos[listCharacterLookAt[randomCombinationList[currentTrial]][i]]);

                }
                //charactersHeadPos[i].transform.position = 

            }
            

           // _characters[randomCombinationListCharacter[currentTrial]].GetComponent<Animator>().SetInteger("ActionGesture", currentTrial);

        }
        else
        {
          
            FinishTask();

        }
    }
    void FinishTask()
    {

        
        _props.SetActive(false);

        for (int i = 0; i < _characters.Length; i++)
        {
            _characters[i].gameObject.SetActive(false);
        }

        Debug.Log("Task body appearance completed");
        writer.Close();
        isRecorderOn = false;
        SocialExpManager.Instance.TaskDone();

    }
    public void ForceStopTask()
    {
        _props.SetActive(false);

        for (int i = 0; i < _characters.Length; i++)
        {
            _characters[i].gameObject.SetActive(false);
        }

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
        float limithalfTimer = Time.time + _durationTimer /2.0f;

        while (Time.time < _limitTimer && _isTimerOn)
        {
            if(!hasRing && Time.time > limithalfTimer)
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
