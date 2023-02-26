using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.Core;
using Revanche.GameObjects;
using Revanche.GameObjects.FriendlyUnits;
using Revanche.Screens.Menu.UIComponents;
namespace Revanche.Managers;

internal sealed class HudManager
{
    private readonly List<MenuElement> mHudObjects;
    private readonly AssetManager mAssetManager;
    private Pane mHudPane;
    private readonly LevelState mLevelState;
    private Vector2 mDimensions;

    public HudManager(LevelState levelState, AssetManager assetManager)
    {
        mLevelState = levelState;
        mAssetManager = assetManager;
        mHudObjects = new List<MenuElement>();
    }

    private Vector2 SetDimensions()
    {
        if (Game1.mFullScreen)
        {
            var height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            var width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            mDimensions = new Vector2(width, height);
        }
        else
        {
            var height = Game1.mScreenHeight;
            var width = Game1.mScreenWidth;
            mDimensions = new Vector2(width, height);
        }
        return mDimensions;
    }
    private List<float> GetStatusMc(MainCharacter mainCharacter)
    {
        var charLife = (float)mainCharacter.CurrentLifePoints / mainCharacter.MaxLifePoints;
        var charXp = (float)(mainCharacter.AccumulatedXp % mainCharacter.MaxXp) / mainCharacter.MaxXp;
        var charMana = (float)mainCharacter.CurrentMana / mainCharacter.MaxMana;
        return new List<float> { charLife, charXp, charMana };
    }

    private List<List<Color>> GetColors()
    {
        var health = new List<Color> { Color.DarkRed, Color.DarkGreen };
        var xp = new List<Color> { Color.LightCoral, Color.MediumVioletRed };
        var mana = new List<Color> { Color.LightBlue, Color.DarkSlateBlue };
        var colors = new List<List<Color>> { health, xp, mana };
        return colors;
    }
    private void DrawStatusBarsMc(SpriteBatch spriteBatch, MainCharacter mainCharacter, int i)
    {
        var y = mDimensions.Y / 2 - (i + 1) * 65;
        var x = mDimensions.X - 100;
        var info = GetStatusMc(mainCharacter);
        var colors = GetColors();

        const string charString = "Leben:\nEP:\nMana:"; // removed $
        spriteBatch.DrawString(AssetManager.mHudFont, charString, new Vector2(x - 38, 35), Color.Indigo);

        var space = y - 35;
        for (var k = 0; k < info.Count; k++)
        {
            var pos = new Vector2(x + 10, y - space + (3 - k) - (k - 1));
            var rect = new Rectangle((int)x + 5, (int)y, 70, 10);
            spriteBatch.Draw(AssetManager.mHudPaneBar, pos, rect, colors[k][0]);
            spriteBatch.Draw(AssetManager.mHudPaneBar, pos, new Rectangle((int)x + 5, (int)y, (int)(info[k] * 70), 10),
                colors[k][1]);
            space -= 20;
        }
    }

    private void DrawStatusBarsSummon(SpriteBatch spriteBatch, Character summon, int i) // Changed Summon to Character
    {
        // Just wrote this function in case the Summons get other attributes to display
        var info = (float)summon.CurrentLifePoints / summon.MaxLifePoints;
        const string charString = "Leben:";
        spriteBatch.DrawString(AssetManager.mHudFont, charString, new Vector2(mDimensions.X - 138, 40 - (i + 1) * 65), Color.Indigo);
        var pos = new Vector2(mDimensions.X - 93 + 3, 40 - (i + 1) * 65 + 4);
        var rect = new Rectangle((int)mDimensions.X - 93, 40 - (i + 1) * 65, 70, 10);
        spriteBatch.Draw(AssetManager.mHudPaneBar, pos, rect, Color.DarkRed);
        spriteBatch.Draw(AssetManager.mHudPaneBar, pos, new Rectangle((int)mDimensions.X - 93, 40 - (i + 1) * 65, (int)(info * 70), 10),
            Color.DarkGreen);
    }

