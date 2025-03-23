using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Microsoft.Speech.Synthesis;
using Crosstales.RTVoice;
using TMPro;
using System.IO;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance { get; private set; }

    [SerializeField]
    private Button _buttonSaveNote;
    [SerializeField]
    private TMP_InputField _text;
    private StreamWriter writer;
    [SerializeField]
    private GameObject panelNote;
    bool isInit = false;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        panelNote.SetActive(false);
        _buttonSaveNote.onClick.AddListener(SaveNote);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InitNote(string path)
    {
        if (!isInit)
        {

            writer = new StreamWriter(path, false);
            writer.WriteLine("NOTES Exp " + System.DateTime.Now.Day + " " + System.DateTime.Now.Month + " " + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute);
            panelNote.SetActive(true);
            isInit = true;
        }
    }
    void SaveNote()
    {
        Debug.Log("Task: " + SocialExpManager.Instance.GetCurrentTask());
        writer.WriteLine(_text.text);
        Debug.Log("SaveNote: "+_text.text);
        _text.text = "";
    }
    public void WriteNote(string txt)
    {
        writer.WriteLine(txt);
    }
    public void StartTask(int task, string name)
    {
        writer.WriteLine("Start task: "+task+"  :  "+ name);
    }

    public void CloseNote()
    {
        writer.WriteLine("Stop exp " + System.DateTime.Now.Day + " " + System.DateTime.Now.Month + " " + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute);
        writer.WriteLine("Exit application");
        writer.Close();
    }
}
