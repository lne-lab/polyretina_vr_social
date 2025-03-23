using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class Task_BodyAppearance : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _characters;
    [SerializeField]
    private GameObject _props;

    [SerializeField] int currentTrial = 0;

    private List<int> combinationList = new List<int>();
    public List<int> randomCombinationListCharacter = new List<int>();
    int nbtrial = 4;

    bool taskStarted = false;
    [SerializeField]
    private GameObject _UICanvas;

    float startTime, startQuestion;

    [SerializeField]
    private TMP_Text _time;

  

    [SerializeField]
    private TMP_Dropdown _featureOption1, _featureOption2, _featureOption3;

    [SerializeField]
    private Button _buttonValideForced;
    [SerializeField]
    private Button _buttonFeature1, _buttonFeature2, _buttonFeature3;

    [SerializeField]
    private Slider confidenceSlider1, confidenceSlider2, confidenceSlider3;

    [SerializeField] public int[] _ageValues;
    [SerializeField] public int[] _genderValues;
    [SerializeField] public int[] _accessoriesValues;
    

    private StreamWriter writer;
    bool isRecorderOn = false;
    float _durationTimer = 150.0f;
    bool _isTimerOn = false;
    bool _trialCompleted = false;
    float _limitTimer;
    float[] _answersTime;
    float[] _angle, _distance;
    int subCategory = 0;

    // Start is called before the first frame update
    void Start()
    {
        //0: Female adult bag
        //1: Medical old stethoscope
        //2: Male adult black scarf
        //3: Male adult asian cap

        _props.SetActive(false);

        combinationList = new List<int>() { 0, 1, 2, 3 };
        randomCombinationListCharacter = UtilityRandom.ShuffleList(combinationList);
       

        _UICanvas.SetActive(false);
        //_buttonValideForced.onClick.AddListener(ValidateChoice);

        _answersTime = new float[3];
        _angle = new float[3];
        _distance = new float[3];

        List<string>  options = new List<string> {
            "Pas de réponse",
            "Male",
            "Female"};
        _featureOption1.AddOptions(options);
        _featureOption1.onValueChanged.AddListener(delegate { ValidateFeatureOptionTime(0); });

        options = new List<string> {
            "Pas de réponse",
            "Jeune",
            "Adulte",
            "Agé"};
        _featureOption2.AddOptions(options);
        _featureOption2.onValueChanged.AddListener(delegate { ValidateFeatureOptionTime(1); });

       
        
        

        options = new List<string> {
            "Pas de réponse",
            "Stéthoscope",
            "Sac à main",
            "Bonnet",
            "Echarpe",
            "Autre"};
        _featureOption3.AddOptions(options);
        _featureOption3.onValueChanged.AddListener(delegate { ValidateFeatureOptionTime(2); });

        confidenceSlider1.value = 0;
        confidenceSlider2.value = 0;
        confidenceSlider3.value = 0;

        _buttonFeature1.onClick.AddListener(delegate { ValidateFeatureOption(0); });
        _buttonFeature2.onClick.AddListener(delegate { ValidateFeatureOption(1); });
        _buttonFeature3.onClick.AddListener(delegate { ValidateChoice(); });

        //_featureOption1.onValueChanged.AddListener(delegate { ValidateFeatureOptionTime(3); });
        //  buttonFeature1.On
        //  _buttonFeature2, _buttonFeature3, _buttonFeatureOther;

        //   StartTask();

    }

    void Update()
    {
        if (taskStarted)
        {
            //Update UI
            UpdateUI();
            /*
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                _characters[randomCombinationListCharacter[currentTrial]].SetActive(false);
                currentTrial++;
                StartTrial();
            }*/
        }
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
        Debug.Log("Task body appearance start");

        SocialExpUIManager.Instance.SetTest("Task body appearance");
        currentTrial = 0;
        taskStarted = true;
        startTime = Time.time;
        subCategory = 0;

        UpdateUI();
        _UICanvas.SetActive(false);

        writer = new StreamWriter(SocialExpManager.Instance.GetUserPath() + SocialExpManager.Instance.GetUserID() + "_TASK" + SocialExpManager.Instance.GetCurrentTask() + "_SUMMARY.csv", false);
        isRecorderOn = true;
        writer.WriteLine("sep =;");
        writer.WriteLine("UserID;Trial;IndexCharacter;ValueAge;ValueGender;ValueAccessory;Duration;ChoiceAge;TimeChoiceAge;AngleChoiceAge;DistanceChoiceAge;ConfidenceAge;ChoiceGender;TimeChoiceGender;AngleChoiceGender;DistanceChoiceGender;ConfidenceGender;ChoiceAccessory;TimeChoiceAccessory;AngleChoiceAccessory;DistanceChoiceAccessory;ConfidenceAccessory");
        _isTimerOn = false;
        _trialCompleted = false;
        PositionningUserManager.Instance.NeedRepositioning(this, DirectionPositioning.Center);

    }
    public void PositioningCompleted()
    {

        Debug.Log("User in position!");
        taskStarted = true;
        startTime = Time.time;
        startQuestion = Time.time;
        _UICanvas.SetActive(true);
        UpdateUI();
        StartTrial();
    }

    void ValidateFeatureTime(int indexFeature)
    {
        if (isRecorderOn)
        {
            Debug.Log("Validate: " + indexFeature);
            _answersTime[indexFeature] = Time.time - startQuestion;
            _angle[indexFeature] = SocialExpManager.Instance.GetUserTargetAngle(_characters[randomCombinationListCharacter[currentTrial]].transform.position);
            _distance[indexFeature] = Vector3.Distance(SocialExpManager.Instance.GetTargetFloorPosition(_characters[randomCombinationListCharacter[currentTrial]].transform.position), SocialExpManager.Instance.GetUserFloorPosition());
        }
    }

    void ValidateFeatureOptionTime(int indexFeature)
    {
        if (isRecorderOn)
        {
            Debug.Log("Validate option: " + indexFeature);
            _answersTime[indexFeature] = Time.time - startQuestion;
            _angle[indexFeature] = SocialExpManager.Instance.GetUserTargetAngle(_characters[randomCombinationListCharacter[currentTrial]].transform.position);
            _distance[indexFeature] = Vector3.Distance(SocialExpManager.Instance.GetTargetFloorPosition(_characters[randomCombinationListCharacter[currentTrial]].transform.position), SocialExpManager.Instance.GetUserFloorPosition());
            TTS_manager.Instance.ReadConfidenceShort();
        }
    }

    void ValidateFeatureOption(int indexFeature)
    {
        if (isRecorderOn)
        {
            Debug.Log("Validate feature: " + indexFeature);
            startQuestion = Time.time;
            if (indexFeature == 0)
            {
               
                TTS_manager.Instance.BodyAppearanceAge();
            }else if (indexFeature == 1)
            {
                TTS_manager.Instance.BodyAppearanceAccessory();
            }
            
            
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


    void ValidateChoice()
    {
        //Vector3 pos = SocialExpManager.Instance.GetUserFloorPosition();
        //Vector3 rot = SocialExpManager.Instance.GetUserHeadRotationEuler();

        //float distance = Vector3.Distance(SocialExpManager.Instance.GetTargetFloorPosition(_characters[randomCombinationListCharacter[currentTrial]].transform.position), SocialExpManager.Instance.GetUserFloorPosition());
        //float angle = 
        // Vector3.Angle(SocialExpManager.Instance.GetUserForward(), _characters[randomCombinationListCharacter[currentTrial]].transform.position - SocialExpManager.Instance.GetUserHeadPosition());

        Debug.Log("Validate choice: ");
        _props.SetActive(false);
        _characters[randomCombinationListCharacter[currentTrial]].SetActive(false);
        RecorderPositionEyeTracking.Instance.StopRecording();
        _isTimerOn = false;
        _trialCompleted = true;
        isRecorderOn = false;

        //"UserID;Trial;IndexCharacter;ValueAge;ValueGender;ValueAccessory;Duration;ChoiceAge;TimeChoiceAge;ConfidenceAge;ChoiceGender;TimeChoiceGender;ConfidenceGender;ChoiceAccessory;TimeChoiceAccessory;ConfidenceAccessory"
        writer.WriteLine(SocialExpManager.Instance.GetUserID() + ";" + currentTrial + ";" +
            randomCombinationListCharacter[currentTrial] + ";" +
            _ageValues[randomCombinationListCharacter[currentTrial]] + ";" +
            _genderValues[randomCombinationListCharacter[currentTrial]] + ";" +
            _accessoriesValues[randomCombinationListCharacter[currentTrial]] + ";" +
            (Time.time - startTime) + ";" +
            _featureOption1.value + ";"+
            _answersTime[0] +";" +
            _angle[0] + ";" +
            _distance[0] + ";" +
            confidenceSlider1.value + ";" +
            _featureOption2.value + ";" +
            _answersTime[1] + ";" +
             _angle[1] + ";" +
            _distance[1] + ";" +
             confidenceSlider2.value + ";" +
            _featureOption3.value + ";" +
            _answersTime[2] + ";" +
             _angle[2] + ";" +
            _distance[2] + ";" +
             confidenceSlider3.value
         
            );


        _featureOption1.value =0;
        _featureOption2.value = 0;
        _featureOption3.value = 0;
  

        confidenceSlider1.value = 0;
        confidenceSlider2.value = 0;
        confidenceSlider3.value = 0;

        _answersTime = new float[3]; 

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
    void FinishTask()
    {

        _props.SetActive(false);

        taskStarted = false;
        _UICanvas.SetActive(false);

        Debug.Log("Task body appearance completed");
        writer.Close();
        isRecorderOn = false;
        SocialExpManager.Instance.TaskDone();

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

    void StartTrial()
    {
        if (currentTrial < nbtrial)
        {
            NoteManager.Instance.WriteNote("Task " + SocialExpManager.Instance.GetCurrentTask() + " Trial" + (currentTrial + 1));
            _props.SetActive(true);
            startTime = Time.time;

            _featureOption1.value = 0;
            _featureOption2.value = 0;
            _featureOption3.value = 0;
    

            confidenceSlider1.value = 0;
            confidenceSlider2.value = 0;
            confidenceSlider3.value = 0;
   

            _trialCompleted = false;
            _isTimerOn = false;
            RecorderPositionEyeTracking.Instance.SetSingleTarget(_characters[randomCombinationListCharacter[currentTrial]].transform);
            RecorderPositionEyeTracking.Instance.StartRecording(SocialExpManager.Instance.GetUserTaskPath(),
                SocialExpManager.Instance.GetUserID() + "_Task" + SocialExpManager.Instance.GetCurrentTask() + "_Trial" + (currentTrial + 1),
                true);
            startTime = Time.time;
            isRecorderOn = true;

            StartCoroutine(StartTimer());

            TTS_manager.Instance.StartSound();

            UpdateUI();

            subCategory = 0;
            TTS_manager.Instance.BodyAppearanceGender();

            SocialExpUIManager.Instance.SetTrial(currentTrial + 1, nbtrial);
            Debug.Log("Task body appearance, trial: " + currentTrial + " index: " + randomCombinationListCharacter[currentTrial]);
            _characters[randomCombinationListCharacter[currentTrial]].SetActive(true);
            
        }
        else
        {
            Debug.Log("Task body appearance completed");
            SocialExpManager.Instance.TaskDone();
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
