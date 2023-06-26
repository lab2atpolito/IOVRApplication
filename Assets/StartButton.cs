using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    private Button _button;

    void Start()
    {
        _button = this.GetComponent<Button>();
    }

    public void UpdateButtonState(TextMeshProUGUI name)
    {
        _button.interactable = !string.IsNullOrWhiteSpace(name.text);
    }
}
