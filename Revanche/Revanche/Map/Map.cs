using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Revanche.Extensions;
using Revanche.Managers;
using System;
using System.Collections.Generic;
using Revanche.Map.Pathfinding;

namespace Revanche.Map
{
    public sealed class Map : IWeightedGraph
    {
        [JsonProperty] public int[,] DungeonBackGround { get; private set; }
        [JsonProperty] public int[,] DungeonMidGround { get; private set; }
        [JsonProperty] public int RoomCount { get; private set; }
        [JsonIgnore] public List<Room> RoomList { get; private set; }
        [JsonProperty] public List<Vector2> RoomTopLeftCornerList { get; private set; }
        [JsonProperty] public int DungeonDimension { get; private set; }
        [JsonProperty] public bool[,] Collidable { get; private set; } //for Path-finding
        [JsonProperty] public Vector2 SpawnPos { get; private set; }
        [JsonProperty] public Vector2 BossPos { get; private set; }
        [JsonProperty] public Grid Grid { get; private set; }


        private const int I1 = 1;
        private const int I2 = 2;
        private const int I3 = 3;
        
        private const int I5 = 5;
        
        private const int I7 = 7;
        
        private const int I10 = 10;
        
        private const int I12 = 12;
        
        private const int I15 = 15;

        private const int I19 = 19;
        private const int I20 = 20;

        private const int I25 = 25;
        private const int I31 = 31;
        private const int I35 = 35;
        private const int I50 = 50;
        private const int I55 = 55;
        private const int I85 = 85;
        private const int I95 = 95;
        private const int I100 = 100;
        private const int I200 = 200;

        private const float F01 = 0.1f;
        private const float F2 = 2f;
        private const float F10 = 10f;

        private Map()
        {
        }

        public static Map CreateMap(int stageNumber = 1)
        {
            var map = new Map();
            map.GenerateNewMap(stageNumber);
            return map;
        }

        public void GenerateNewMap(int stageNumber = 1)
        {
            RoomCount = I7 + I5 * stageNumber;
            DungeonDimension = I100 + I20 * stageNumber;
            var dungeon = new Dungeon(RoomCount,
                DungeonDimension,
                DungeonDimension,
                I12,
                DungeonDimension - I12,
                I12,
                DungeonDimension - I12,
                I19,
                I15,
                I55);
            RoomList = dungeon.mRooms;
            RoomTopLeftCornerList = new List<Vector2>();
            foreach (var room in RoomList)
            {
                RoomTopLeftCornerList.Add(new Vector2((float)room.mTopLeftCorner.mX, (float)room.mTopLeftCorner.mY));
            }
            SpawnPos = new Vector2((int)dungeon.mRooms[0].mMid.mX, (int)dungeon.mRooms[0].mMid.mY - I3);
            BossPos = new Vector2((int)dungeon.mRooms[1].mMid.mX, (int)dungeon.mRooms[1].mMid.mY);
            DungeonBackGround = dungeon.mTiles.BackgroundSpriteMatrix();
            DungeonMidGround = dungeon.mTiles.MidGroundSpriteMatrix();
            Collidable = dungeon.mTiles.CollisionMatrix();
            Grid = dungeon.mTiles;
        }

        public static Map TechDemoMap()
        {
            var map = new Map();
            map.RoomCount = I3;
            map.DungeonDimension = I200;
            var dungeon = new Dungeon();
            dungeon = dungeon.CreateTechDemoDungeon();
            map.RoomList = dungeon.mRooms;
            map.RoomTopLeftCornerList = new List<Vector2>();
            foreach (var room in map.RoomList)
            {
                map.RoomTopLeftCornerList.Add(new Vector2((float)room.mTopLeftCorner.mX, (float)room.mTopLeftCorner.mY));
            }
            map.SpawnPos = new Vector2(I85, I100);
            map.BossPos = new Vector2(I95, I100);
            map.DungeonBackGround = dungeon.mTiles.BackgroundSpriteMatrix();
            map.DungeonMidGround = dungeon.mTiles.MidGroundSpriteMatrix();
            map.Collidable = dungeon.mTiles.CollisionMatrix();
            map.Grid = dungeon.mTiles;
            return map;
        }

