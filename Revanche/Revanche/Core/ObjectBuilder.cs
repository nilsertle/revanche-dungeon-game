using Revanche.GameObjects.HostileUnits;
using Revanche.GameObjects.Items;
using Revanche.GameObjects;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Revanche.GameObjects.FriendlyUnits;
using Revanche.Map;
using System;
using Revanche.GameObjects.Environment;

namespace Revanche.Core;

/// <summary>
/// This class merely holds some functions to
/// initialize an empty level for now to remove clutter
/// from the level State
/// </summary>
internal static class ObjectBuilder
{
    private const int I0 = 0;
    private const int I1 = 1;
    private const int I2 = 2;
    private const int I3 = 3;
    private const int I4 = 4;
    private const int I5 = 5;
    private const int I6 = 6;
    private const int I7 = 7;
    private const int I9 = 9;
    private const int I10 = 10;
    private const int I11 = 11;
    private const int I12 = 12;
    private const int I13 = 13;
    private const int I14 = 14;

    private const int I17 = 17;

    private const int I33 = 33;

    private const int PerHundred = 100;

    private const int PerThousand = 1000;

    private const float F2 = 2f;

    private const int EmptyRoomSummonLimit = 7;
    private const int RoomHeight = 15;
    private const int RoomWidth = 19;

    private const int ChestChance = 40;

    private const int TechDemoEnemyY = 20;
    private const int TechDemoX = 25;
    private const int TechDemoFriendlyY = 40;


