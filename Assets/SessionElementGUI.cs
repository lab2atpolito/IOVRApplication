using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SessionElementGUI : MonoBehaviour
{
    private SimulationSaveData _sessionData;

    private Transform _levelTwoScroll;
    private Transform _levelThree;

    private Transform quitButtonTwo;
    private Transform quitButtonThree;

    private int _sessionId;

    [SerializeField] private TextMeshProUGUI _sessionIdText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _dateText;


    public void ChangeValues(SimulationSaveData sessionData, int sessionID, Transform levelTwoScroll, Transform levelThree, Transform quitButtonTwo, Transform quitButtonThree)
    {
        this.quitButtonTwo = quitButtonTwo;
        this.quitButtonThree = quitButtonThree;

        _sessionId = sessionID;
        _levelTwoScroll = levelTwoScroll;
        _levelThree = levelThree;

        _sessionData = sessionData;
        _sessionIdText.text = "Simulation Session n. " + sessionID.ToString();
        _dateText.text = "Date: " + _sessionData.date;
        _scoreText.text = "Score: " + _sessionData.score.ToString();
    }

    public void OnClick()
    {
        _levelTwoScroll.gameObject.SetActive(false);
        _levelThree.gameObject.SetActive(true);
        _levelThree.gameObject.GetComponent<SimulationDataUI>().ChangeValues(_sessionData, _sessionId);
        quitButtonTwo.gameObject.SetActive(false);
        quitButtonThree.gameObject.SetActive(true);
    }
}
