using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSafetyCapState : ATask
{
    [SerializeField] private PowerDrill _drill;

    [SerializeField] private GameObject[] _handGrabs;

    private TasksManager _simulation; 

    private void Start()
    {
        //_description = "Remove needle safety cap.";
        _simulation = GetComponentInParent<TasksManager>();
    }

    public override void OnEntry(TasksManager controller)
    {
        _isCompleted = false;
        foreach(GameObject handGrab in _handGrabs)
        {
            handGrab.SetActive(true);
        }
        _drill.GetNeedle().GetComponentInChildren<Interactable>().EnableMaterial();
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
        if (!_drill.GetNeedle().HasSafetyCap() && !_isCompleted)
        {
            _isCompleted = true;
            controller.EnableButton();
            controller.PlayTaskCompletedSound();
        }
    }
}
