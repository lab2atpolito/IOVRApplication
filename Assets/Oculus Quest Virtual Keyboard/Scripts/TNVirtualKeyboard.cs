using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TNVirtualKeyboard : MonoBehaviour
{
	
	public static TNVirtualKeyboard instance;
	
	public string words = "";
	
	public GameObject vkCanvas;
	
	public TMP_InputField targetText;
	
	public Text debugText;

    private bool _maiuscEnabled = false;
	
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
		HideVirtualKeyboard();
		
    }
	
	public void KeyPress(string k){
		if (targetText.text == "Listening...") targetText.text = "";
		words = targetText.text + k;
		//words += k;
		targetText.text = words;	
		debugText.text = words;
	}
	
	public void Del(){
		words = words.Remove(words.Length - 1, 1);
		targetText.text = words;	
		debugText.text = words;
	}
	
	public void ShowVirtualKeyboard(){
		vkCanvas.SetActive(true);
	}
	
	public void HideVirtualKeyboard(){
		vkCanvas.SetActive(false);
        words = "";
	}

    public void ToggleMaiusc()
    {
        if (_maiuscEnabled)
            _maiuscEnabled = false;
        else
            _maiuscEnabled = true;
    }

    public bool IsMaiuscEnabled()
    {
        return _maiuscEnabled;
    }

    public void SetTargetText(TMP_InputField target)
    {
        targetText = target;
        words = target.text;
    }
}
