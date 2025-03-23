using LNE.ProstheticVision;
using LNE.UI.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplantController : MonoBehaviour
{
    public static ImplantController Instance { get; private set; }
    [Range(0.0f, 100.0f)]
    public float implantPercentage = 100f;
    [Range(0.0f, 180.0f)]
    public float fieldOfView = 45f;
    public bool isImplantActive = false;

    private StereoTargetEyeMask currentEye = StereoTargetEyeMask.Right;
    public StereoTargetEyeMask CurrentEye => currentEye;
    private ElectrodeLayout currentLayout = ElectrodeLayout._80x120;// ElectrodeLayout._80xPERCENT;
    public ElectrodeLayout CurrentLayout => currentLayout;
    public float FieldOfView => fieldOfView;
    public float ImplantPercentage => implantPercentage;

    float previousFieldOfView = 45f;
    /*
    [SerializeField, LinkWithProperty]
    
    private Implant implant;*/
    [SerializeField]
    private Camera _implantCamera;

    [SerializeField]
    private Camera _otherEyeCamera;
 
    
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        // implant = Prosthesis.Instance.Implant;
       
    }

    void Update()
    {
        if (fieldOfView != previousFieldOfView)
        {
            previousFieldOfView = fieldOfView;
            if (Prosthesis.Instance.Implant is EpiretinalImplant epiretinalImplant)
            {
                epiretinalImplant.fieldOfView = fieldOfView;
                Debug.Log("Field of View set to: " + epiretinalImplant.fieldOfView);
            }
            else
            {
                Debug.LogWarning("The implant is not of type EpiretinalImplant.");
            }
        }
        
    }
    void LateUpdate()
    {
        _implantCamera.fieldOfView = fieldOfView;
    }

    public void SetImplantActive(bool isActive)
    {
        isImplantActive = isActive;
        Debug.Log("Implant active: " + isImplantActive);
        Prosthesis.Instance.GetComponent<Prosthesis>().enabled = isImplantActive;
        Prosthesis.Instance.GetComponent<TunnelVision>().enabled = !isImplantActive;
        // Activate/deactivate logic if required
    }
    public void SetImplantActiveCalibration(bool isActive)
    {


            if (isImplantActive)
            {
                Prosthesis.Instance.GetComponent<Prosthesis>().enabled = isActive;
            }
            else
            {
                Prosthesis.Instance.GetComponent<TunnelVision>().enabled = isActive;
            }
        
        // Activate/deactivate logic if required
    }
    public void SetTargetEye(StereoTargetEyeMask eye)
    {
        currentEye = eye;
       
        if (Prosthesis.Instance.Implant is EpiretinalImplant epiretinalImplant)
        {

            epiretinalImplant.targetEye = eye;

            Debug.Log("Target eye set to: " + epiretinalImplant.targetEye);
            if(eye == StereoTargetEyeMask.Right)
            {
                _otherEyeCamera.stereoTargetEye = StereoTargetEyeMask.Left;
                _implantCamera.stereoTargetEye = StereoTargetEyeMask.Right;
                Prosthesis.Instance.GetComponent<TunnelVision>().SetTargetEye(StereoTargetEyeMask.Right);
            }
            else
            {
                _otherEyeCamera.stereoTargetEye = StereoTargetEyeMask.Right;
                _implantCamera.stereoTargetEye = StereoTargetEyeMask.Left;
                Prosthesis.Instance.GetComponent<TunnelVision>().SetTargetEye(StereoTargetEyeMask.Left);

            }
           
        }
    }
   
    public void SetLayout(ElectrodeLayout layout)
    {
        currentLayout = layout;
        if (Prosthesis.Instance.Implant is EpiretinalImplant epiretinalImplant)
        {
            epiretinalImplant.layout = layout;
            Debug.Log("Layout set to: " + epiretinalImplant.layout);
            epiretinalImplant.fieldOfView = fieldOfView;

        }
    }

    public void SetFieldOfView(float value)
    {
        fieldOfView = value;
        if (Prosthesis.Instance.Implant is EpiretinalImplant epiretinalImplant)
        {
            epiretinalImplant.fieldOfView = value;
            Debug.Log("Field of View set to: " + epiretinalImplant.fieldOfView);
        }
        Prosthesis.Instance.GetComponent<TunnelVision>().fov = value;
    }

    public void SetImplantPercentage(float percentage)
    {
        implantPercentage = percentage;
        Debug.Log("Implant percentage set to: " + implantPercentage);
        // Additional logic to handle percentage change
    }
    public bool IsRightEye()
    {
        if(currentEye == StereoTargetEyeMask.Right)
        {
            return true;
        }
        else
        {
            return false;
        }
         
    }
    public string GetCurrentLayout()
    {
        if (isImplantActive)
        {
            return currentLayout.ToString();

        }
        else
        {
            return "control";
        }
        
    }
    public int GetCurrentFOV()
    {

        return (int)fieldOfView;

    }
}
