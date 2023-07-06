using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSafetyCapState : ATask
{
    [SerializeField] private PowerDrill _drill;

    [SerializeField] private GameObject[] _handGrabs;
    [SerializeField] private GameObject[] _handGrabsNeedles;

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

        if (controller.IsGuideActive())
        {
            _drill.GetNeedle().GetSafetyCap().GetComponent<Interactable>().EnableMaterial();
            controller.DisableButton();
            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
        }
    }

    public override void OnExit(TasksManager controller)
    {
        /*if (controller.IsGuideActive())
        {
            foreach (GameObject handGrabNeedle in _handGrabsNeedles)
            {
                handGrabNeedle.SetActive(true);
            }
        }*/
    }

    public override void OnUpdate(TasksManager controller)
    {
        if (!_drill.GetNeedle().HasSafetyCap() && !_isCompleted)
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
