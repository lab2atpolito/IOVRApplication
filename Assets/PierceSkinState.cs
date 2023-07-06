using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceSkinState : ATask
{
    [SerializeField] private PowerDrill _drill;

    [SerializeField] private GameObject _precisionUI;
    [SerializeField] private CanvaAnimator _precisionCanva;
    [SerializeField] private MessageSystemUI _messageSys;

    [SerializeField] private GameObject[] _handGrabs;

    private void Start()
    {
        //_description = "Insert EZ IO needle into selected site with the needle perpendicular to the bone surface until needle tip touches bone. Do not activate power driver yet.";
    }

    public override void OnEntry(TasksManager controller)
    {
        _isCompleted = false;
        //_needleInteractionMgr = _drill.GetNeedle().GetComponent<NeedleInteraction>();
        //_needle = _drill.GetNeedle();

        if (controller.IsGuideActive())
        {
            controller.DisableButton();
            _precisionUI.SetActive(true);
            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
            _precisionCanva.ActivateCanvas();
        }
    }

    public override void OnExit(TasksManager controller)
    {
        //_simulation.AddTaskTimestamp();
        if (controller.IsGuideActive())
        {
            _precisionUI.SetActive(false);
            _precisionCanva.HideCanvas();
        }
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

            // Pop up management
            if (needle.GetCurrentState() == State.IN_SKIN && !_messageSys.IsActive())
            {
                _messageSys.SetMessage("The selected needle hasn't reached the proximal tibia bone surface yet.").SetType(MessageType.WARNING).Show(false);
            }
            else if (needle.GetCurrentState() == State.IN_AIR && _messageSys.IsActive())
            {
                _messageSys.Hide();
            }
            else if (needle.GetCurrentState() == State.IN_BONE && _messageSys.IsActive() && _drill.GetNeedle().GetNeedleType() == NeedleType.PINK)
            {
                _messageSys.SetMessage("The selected needle hasn't reached the proximal tibia bone surface yet.").SetType(MessageType.WARNING).Show(false);
            }

            // Task completion management
            // The user has chosen the blue (25mm) needle or the yellow needle (45mm)
            if (_drill.GetNeedle().GetNeedleType() == NeedleType.BLUE || _drill.GetNeedle().GetNeedleType() == NeedleType.YELLOW)
            {
                // The needle reached the surface of the bone
                if (needle.GetCurrentState() == State.IN_BONE && !_isCompleted)
                {
                    _isCompleted = true;
                    if (controller.IsGuideActive())
                        controller.EnableButton();
                    else
                        controller.NextTask();
                    controller.PlayTaskCompletedSound();
                    _messageSys.SetMessage("The selected needle has correctly reached the proximal tibia bone surface.").SetType(MessageType.NOTIFICATION).Show(false);
                }
                // The needle doesn't reached the surface of the bone
                else if (needle.GetCurrentState() == State.IN_AIR || needle.GetCurrentState() == State.IN_SKIN)
                {
                    _isCompleted = false;
                    if (controller.IsGuideActive())
                        controller.DisableButton();
                }
            }
            else
            {
                if (needle.GetCurrentState() == State.IN_SKIN && !_isCompleted)
                {
                    _isCompleted = true;
                    if (controller.IsGuideActive())
                        controller.EnableButton();
                }
                else if (needle.GetCurrentState() == State.IN_AIR)
                {
                    _isCompleted = false;
                    if (controller.IsGuideActive())
                        controller.DisableButton();
                }
            }
        }
    }
}
