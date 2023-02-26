using System.Collections.Generic;
using Newtonsoft.Json;
using Revanche.Managers;
using System.IO;

namespace Revanche.Stats;

public sealed class StatisticManager
{
    private const string DirectoryName = "Game";
    private const string PathName = DirectoryName + "\\Stats.json";
    private readonly Dictionary<StatisticType, int> mStats;
    public IReadOnlyDictionary<StatisticType, int> Stats => mStats;

    public StatisticManager(EventDispatcher eventDispatcher)
    {
        Directory.CreateDirectory(DirectoryName);
        eventDispatcher.OnStatisticEvent += OnStatEventReceived;
        mStats = LoadStats();
    }

    private static Dictionary<StatisticType, int> CreateEmptyStats()
    {
        return new Dictionary<StatisticType, int>
        {
            {StatisticType.AccumulatedPoints, 0},
            {StatisticType.DefeatedEnemies, 0},
            {StatisticType.ExploredFloors, 0},
            {StatisticType.GainedExp, 0},
            {StatisticType.SummonedMonsters, 0},
            {StatisticType.DefeatedBosses, 0}
        };
    }

    private Dictionary<StatisticType, int> LoadStats()
    {
        if (!File.Exists(PathName))
        {
            return CreateEmptyStats();
        }

        var saveString = File.ReadAllText(PathName);
        return JsonConvert.DeserializeObject<Dictionary<StatisticType, int>>(saveString);

    }

    public void SaveStats()
    {
        File.WriteAllText(PathName, JsonConvert.SerializeObject(mStats, Formatting.Indented));
    }

    private void OnStatEventReceived(StatisticType type, int increment)
    {
        mStats[type] += increment;
    }
}