using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_BodyEmotion : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        combinationList = new List<int>() { 0, 1, 2, 3 };
        randomCombinationListAction = ShuffleList(randomCombinationListAction);
        combinationList = new List<int>() { 3, 2, 1, 0 };
        randomCombinationListCharacter = ShuffleList(randomCombinationListCharacter);
        _props.SetActive(false);
     //   StartTask();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            _characters[randomCombinationListCharacter[currentTrial]].SetActive(false);
            currentTrial++;
            StartTrial();
        }
    }
    public void StartTask()
    {
        Debug.Log("Task body emotion start");
        _props.SetActive(true);
        SocialExpUIManager.Instance.SetTest("Task body emotion");
        currentTrial = 0;
        StartTrial();

    }

    void StartTrial()
    {
        if (currentTrial < nbtrial)
        {
            SocialExpUIManager.Instance.SetTrial(currentTrial + 1, nbtrial);
            Debug.Log("Task body emotion, trial: " + currentTrial);
            _characters[randomCombinationListCharacter[currentTrial]].SetActive(true);
            _characters[randomCombinationListCharacter[currentTrial]].GetComponent<Animator>().SetInteger("ActionEmotion", randomCombinationListAction[currentTrial]);

        }
        else
        {

            Debug.Log("Task body emotion completed");
            _props.SetActive(false);
            SocialExpManager.Instance.TaskDone();
        }
    }
    List<int> ShuffleList(List<int> listToRandomize)
    {
        System.Random random = new System.Random();
        while (combinationList.Count > 0)
        {
            int index = random.Next(combinationList.Count);
            int currentToRandomize = combinationList[index];
            listToRandomize.Add(currentToRandomize);
            combinationList.RemoveAt(index);
        }
        return listToRandomize;
    }
}
