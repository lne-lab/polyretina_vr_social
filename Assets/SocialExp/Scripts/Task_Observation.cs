using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using System.IO;

public class Task_Observation : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _characters;

    [SerializeField]
    private GameObject _props;

    [SerializeField] int currentTrial = 0;
    int nbtrial = 1;

    bool taskStarted = false;
    [SerializeField]
    private GameObject _UICanvas;

    float startTime;

    [SerializeField]
    private TMP_Text _time;

    [SerializeField]
    private TMP_Dropdown[] _responsCharacter0, _responsCharacter1, _responsCharacter2;

    [SerializeField]
    private Button _buttonValideForced;
  
    private StreamWriter writer;
    bool isRecorderOn = false;
    float _durationTimer = 240.0f;
    bool _isTimerOn = false;
    bool _trialCompleted = false;
    float _limitTimer;
    float[] _answersTime0, _answersTime1, _answersTime2;

    [SerializeField]
    private int[] _dataCharacter0, _dataCharacter1, _dataCharacter2;


    [SerializeField]
    private GameObject hospitalSound;

    [SerializeField]
    private TMP_InputField nbPeople;

    // Start is called before the first frame update
    void Start()
    {
        _props.SetActive(false);

        _UICanvas.SetActive(false);
        _buttonValideForced.onClick.AddListener(ValidateChoice);
        _answersTime0 = new float[3];
        _answersTime1 = new float[3];
        _answersTime2 = new float[3];

      for(int i = 0 ; i < _responsCharacter0.Length; i++)
        {
            _responsCharacter0[i].ClearOptions();
            _responsCharacter1[i].ClearOptions();
            _responsCharacter2[i].ClearOptions();
        }

        List<string> options = new List<string> {
            "Pas de réponse",
            "Homme",
            "Femme"};

        _responsCharacter0[0].AddOptions(options);
        _responsCharacter0[0].onValueChanged.AddListener(delegate { ValidateFeatureTime(0, 0); });

        _responsCharacter1[0].AddOptions(options);
        _responsCharacter1[0].onValueChanged.AddListener(delegate { ValidateFeatureTime(1, 0); });

        _responsCharacter2[0].AddOptions(options);
        _responsCharacter2[0].onValueChanged.AddListener(delegate { ValidateFeatureTime(2, 0); });

        options = new List<string> {
            "Pas de réponse",
            "Enfant",
            "Adulte",
            "Agé"};
        _responsCharacter0[1].AddOptions(options);
        _responsCharacter0[1].onValueChanged.AddListener(delegate { ValidateFeatureTime(0, 1); });

        _responsCharacter1[1].AddOptions(options);
        _responsCharacter1[1].onValueChanged.AddListener(delegate { ValidateFeatureTime(1, 1); });

        _responsCharacter2[1].AddOptions(options);
        _responsCharacter2[1].onValueChanged.AddListener(delegate { ValidateFeatureTime(2, 1); });

        options = new List<string> {
            "Pas de réponse",
            "Debout",
            "Assis",
            "Autre"};
        _responsCharacter0[2].AddOptions(options);
        _responsCharacter0[2].onValueChanged.AddListener(delegate { ValidateFeatureTime(0, 2); });

        _responsCharacter1[2].AddOptions(options);
        _responsCharacter1[2].onValueChanged.AddListener(delegate { ValidateFeatureTime(1, 2); });

        _responsCharacter2[2].AddOptions(options);
        _responsCharacter2[2].onValueChanged.AddListener(delegate { ValidateFeatureTime(2, 2); });
        /*
        options = new List<string> {
            "Pas de réponse",
            "Au téléphone",
            "Mange",
            "Fume",
            "Autre"};
        _responsCharacter0[3].AddOptions(options);
        _responsCharacter0[3].onValueChanged.AddListener(delegate { ValidateFeatureTime(3, 0); });

        _responsCharacter1[3].AddOptions(options);
        _responsCharacter1[3].onValueChanged.AddListener(delegate { ValidateFeatureTime(3, 1); });

        _responsCharacter2[3].AddOptions(options);
        _responsCharacter2[3].onValueChanged.AddListener(delegate { ValidateFeatureTime(3, 2); });

        options = new List<string> {
            "Pas de réponse",
            "Chapeau",
            "Sac",
            "Rien",
            "Autre"};
        _responsCharacter0[4].AddOptions(options);
        _responsCharacter0[4].onValueChanged.AddListener(delegate { ValidateFeatureTime(4, 0); });

        _responsCharacter1[4].AddOptions(options);
        _responsCharacter1[4].onValueChanged.AddListener(delegate { ValidateFeatureTime(4, 1); });

        _responsCharacter2[4].AddOptions(options);
        _responsCharacter2[4].onValueChanged.AddListener(delegate { ValidateFeatureTime(4, 2); });
        */
        nbPeople.text = "NaN";
    }

    void Update()
    {
        if (taskStarted)
        {
            //Update UI
            UpdateUI();
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                currentTrial++;
                StartTrial();
            }
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
        Debug.Log("Task observation start");
        hospitalSound.SetActive(false);

        _props.SetActive(true);
        SocialExpUIManager.Instance.SetTest("Task observation");
        currentTrial = 0;
        taskStarted = true;
        startTime = Time.time;

        UpdateUI();
        _UICanvas.SetActive(false);

        writer = new StreamWriter(SocialExpManager.Instance.GetUserPath() + SocialExpManager.Instance.GetUserID() + "_TASK" + SocialExpManager.Instance.GetCurrentTask() + "_SUMMARY.csv", false);
        isRecorderOn = true;
        writer.WriteLine("sep =;");
        writer.WriteLine("UserID;Trial;Duration;NbPeople;Character1Gender;Character1GenderTimeAnswer;Character1GenderAnswer;" +
                                                        "Character1Age;Character1AgeTimeAnswer;Character1AgeAnswer;" +
                                                        "Character1Position;Character1PositionTimeAnswer;Character1PositionAnswer;" +                                           
                                                        "Character2Gender;Character2GenderTimeAnswer;Character2GenderAnswer;" +
                                                        "Character2Age;Character2AgeTimeAnswer;Character2AgeAnswer;" +
                                                        "Character2Position;Character2PositionTimeAnswer;Character2PositionAnswer;"+
                                                        "Character3Gender;Character3GenderTimeAnswer;Character3GenderAnswer; " +
                                                        "Character3Age;Character3AgeTimeAnswer;Character3AgeAnswer;" +
                                                        "Character3Position;Character3PositionTimeAnswer;Character3PositionAnswer");
        _isTimerOn = false;
        _trialCompleted = false;
        PositionningUserManager.Instance.NeedRepositioning(this);

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
    void ValidateFeatureTime(int characterIndex, int indexFeature)
    {
        Debug.Log("validate: " + characterIndex + " feature: " + indexFeature + "  time: " + (Time.time - startTime).ToString());
        if(characterIndex == 0)
        {
            _answersTime0[indexFeature] = Time.time - startTime;
        }
        else if(characterIndex == 1)
        {
            _answersTime1[indexFeature] = Time.time - startTime;
        }
        else if (characterIndex == 2)
        {
            _answersTime2[indexFeature] = Time.time - startTime;
        }
        

    }

    void ValidateChoice()
    {
        //Vector3 pos = SocialExpManager.Instance.GetUserFloorPosition();
        //Vector3 rot = SocialExpManager.Instance.GetUserHeadRotationEuler();

        _props.SetActive(false);
        for (int i = 0; i < _characters.Length; i++)
        {
            _characters[i].SetActive(false);
        }

      
        RecorderPositionEyeTracking.Instance.StopRecording();
        _isTimerOn = false;
        _trialCompleted = true;
        string answerText = SocialExpManager.Instance.GetUserID() + ";" + currentTrial + ";"+(Time.time - startTime) + ";"+nbPeople.text + ";";
        
        for (int i = 0; i < _dataCharacter0.Length; i++)
        {
            answerText += _dataCharacter0[i] + ";" + _answersTime0[i] + ";" + _responsCharacter0[i].value + ";";
        }
        for (int i = 0; i < _dataCharacter1.Length; i++)
        {
            answerText += _dataCharacter1[i] + ";" + _answersTime1[i] + ";" + _responsCharacter1[i].value  + ";";
        }
        for (int i = 0; i < _dataCharacter2.Length; i++)
        {
            answerText += _dataCharacter2[i] + ";" + _answersTime2[i] + ";" + _responsCharacter2[i].value + ";";
        }
        
        writer.WriteLine(answerText);
        _UICanvas.SetActive(false);
        FinishTask();
    }

    void FinishTask()
    {



        _props.SetActive(false);
        for (int i = 0; i < _characters.Length; i++)
        {
            _characters[i].SetActive(false);
        }

        taskStarted = false;
        _UICanvas.SetActive(false);

        Debug.Log("Task observation completed");
        writer.Close();
        isRecorderOn = false;
  
 
        SocialExpManager.Instance.TaskDone();
        PositionningUserManager.Instance.EndExperiment();

    }

    public void ForceStopTask()
    {
        _props.SetActive(false);
        for (int i = 0; i < _characters.Length; i++)
        {
            _characters[i].SetActive(false);
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
        // if (currentTrial < nbtrial)
        //  {

        startTime = Time.time;
        NoteManager.Instance.WriteNote("Task " + SocialExpManager.Instance.GetCurrentTask() + " Trial" + (currentTrial + 1));

        _trialCompleted = false;
        _isTimerOn = false;
        RecorderPositionEyeTracking.Instance.SetMultiTarget(_characters);
        
       RecorderPositionEyeTracking.Instance.StartRecording(SocialExpManager.Instance.GetUserTaskPath(),
            SocialExpManager.Instance.GetUserID() + "_Task" + SocialExpManager.Instance.GetCurrentTask() + "_Trial" + (currentTrial + 1),
            false);
        startTime = Time.time;
        StartCoroutine(StartTimer());
        UpdateUI();

        TTS_manager.Instance.StartSound();

        SocialExpUIManager.Instance.SetTrial(currentTrial + 1, nbtrial);

        _props.SetActive(true);
        for (int i = 0; i < _characters.Length; i++)
        {
            _characters[i].SetActive(true);
        }
        /*   else
           {
               Debug.Log("Task observation completed");
               FinishTask();
           }*/
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

