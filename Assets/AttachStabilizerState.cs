using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachStabilizerState : ATask
{
    [SerializeField] private Stabilizer _stabilizer;

    [SerializeField] private GameObject[] _handGrabs;

    public override void OnEntry(TasksManager controller)
    {
        _isCompleted = false;

        if (controller.IsGuideActive())
        {
            controller.DisableButton();
            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
        }
    }

    public override void OnExit(TasksManager controller)
    {
        foreach (GameObject handGrab in _handGrabs)
        {
            handGrab.SetActive(false);
        }
    }

    public override void OnUpdate(TasksManager controller)
    {
        if (_stabilizer.IsAttached() && !_isCompleted)
        {

            //_stabilizer.blockBone();
            _isCompleted = true;
            if (controller.IsGuideActive())
                controller.EnableButton();
            else
                controller.NextTask();
            controller.PlayTaskCompletedSound();
        }
        else if(!_stabilizer.IsAttached())
        {
            _isCompleted = false;
            if(controller.IsGuideActive())
                controller.DisableButton();
        }
    }
}
