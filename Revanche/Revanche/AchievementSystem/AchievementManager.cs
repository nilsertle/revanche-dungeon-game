using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Revanche.Managers;
using AchievementMap = System.Collections.Generic.Dictionary<Revanche.AchievementSystem.AchievementType, Revanche.AchievementSystem.Achievement>;

namespace Revanche.AchievementSystem;

public sealed class AchievementManager
{
    private const int UnlockThreshold100 = 100;
    private const int UnlockThreshold5 = 5;
    private const int UnlockThreshold50 = 50;

    private AchievementMap mAchievements;
    private readonly EventDispatcher mEventDispatcher;
    private const string DirectoryName = "Game";
    private const string PathName = DirectoryName + "\\Achievements.json";
    public List<Achievement> Achievements => mAchievements.Values.ToList();

    public AchievementManager(EventDispatcher eventDispatcher)
    {
        Directory.CreateDirectory(DirectoryName);
        mEventDispatcher = eventDispatcher;
        mEventDispatcher.OnAchievementEvent += OnAchievementEventReceived;
        mAchievements = LoadAchievements();
    }

    private static AchievementMap CreateEmptyAchievements()
    {
        return new AchievementMap
        { 
            { AchievementType.JackTheRipper, new Achievement("Jonas der Reisser", "Jetzt auch in Deutsch!","Gegner besiegt" , UnlockThreshold100) },
            { AchievementType.DungeonExplorer, new Achievement("Dungeon Explorer", "Backpack Rucksack!","Ebenen erkundet", UnlockThreshold5)},
            { AchievementType.JackOfAllTrades, new Achievement("Jakob Jeden Gewerbes", "Alles können... aber nur so halb.","Jedes Talent auf Stufe 3", 1)},
            { AchievementType.KarnickelAusDemHut, new Achievement("Karnickel aus dem Hut", "Und für meinen nächsten Trick...!","Monster Beschworen", UnlockThreshold100)},
            { AchievementType.Nimmersatt, new Achievement("Nimmersatt", "Die kleine Raupe zieht ihren Hut vor dir","Tränke getrunken", UnlockThreshold50)},
            { AchievementType.MeinPersönlicherTiefpunkt, new Achievement("Mein persönlicher Tiefpunkt", "Es kann nur noch Bergauf gehen!","Habe genau 0 Mana", 1)},
            { AchievementType.EndlichFrei, new Achievement("Endlich... frei!", "Meine Ziege!","Gewinne ein Spiel", 1)},
            { AchievementType.Firestarter, new Achievement("Firestarter", "Mit Feuer spielt man!","Feuer auf talent Stufe 5", 1)},
            { AchievementType.FeuchterBandit, new Achievement("Feuchter Bandit", "Und denkt dran, wir sind die feuchten Banditen!","Wasser Talent auf Stufe 5", 1)},
            { AchievementType.GeisterMeister, new Achievement("Geistermeister", "Who you gonna call?","Geist Talent auf Stufe 5", 1)},
            { AchievementType.Potzblitz, new Achievement("Potzblitz", "1.21 Gigawatt Marty!","Donner Talent Stufe 5", 1)},
            { AchievementType.DuBistEinZauberer, new Achievement("Du bist ein Zauberer Harry!", "50 Punkte für Gryffindor","Magie auf Talent Stufe 5", 1)},
        };

    }

    private static AchievementMap LoadAchievements()
    {
        if (!File.Exists(PathName))
        {
            return CreateEmptyAchievements();
        }

        var saveString = File.ReadAllText(PathName);
        return JsonConvert.DeserializeObject<AchievementMap>(saveString);

    }

    public  void SaveAchievements()
    {
         File.WriteAllText(PathName, JsonConvert.SerializeObject(mAchievements, Formatting.Indented));
    }

    private void OnAchievementEventReceived(AchievementType type, int increment)
    {
        if (mAchievements[type].IsUnlocked)
        {
            return;
        }

        mAchievements[type].Progress += increment;

        if (mAchievements[type].IsUnlocked)
        {
            mEventDispatcher.SendPopupEvent(new IPopupEvent.AchievementPopup(mAchievements[type]));
        }
    }

    public void ClearAchievements()
    {
        mAchievements = CreateEmptyAchievements();
        SaveAchievements();
    }
}