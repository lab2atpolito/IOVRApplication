using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.HandGrab;

public class FootPositionState : ATask
{
    [SerializeField] private SuggestionLeg _suggestionLeg;
    [SerializeField] private GameObject _suggestionLegMesh;
    [SerializeField] private GameObject _handGrabL;
    [SerializeField] private GameObject _handGrabR;

    public TimeManager timeManager;

    private void Start()
    {
        //_description = "Grab the ankle and straighten the leg.";
    }

    public override void OnEntry(TasksManager controller)
    {
        _handGrabL.SetActive(true);
        _handGrabR.SetActive(true);
        timeManager.StartTimer();

        if (controller.IsGuideActive())
        {
            _suggestionLegMesh.SetActive(true);
            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
        }
    }

    public override void OnExit(TasksManager controller)
    {
        if (controller.IsGuideActive())
        {
            _handGrabL.SetActive(false);
            _handGrabR.SetActive(false);
            _suggestionLegMesh.SetActive(false);
        }
    }

    public override void OnUpdate(TasksManager controller)
    {
        if(_suggestionLeg.HasReachedTarget() && !_isCompleted)
        {
            _isCompleted = true;

            if (controller.IsGuideActive())
                controller.EnableButton();
            else
                controller.NextTask();
            controller.PlayTaskCompletedSound();
        }
        else if(!_suggestionLeg.HasReachedTarget())
        {
            _isCompleted = false;
            if(controller.IsGuideActive())
                controller.DisableButton();
        }
    }
}
