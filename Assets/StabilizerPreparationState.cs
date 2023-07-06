using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabilizerPreparationState : ATask
{
    [SerializeField] private Stabilizer _stabilizer;
    [SerializeField] private GameObject[] _handGrabs;

    //private Sticker[] _stickers; 

    //private TasksManager _simulation;

    private void Start()
    {
        //_description = "Secure insertion site with EZ Stabilizer.";
        //_simulation = GetComponentInParent<TasksManager>();
    }
    public override void OnEntry(TasksManager controller)
    {
        _isCompleted = false;

        if (controller.IsGuideActive())
        {
            controller.DisableButton();
            foreach (GameObject handGrab in _handGrabs)
            {
                handGrab.SetActive(true);
            }
            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
        }
        //_stabilizer.GetComponentsInChildren<Sticker>();
    }

    public override void OnExit(TasksManager controller)
    {
    }

    public override void OnUpdate(TasksManager controller)
    {
        if (_stabilizer.IsGrabbed() && !_isCompleted)
        {
            _isCompleted = true;
            if (controller.IsGuideActive())
                controller.EnableButton();
            else
                controller.NextTask();
            controller.PlayTaskCompletedSound();
        }
        else if (!_stabilizer.IsGrabbed())
        {
            _isCompleted = false;
            if(controller.IsGuideActive())
                controller.DisableButton();
        }
    }
}
