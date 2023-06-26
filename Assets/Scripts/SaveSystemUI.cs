using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemUI : MonoBehaviour
{
    [SerializeField] private Transform _usersPanel;
    [SerializeField] private Transform _sessionsPanel;

    [SerializeField] private Transform levelOneScroll;
    [SerializeField] private Transform levelTwoScroll;
    [SerializeField] private Transform levelThree;

    [SerializeField] private Transform quitButtonOne;
    [SerializeField] private Transform quitButtonTwo;
    [SerializeField] private Transform quitButtonThree;

    [SerializeField] private GameObject _userElementPrefab;
  
    private SaveData _saveData; 


    public void CreateUsersPanel(SaveData data)
    {
        if( data != null )
        {
            _saveData = data;
            List<string> users = data.GetAllUsernames();
            if (users != null)
            {
                foreach (string user in users)
                {
                    List<SimulationSaveData> sessionsData = data.savedData[user];
                    int sessionsNumber = sessionsData.Count;

                    GameObject newUserGUI = Instantiate(_userElementPrefab, _usersPanel);
                    newUserGUI.GetComponent<UserElementGUI>().SetValues(user, sessionsNumber, sessionsData, _sessionsPanel, levelOneScroll, levelTwoScroll, levelThree, quitButtonOne, quitButtonTwo, quitButtonThree);
                }
            }
        }
    }

    public void DeleteSessionElements()
    {
        // Delete all children
        for (int i = _sessionsPanel.childCount - 1; i >= 0; i--)
        {
            Transform child = _sessionsPanel.GetChild(i);
            Destroy(child.gameObject);
        }
    }
}
