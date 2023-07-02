using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.TTS.Samples;
using UnityEngine;
using UnityEngine.UI;

public class ControllerButtonsManager : MonoBehaviour
{
    private bool _isActive = false;
    [SerializeField] private Button _sendButton;
    [SerializeField] private Button _voiceButton;
    [SerializeField] private Button _repeatButton;

    void Update()
    {
        if( _isActive && !OVRPlugin.GetHandTrackingEnabled())
        {
            // Button "A" pressed
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Touch))
            {
                _sendButton.onClick.Invoke();
                Debug.Log("A Pressed!");
            }
            // Button "B" pressed
            else if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Touch))
            {
                _voiceButton.onClick.Invoke();
                Debug.Log("B Pressed!");
            }
            // Button "B" pressed
            else if (OVRInput.GetDown(OVRInput.Button.Three, OVRInput.Controller.Touch))
            {
                _repeatButton.onClick.Invoke();
                Debug.Log("X Pressed!");
            }
        }
    }

    public void ActiveVirtualAssistant()
    {
        _isActive = true;
    }

    public void DisableVirtualAssistant()
    {
        _isActive = false;
    }
}
