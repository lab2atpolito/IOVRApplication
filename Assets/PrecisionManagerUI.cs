using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrecisionManagerUI : MonoBehaviour
{
    [SerializeField] private Image _fillPosition;
    [SerializeField] private TextMeshProUGUI _textPosition;

    [SerializeField] private Image _fillInclination;
    [SerializeField] private TextMeshProUGUI _textInclination;

    [SerializeField] PowerDrill _drill; 

    // Update is called once per frame
    void Update()
    {
        if (!_drill.IsEmpty())
        {
            NeedleInteraction needle = _drill.GetNeedle().GetComponent<NeedleInteraction>();
            if(needle.IsHittingSkin())
            {
                _fillPosition.fillAmount = needle.GetPositionPrecision() / 100f;
                _textPosition.text = Mathf.RoundToInt(needle.GetPositionPrecision()).ToString();
                _fillInclination.fillAmount = needle.GetAnglePrecision() / 100f;
                _textInclination.text = Mathf.RoundToInt(needle.GetAnglePrecision()).ToString();
            }
            else
            {
                _fillPosition.fillAmount = 0f;
                _textPosition.text = "--";
                _fillInclination.fillAmount = 0f;
                _textInclination.text = "--";
            }
        }
    }
}
