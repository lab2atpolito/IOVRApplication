using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.Globalization;
using Meta.WitAi.TTS.Utilities;
using Meta.WitAi.TTS.Samples;

public class TasksManager : MonoBehaviour
{
    [Header("Tasks List")]
    [SerializeField] private List<ATask> _tasks;

    [Header("Task GUI")]
    [SerializeField] private Transform _canvas;
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _currentTaskIndexText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private GameObject _completeSceneCanva;
    [SerializeField] private Chatbot _chatbot;

    [SerializeField] private GameObject _virtualAssistentCanva;
    [SerializeField] private GameObject _timeTaskCanva;
    [SerializeField] private GameObject _messageCanva;
    [SerializeField] private GameObject _keyboardCanva;
    [SerializeField] private Questionnaire questionnaire;
    [SerializeField] private GameObject _menuCanva;

    public bool IsGuideActive()
    {
        return _isGuideActive;
    }

    //[SerializeField] private Vector3 _maxCanvasScale;
    //[SerializeField] private float _animationDuration;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _taskCompletedClip;

    [Header("Time")]
    [SerializeField] public  TimeManager _time;

    [Header("Text To Speech")]
    [SerializeField] private TTSSpeaker _tts;
    [SerializeField] private TextMeshProUGUI _vaText;

    private AudioManager _audioMgr; 
    private Needle _choosenNeedle;
    private int _currentTaskIndex = 0;
    private bool _isStarted = false;
    private string _currentUsername;
    private float _positionPrecision;
    private float _inclinationPrecision;
    private int _score;

    private SaveData _savedData;

    public bool _isGuideActive;

    private int _puncturesCount = 0;

