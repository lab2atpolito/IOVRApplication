using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionsManagerMenu : MonoBehaviour
{
    public List<SliderValue> sliders = new List<SliderValue>();
    private string resultsMenu;
    public Questionnaire questionnaire;

    public void valueFromSliders()
    {
        foreach (SliderValue s in sliders)
        {
            resultsMenu += s._question.text + " " + s._slider.value + "\n";

        }
    }

    public void takeValueFromSliders()
    {
        valueFromSliders();
        questionnaire.addResultsQuestionnaire(resultsMenu);

    }
}
