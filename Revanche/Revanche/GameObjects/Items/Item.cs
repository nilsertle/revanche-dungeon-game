using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Revanche.Core;

namespace Revanche.GameObjects.Items;

public abstract class Item : GameObject, IUsableItem
{
    private const int RegenerationStrength = 3;
    private const int RegenerationDuration = 5;
    private const int SpeedStrength = 5;
    private const int SpeedDuration = 7;
    private const int DamageStrength = 50;
    private const int DamageDuration = 20;

    private const int PermanentSpeedIncrease = 50;

    private const float HealPotionCoefficient = 0.15f;

    private const float ItemLayer = 0.6f;

    [JsonProperty] public List<ItemEffects> Effects {get; private set;}

    protected Item(Vector2 position, List<ItemEffects> effects) : base(position)
    {
        Effects = effects;
        CurrentSpriteId = SpriteId;
        LayerDepth = ItemLayer;
    }

    public void Use(Character character)
    {
        foreach (var effect in Effects)
        {
            switch (effect)
            {
                case ItemEffects.HealingEffect:
                    var newHp = character.CurrentLifePoints + character.MaxLifePoints * HealPotionCoefficient;
                    character.CurrentLifePoints = (int)Math.Clamp(newHp, 1, character.MaxLifePoints);
                    break;
                case ItemEffects.SpeedEffect:
                    character.Velocity += PermanentSpeedIncrease;
                    break;
                case ItemEffects.TimedHealingEffect:
                    character.HealUp = TemporaryEffect.CreateTemporaryEffect(RegenerationDuration, RegenerationStrength);
                    break;
                case ItemEffects.TimedSpeedUpEffect:
                    character.SpeedUp = TemporaryEffect.CreateTemporaryEffect(SpeedDuration, SpeedStrength);
                    break;
                case ItemEffects.TimedDamageUpEffect:
                    character.DamageUp = TemporaryEffect.CreateTemporaryEffect(DamageDuration, DamageStrength);
                    break;
            }
        }
    }
}
