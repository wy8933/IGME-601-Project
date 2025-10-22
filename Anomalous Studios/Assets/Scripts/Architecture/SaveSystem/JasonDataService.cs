using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class JasonDataService : IDataService
{
    public T LoadData<T>(string relativePath, bool encrypted)
    {
        throw new System.NotImplementedException();
    }

    public bool SaveData<T>(string relativePath, T data, bool encrypted)
    {
        string path = Application.persistentDataPath + relativePath;

        try
        {
            if (File.Exists(path))
            {
                Debug.Log("Data exist, deleting old file and writing a new one!");
                File.Delete(path);
            }
            else 
            {
                Debug.Log("Writing file for the first time");
            }

                using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(data));
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data due to :{e.Message} {e.StackTrace}");
            return false;
        }
       
    }
}
