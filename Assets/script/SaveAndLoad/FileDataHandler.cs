using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = "";

    private bool encryptData = false;
    private string encryptionKey = "encryptionKey"; // Replace with your own encryption key

    public FileDataHandler(string dataDirPath, string dataFileName,bool encryptData)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.encryptData = encryptData;
    }

    public void SaveData(GameData data)
    {
        string filePath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            string dataToStore = JsonUtility.ToJson(data, true);
            if (encryptData)
            {
                dataToStore = EncryptData(dataToStore);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving data: " + e.Message);

        }
    }

    public GameData LoadData()
    {
        string filePath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;

        if (File.Exists(filePath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                if (encryptData)
                {
                    dataToLoad = DecryptData(dataToLoad);
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data: " + e.Message);
            }
        }

        return loadedData;
    }

    public void DeleteData()
    {
       // Debug.Log("Deleting data file: ");
        string filePath = Path.Combine(dataDirPath, dataFileName);
       // Debug.Log("Deleting data file: " + filePath);
        try
        {
            if (File.Exists(filePath))
            {
               // Debug.Log("Deleting data file: " + filePath);
                File.Delete(filePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error deleting data file: " + e.Message);
        }
    }
    public string EncryptData(string data)
    {
        string encryptedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            encryptedData += (char)(data[i] ^ encryptionKey[i % encryptionKey.Length]);
        }
        return encryptedData;
    }
    public string DecryptData(string data)
    {
        return EncryptData(data); // Since encryption and decryption are the same in this example
    }
    public string LoadRawJson()
    {
        if (File.Exists(Path.Combine(dataDirPath, dataFileName)))
        {
            return File.ReadAllText(Path.Combine(dataDirPath, dataFileName));
        }
        return null;
    }
}
