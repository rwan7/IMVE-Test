using UnityEngine;
using System.IO;

public class SaveManager
{
    private string saveFilePath;

    public SaveManager()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");
    }

    public void SaveTimeSurvived(float timeSurvived)
    {
        float highestTime = LoadHighestTimeSurvived();

        // Check if the current timeSurvived is greater than the highest recorded time.
        if (timeSurvived > highestTime)
        {
            highestTime = timeSurvived;
        }

        SaveData data = new SaveData { TimeSurvived = timeSurvived, HighestTimeSurvived = highestTime };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
    }

    public float LoadTimeSurvived()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data.TimeSurvived;
        }
        return 0f;
    }

    public float LoadHighestTimeSurvived()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data.HighestTimeSurvived;
        }
        return 0f;
    }

    [System.Serializable]
    private class SaveData
    {
        public float TimeSurvived;
        public float HighestTimeSurvived;
    }
}
