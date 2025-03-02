using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    public TextMeshProUGUI _question;
    public Slider _slider;
    public TextMeshProUGUI _value;

    public void setValue()
    {
        _value.text = _slider.value.ToString();
    }
}
