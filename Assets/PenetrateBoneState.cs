using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetrateBoneState : ATask
{
    [SerializeField] private PowerDrill _drill;
    [SerializeField] private GameObject[] _handGrabs;

    private TasksManager _simulation;
    [SerializeField] private MessageSystemUI _messageSys; 

    private void Start()
    {
        //_description = "Penetrate the bone cortex by squeezing the driver’s trigger and applying gentle, consistent, downward pressure.";
        _simulation = GetComponentInParent<TasksManager>();
    }

    public override void OnEntry(TasksManager controller)
    {
        _isCompleted = false;
        if (controller.IsGuideActive())
        {
            controller.DisableButton();
            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
        }
        //_messageSys.SetMessage("The tip of the needle hasn't reached the medullary cavity of the bone yet.").SetHasDuration(false).SetType(MessageType.WARNING).Show();
    }

    public override void OnExit(TasksManager controller)
    {
        NeedleInteraction needle = _drill.GetNeedle().GetComponent<NeedleInteraction>();
        float positionPrecision = needle.GetPositionPrecision();
        float inclinationPrecision = needle.GetAnglePrecision();
        _simulation.SetPositionPrecision(positionPrecision);
        _simulation.SetInclinationPrecision(inclinationPrecision);
        //Debug.Log("Position precision " + positionPrecision + " has been saved correctly!");
        //Debug.Log("Inclination precision " + inclinationPrecision + " has been saved correctly!");
        //_messageSys.Hide();
    }

    public override void OnUpdate(TasksManager controller)
    {
        if (!_drill.IsEmpty())
        {
            NeedleInteraction needle = _drill.GetNeedle().GetComponent<NeedleInteraction>();
            if (!controller.IsGuideActive())
            {
                if (needle.GetCurrentState() == State.IN_AIR)
                {
                    foreach (GameObject obj in _handGrabs)
                    {
                        obj.SetActive(true);
                    }
                }
                else
                {
                    foreach (GameObject obj in _handGrabs)
                    {
                        obj.SetActive(false);
                    }
                }
            }

            if (needle.GetCurrentState() == State.MEDULLARY_CAVITY && !_isCompleted)
            {
                _isCompleted = true;
                if (controller.IsGuideActive())
                    controller.EnableButton();
                controller.PlayTaskCompletedSound();

                if (_drill.GetNeedle().GetNeedleType() == NeedleType.BLUE)
                {
                    if( !controller.IsGuideActive() )
                        controller.NextTask();
                    _messageSys.SetMessage("The tip of the needle has successfully reached the medullary cavity of the bone!").SetType(MessageType.NOTIFICATION).Show(true);
                }

                else
                    _messageSys.SetMessage("The selected needle seems to be too long!").SetType(MessageType.WARNING).Show(true); 
            }
            else if (needle.GetCurrentState() != State.MEDULLARY_CAVITY)
            {
                _isCompleted = false;
                if (controller.IsGuideActive())
                    controller.DisableButton();
            }
        }
    }
}
