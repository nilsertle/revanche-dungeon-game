using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.Core;
using Revanche.Managers;

namespace Revanche.Screens.Game;

public class TargetRenderer
{
    public void Draw(SpriteBatch spriteBatch, LevelState levelState)
    {
        if (levelState.InTechDemo && !levelState.InAiDemo)
        {
            return;
        }

        if (levelState.Summoner.Path.Count > 0 && levelState.Summoner.CurrentState != CharacterState.Attacking)
        {
            DrawTarget(spriteBatch, levelState.Summoner.Path[0]);
        }

        foreach (var summon in levelState.FriendlySummons.Values)
        {
            if (summon.Path.Count > 0 && summon.CurrentState != CharacterState.Attacking)
            {
                DrawTarget(spriteBatch, summon.Path[0]);
            }
        }
    }

    private void DrawTarget(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.Draw(AssetManager.mSpriteSheet,
            position,
            AssetManager.GetRectangleFromId16(3048),
            Color.White,
            0f,
            Game1.mOrigin,
            Game1.mScale,
            SpriteEffects.None,
            0.45f);
    }
}