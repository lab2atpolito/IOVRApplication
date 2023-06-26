using System;
using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.TTS.Utilities;
using TMPro;
using UnityEngine;

abstract public class ATask : MonoBehaviour
{
    [SerializeField] protected string _description;
    [SerializeField] protected string _speakingText;
    [SerializeField] protected Interactable[] _interactables; // set of tools the user has to interact with during the task
    [SerializeField] protected TTSSpeaker _tts;
    [SerializeField] protected TextMeshProUGUI _virtualAssistantText;
    [SerializeField] protected TimeManager _timeManager;
    [SerializeField] protected TasksListUI _tasksListUI;

    protected bool _isCompleted = false;
    protected TimeSpan _completionTime; 

    public abstract void OnEntry(TasksManager controller);
    public abstract void OnUpdate(TasksManager controller);
    public abstract void OnExit(TasksManager controller);

    public void EnableSuggestions()
    {
        foreach(Interactable interactable in _interactables)
        {
            interactable.EnableMaterial();
        }
    }

    public void DisableSuggestions()
    {
        foreach (Interactable interactable in _interactables)
        {
            interactable.DisableMaterial();
        }
    }

    public string GetDescription()
    {
        return _description;
    }

    public int GetCompletionTimeInSeconds()
    {
        return _completionTime.Seconds;
    }

    public string GetFormattedCompletionTime()
    {
        return _completionTime.Minutes.ToString("d2") + ":" + _completionTime.Seconds.ToString("d2");
    }

    public bool IsCompleted()
    {
        return _isCompleted;
    }

    public void SetAsCompleted()
    {
        _isCompleted = true;
        if( _completionTime != null)
        {
            _completionTime += _timeManager.NewTaskTimestamp();
        }
        else
        {
            _completionTime = _timeManager.NewTaskTimestamp();
        }
        _tasksListUI.SetCurrentTaskAsCompleted(GetFormattedCompletionTime());
    }

    public virtual int GetNextTaskId(int currentId)
    {
        int nextTaskId = currentId + 1;
        return nextTaskId;
    }
}