    private void DrawIcon(SpriteBatch spriteBatch, Character character, int i)
    {
        var sprite = character.Selected ? character.SpriteId + 32 + character.DrawLevel * 64 : character.SpriteId + character.DrawLevel * 64;
        spriteBatch.Draw(AssetManager.mSpriteSheet, destinationRectangle: new Rectangle((int)(mDimensions.X - 180 - 5), 40 - (i + 1) * 65, width: 45, height: 45), AssetManager.GetRectangleFromId16(sprite), Color.White);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        mDimensions = SetDimensions();
        mHudPane = new Pane(mHudObjects, new Vector2(200, mDimensions.Y),
            new Vector2(mDimensions.X - 200, mDimensions.Y / 2), mAssetManager.mHudPaneBackground);
        mHudPane.Draw(spriteBatch, new Rectangle((int)mDimensions.X - 200, 0, 200, (int)mDimensions.Y));
        var i = -1;
        DrawStatusBarsMc(spriteBatch, mLevelState.Summoner, i);
        DrawIcon(spriteBatch, mLevelState.Summoner, i);
        DrawSouls(spriteBatch);
        i--;
        var j = 0;
        foreach (var character in mLevelState.FriendlySummons)
        {
            if (j == 5)
            {
                break;
            }

            DrawIcon(spriteBatch, character.Value, i);
            DrawStatusBarsSummon(spriteBatch, character.Value, i);
            i--;
            j++;
        }
        DrawSkills(spriteBatch);
        DrawSpell(spriteBatch, mLevelState.Summoner.EnoughManaFireBall, (int)SpellIcon.FireBall, 
            Summoner.CoolDownLimitFire, mLevelState.Summoner.FireBallCoolDown, 0);
        DrawSpell(spriteBatch, mLevelState.Summoner.EnoughManaSpeed, (int)SpellIcon.SpeedSpell, 
            Summoner.CoolDownLimitSpeed, mLevelState.Summoner.SpeedSpellCoolDown, 55);
        DrawSpell(spriteBatch, mLevelState.Summoner.EnoughHealthHealing, (int)SpellIcon.HealingSpell, 
            Summoner.CoolDownLimitHeal, mLevelState.Summoner.HealingSpellCoolDown, 110);
        DrawSpellKey(spriteBatch, 3042, 0);
        DrawSpellKey(spriteBatch, 3043, 55);
        DrawSpellKey(spriteBatch, 3044, 110);

    }

    private void DrawSouls(SpriteBatch spriteBatch)
    {
        var charString = $"Seelen: {mLevelState.Summoner.Souls}";
        spriteBatch.DrawString(AssetManager.mHudFont, charString, new Vector2(mDimensions.X - 180, mDimensions.Y - 150 + 40), Color.Indigo);
    }

    /*
    private void HudInit() //These are causing problems when the screensize changes, so I'll comment it out for now
    {
        //Created Buttons so that maybe the level up etc. can work through there like a menu
        mHudObjects.Add(item: CreateButton());
        var len = 5;
        for (int i = 0; i<len; i++)
        {
            mHudObjects.Add(CreateButton());
        }
    }
    */

    private void DrawSkillIcons(SpriteBatch spriteBatch)
    {
        var spriteIds = new List<int> { (int)ElementIcon.Fire, (int)ElementIcon.Ghost, (int)ElementIcon.Lightning, (int)ElementIcon.Water, (int)ElementIcon.Magic };
        var skills = mLevelState.Summoner.Skills.Values.ToList();
        int j = 0;
        foreach (var spriteId in spriteIds)
        {
            spriteBatch.Draw(AssetManager.mSpriteSheet, new Vector2(mDimensions.X - 180, mDimensions.Y / 2 + 60 + (j * 18) + 10), AssetManager.GetRectangleFromId16(spriteId + 64), 1f * Color.White);

            for (var i = 0; i < 5; i++) //Drawing empty skill slots
            {
                spriteBatch.Draw(AssetManager.mSpriteSheet, new Vector2(mDimensions.X - 105 + i * 18, mDimensions.Y / 2 + 60 + (j * 18) + 10), AssetManager.GetRectangleFromId16(spriteId), 0.2f * Color.White);
            }
            for (var i = 0; i < skills[j]; i++) //Drawing current skills
            {
                spriteBatch.Draw(AssetManager.mSpriteSheet, new Vector2(mDimensions.X - 105 + i * 18, mDimensions.Y / 2 + 60 + (j * 18) + 10), AssetManager.GetRectangleFromId16(spriteId), 1f * Color.White);
            }
            j++;
        }
    }

