using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Revanche.GameObjects;

public abstract class MainCharacter : Character
{
    [JsonProperty] public int MaxXp { get; protected set; }
    [JsonProperty] public int AccumulatedXp { get; protected set; } // = 0; // by default
    [JsonProperty] public int XpLevel { get; protected set; } = 1;
    [JsonProperty] public int MaxMana { get; protected set; }
    [JsonProperty] public int CurrentMana { get; set; }
    [JsonProperty] public Dictionary<ElementType,int> Skills { get; set; }

    protected MainCharacter(
        Vector2 position) : base(position)
    {
    }

    protected int McAddIntScaling(int baseVal, float increment)
    {
        return (int)(baseVal + increment * (XpLevel - 1));
    }

    protected float McAddFloatScaling(float baseVal, float increment)
    {
        return baseVal + increment * (XpLevel - 1);
    }
}