using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSyringeState : ATask
{
    [SerializeField] private GameObject[] _handGrabsConnector;
    [SerializeField] private ConnectorEndDx _connector;

    private TasksManager _simulation;

    private void Start()
    {
        //_description = "Disconnect 10 ml syringe from EZ-Connect extension set.";
        _simulation = GetComponentInParent<TasksManager>();
    }

    public override void OnEntry(TasksManager controller)
    {
        _isCompleted = false;
        foreach(GameObject handGrab in _handGrabsConnector)
        {
            handGrab.SetActive(true);
        }

        if (controller.IsGuideActive())
        {
            controller.DisableButton();
            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
        }
    }

    public override void OnExit(TasksManager controller)
    {
        //_simulation.AddTaskTimestamp();
    }

    public override void OnUpdate(TasksManager controller)
    {
        if(!_connector.IsSnapped() && !_isCompleted)
        {
            _isCompleted = true;
            if (controller.IsGuideActive())
                controller.EnableButton();
            else
                controller.NextTask();
            controller.PlayTaskCompletedSound();
        }
        else if (_connector.IsSnapped())
        {
            _isCompleted = false;
            if (controller.IsGuideActive())
                controller.DisableButton();
        }
    }
}
