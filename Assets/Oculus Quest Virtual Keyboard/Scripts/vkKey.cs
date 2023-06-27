using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class vkKey : MonoBehaviour
{
	
	public string k = "xyz";
    public bool isNumber = false;
    private Text text;
	
    // Start is called before the first frame update
    void Start()
    {
        //KeyClick();
        text = this.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isNumber && TNVirtualKeyboard.instance.IsMaiuscEnabled())
        {
            k = k.ToUpper();
            text.text = k;
        }
        else
        {
            k = k.ToLower();
            text.text = k;
        }
    }
	
	public void KeyClick(){
		TNVirtualKeyboard.instance.KeyPress(k);
	}
}
