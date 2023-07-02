using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VirtualAssistantUI : MonoBehaviour
{
    [SerializeField] private TTSSpeaker _tts;

    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _sendButton;
    [SerializeField] private Button _voiceButton;

    public void EnableUI()
    {
        _inputField.interactable = true;
        _sendButton.interactable = true;
        _voiceButton.interactable = true;
    }

    public void DisableUI()
    {
        _inputField.interactable = false;
        _sendButton.interactable = false;
        _voiceButton.interactable = false;
    }
}
