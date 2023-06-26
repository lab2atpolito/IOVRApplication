using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public Dictionary<string, List<SimulationSaveData>> savedData;

    public SaveData()
    {
        savedData = new Dictionary<string, List<SimulationSaveData>>();
    }

    // Add user's information to saved data
    public void AddSimulationData(SimulationSaveData data)
    {
        string userName = data.username;

        if (savedData.ContainsKey(userName))
        {
            savedData[userName].Add(data);
        }
        else
        {
            List<SimulationSaveData> newList = new List<SimulationSaveData>();
            newList.Add(data);
            savedData.Add(userName, newList);
        }
    }

    // Get user specific information from saved data
    public List<SimulationSaveData> GetUserData(string username)
    {
        if (savedData.ContainsKey(username))
        {
            return savedData[username];
        }
        else
        {
            Debug.LogWarning("User not found.");
            return null;
        }
    }

    public List<string> GetAllUsernames()
    {
        if( savedData.Keys.Count != 0)
        {
            List<string> usernames = new List<string>(savedData.Keys);
            return usernames;
        }
        else
        {
            return null;
        }
    }

    // Remove user's information from saved data
    public void ClearUserSaveData(string username)
    {
        if (savedData.ContainsKey(username))
        {
            savedData.Remove(username);
        }
        else
        {
            Debug.LogWarning("User not found.");
        }
    }

    public override string ToString()
    {
        string res = "";
        foreach(string user in new List<string>(savedData.Keys))
        {
            res += user + "\n";
            List<SimulationSaveData> list = savedData[user];
            foreach(SimulationSaveData simData in list)
            {
                res += simData.ToString() + "\n";
            }
        }
        return res;
    }
}
