using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public FadeScreen fadeScreen;
    private AudioManager audioMgr;

    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _mainMenu; 

    private void Start()
    {
        audioMgr = GameObject.FindAnyObjectByType<AudioManager>();
        audioMgr.UpdateAudioManager();
    }

    public void StartSimulation()
    {
        LoadScene("Simulation");
        //StartCoroutine(LoadSceneAsync(1));
    }

    public void StartTutorial()
    {
        LoadScene("Tutorial");
    }

    public void StartTheoryModule()
    {
        //LoadScene("TheoryModule");
    }

    public void Quit()
    {
        Application.Quit(); 
    }

    public void ReturnToMainMenu()
    {
        LoadScene(0);
    }

    public void LoadScene(string name)
    {
        StartCoroutine(LoadSceneRoutine(name));
    }

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneRoutine(sceneId));
    }

    IEnumerator LoadSceneRoutine(string name)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        SceneManager.LoadScene(name);
    }

    IEnumerator LoadSceneRoutine(int id)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        SceneManager.LoadScene(id);
    }

    IEnumerator FadeOutRoutine()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);
    }

    IEnumerator LoadSceneAsync(int id)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(id);

        _loadingScreen.SetActive(true);
        _mainMenu.SetActive(false);

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }

        StartCoroutine(FadeOutRoutine());
    }
}
