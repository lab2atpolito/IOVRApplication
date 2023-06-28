using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceSkinState : ATask
{
    [SerializeField] private PowerDrill _drill;

    [SerializeField] private GameObject _precisionUI;
    [SerializeField] private CanvaAnimator _precisionCanva;
    [SerializeField] private MessageSystemUI _messageSys; 

    private NeedleInteraction _needleInteractionMgr;
    private Needle _needle;

    private void Start()
    {
        //_description = "Insert EZ IO needle into selected site with the needle perpendicular to the bone surface until needle tip touches bone. Do not activate power driver yet.";
    }

    public override void OnEntry(TasksManager controller)
    {
        _isCompleted = false;
        controller.DisableButton();
        _precisionUI.SetActive(true);
        _tts.SpeakQueued(_speakingText);
        _virtualAssistantText.text = _speakingText;
        _needleInteractionMgr = _drill.GetNeedle().GetComponent<NeedleInteraction>();
        _needle = _drill.GetNeedle();
        _precisionCanva.ActivateCanvas();
    }

    public override void OnExit(TasksManager controller)
    {
        //_simulation.AddTaskTimestamp();
        _precisionUI.SetActive(false);
        _precisionCanva.HideCanvas();
    }

    public override void OnUpdate(TasksManager controller)
    {
        // Pop up management
        if(_needleInteractionMgr.GetCurrentState() == State.IN_SKIN && !_messageSys.IsActive())
        {
            _messageSys.SetMessage("The selected needle hasn't reached the proximal tibia bone surface yet.").SetType(MessageType.WARNING).Show(false);
        }
        else if(_needleInteractionMgr.GetCurrentState() == State.IN_AIR && _messageSys.IsActive())
        {
            _messageSys.Hide();
        }
        else if (_needleInteractionMgr.GetCurrentState() == State.IN_BONE && _messageSys.IsActive() && _needle.GetNeedleType() == NeedleType.PINK)
        {
            _messageSys.SetMessage("The selected needle hasn't reached the proximal tibia bone surface yet.").SetType(MessageType.WARNING).Show(false);
        }

        // Task completion management
        // The user has chosen the blue (25mm) needle or the yellow needle (45mm)
        if ( _needle.GetNeedleType() == NeedleType.BLUE || _needle.GetNeedleType() == NeedleType.YELLOW)
        {
            // The needle reached the surface of the bone
            if (_needleInteractionMgr.GetCurrentState() == State.IN_BONE && !_isCompleted)
            {
                _isCompleted = true;
                controller.EnableButton();
                controller.PlayTaskCompletedSound();
                _messageSys.SetMessage("The selected needle has correctly reached the proximal tibia bone surface.").SetType(MessageType.NOTIFICATION).Show(false);
            }
            // The needle doesn't reached the surface of the bone
            else if (_needleInteractionMgr.GetCurrentState() == State.IN_AIR || _needleInteractionMgr.GetCurrentState() == State.IN_SKIN)
            {
                _isCompleted = false;
                controller.DisableButton();
            }
        }
        else
        {
            if (_needleInteractionMgr.GetCurrentState() == State.IN_SKIN && !_isCompleted)
            {
                _isCompleted = true;
                controller.EnableButton();
            }
            else if (_needleInteractionMgr.GetCurrentState() == State.IN_AIR)
            {
                _isCompleted = false;
                controller.DisableButton();
            }
        }
    }
}
