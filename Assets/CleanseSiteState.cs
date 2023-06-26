using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanseSiteState : ATask
{
    [SerializeField] private CleansingInteraction _cottonBall;

    [SerializeField] private GameObject[] _handGrabs;

    private TasksManager _simulation; 

    private void Start()
    {
        //_description = "Cleanse site using antiseptic solution and allow to dry.";
        _simulation = GetComponentInParent<TasksManager>();
    }

    public override void OnEntry(TasksManager controller)
    {
        controller.DisableButton();
        _tts.SpeakQueued(_speakingText);
        _virtualAssistantText.text = _speakingText;
    }

    public override void OnExit(TasksManager controller)
    {
        //_simulation.AddTaskTimestamp();
    }

    public override void OnUpdate(TasksManager controller)
    {
        if(_cottonBall.IsCleansing() && !_isCompleted)
        {
            _isCompleted = true;
            controller.EnableButton();
            controller.PlayTaskCompletedSound();
        }
    }
}
