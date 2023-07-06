using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyringeFlushState : ATask
{
    [SerializeField] private Syringe _syringe;
    [SerializeField] private MessageSystemUI _messageSys;

    private void Start()
    {
        //_description = "Flush the catheter with a rapid and vigorous 10 ml flush of normal saline prior to infusion. Verify that the flow is adequate.";
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
    }

    public override void OnExit(TasksManager controller)
    {
        //DO NOTHING
    }

    public override void OnUpdate(TasksManager controller)
    {
        if(_syringe.IsFlushed() && !_isCompleted)
        {
            _isCompleted = true;
            if (controller.IsGuideActive())
                controller.EnableButton();
            else
                controller.NextTask();
            controller.PlayTaskCompletedSound();
            _messageSys.SetMessage("You've successfully flushed the 10 ml syringe of saline solution into the medullary cavity of the bone.").SetType(MessageType.NOTIFICATION).Show(true);
        }
    }
}
