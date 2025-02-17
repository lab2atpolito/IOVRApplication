using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SimulationSaveData
{
    public string sessionType;
    public string date;
    public string username;
    public string totalTime;
    public SerializableList<string> tasksTime;
    public string averageTimePerTask;
    public int puncturesCount;
    public float positionPrecision;
    public float inclinationPrecision;
    public int score;
    public string resultsQuestionnaire;

    public SimulationSaveData(string sessionType, string date, string username, string totalTime,  List<string> tasksTime, string averageTimePerTask,
        int puncturesCount, float positionPrecision, float inclinationPrecision, int score, string resultsQuestionnaire)
    {
        this.sessionType = sessionType;
        this.date = date;
        this.username = username;
        this.totalTime = totalTime;
        this.tasksTime = new SerializableList<string>(tasksTime);
        this.puncturesCount = puncturesCount;
        this.positionPrecision = positionPrecision;
        this.inclinationPrecision = inclinationPrecision;
        this.score = score;
        this.averageTimePerTask = averageTimePerTask;
        this.resultsQuestionnaire = resultsQuestionnaire;
    }

    public override string ToString()
    {
        return
            "Session Type" + sessionType + "\n" +
            "Date: " + date + "\n" +
            "username: " + username + "\n" +
            "Total time: " + totalTime + "\n" +
            "Punctures Count: " + puncturesCount + "\n" +
            "Position precision: " + positionPrecision + "\n" +
            "Inclination precision: " + inclinationPrecision + "\n" +
            "Score: " + score + "\n" +
            "Questionnaire: " + resultsQuestionnaire + "\n";    
    }
}

[Serializable]
public class SerializableList<T>
{
    public List<T> items;

    public SerializableList(List<T> list)
    {
        items = list;
    }
}