using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWindowButton : MonoBehaviour
{
    [SerializeField] private Transform _tasksCanvas;
    [SerializeField] private Transform _completedCanvas;

    [SerializeField] private Transform _pauseCanvas;

    [SerializeField] private TasksManager _simulation;

    public void OnClick()
    {
        _pauseCanvas.gameObject.SetActive(false);
        if( _simulation.IsStarted() )
        {
            _tasksCanvas.gameObject.SetActive(true);
        }
        else
        {
            _completedCanvas.gameObject.SetActive(true);
        }
    }
}
