using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    private TextMeshProUGUI _countdown;
    public int countdownTime;

    [SerializeField] private GameObject _countdownCanvas;
    [SerializeField] private GameObject _taskCanvas;
    [SerializeField] private TasksManager _simulation;

    IEnumerator CountdownStart()
    {
        while( countdownTime > 0 )
        {
            _countdown.text = countdownTime.ToString();

            yield return new WaitForSeconds(2f);

            countdownTime--;
        }

        // Start simulation
        if(_simulation.IsGuideActive())
            _taskCanvas.SetActive(true);

        _countdownCanvas.SetActive(false);
        _simulation.StartSimulation();
    }

    void Awake()
    {
        _countdown = this.GetComponent<TextMeshProUGUI>();
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownStart());
    }

    
}
