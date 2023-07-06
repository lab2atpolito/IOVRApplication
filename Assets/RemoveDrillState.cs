using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveDrillState : ATask
{
    [SerializeField] private PowerDrill _drill;

    [SerializeField] private GameObject[] _handGrabs;

    private TasksManager _simulation;

    private void Start()
    {
        //_description = "Stabilise the catheter hub and remove the driver from the needle.";
        _simulation = GetComponentInParent<TasksManager>();
    }

    public override void OnEntry(TasksManager controller)
    {
        _isCompleted = false;
        foreach(GameObject handGrab in _handGrabs)
        {
            handGrab.SetActive(true);
        }

        if (controller.IsGuideActive())
        {
            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
            controller.DisableButton();
            controller.GetNeedle().GetComponent<Interactable>().EnableMaterial();
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
        if (_drill.IsEmpty() && !_isCompleted)
        {
            _isCompleted = true;
            if (controller.IsGuideActive())
                controller.EnableButton();
            else
                controller.NextTask();
            controller.PlayTaskCompletedSound();
        }
    }
}
