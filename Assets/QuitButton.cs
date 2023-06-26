using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    [SerializeField] private GameObject _buttons;
    [SerializeField] private GameObject _quitVerify;
    [SerializeField] private TasksManager _simulation;
    [SerializeField] private ScenesManager _sceneManager;

    public void OnClick()
    {
        if( _simulation.IsStarted())
        {
            _buttons.SetActive(false);
            _quitVerify.SetActive(true);
        }
        else
        {
            _sceneManager.ReturnToMainMenu();
        }
    }
}
