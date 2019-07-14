using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager
{
    public static string Saves_Directory
    {
        get
        {
            var path = $"{Application.dataPath}/LocalData";

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }

    private const string file_extention = ".json";

    public static void Save(SavedTiles tiles, string saveName = "0")
    {
        // Save all tiles on screen
        var data = JsonUtility.ToJson(tiles);

        // Create a file to locate these saves later
        File.WriteAllText(Saves_Directory + $"/{saveName}" + file_extention, data);
    }

    public static string[] LoadAllSaves()
    {
        var list = new List<string>();

        foreach (var file in Directory.EnumerateFiles(Saves_Directory))
        {
            var fileSplit = file.Split('\\');
            var fileNameWithExtension = fileSplit[fileSplit.Length - 1];

            if (fileNameWithExtension.ToLower().Contains("meta")) continue;

            var fileName = fileNameWithExtension.Split('.').FirstOrDefault();

            list.Add(fileName);
        }

        return list.ToArray();
    }

    public static SavedTiles Load(string saveName = "0")
    {
        var filePath = Saves_Directory + $"/{saveName}" + file_extention;

        SavedTiles loadedTiles = null;

        // Look into the saves folder and pick out the save
        if (File.Exists(filePath) == false)
        {
            Debug.LogError($"{filePath} does not exist! Please try again.");

            return loadedTiles;
        }

        // Grab the save and decode it.
        var data = File.ReadAllText(filePath);

        loadedTiles = JsonUtility.FromJson<SavedTiles>(data);

        return loadedTiles;
    }

    public class SavedTiles
    {
        public Vector2[] tiles;
    }
}