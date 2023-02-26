using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Revanche.Core;

namespace Revanche.Managers;

public sealed class SaveManager
{
    private static readonly JsonSerializerSettings sSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto
    };
    private const string DirectoryName = "Saves";
    private string mCurrentSaveGame;

    internal SaveManager()
    {
        Directory.CreateDirectory(DirectoryName);
        ResetCurrentSave();
    }

    internal LevelState LoadGame(string file)
    {
        mCurrentSaveGame = file;
        var saveString = File.ReadAllText(file);
        return JsonConvert.DeserializeObject<LevelState>(saveString, sSettings);
    }

    internal void SaveGame(LevelState levelState)
    {
        File.WriteAllText( mCurrentSaveGame, JsonConvert.SerializeObject(levelState, Formatting.Indented, sSettings));
    }

    internal void ResetCurrentSave()
    {
        var files = Directory.GetFiles(DirectoryName);
        var saveNumbers = files.Select(file =>
            Convert.ToInt32(file.Substring(10, file.IndexOf('.') - 10))).ToList();
        var saveNumber = saveNumbers.Any() ? saveNumbers.Max() + 1 : 1;
        mCurrentSaveGame = "Saves\\Save" + saveNumber + ".json";
    }

    internal void DeleteSave(string file)
    {
        File.Delete(file);
    }
}