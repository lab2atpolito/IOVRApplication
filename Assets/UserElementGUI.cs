using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserElementGUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _usernameText;
    [SerializeField] private TextMeshProUGUI _sessionsNumberText;
    [SerializeField] private GameObject _sessionElementPrefab;

    private Transform quitButtonOne;
    private Transform quitButtonTwo;
    private Transform quitButtonThree;

    private Transform userSessionsPanel;
    private string username;
    private int sessionsNumber;
    private List<SimulationSaveData> sessionsData;

    private Transform levelOneScroll;
    private Transform levelTwoScroll;
    private Transform levelThree;

    public void SetValues(string username, int sessionsNumber, List<SimulationSaveData> sessionsData, Transform userSessionsPanel, Transform levelOneScroll, Transform levelTwoScroll, Transform levelThree, Transform quitButtonOne, Transform quitButtonTwo, Transform quitButtonThree)
    {
        this.username = username;
        this.sessionsNumber = sessionsNumber;
        this.sessionsData = sessionsData;
        this.userSessionsPanel = userSessionsPanel;
        this.levelOneScroll = levelOneScroll;
        this.levelTwoScroll = levelTwoScroll;
        this.levelThree = levelThree;
        this.quitButtonOne = quitButtonOne;
        this.quitButtonTwo = quitButtonTwo;
        this.quitButtonThree = quitButtonThree;

        _usernameText.text = username;
        _sessionsNumberText.text = sessionsNumber.ToString();

        Debug.Log("Username: " + username + "\n" +
            "Session Number: " + sessionsNumber + "\n" +
            "Session Date: " + sessionsData + "\n" +
            "user session panel: " + userSessionsPanel.gameObject.name + "\n" +
            "levelOneScroll: " + levelOneScroll.gameObject.name + "\n" +
            "levelTwoScrool: " + levelTwoScroll.gameObject.name + "\n");
    }

    public string GetUsername()
    {
        return username;
    }

    public void CreateSessionElements()
    {
        int id = 1;
        foreach(SimulationSaveData sessionData in sessionsData)
        {

            GameObject sessionElement = Instantiate(_sessionElementPrefab, userSessionsPanel);
            sessionElement.GetComponent<SessionElementGUI>().ChangeValues(sessionData, id, levelTwoScroll, levelThree, quitButtonTwo, quitButtonThree);
            id++;
        }
    }

    public void OnClick()
    {
        levelOneScroll.gameObject.SetActive(false);
        levelTwoScroll.gameObject.SetActive(true);
        CreateSessionElements();
        quitButtonOne.gameObject.SetActive(false);
        quitButtonTwo.gameObject.SetActive(true);
    }
}