    internal static Dictionary<string, GameObject> CreateDefaultItems(List<Room> roomList)
    {
        var rand = new Random((int)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds % int.MaxValue));
        var outList = new List<GameObject>();
        var referencePoint = new Vector2((float)((roomList[0].mMid.mX + 0.5f) * Game1.sScaledPixelSize), (float)((roomList[0].mMid.mY + 0.5f) * Game1.sScaledPixelSize));
        for (var i = I2; i < roomList.Count; i++)
        {
            if (rand.Next() % PerHundred < ChestChance) 
            {
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(new Vector2((int)roomList[i].mTopLeftCorner.mX + I10, (int)roomList[i].mTopLeftCorner.mY + I10))));
            }
        }
        outList.Add(new DamagePotion(referencePoint + new Vector2(-I1, I4) * Game1.sScaledPixelSize));
        outList.Add(new HealthPotion(referencePoint + new Vector2(I0, I4) * Game1.sScaledPixelSize));
        outList.Add(new SpeedPotion(referencePoint + new Vector2(I1, I4) * Game1.sScaledPixelSize));

        outList.Add(new Soul(referencePoint + new Vector2(-I1, I5) * Game1.sScaledPixelSize));
        outList.Add(new Soul(referencePoint + new Vector2(I0, I5) * Game1.sScaledPixelSize));
        outList.Add(new Soul(referencePoint + new Vector2(I1, I5) * Game1.sScaledPixelSize));

        outList.Add(new TreasureChest(referencePoint + new Vector2(0, I6) * Game1.sScaledPixelSize));

        outList.Add(new TreasureChest(Camera.TileCenterToWorld(new Vector2((int)roomList[1].mTopLeftCorner.mX + I17, (int)roomList[1].mTopLeftCorner.mY + I14))));
        outList.Add(new TreasureChest(Camera.TileCenterToWorld(new Vector2((int)roomList[1].mTopLeftCorner.mX + I1, (int)roomList[1].mTopLeftCorner.mY + I14))));
        outList.Add(new TreasureChest(Camera.TileCenterToWorld(new Vector2((int)roomList[1].mTopLeftCorner.mX + I17, (int)roomList[1].mTopLeftCorner.mY))));
        outList.Add(new TreasureChest(Camera.TileCenterToWorld(new Vector2((int)roomList[1].mTopLeftCorner.mX + 1, (int)roomList[1].mTopLeftCorner.mY))));

        return outList.ToDictionary(summon => summon.Id, summon => summon);
    }


    internal static Dictionary<string, Summon> CreateTechDemoFriendlies(Vector2 roomPos)
    {
        var outList = new List<Summon>();
        for (var y = 0; y < TechDemoFriendlyY; y++)
        {
            for (var x = 0; x < TechDemoX; x++)
            {
                Summon enemy = new Demon(new Vector2((roomPos.X + I1 + x) * Game1.sScaledPixelSize + Game1.sScaledPixelSize / F2,
                    (roomPos.Y + I1 + y) * Game1.sScaledPixelSize + Game1.sScaledPixelSize / F2), I1);
                outList.Add(enemy);
            }
        }

        return outList.ToDictionary(summon => summon.Id, summon => summon);
    }

    internal static Dictionary<string, Summon> CreateTechDemoHostiles(Vector2 roomPos)
    {
        var outList = new List<Summon>();
        for (var y = 0; y < TechDemoEnemyY; y++)
        {
            for (var x = 0; x < TechDemoX; x++)
            {
                Summon enemy = new FrontPensioner(new Vector2((roomPos.X + I1 + x) * Game1.sScaledPixelSize + Game1.sScaledPixelSize / F2,
                    (roomPos.Y + I1 + y) * Game1.sScaledPixelSize + Game1.sScaledPixelSize / F2), I1);
                outList.Add(enemy);
            }
        }
        return outList.ToDictionary(summon => summon.Id, summon => summon);
    }

    internal static Dictionary<string, Summon> PopulateDungeon(List<Room> roomList, List<RoomType> typeList, int stage)
    {
        var rand = new Random((int)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds % int.MaxValue));
        var outList = new List<Summon>();
        outList.AddRange(PopulateBossRoom(rand, new Vector2((float)roomList[1].mTopLeftCorner.mX, (float)roomList[1].mTopLeftCorner.mY), stage));
        for (var i = I2; i < roomList.Count; i++)
        {
            var current = PopulateRoom(rand, new Vector2((float)roomList[i].mTopLeftCorner.mX, (float)roomList[i].mTopLeftCorner.mY), typeList[i % typeList.Count], stage);
            outList.AddRange(current);
        }

        return outList.ToDictionary(summon => summon.Id, summon => summon);
    }

    private static List<Summon> PopulateRoom(Random rand, Vector2 tlc, RoomType roomType, int stage)
    {
        var outList = new List<Summon>();
        switch (roomType)
        {
            case RoomType.EmptyRoom:
                outList = PopulateEmptyRoom(rand, tlc, stage);
                break;
            case RoomType.LabyrinthRoom:
                outList = PopulateLabyrinthRoom(rand, tlc, stage);
                break;
            case RoomType.LayerRoom:
                outList = PopulateLayerRoom(rand, tlc, stage);
                break;
            case RoomType.PillarRoom:
                outList = PopulatePillarRoom(rand, tlc, stage);
                break;
        }

        return outList;
    }

    private static List<Summon> PopulateEmptyRoom(Random rand, Vector2 tlc, int stage)
    {
        var outList = new List<Summon>();
        var current = 0;
        for (var i = 0; i < (RoomHeight - 1) * (RoomWidth - 1); i++)
        {
            if (current > EmptyRoomSummonLimit)
            {
                break;
            }
            if (rand.Next() % PerThousand < I33 - I2 * current)
            {
                current++;
                switch (rand.Next() % I5)
                {
                    case I0:
                        outList.Add(new BombMagician(Camera.TileCenterToWorld(new Vector2(tlc.X + i % RoomWidth, tlc.Y + (int)((float)i/RoomWidth))), stage));
                        break;
                    case I1:
                        outList.Add(new ConanTheBarbarian(Camera.TileCenterToWorld(new Vector2(tlc.X + i % RoomWidth, tlc.Y + (int)((float)i / RoomWidth))), stage));
                        break;
                    case I2:
                        outList.Add(new Paladin(Camera.TileCenterToWorld(new Vector2(tlc.X + i % RoomWidth, tlc.Y + (int)((float)i / RoomWidth))), stage));
                        break;
                    case I3:
                        outList.Add(new FrontPensioner(Camera.TileCenterToWorld(new Vector2(tlc.X + i % RoomWidth, tlc.Y + (int)((float)i / RoomWidth))), stage));
                        break;
                    case I4:
                        outList.Add(new Pirate(Camera.TileCenterToWorld(new Vector2(tlc.X + i % RoomWidth, tlc.Y + (int)((float)i / RoomWidth))), stage));
                        break;
                }
            }

        }
        return outList;
    }

    private static List<Summon> PopulateLabyrinthRoom(Random rand, Vector2 tlc, int stage)
    {
        var outList = new List<Summon>();
        outList.Add(new BombMagician(Camera.TileCenterToWorld(new Vector2(tlc.X + I9, tlc.Y + I6)), stage));
        outList.Add(new FrontPensioner(Camera.TileCenterToWorld(new Vector2(tlc.X + I14, tlc.Y + I5)), stage));
        outList.Add(new Pirate(Camera.TileCenterToWorld(new Vector2(tlc.X + I12, tlc.Y + I12)), stage));
        outList.Add(new Paladin(Camera.TileCenterToWorld(new Vector2(tlc.X + I17, tlc.Y + I13)), stage));
        // outList.Add(new Pirate(Camera.TileCenterToWorld(new Vector2(tlc.X + I17, tlc.Y + I2)), stage));
        outList.Add(new ConanTheBarbarian(Camera.TileCenterToWorld(new Vector2(tlc.X, tlc.Y + I4)), stage));
        //outList.Add(new BombMagician(Camera.TileCenterToWorld(new Vector2(tlc.X + I3, tlc.Y + I7)), stage));
        outList.Add(new FrontPensioner(Camera.TileCenterToWorld(new Vector2(tlc.X + I1, tlc.Y + I12)), stage));
        if (rand.Next() % I3 == 0)
        {
            outList.Add(new BombMagician(Camera.TileCenterToWorld(new Vector2(tlc.X + I12, tlc.Y + I6)), stage));
            // outList.Add(new BombMagician(Camera.TileCenterToWorld(new Vector2(tlc.X + I6, tlc.Y + I8)), stage));
        }
        return outList;
    }
    private static List<Summon> PopulateLayerRoom(Random rand, Vector2 tlc, int stage)
    {
        var outList = new List<Summon>();
        outList.Add(new FrontPensioner(Camera.TileCenterToWorld(new Vector2(tlc.X + I5, tlc.Y + I3)), stage));
        outList.Add(new Paladin(Camera.TileCenterToWorld(new Vector2(tlc.X + I13, tlc.Y + I3)), stage));
        outList.Add(new ConanTheBarbarian(Camera.TileCenterToWorld(new Vector2(tlc.X + I5, tlc.Y + I11)), stage));
        outList.Add(new Pirate(Camera.TileCenterToWorld(new Vector2(tlc.X + I13, tlc.Y + I11)), stage));
        for (var y = -1; y <= 1; y++)
        {
            if (rand.Next() % I4 == 0)
            {
                outList.Add(new ConanTheBarbarian(Camera.TileCenterToWorld(new Vector2(tlc.X + I9 + y, tlc.Y + I7 + y)), stage));
            }
        }
        return outList;
    }
    private static List<Summon> PopulatePillarRoom(Random rand, Vector2 tlc, int stage)
    {
        var outList = new List<Summon>();
        outList.Add(new ConanTheBarbarian(Camera.TileCenterToWorld(new Vector2(tlc.X + I7, tlc.Y + I13)), stage));
        outList.Add(new ConanTheBarbarian(Camera.TileCenterToWorld(new Vector2(tlc.X + I11, tlc.Y + I1)), stage));
        outList.Add(new Paladin(Camera.TileCenterToWorld(new Vector2(tlc.X + I6, tlc.Y + I7)), stage));
        outList.Add(new BombMagician(Camera.TileCenterToWorld(new Vector2(tlc.X + I11, tlc.Y + I7)), stage));
        for (var i = 0; i < rand.Next() % I4; i++)
        {
            outList.Add(new FrontPensioner(Camera.TileCenterToWorld(new Vector2(tlc.X + I2 * i + I6, tlc.Y + I4)), stage));
        }
        return outList;
    }
    private static List<Summon> PopulateBossRoom(Random rand, Vector2 tlc, int stage)
    {
        var outList = new List<Summon>();
        for (var i = 0; i < I2; i++)
        {
            switch (rand.Next() % I5)
            {
                case I0:
                    outList.Add(new BombMagician(Camera.TileCenterToWorld(new Vector2(tlc.X + I7 + I4 * i, tlc.Y + I7)), stage));
                    break;
                case I1:
                    outList.Add(new ConanTheBarbarian(Camera.TileCenterToWorld(new Vector2(tlc.X + I7 + I4 * i, tlc.Y + I7)), stage));
                    break;
                case I2:
                    outList.Add(new Paladin(Camera.TileCenterToWorld(new Vector2(tlc.X + I7, tlc.Y + I7 + I4 * i)), stage));
                    break;
                case I3:
                    outList.Add(new FrontPensioner(Camera.TileCenterToWorld(new Vector2(tlc.X + I7 + I4 * i, tlc.Y + I7)), stage));
                    break;
                case I4:
                    outList.Add(new Pirate(Camera.TileCenterToWorld(new Vector2(tlc.X + I7 + I4 * i, tlc.Y + I7)), stage));
                    break;
            }
        }
        return outList;
    }

    internal static Dictionary<string, GameObject> SpawnLoot(List<Room> roomList, int stage)
    {
        var rand = new Random((int)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds % int.MaxValue));
        var outList = new List<GameObject>();

        outList.AddRange(BossRoomLoot(new Vector2((int)roomList[1].mTopLeftCorner.mX, (int)roomList[1].mTopLeftCorner.mY), stage));
        for (var i = 2; i < roomList.Count; i++)
        {

            if (rand.Next() % PerHundred < ChestChance)
            {
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(new Vector2((int)roomList[i].mTopLeftCorner.mX + I10, (int)roomList[i].mTopLeftCorner.mY + I10))));
            }
        }
        return outList.ToDictionary(summon => summon.Id, summon => summon);
    }


    private static List<GameObject> BossRoomLoot(Vector2 tlc, int stage)
    {
        var outList = new List<GameObject>();
        if (stage <= 0)
        {
            return outList;
        }

        outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc + new Vector2(I17, I14))));
        outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc + new Vector2(I1, I14))));
        outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc + new Vector2(I17, I0))));
        outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc + new Vector2(1, 0))));

        return outList;
    }

    /*
    private static List<GameObject> BossRoomLoot(Vector2 tlc, int stage)
    {
        var outList = new List<GameObject>();

        switch (stage)
        {
            case Stage5:
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc + new Vector2(I18, I14))));
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc + new Vector2(I0, I14))));
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc + new Vector2(I18, I0))));
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc)));
                break;
            case Stage4:
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc + new Vector2(I0, I14))));
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc + new Vector2(I18, I0))));
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc)));
                break;
            case Stage3:
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc + new Vector2(I18, I0))));
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc)));
                break;
            case Stage2:
                outList.Add(new TreasureChest(Camera.TileCenterToWorld(tlc)));
                break;
        }
        return outList;
    }
    */

}