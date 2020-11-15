using System.IO;
using UnityEngine;

public static class DataSaver
{
    /// <summary>
    /// Save the data on the disk.
    /// </summary>
    /// <param name="dataToSave"> Data variables containing all the data to save on disk </param>
    public static void save(Data dataToSave)
    {
        string jsonData = JsonUtility.ToJson(dataToSave);

        StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/SaveFile.txt");
        sw.Write(jsonData);

        sw.Close();
    }

    /// <summary>
    /// Load the data from the disk and return them. Return null if there is no data.
    /// </summary>
    public static Data load()
    {
        if (File.Exists(Application.dataPath + "/SaveFile.txt"))
        {
            StreamReader sr = new StreamReader(Application.persistentDataPath + "/SaveFile.txt");
            string jsonData = sr.ReadToEnd();

            sr.Close();

            return JsonUtility.FromJson<Data>(jsonData);
        }

        return null;
    }

    /// <summary>
    /// Delete player's data.
    /// </summary>
    public static void deleteData()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveFile.txt"))
        {
            File.Delete(Application.persistentDataPath + "/SaveFile.txt");
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}
