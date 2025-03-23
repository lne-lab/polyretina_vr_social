using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DirectionPositioning { Front = 0, Center = 1, Back = 2}

public class PositionningUserManager : MonoBehaviour
{
    public static PositionningUserManager Instance { get; private set; }

    [SerializeField]
    private GameObject _hospitalEnvironment, _calibrationEnvironment, _targetFaceOrientation;

    [SerializeField]
    private Button _buttonValidate;

    [SerializeField]
    private GameObject[] _taskReferencePoint;

    [SerializeField]
    private GameObject _userParent;

    [SerializeField]
    private GameObject _centerCalibration, _frontCalibration, _backCalibration;

    [SerializeField]
    private GameObject _frontDirectionCalibration, _backDirectionCalibration;



   bool _isPositionned = false;
    int _task = 0;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _buttonValidate.onClick.AddListener(ValidatePosition);
        _buttonValidate.gameObject.SetActive(false);
        _isPositionned = false;

        _hospitalEnvironment.SetActive(false);
        _calibrationEnvironment.SetActive(true);
        _centerCalibration.SetActive(false);
        _frontCalibration.SetActive(true);
        _backCalibration.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Component componentTmp;

    public void NeedRepositioning(Component component, DirectionPositioning direction = DirectionPositioning.Front )
    {
        ImplantController.Instance.SetImplantActiveCalibration(false);
        _isPositionned = false;
        _hospitalEnvironment.SetActive(false);
        _calibrationEnvironment.SetActive(true);
        _buttonValidate.gameObject.SetActive(true);

        _frontCalibration.SetActive(false);
        _centerCalibration.SetActive(false);
        _backCalibration.SetActive(false);

        _frontDirectionCalibration.SetActive(false);
        _backDirectionCalibration.SetActive(false);

        if (direction == DirectionPositioning.Front )
        {
            _frontCalibration.SetActive(true);
            _frontDirectionCalibration.SetActive(true);
        }
        else if(direction == DirectionPositioning.Center)
        {
            _centerCalibration.SetActive(true);
            _frontDirectionCalibration.SetActive(true);
        }
        else if (direction == DirectionPositioning.Back )
        {
            _backCalibration.SetActive(true);
            _backDirectionCalibration.SetActive(true);
        }


        Debug.Log("Repositionned requested from: " + component);
        componentTmp = component;
    }

    public void NeedRepositioningSpecificLocation(Component component, Transform refPos)
    {
        _isPositionned = false;
        ImplantController.Instance.SetImplantActiveCalibration(false);
        _userParent.transform.position = refPos.position;
        _userParent.transform.rotation = refPos.rotation;
        _calibrationEnvironment.transform.position = refPos.position + new Vector3(0f, _calibrationEnvironment.transform.localScale.y / 2.0f, 0f);
        _calibrationEnvironment.transform.rotation = refPos.rotation;

        _hospitalEnvironment.SetActive(false);
        _calibrationEnvironment.SetActive(true);
        _buttonValidate.gameObject.SetActive(true);
        Debug.Log("Repositionned requested from: " + component);
        componentTmp = component;
    }

    public bool IsPositionned()
    {
        return _isPositionned;
    }
    public void ValidatePosition()
    {
        ImplantController.Instance.SetImplantActiveCalibration(true);
        _isPositionned = true;
        _buttonValidate.gameObject.SetActive(false);
        _hospitalEnvironment.SetActive(true);
        _calibrationEnvironment.SetActive(false);
        componentTmp.SendMessage("PositioningCompleted");
    }
    public void StartNewTask(int task)
    {
        Debug.Log("Start task positioning: " + task);
        _task = task;
        _userParent.transform.position = _taskReferencePoint[_task].transform.position;
        _userParent.transform.rotation = _taskReferencePoint[_task].transform.rotation;
        _calibrationEnvironment.transform.position = _taskReferencePoint[_task].transform.position + new Vector3(0f, _calibrationEnvironment.transform.localScale.y / 2.0f, 0f);
        _calibrationEnvironment.transform.rotation = _taskReferencePoint[_task].transform.rotation;

        _hospitalEnvironment.SetActive(false);
        _calibrationEnvironment.SetActive(true);

    }
    public void EndExperiment()
    {
        _hospitalEnvironment.SetActive(false);
        _calibrationEnvironment.SetActive(true);
    }
}
