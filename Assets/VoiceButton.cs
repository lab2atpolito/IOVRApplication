using System.Collections;
using System.Collections.Generic;
using Oculus.Voice;
using UnityEngine;

public class VoiceButton : MonoBehaviour
{
    [SerializeField] private AppVoiceExperience _voiceSystem;
    private bool _isActivated;

    public void OnClick()
    {
        if( !_isActivated )
        {
            _isActivated = true;
            _voiceSystem.Activate();
        }
        else
        {
            _isActivated = false;
            _voiceSystem.Deactivate();
        }
    }
}