    private void Start()
    {
        _savedData = SavingSystem.LoadData();
        _audioMgr = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    public void StartSimulation()
    {
        _isStarted = true;
        _time.StartTimer();

        _tasks[_currentTaskIndex].OnEntry(this);

        if( _isGuideActive)
        {
            _tasks[_currentTaskIndex].EnableSuggestions();
            InTaskGUI(_tasks[_currentTaskIndex].GetDescription());
        }
        //Debug.Log("Simulation started!");
    }

    void Update()
    {
        if (_isStarted)
        {
            // Check if the state of the simulation needs to change
            _tasks[_currentTaskIndex].OnUpdate(this);
        }
    }

    public void EndSimulation()
    {
        //_isStarted = false;
        //_time.StopTimer();

        //if( _isGuideActive)
        //{
        //    OutTaskGUI();
        //}

        string resultsQuestionnaire = questionnaire.sendResultsQuestionnaire();

        questionnaire.gameObject.SetActive(false);


        _completeSceneCanva.SetActive(true);
        Debug.Log("Simulation completed!");

        // Text to Sppech
        _tts.SpeakQueued("Congratulations, you have successfully completed the intraosseous insertion procedure.");
        _tts.SpeakQueued("Now you can proceed with the administration of fluids and medications necessary for the resuscitation of the patient. If you have any other doubts about the procedure, you can ask me for information.");
        _vaText.text = "Congratulations, you have successfully completed the intraosseous insertion procedure, now you can proceed with the administration of fluids and medications necessary for the resuscitation of the patient.";

        // Calculate score value
        _score = ScoreCalculator.CalculateScore(_time.GetTimeInSeconds(), _positionPrecision, _inclinationPrecision);

        // Save data information about current simulation
        List<string> tasksTime = new List<string>();

        // Calculate average completion time per task
        float sum = 0f;
        foreach( ATask task in _tasks)
        {
            float timeInSeconds = task.GetCompletionTimeInSeconds();
            string formattedTime = task.GetFormattedCompletionTime();
            tasksTime.Add(formattedTime);
            sum += timeInSeconds;
        }

        float averageTimePerTask = sum / _tasks.Count;
        TimeSpan averageTimeSpan = TimeSpan.FromSeconds(averageTimePerTask);
        string formattedAverageTime = averageTimeSpan.Minutes.ToString("d2") + ":" + averageTimeSpan.Seconds.ToString("d2");

        string sessionType;
        if (_isGuideActive)
            sessionType = "Guided";
        else
            sessionType = "Free";

        // Save the current session infromation on a .json file
        SimulationSaveData simulationData = new SimulationSaveData(sessionType, DateTime.Now.ToString(CultureInfo.InstalledUICulture), _currentUsername, _time.GetTimeInString(), tasksTime, formattedAverageTime, _puncturesCount, _positionPrecision, _inclinationPrecision, _score, resultsQuestionnaire);
        SavingSystem.Save(_currentUsername, sessionType, simulationData);

        _chatbot.SaveInfoChatbot();

        //Debug.Log(simulationData.ToString());
    }

    public void NextTask()
    {
        _tasks[_currentTaskIndex].OnExit(this);

        if( _isGuideActive )
            _tasks[_currentTaskIndex].DisableSuggestions();

        _tasks[_currentTaskIndex].SetAsCompleted();

        _currentTaskIndex = _tasks[_currentTaskIndex].GetNextTaskId(_currentTaskIndex);
        //Debug.Log("Current Task Id: " + _currentTaskIndex);
        //Debug.Log("Current task index: " + _currentTaskIndex + " < " + _tasks.Count);

        if (_currentTaskIndex < _tasks.Count)
        {
            _tasks[_currentTaskIndex].OnEntry(this);

            if( _isGuideActive)
            {
                _tasks[_currentTaskIndex].EnableSuggestions();
                UpdateTaskGUI(_tasks[_currentTaskIndex].GetDescription());
            }
        }
        else {
            //EndSimulation();
            Questionnaire();
        }
    }

    public void UpdateTaskGUI(string newDescription)
    {
        //_canvas.DOScale(new Vector3(0, 0, 0), _animationDuration).SetEase(Ease.InOutSine).OnComplete(() =>
        //{
        _descriptionText.text = newDescription;
        _currentTaskIndexText.text = _currentTaskIndex + 1 + " / " + _tasks.Count;
        //_canvas.DOScale(_maxCanvasScale, _animationDuration).SetEase(Ease.InOutSine);
        //});
    }
    public void InTaskGUI(string newDescription)
    {
        _descriptionText.text = newDescription;
        _currentTaskIndexText.text = _currentTaskIndex + 1 + " / " + _tasks.Count;
        //_canvas.DOScale(_maxCanvasScale, _animationDuration).SetEase(Ease.InOutSine);
    }
    public void OutTaskGUI()
    {
        _canvas.gameObject.SetActive(false);
        //_canvas.DOScale(new Vector3(0, 0, 0), _animationDuration).SetEase(Ease.InOutSine);
    }

    public Needle GetNeedle()
    {
        return _choosenNeedle;
    }
    public int GetCurrentTask()
    {
        return _currentTaskIndex;
    }
    public List<ATask> GetTasksList() => _tasks;

    public void SetNeedle(Needle needle)
    {
        _choosenNeedle = needle;
    }
    public void SetUsername(TextMeshProUGUI usernameText)
    {
        _currentUsername = usernameText.text;
        Debug.Log("Username has been setted");
    }
    public void SetPositionPrecision(float value) => _positionPrecision = value;
    public void SetInclinationPrecision(float value) => _inclinationPrecision = value;
    public void SetCurrentTaskAsCompleted()
    {
        _tasks[_currentTaskIndex].SetAsCompleted();
    }
    public void SetScore(int score) => _score = score;

    public bool IsStarted() => _isStarted;
    public void PlayTaskCompletedSound()
    {
        _audioSource.PlayOneShot(_taskCompletedClip);
    }
    public void EnableButton()
    {
        if( _isGuideActive )
            _nextButton.interactable = true;
    }
    public void DisableButton()
    {
        if( _isGuideActive )
            _nextButton.interactable = false;
    }

    public void ToggleBackground()
    {
        _audioMgr.ToggleBackgroundSound();
    }

    public void ToggleSounds()
    {
        _audioMgr.ToggleSounds();
    }

    public void UpdateSoundsVolume(float value)
    {
        _audioMgr.UpdateSoundsVolume(value);
    }

    public void IncreasePunctureCount()
    {
        _puncturesCount += 1;
        Debug.Log("Puncture count: " + _puncturesCount);
    }

    public void Questionnaire()
    {
        _isStarted = false;
        _time.StopTimer();

        if (_isGuideActive)
        {
            OutTaskGUI();
        }

        if (_menuCanva != null)
        {
            _menuCanva.SetActive(false);
        }

        _virtualAssistentCanva.SetActive(false);
        _messageCanva.SetActive(false);
        _timeTaskCanva.SetActive(false);
        _keyboardCanva.SetActive(false);
        questionnaire.gameObject.SetActive(true);
    }

}
