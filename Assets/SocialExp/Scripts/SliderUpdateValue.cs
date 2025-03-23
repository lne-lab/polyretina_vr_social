using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderUpdateValue : MonoBehaviour
{

   private Slider _slider;
    [SerializeField]
    private TMP_Text _label;
    // Start is called before the first frame update
    void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        ValueChangeCheck();
    }


    public void ValueChangeCheck()
    {
        _label.text = _slider.value.ToString();
    }
}
