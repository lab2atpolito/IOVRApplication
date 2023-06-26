using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SavingSystem
{
    public static SaveData LoadData()
    {
        Debug.Log("Trying to load save data.");
        //string path = Application.persistentDataPath + $"/savedData.json";
        /*if (File.Exists(path))
        {
            Debug.Log("Save data file has been found!");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            stream.Position = 0;

            SaveData data = (SaveData) formatter.Deserialize(stream);
            stream.Close();

            //string jsonText = File.ReadAllText(path);
            //SaveData data = JsonUtility.FromJson<SaveData>(jsonText);

            Debug.Log(data.ToString());
            return data;
        }
        else
        {
            Debug.Log("Save data file has not been found!");
            return new SaveData();
        }*/

        string saveDataFolderPath = Path.Combine(Application.persistentDataPath, "SaveData");
        Debug.Log(saveDataFolderPath);

        if (!Directory.Exists(saveDataFolderPath))
        {
            Debug.Log("SaveData folder not found.");
            return null;
        }

        SaveData saveData = new SaveData();

        string[] userFolders = Directory.GetDirectories(saveDataFolderPath);

        foreach (string userFolder in userFolders)
        {
            string[] saveFiles = Directory.GetFiles(userFolder, "*.json");

            foreach (string saveFile in saveFiles)
            {
                string jsonData = File.ReadAllText(saveFile);
                SimulationSaveData simulationData = JsonUtility.FromJson<SimulationSaveData>(jsonData);
                saveData.AddSimulationData(simulationData);
            }
        }

        return saveData;
    }

    public static void Save(string username, SimulationSaveData simulationData)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "SaveData", username);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        int simulationNumber = Directory.GetFiles(folderPath, "*.json").Length;

        string filePath = Path.Combine(folderPath, "simulation_" + simulationNumber + ".json");
        string jsonData = JsonUtility.ToJson(simulationData);

        File.WriteAllText(filePath, jsonData);

        /*BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"/savedData.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();*/

        //string path = Application.persistentDataPath + $"/savedData.json";
        //string jsonText = JsonUtility.ToJson(data, true);
        //File.WriteAllText(path, jsonText);

        Debug.Log("Saving user data... a new file has been created!");
    }

    public static void DeleteSave(string username)
    {
        /*string path = Application.persistentDataPath + $"/savedData.sav";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.Log("Save file not found.");
        }*/

        string userFolderPath = Path.Combine(Application.persistentDataPath, "SaveData", username);
        if (!Directory.Exists(userFolderPath))
        {
            Debug.Log("User folder not found.");
            return;
        }
        string[] saveFiles = Directory.GetFiles(userFolderPath, "*.json");
        foreach (string saveFile in saveFiles)
        {
            File.Delete(saveFile);
        }
        Debug.Log("Save files deleted for user: " + username);
    }
}
