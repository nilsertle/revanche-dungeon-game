using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Revanche.AchievementSystem;
using Revanche.Core;
using Revanche.Extensions;
using Revanche.GameObjects;
using Revanche.Managers;
using Revanche.Screens.Menu;
using Revanche.Screens.Menu.UIComponents;
using Revanche.Sound;

namespace Revanche.Screens.Game;

internal sealed class TalentScreen : MenuScreen
{
    private const int OffsetScale2 = 2;
    private const int OffsetScale3 = 3;
    private const int SkillThreshold3 = 3;
    private const int SkillPointLimit = 5;

    private const float OffsetScale25 = 2.5f;

    private readonly AssetManager mAssetManager;
    private readonly Dictionary<ElementType, int> mCurrentSkills;
    private readonly Action<TalentTreeResult> mSkillUpAction;
    private int mSkillPoints;
    private readonly int mSkillPointsBeginning;
    public TalentScreen(AssetManager assetManager, EventDispatcher eventDispatcher, Dictionary<ElementType, int> currentSkills, int skillPoints, Action<TalentTreeResult> skillUpAction)
    {
        UpdateLower = true;
        DrawLower = true;
        mScaleBackground = false;
        mAssetManager = assetManager;
        mEventDispatcher = eventDispatcher;
        mCurrentSkills = currentSkills.Copy();
        mSkillPoints = skillPoints;
        mSkillPointsBeginning = mSkillPoints;
        mSkillUpAction = skillUpAction;
        mBackgroundTexture = assetManager.mUpgradeBackground;
        CreateMenuElements();
    }
    
    protected override void CreateMenuElements()
    {
        var center = Game1.mCenter;
        var yOffset = new Vector2(0, 70);
        var xOffset = new Vector2(80, 0);

        var doneButton = new Button(mAssetManager.mBaseButtonTexture, center - new Vector2(0, 30), new Vector2(128, 64), true, "Zuweisen", mAssetManager.mFont);
        doneButton.Subscribe(OnDoneButtonClick);

        AddElementButton(center - yOffset - OffsetScale3 * xOffset, ElementType.Magic, (int)ElementIcon.Magic);
        AddElementButton(center + OffsetScale25 * yOffset + OffsetScale2 * xOffset, ElementType.Ghost, (int)ElementIcon.Ghost);
        AddElementButton(center + OffsetScale25 * yOffset - OffsetScale2 * xOffset, ElementType.Lightning, (int)ElementIcon.Lightning);
        AddElementButton(center - yOffset + OffsetScale3 * xOffset, ElementType.Water, (int)ElementIcon.Water);
        AddElementButton(center - OffsetScale3 * yOffset, ElementType.Fire, (int)ElementIcon.Fire);

        mMenuElements.Add(doneButton);
    }

    private void AssignSkillPoint(ElementType elementType)
    {
        if (mSkillPoints <= 0 || mCurrentSkills[elementType] >= SkillPointLimit)
        {
            mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.InvalidAction, null));
            return;
        }
        mEventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ButtonClick, null));
        mCurrentSkills[elementType] += 1;
        mSkillPoints--;
    }

    private void AddElementButton(Vector2 position, ElementType element, int iconId)
    {
        var buttonDescription = "";
        var buttonToolTip = "";
        switch (element)
        {
            case ElementType.Fire:
                buttonDescription = "Feuer";
                buttonToolTip = "Verbessert den Dämon" + "\n" + "Verstärkt Schaden des Feuerballs";
                break;
            case ElementType.Ghost:
                buttonDescription = "Geist";
                buttonToolTip = "Verbessert das Skelett" + "\n" + "Verstärkt maximale Manakapazität";
                break;
            case ElementType.Lightning:
                buttonDescription = "Donner";
                buttonToolTip = "Verbessert die Sturmwolke" + "\n" + "Verstärkt Geschwindigkeitszauber";
                break;
            case ElementType.Water:
                buttonDescription = "Wasser";
                buttonToolTip = "Verbessert das Wasserelementar" + "\n" + "Verstärkt Heilungszauber";
                break;
            case ElementType.Magic:
                buttonDescription = "Magie";
                buttonToolTip = "Verbessert den magischen Setzling" + "\n" + "Verstärkt Manaregeneration";
                break;
        }
        var elementButton = new Button(mAssetManager.mBaseButtonRoundTexture,
            position, new Vector2(128, 64),
            true,
            buttonDescription + "\n" + mCurrentSkills[element] + "/5",
            mAssetManager.mFont, iconId, position + new Vector2(30, 10), buttonToolTip);
        elementButton.Subscribe(() =>
        {
            AssignSkillPoint(element);
            elementButton.SetText(buttonDescription + "\n" + mCurrentSkills[element] + "/5");
        });
        mMenuElements.Add(elementButton);
    }

    private void OnDoneButtonClick()
    {
        mSkillUpAction(new TalentTreeResult(mCurrentSkills, mSkillPoints));
        SendAchievementProgress();
        mEventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        mEventDispatcher.SendAudioRequest(mSkillPoints != mSkillPointsBeginning
            ? new SoundEvent(SoundEffects.UpgradesConfirmed, null)
            : new SoundEvent(SoundEffects.ButtonClick, null));
    }

    private void SendAchievementProgress()
    {
        if(mCurrentSkills.Values.All(point => point >= SkillThreshold3)) 
        {
            mEventDispatcher.SendAchievementEvent(AchievementType.JackOfAllTrades);
        }

        if (mCurrentSkills[ElementType.Fire] == SkillPointLimit)
        {
            mEventDispatcher.SendAchievementEvent(AchievementType.Firestarter);
        }

        if (mCurrentSkills[ElementType.Water] == SkillPointLimit)
        {
            mEventDispatcher.SendAchievementEvent(AchievementType.FeuchterBandit);
        }

        if (mCurrentSkills[ElementType.Ghost] == SkillPointLimit)
        {
            mEventDispatcher.SendAchievementEvent(AchievementType.GeisterMeister);
        }

        if (mCurrentSkills[ElementType.Magic] == SkillPointLimit)
        {
            mEventDispatcher.SendAchievementEvent(AchievementType.DuBistEinZauberer);
        }

        if (mCurrentSkills[ElementType.Lightning] == SkillPointLimit)
        {
            mEventDispatcher.SendAchievementEvent(AchievementType.Potzblitz);
        }
    }
}

public sealed class TalentTreeResult
{
    internal Dictionary<ElementType, int> NewSkills { get; }
    internal int NewSkillPoints { get; }

    internal TalentTreeResult(Dictionary<ElementType, int> newSkills, int newSkillPoints)
    {
        NewSkills = newSkills;
        NewSkillPoints = newSkillPoints;
    }
}