using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Verify5mmState : ATask
{
    [SerializeField] private PowerDrill _drill;
    [SerializeField] private MessageSystemUI _messageSys;

    private Needle _needle; 

    private void Start()
    {
        //_description = "Confirm that at least one black line on the catheter is visible, otherwise the needle is too short and larger size should be used";
    }

    public override void OnEntry(TasksManager controller)
    {
        if (controller.IsGuideActive())
        {
            controller.DisableButton();
            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
            StartCoroutine(Wait(5f, controller));
        }
        _needle = _drill.GetNeedle();
    }

    public override void OnExit(TasksManager controller)
    {
        //DO NOTHING
    }

    public override void OnUpdate(TasksManager controller)
    {
        //DO NOTHING
    }

    public override int GetNextTaskId(int currentTask)
    {
        NeedleInteraction needle = _drill.GetNeedle().GetComponent<NeedleInteraction>();
        int nextTaskId;
        if(_needle.GetNeedleType() == NeedleType.BLUE && needle.IsOneLineVisible() && (needle.GetAnglePrecision() / 100f) >= 0.9f && (needle.GetPositionPrecision() / 100f) >= 0.9f && needle.GetCurrentState() == State.IN_BONE)
        {
            nextTaskId = currentTask + 1;
            _messageSys.SetMessage("You've chosen the correct sized needle, at least one line (5mm) is visible.").SetType(MessageType.NOTIFICATION).Show(true);
        }
        else if(_needle.GetNeedleType() != NeedleType.BLUE)
        {
            nextTaskId = 6;
            _messageSys.SetMessage("You've chosen the wrong sized needle. Let's go back a few steps. Choose another needle to proceed with the Intraosseous Insertion.").SetType(MessageType.WARNING).Show(true);
        }else if ((needle.GetAnglePrecision() / 100f) < 0.9f || (needle.GetPositionPrecision() / 100f) < 0.9f)
        {
            nextTaskId = 8;
            _messageSys.SetMessage("The needle's position or angle are wrong, you need to reinsert in the correct way").SetType(MessageType.WARNING).Show(true);
        }
        else
        {
            nextTaskId = 8;
            _messageSys.SetMessage("The needle has not been placed correctly. Ensure that at least one line (5mm) is visible.").SetType(MessageType.WARNING).Show(true);
        }
        return nextTaskId;
    }

    private IEnumerator Wait(float seconds, TasksManager controller)
    {
        yield return new WaitForSeconds(seconds);
        controller.EnableButton();
    }
}