    private void DrawSkills(SpriteBatch spriteBatch)
    {
        DrawSkillIcons(spriteBatch);
        var skillStrings = new List<String> { "Talente", "Talentpunkte frei:" + mLevelState.Summoner.SkillPoints, "Feuer:\nGeist:\nDonner:\nWasser:\nMagie:" };
        /*var skillStrings = new List<String> { "Skills", "Unassigned:" + mLevelState.Summoner.SkillPoints };
        foreach (var element in mLevelState.Summoner.Skills.Keys)
        {
            skillStrings.Add(element.ToString());
        }
        */
        for (var i = 1; i < skillStrings.Count + 1; i++)
        {
            if (i == 1 || i == 2)
            {
                spriteBatch.DrawString(AssetManager.mHudFont, skillStrings[i - 1], new Vector2(mDimensions.X - 180, mDimensions.Y / 2 + i * 20 + 10), Color.Indigo);
                continue;

            }
            spriteBatch.DrawString(AssetManager.mHudFont, skillStrings[i - 1], new Vector2(mDimensions.X - 160, mDimensions.Y / 2 + i * 20 + 10), Color.Indigo);
        }
    }

    private void DrawSpell(SpriteBatch spriteBatch, bool enoughMana, int spriteNumber, int coolDownLimit, float actualCoolDown, int scaling)
    {
        if (!enoughMana)
        {
            //spriteBatch.Draw(AssetManager.mSpriteSheet, new Vector2(mDimensions.X - 180, mDimensions.Y / 2 + scaling), 
            //    AssetManager.GetRectangleFromId16(spriteNumber), 1f * Color.White);
            spriteBatch.Draw(AssetManager.mSpriteSheet, 
                destinationRectangle: new Rectangle((int)(mDimensions.X - 180 + scaling), (int)mDimensions.Y / 2 + 150 + 10, width: 45, height: 45), 
                AssetManager.GetRectangleFromId16(spriteNumber), Color.White);
            return;
        }
        
        if (actualCoolDown < 0.125f * coolDownLimit)
        {
            //spriteBatch.Draw(AssetManager.mSpriteSheet, new Vector2(mDimensions.X - 180, mDimensions.Y / 2 + scaling), 
            //    AssetManager.GetRectangleFromId16(spriteNumber), 1f * Color.White);
            spriteBatch.Draw(AssetManager.mSpriteSheet, 
                destinationRectangle: new Rectangle((int)(mDimensions.X - 180 + scaling), (int)mDimensions.Y / 2 + 150 + 10, width: 45, height: 45), 
                AssetManager.GetRectangleFromId16(spriteNumber), Color.White);
            return;
        }
        for (int i = 0; i < 8; i++)
        {
            if (actualCoolDown > 0.125f * (i + 1) * coolDownLimit 
                && actualCoolDown < 0.125f * (i + 2) * coolDownLimit)
            {
                //spriteBatch.Draw(AssetManager.mSpriteSheet, new Vector2(mDimensions.X - 180, mDimensions.Y / 2 + scaling), 
                //    AssetManager.GetRectangleFromId16(spriteNumber + i), 1f * Color.White);
                spriteBatch.Draw(AssetManager.mSpriteSheet, 
                    destinationRectangle: new Rectangle((int)(mDimensions.X - 180 + scaling), (int)mDimensions.Y / 2 + 150 + 10, width: 45, height: 45), 
                    AssetManager.GetRectangleFromId16(spriteNumber + i), Color.White);
                return;
            }
        }

        if (actualCoolDown >= coolDownLimit)
        {
            //spriteBatch.Draw(AssetManager.mSpriteSheet, new Vector2(mDimensions.X - 180, mDimensions.Y / 2 + scaling), 
            //    AssetManager.GetRectangleFromId16(spriteNumber + 7), 1f * Color.White);
            spriteBatch.Draw(AssetManager.mSpriteSheet, 
                destinationRectangle: new Rectangle((int)(mDimensions.X - 180), (int)mDimensions.Y / 2 + 150 + 10, width: 45, height: 45), 
                AssetManager.GetRectangleFromId16(spriteNumber + 7), Color.White);
        }
    }

    private void DrawSpellKey(SpriteBatch spriteBatch, int spriteId, int offset)
    {
        spriteBatch.Draw(AssetManager.mSpriteSheet,
            destinationRectangle: new Rectangle((int)(mDimensions.X - 180 + offset), (int)mDimensions.Y / 2 + 150 + 60, width: 45, height: 45),
            AssetManager.GetRectangleFromId16(spriteId + 7), Color.White);
    }

}