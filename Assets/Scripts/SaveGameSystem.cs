using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveGameSystem
{
    public static bool SaveGameToFile(SaveGame saveGame, int slot=1)
    {
        string filePath = GetSavePath(slot);
        string dataAsJson = JsonUtility.ToJson(saveGame);
        try
        {
            File.WriteAllText(filePath, dataAsJson);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static SaveGame LoadGameFromFile(int slot=1)
    {
        if (!DoesSaveGameFileExist(slot))
        {
            return null;
        }

        string filePath = GetSavePath(slot);
        try
        {
            string dataAsJson = File.ReadAllText(filePath);
            SaveGame saveGame = JsonUtility.FromJson<SaveGame>(dataAsJson);
            return saveGame;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static bool DeleteSaveGameFile(int slot=1)
    {
        try
        {
            File.Delete(GetSavePath(slot));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool DoesSaveGameFileExist(int slot=1)
    {
        return File.Exists(GetSavePath(slot));
    }

    private static string GetSavePath(int slot=1)
    {
        return Path.Combine(Application.persistentDataPath, "save" + slot + ".sav");
    }
}
