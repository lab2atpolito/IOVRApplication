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
        _tts.SpeakQueued(_speakingText);
        _virtualAssistantText.text = _speakingText;
        _suggestionLegMesh.SetActive(true);
    }

    public override void OnExit(TasksManager controller)
    {
        _handGrabL.SetActive(false);
        _handGrabR.SetActive(false);
        _suggestionLegMesh.SetActive(false);
    }

    public override void OnUpdate(TasksManager controller)
    {
        if(_suggestionLeg.HasReachedTarget() && !_isCompleted)
        {
            _isCompleted = true;
            controller.EnableButton();
            controller.PlayTaskCompletedSound();
        }
        else if(!_suggestionLeg.HasReachedTarget())
        {
            _isCompleted = false;
            controller.DisableButton();
        }
    }
}