        public static Map AiMap()
        {
            var map = new Map();
            map.RoomCount = I1;
            map.DungeonDimension = I50;
            var dungeon = new Dungeon();
            dungeon = dungeon.CreateAiDungeon();
            map.RoomList = dungeon.mRooms;
            map.RoomTopLeftCornerList = new List<Vector2>() { new Vector2((float)map.RoomList[0].mTopLeftCorner.mX, (float)map.RoomList[0].mTopLeftCorner.mY) };
            map.SpawnPos = new Vector2(I15, I25);
            map.BossPos = new Vector2(I35, I25);
            map.DungeonBackGround = dungeon.mTiles.BackgroundSpriteMatrix();
            map.DungeonMidGround = dungeon.mTiles.MidGroundSpriteMatrix();
            map.Collidable = dungeon.mTiles.CollisionMatrix();
            map.Grid = dungeon.mTiles;
            return map;
        }

        public Vector2 GetSpawnPoint()
        {
            return new Vector2(SpawnPos.X * Game1.sScaledPixelSize + Game1.sScaledPixelSize / F2,
                (SpawnPos.Y + I5) * Game1.sScaledPixelSize + Game1.sScaledPixelSize / F2);
        }

        public Vector2 GetBossSpawnPoint()
        {
            return new Vector2(BossPos.X * Game1.sScaledPixelSize + Game1.sScaledPixelSize / F2,
                BossPos.Y * Game1.sScaledPixelSize + Game1.sScaledPixelSize / F2);
        }

        /// <summary>
        /// Draws the selected part of the Map.
        /// visibleMapArea must be in Map-coordinates. 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="visibleMapArea"></param>
        /// <param name="fog"></param>
        /// <param name="zoom"></param>
        public void Draw(SpriteBatch spriteBatch, Rectangle visibleMapArea, FogOfWar fog, float zoom)
        {
            for (var y = Math.Max(0, visibleMapArea.Top); y < Math.Min(this.DungeonDimension - I1, visibleMapArea.Bottom); y++)
            {
                for (var x = Math.Max(0, visibleMapArea.Left); x < Math.Min(this.DungeonDimension - I1, visibleMapArea.Right); x++)
                {
                    if (DungeonMidGround[y, x] != I31 && (!fog.FogMask[x, y] || Game1.mDebugMode || !fog.Use))
                    {
                        spriteBatch.Draw(AssetManager.mSpriteSheet,
                            new Vector2(Game1.sScaledPixelSize * x, Game1.sScaledPixelSize * y),
                            AssetManager.GetRectangleFromId16(DungeonMidGround[y, x]),
                            Color.White,
                            0f,
                            Vector2.Zero,
                            Game1.mScale,
                            SpriteEffects.None,
                            1f);
                    }

                    // DebugMode rectangles
                    if (Game1.mDebugMode && DungeonBackGround[y, x] != I31)
                    {
                        spriteBatch.DrawRectangleOutline(new Rectangle((int)(Game1.sScaledPixelSize * x),
                                (int)(Game1.sScaledPixelSize * y),
                                (int)Game1.sScaledPixelSize,
                                (int)Game1.sScaledPixelSize),
                            (int)(2 / zoom) + 1, Color.Black, F01);
                    }
                }
            }
        }

        // Pathfinding utils

        private static readonly Vector2[] sDirections = new Vector2[]
        {
            new Vector2(0, 1),     //  Up
            new Vector2(0, -1),    //  Down
            new Vector2(-1, 0),    //  Left
            new Vector2(1, 0),     //  Right
            new Vector2(1,1),       // Diagonals (below 4)
            new Vector2(-1,-1),
            new Vector2(-1,1),
            new Vector2(1,-1),
        };

        public bool Passable(Vector2 id)
        {
            if (id.X < 0 || id.X >= DungeonDimension - 1 || id.Y < 0 || id.Y >= DungeonDimension)
            {
                return false;
            }

            return !Collidable[(int)id.Y, (int)id.X];
        }

        public double Cost(Vector2 a, Vector2 b)
        {
            return Math.Round(Math.Sqrt(Math.Pow(a.X - b.X, I2) + Math.Pow(a.Y - b.Y, I2)) * I10) / F10;
        }

        public IEnumerable<Vector2> PassableNeighbors(Vector2 id)
        {
            foreach (Vector2 direction in sDirections)
            {
                Vector2 next = new Vector2(id.X + direction.X, id.Y + direction.Y);

                //InBounds(next) not needed right now -> could be used if you want the character to continue walking once clicked into the blue (right now character stops)
                if (Passable(next))
                {
                    yield return next;
                }
            }

        }
    }
}
