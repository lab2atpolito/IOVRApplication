using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabDrillState : ATask
{
    [SerializeField] private PowerDrill _drill;

    [SerializeField] private GameObject[] _handGrabs;

    private TasksManager _simulation;

    private void Start()
    {
        //_description = "Grab the power driver with your dominant hand.";
        _simulation = GetComponentInParent<TasksManager>();
    }

    public override void OnEntry(TasksManager controller)
    {
        foreach(GameObject handGrab in _handGrabs)
        {
            handGrab.SetActive(true);
        }
        _tts.SpeakQueued(_speakingText);
        _virtualAssistantText.text = _speakingText;
    }

    public override void OnExit(TasksManager controller)
    {
        //_simulation.AddTaskTimestamp();
    }

    public override void OnUpdate(TasksManager controller)
    {
        if(_drill.IsGrabbed() && !_isCompleted)
        {
            _isCompleted = true;
            controller.EnableButton();
            controller.PlayTaskCompletedSound();
        }
        else if(!_drill.IsGrabbed())
        {
            _isCompleted = false;
            controller.DisableButton();
        }
    }
}
