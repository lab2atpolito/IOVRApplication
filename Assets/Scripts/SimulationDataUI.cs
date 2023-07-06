using System;
using TMPro;
using UnityEngine;

public class SimulationDataUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _simulationText; 
    [SerializeField] private TextMeshProUGUI _dateText;
    [SerializeField] private TextMeshProUGUI _puncturesText;
    [SerializeField] private TextMeshProUGUI _positionText;
    [SerializeField] private TextMeshProUGUI _inclinationText;
    [SerializeField] private TextMeshProUGUI _totalTimeText;
    [SerializeField] private TextMeshProUGUI _timePerTaskText;
    [SerializeField] private TextMeshProUGUI _finalScoreText;

    public void ChangeValues(SimulationSaveData sessionData, int sessionId)
    {
        _simulationText.text = sessionData.sessionType + " Simulation Session n. " + sessionId;
        _dateText.text = sessionData.date;
        _puncturesText.text = sessionData.puncturesCount.ToString();
        _positionText.text = (Math.Round(sessionData.positionPrecision * 100f) / 100f).ToString();
        _inclinationText.text = (Math.Round(sessionData.inclinationPrecision * 100f) / 100f).ToString();
        _totalTimeText.text = sessionData.totalTime;
        _timePerTaskText.text = sessionData.averageTimePerTask;
        _finalScoreText.text = sessionData.score.ToString();
    }
}
