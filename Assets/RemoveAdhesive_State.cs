using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAdhesive_State : ATask
{
    [SerializeField] private Sticker _sticker1;
    [SerializeField] private Sticker _sticker2;
    [SerializeField] private GameObject[] _handGrabs;
    public bool _remouveSticker = false;

    public override void OnEntry(TasksManager controller)
    {
        _isCompleted = false;
        foreach (GameObject handGrab in _handGrabs)
        {
            handGrab.SetActive(true);
        }

        if (controller.IsGuideActive())
        {
            controller.DisableButton();
            _tts.SpeakQueued(_speakingText);
            _virtualAssistantText.text = _speakingText;
        }
    }

    public override void OnExit(TasksManager controller)
    {
    }

    public override void OnUpdate(TasksManager controller)
    {
        if (!_sticker1.IsAttached() && !_sticker2.IsAttached() && !_isCompleted)
        {
            _remouveSticker = true;
            _isCompleted = true;
            if (controller.IsGuideActive())
                controller.EnableButton();
            else
                controller.NextTask();
            controller.PlayTaskCompletedSound();
        }
    }
}
