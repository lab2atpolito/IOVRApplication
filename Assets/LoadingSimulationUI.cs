using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSimulationUI : MonoBehaviour
{
    public float fillDuration;

    [SerializeField] private Image _loadingCircle;
    [SerializeField] private GameObject _countdownCanvas;
    [SerializeField] private GameObject _taskCanvas;
    [SerializeField] private TasksManager _simulation;
    
    public void StartLoading()
    {
        StartCoroutine(StartLoadingCoroutine());
    }

    public IEnumerator StartLoadingCoroutine()
    {
        float timer = 0f;

        while(timer < fillDuration)
        {
            timer += Time.deltaTime;

            float fillPercentage = timer / fillDuration;
            fillPercentage = Mathf.Clamp01(fillPercentage);

            _loadingCircle.fillAmount = fillPercentage;
            yield return null;
        }

        // Start simulation
        _taskCanvas.SetActive(true);
        _countdownCanvas.SetActive(false);
        _simulation.StartSimulation();
    }
}
