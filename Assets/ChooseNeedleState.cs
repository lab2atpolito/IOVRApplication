using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseNeedleState : ATask
{
    [SerializeField] private PowerDrill _drill;

    [SerializeField] private GameObject[] _handGrabs;

    [SerializeField] private Collider[] _needleSnapping;

    [SerializeField] private GameObject _precisionUI;

    private TasksManager _simulation;

    private void Start()
    {
        //_description = "Choose appropriate sized needle for insertion site and connect it to driver.";
        _simulation = GetComponentInParent<TasksManager>();
    }

    public override void OnEntry(TasksManager controller)
    {
        _isCompleted = false;

        if (controller.IsGuideActive())
        {
            // Enable the possibility to grab the needles
            foreach (GameObject handGrab in _handGrabs)
            {
                handGrab.SetActive(true);
            }

            // Enable the possibility to snap the needle to power drill
            foreach (Collider collider in _needleSnapping)
            {
                collider.enabled = true;
            }

            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
        }
    }

    public override void OnExit(TasksManager controller)
    {
        // Disable the possibility to grab the needles
        foreach (GameObject handGrab in _handGrabs)
        {
            handGrab.SetActive(false);
        }

        // Disable the possibility to snap the needle to power drill
        /*foreach (Collider collider in _needleSnapping)
        {
            collider.enabled = false;
        }*/

        // Set the chosen needle for the procedure as the one which has been snapped to power drill
        controller.SetNeedle(_drill.GetNeedle());
    }

    public override void OnUpdate(TasksManager controller)
    {
        // When no needle is snapped to power drill the next button is disable
        if (!_drill.IsEmpty() && !_isCompleted)
        {
            _isCompleted = true;
            if (controller.IsGuideActive())
                controller.EnableButton();
            else
                controller.NextTask();
            controller.PlayTaskCompletedSound();
        }
        else if(_drill.IsEmpty())
        {
            _isCompleted = false;
            if(controller.IsGuideActive())
                controller.DisableButton();
        }
    }
}
