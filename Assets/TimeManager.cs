using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private float _currentTime = 0f;
    [SerializeField] private bool _isTimeOn = false;
    private float _lastTimestamp = 0f;

    [SerializeField] private TasksManager _simulation;
    [SerializeField] private TextMeshProUGUI _currentTimeText;
    [SerializeField] private TextMeshProUGUI _freeModeTimeText;

    void Update()
    {
        if (_isTimeOn)
        {
            _currentTime += Time.deltaTime;
            _currentTimeText.text = GetTimeInString();
            if (!_simulation.IsGuideActive())
            {
                _freeModeTimeText.text = GetTimeInString();
            }
        }
    }

    public void StartTimer() => _isTimeOn = true;

    public void StopTimer() => _isTimeOn = false;

    /*public string GetNewTimestamp()
    {
        float timeInterval = _currentTime - _lastTimestamp;
        _lastTimestamp = _currentTime;
        TimeSpan duration = TimeSpan.FromSeconds(timeInterval);
        return duration.Minutes.ToString("d2") + ":" + duration.Seconds.ToString("d2");
    }*/

    public TimeSpan NewTaskTimestamp()
    {
        float timeInterval = _currentTime - _lastTimestamp;
        _lastTimestamp = _currentTime;
        TimeSpan duration = TimeSpan.FromSeconds(timeInterval);
        return duration;
        //return duration.Minutes.ToString("d2") + ":" + duration.Seconds.ToString("d2");
    }

    public float GetTimeInSeconds() => _currentTime;
    public string GetTimeInString()
    {
        TimeSpan time = TimeSpan.FromSeconds(_currentTime);
        return time.Minutes.ToString("d2") + ":" + time.Seconds.ToString("d2");
    }
}
