using Newtonsoft.Json;

namespace Revanche.AchievementSystem;

public sealed class Achievement
{
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Description { get; private set; }
    [JsonProperty] public string Hint { get; private set; }
    [JsonProperty] public int Progress { get; set; }
    [JsonProperty] public int UnlockThreshold { get; private set; }
    [JsonIgnore] public bool IsUnlocked => Progress >= UnlockThreshold;

    public Achievement(string name, string description, string hint, int unlockThreshold, int progress=0)
    {
        Name = name;
        Description = description;
        Hint = hint;
        Progress = progress;
        UnlockThreshold = unlockThreshold;
    }
}