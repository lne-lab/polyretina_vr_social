using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class SocialExpUIManager : MonoBehaviour
{
    public static SocialExpUIManager Instance { get; private set; }

    [SerializeField]
    private TMP_Text labelTrial;
    [SerializeField]
    private TMP_Text labelTest;
    [SerializeField]
    private TMP_Text labelInfoConditiont;
    [SerializeField]
    private TMP_InputField _participantID;

    [SerializeField]
    private Button _startExp;
    [SerializeField]
    private Button _restartExp;

    [SerializeField]
    private TMP_Dropdown _taskStart;

    [SerializeField]
    private TMP_Dropdown _condition;

    [SerializeField]
    private GameObject _infoScreen;

    [SerializeField]
    private Button _quitExp;

    [SerializeField]
    private Button _backMenuExp;

    [SerializeField]
    private Toggle _dominantEyeToggle;

    [SerializeField]
    private TMP_Dropdown _language;

    [SerializeField]
    private Toggle _skipFaceEmotion;


    [SerializeField]
    private TMP_Dropdown _gender;
    
    [SerializeField]
    private TMP_Dropdown _expVR;

    [SerializeField]
    private TMP_InputField _age;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        labelTest.text = "N/A";
        labelTrial.text = "N/A";

        _startExp.onClick.AddListener(StartExp);
        _restartExp.onClick.AddListener(RestartApplication);
        _quitExp.onClick.AddListener(ForceExitExp);

        _backMenuExp.onClick.AddListener(BackToMenu);

        List<string> startOptions = new List<string> {
            "1. Seat search",
            "2. Body orientation",
            "3. Body appearance",
            "4. Face emotion with body",
            "5. Face emotion only",
            "6. Gesture relationship",
            "7. Recognize role",
            "8. Detecting motion",
            "9. Body language",
            "10. Observation"};
        _taskStart.AddOptions(startOptions);

        List<string> conditionsOptions = new List<string> {
            "1. Control group",
            "2. Simulation FOV 20",
            "3. Simulation FOV 45",
            "4. Simulation FOV 110"};
        _condition.AddOptions(conditionsOptions);

        List<string> languageOptions = new List<string> {
            "French",
            "English"
           };
        _language.AddOptions(languageOptions);

        List<string> genderOptions = new List<string> {
            "Woman",
            "Man",
            "Transgender",
            "Non-binary",
            "Prefer not to respond"
        };
        _gender.AddOptions(genderOptions);

        List<string> expVrOptions = new List<string> {
            "no experience",
            "some experience",
            "very experienced"
        };
        _expVR.AddOptions(expVrOptions);


        _skipFaceEmotion.onValueChanged.AddListener(delegate {
            SetSkipFaceTask(_skipFaceEmotion);
        });

    }

    void ForceExitExp()
    {
        SocialExpManager.Instance.ForceQuitExp();
    }
    void SetSkipFaceTask(bool val)
    {
        SocialExpManager.Instance.SetSkipAnimationFaceTask(val);
    }
    void BackToMenu()
    {

        _infoScreen.SetActive(true);
        SocialExpManager.Instance.StopTaskBackMenu();
    }
    public void SetTest(string t)
    {
        labelTest.text = t;
    }
    public void SetTrial(int t, int nbTrial)
    {
        labelTrial.text = t.ToString()+"/"+ nbTrial;
    }
    void RestartApplication()
    {

        Debug.Log("Restart application");
        SocialExpManager.Instance.StopTaskBackMenu();
        Destroy(SocialExpManager.Instance.gameObject);
        SceneManager.LoadScene(0);

       
    }
    void StartExp()
    {

        if (_participantID.text != "")
        {
            SocialExpManager.Instance.SetPreferencesParticipant(_gender.options[_gender.value].text, _age.text, _expVR.options[_expVR.value].text, _language.options[_language.value].text);
            Debug.Log("-" + _participantID.text + "-");
            Debug.Log("Participant info: " + _participantID.text + "  start: " + _taskStart.value);
            _infoScreen.SetActive(false);
            if(_language.value == 0)
            {
                //French
                TTS_manager.Instance.SetLanguage(true);
            }
            else
            {
                TTS_manager.Instance.SetLanguage(false);
            }
            /*
            List<string> conditionsOptions = new List<string> {
            "Control",
            "FOV 20",
            "FOV 45",
            "FOV 110"};
            
            if (_dominantEyeToggle.isOn)
            {
                labelInfoConditiont.text = conditionsOptions[_condition.value] + " R";
            }
            else
            {
                labelInfoConditiont.text = conditionsOptions[_condition.value] + " L";
            }
            */
            SocialExpManager.Instance.PrepareStartParticipant(_participantID.text, _taskStart.value);
            //SocialExpManager.Instance.PrepareStartParticipant(_participantID.text, _taskStart.value, _condition.value, _dominantEyeToggle.isOn);
        }
        else
        {
            Debug.Log("Error info");
        }

    }
    public void SetInfoCondition(string t)
    {
     
    }
}
