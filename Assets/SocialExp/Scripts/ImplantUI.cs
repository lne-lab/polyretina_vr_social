using LNE.ProstheticVision;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;
using System.Linq;
using LNE.ArrayExts;
public class ImplantUI : MonoBehaviour
{
    public Toggle activateToggle;
    public TMP_Dropdown eyeDropdown;
    public TMP_Dropdown fovDropdown;
    public TMP_Dropdown layoutDropdown;
    //public Slider fieldOfViewSlider;
    public Slider percentageSlider;
    //public TMP_Text fovText;  
    public TMP_Text percentageText;
    public Button hideUI;
    bool isVisibleUI = false;
    public GameObject canvasUI;
    public int[] fovVariations;

    void Start()
    {
        // Populate dropdown options
        PopulateEyeDropdown();
        PopulateLayoutDropdown();
        PopulateFOVDropdown();

        // Subscribe to UI events
        activateToggle.onValueChanged.AddListener(OnActivateToggleChanged);
        eyeDropdown.onValueChanged.AddListener(OnEyeDropdownChanged);
        layoutDropdown.onValueChanged.AddListener(OnLayoutDropdownChanged);
        //fieldOfViewSlider.onValueChanged.AddListener(OnFieldOfViewSliderChanged);
       // percentageSlider.onValueChanged.AddListener(OnPercentageSliderChanged);
        fovDropdown.onValueChanged.AddListener(OnFOVDropdownChanged);
        isVisibleUI = false;
        hideUI.onClick.AddListener(HideUI);
        Invoke("InitializeUI", 0.5f);
        // Initialize UI with current controller values
        //InitializeUI();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            HideUI();
        }
    }
    void InitializeUI()
    {
        ImplantController controller = ImplantController.Instance;
        activateToggle.isOn = false;
        eyeDropdown.value = (int)controller.CurrentEye;
        layoutDropdown.value = (int)controller.CurrentLayout;
        if (fovVariations.Contains((int)controller.FieldOfView) )
        {
           
            fovDropdown.value = fovVariations.IndexOf((int)controller.FieldOfView);
        }
        //fieldOfViewSlider.value = controller.FieldOfView;
        percentageSlider.value = controller.ImplantPercentage;
        HideUI();
    }
    void HideUI()
    {
        isVisibleUI = !isVisibleUI;
        canvasUI.SetActive(isVisibleUI);  
    }
    void PopulateEyeDropdown()
    {
        eyeDropdown.ClearOptions();
        var options = new List<string>(Enum.GetNames(typeof(StereoTargetEyeMask)));
        eyeDropdown.AddOptions(options);
    }

    void PopulateLayoutDropdown()
    {
        layoutDropdown.ClearOptions();
        var options = new List<string>(Enum.GetNames(typeof(ElectrodeLayout)));
        layoutDropdown.AddOptions(options);
    }
    void PopulateFOVDropdown()
    {
        fovDropdown.ClearOptions();
        var options = new List<string>();
        for(int i = 0; i < fovVariations.Length; i++)
        {
            options.Add("FOV " + fovVariations[i]);
        }
        fovDropdown.AddOptions(options);
    }
    void OnActivateToggleChanged(bool isActive)
    {
        ImplantController.Instance.SetImplantActive(isActive);
    }

    void OnEyeDropdownChanged(int eyeIndex)
    {
        ImplantController.Instance.SetTargetEye((StereoTargetEyeMask)eyeIndex);
    }

    void OnLayoutDropdownChanged(int layoutIndex)
    {
        ImplantController.Instance.SetLayout((ElectrodeLayout)layoutIndex);
    }
    void OnFOVDropdownChanged(int fovIndex)
    {
        ImplantController.Instance.SetFieldOfView(fovVariations[fovIndex]);
    }
    /*
    void OnFieldOfViewSliderChanged(float value)
    {
        ImplantController.Instance.SetFieldOfView(value);
        UpdateFOVText(value);
    }

    void OnPercentageSliderChanged(float value)
    {
        ImplantController.Instance.SetImplantPercentage(value);
        UpdatePercentageText(value);
    }
   
    /*void UpdateFOVText(float value)
    {
        fovText.text = $"FOV: {value}";
    }*/

    void UpdatePercentageText(float value)
    {
        percentageText.text = $"{value}% - 80-{120 * (value / 100)}";
    }

}
