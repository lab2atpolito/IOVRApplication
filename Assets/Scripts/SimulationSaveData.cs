using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SimulationSaveData
{
    public string date;
    public string username;
    public string totalTime;
    public SerializableList<string> tasksTime;
    public string averageTimePerTask;
    public float positionPrecision;
    public float inclinationPrecision;
    public int score;

    public SimulationSaveData(string date, string username, string totalTime,  List<string> tasksTime, string averageTimePerTask,
        float positionPrecision, float inclinationPrecision, int score)
    {
        this.date = date;
        this.username = username;
        this.totalTime = totalTime;
        this.tasksTime = new SerializableList<string>(tasksTime);
        this.positionPrecision = positionPrecision;
        this.inclinationPrecision = inclinationPrecision;
        this.score = score;
        this.averageTimePerTask = averageTimePerTask;
    }

    public override string ToString()
    {
        return
            "Date: " + date + "\n" +
            "username: " + username + "\n" +
            "Total time: " + totalTime + "\n" +
            "Position precision: " + positionPrecision + "\n" +
            "Inclination precision: " + inclinationPrecision + "\n" +
            "Score: " + score + "\n";
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