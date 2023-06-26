using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuManager : MonoBehaviour
{
    private SaveData _saveData;
    private SaveSystemUI _saveSystemUI; 

    void Start()
    {
        _saveData = SavingSystem.LoadData();
        _saveSystemUI = this.GetComponent<SaveSystemUI>();
        _saveSystemUI.CreateUsersPanel(_saveData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
