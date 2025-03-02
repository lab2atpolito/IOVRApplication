using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Questionnaire : MonoBehaviour
{
    private string resultsQuestionnaire;
    // Start is called before the first frame update
    public string sendResultsQuestionnaire()
    {
        return resultsQuestionnaire;
    }

    public void addResultsQuestionnaire(string results)
    {
        resultsQuestionnaire += results;
    }

    public void reciveResultsQuestionnaire()
    {
        string resultsQuestionnaire = sendResultsQuestionnaire();
        Debug.Log("Risultati questionari " + resultsQuestionnaire);
    }
}
