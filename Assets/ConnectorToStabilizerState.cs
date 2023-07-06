using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorToStabilizerState : ATask
{
    [SerializeField] private ConnectorEnd _connector;

    [SerializeField] private GameObject[] _handGrabs;

    private TasksManager _simulation;

    private void Start()
    {
        //_description = "Connect one end of the connector to the stabilizer.";
        _simulation = GetComponentInParent<TasksManager>();
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
    }

    public override void OnExit(TasksManager controller)
    {
        foreach (GameObject handGrab in _handGrabs)
        {
            handGrab.SetActive(false);
        }
        //_simulation.AddTaskTimestamp();
    }

    public override void OnUpdate(TasksManager controller)
    {
        if (_connector.IsSnapped() && !_isCompleted)
        {
            _isCompleted = true;
            if (controller.IsGuideActive())
                controller.EnableButton();
            else
                controller.NextTask();
            controller.PlayTaskCompletedSound();
        }
        else if(!_connector.IsSnapped())
        {
            _isCompleted = false;
            if(controller.IsGuideActive())
                controller.DisableButton();
        }
    }
}
