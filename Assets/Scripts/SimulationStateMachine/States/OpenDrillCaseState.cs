using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDrillCaseState : ATask
{
    [SerializeField] private DrillCase _drillCase;

    [SerializeField] private GameObject[] _handGrabs;

    private TasksManager _simulation;

    private void Start()
    {
        //_description = "Open the power drill case on the floor with your hands.";
        _simulation = GetComponentInParent<TasksManager>();
    }

    public override void OnEntry(TasksManager controller)
    {
        controller.DisableButton();
        foreach (GameObject handGrab in _handGrabs)
        {
            handGrab.SetActive(true);
        }
        _tts.SpeakQueued(_speakingText);
        _virtualAssistantText.text = _speakingText;
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
        if(_drillCase.IsOpened() && !_isCompleted)
        {
            _isCompleted = true;
            controller.EnableButton();
            controller.PlayTaskCompletedSound();
        }
        else if(!_drillCase.IsOpened())
        {
            _isCompleted = false;
            controller.DisableButton();
        }
    }
}
