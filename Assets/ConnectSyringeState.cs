using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectSyringeState : ATask
{
    [SerializeField] private ConnectorEndDx _connector;

    [SerializeField] private GameObject[] _handGrabsSyringe;
    [SerializeField] private GameObject[] _handGrabsConnector;

    private TasksManager _simulation;

    private void Start()
    {
        //_description = "Connect the other end of the connector to the 10 ml syringe of normal saline.";
        _simulation = GetComponentInParent<TasksManager>();
    }

    public override void OnEntry(TasksManager controller)
    {
        _isCompleted = false;

        if (controller.IsGuideActive())
        {
            controller.DisableButton();
            foreach (GameObject handGrab in _handGrabsSyringe)
            {
                handGrab.SetActive(true);
            }
            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
        }
        foreach (GameObject handGrab in _handGrabsConnector)
        {
            handGrab.SetActive(true);
        }
    }

    public override void OnExit(TasksManager controller)
    {
    }

    public override void OnUpdate(TasksManager controller)
    {
        if (_connector.IsSnapped() && !_isCompleted)
        {
            _isCompleted = true;

            foreach (GameObject handGrab in _handGrabsConnector)
            {
                handGrab.SetActive(false);
            }

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
