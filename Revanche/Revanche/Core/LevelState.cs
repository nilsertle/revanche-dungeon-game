#nullable enable
using System;
using Revanche.GameObjects;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Revanche.AchievementSystem;
using Revanche.Extensions;
using Revanche.GameObjects.Environment;
using Revanche.GameObjects.Items;
using Revanche.GameObjects.HostileUnits;
using Revanche.GameObjects.FriendlyUnits;
using Revanche.GameObjects.Projectiles;
using Revanche.Managers;
using Revanche.Map;
using Revanche.Sound;
using System.Linq;
using Revanche.Stats;
using Revanche.AI.HostileSummonBehaviour;

namespace Revanche.Core;

public sealed class LevelState
{
    [JsonProperty] public int LevelCount { get; set; } = 1;
    [JsonIgnore] public EventDispatcher EventDispatcher { get; set; } = null!;
    [JsonProperty] public Camera Camera2d { get; private set; } = null!;
    [JsonProperty] internal Summoner Summoner { get; private set; } = null!;
    [JsonProperty] public Archenemy? ArchEnemy { get; private set; }

    // Mutable lists. Only modify them through public functions in LevelState that can be called by others
    [JsonProperty] private Dictionary<string, Summon> MutableFriendlySummons { get; set; } = null!;
    [JsonProperty] private Dictionary<string, Summon> MutableHostileSummons { get; set; } = null!;
    [JsonProperty] public Dictionary<string, string> AttackerToTarget { get; private set; } = null!;
    [JsonProperty] public Dictionary<string, HashSet<string>> Attackers { get; private set; } = null!;
    [JsonProperty] private List<Projectile> MutableProjectiles { get; set; } = null!;
    [JsonProperty] private Dictionary<string, GameObject> MutableUsable { get; set; } = null!;

    // ReadOnly lists to not let other components directly modify the LevelState
    //The now private Properties can be changed back to public if needed (possibly for enemy summons, drops, etc.)
    [JsonIgnore] public IReadOnlyDictionary<string, Summon> FriendlySummons => MutableFriendlySummons;
    [JsonIgnore] public IReadOnlyDictionary<string, Summon> HostileSummons => MutableHostileSummons;
    [JsonProperty] public Map.Map GameMap { get; private set; } = null!;
    [JsonProperty] public FogOfWar FogOfWar { get; set; } = null!;
    [JsonIgnore] public SafeStaticGameObjectQuadTree QuadTree { get; private set; } = null!;
    [JsonIgnore] public SafeStaticGameObjectQuadTree MapTree { get; private set; } = null!;
    [JsonProperty] public BloodShrine BloodShrine { get; private set; } = null!;
    [JsonProperty] public Ladder? Ladder { get; private set; }
    [JsonProperty] public bool InTechDemo { get; set; }
    [JsonProperty] public bool InAiDemo { get; set; }
    [JsonProperty] public int mCurrentPoints; // 0 by default
    [JsonProperty] private bool mBossMusicFlag;
    [JsonProperty] private bool mRaomingMusicFlag;
    
    private LevelState()
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        GameMap.Draw(spriteBatch, Camera2d.mVisibleMap, FogOfWar, Camera2d.Zoom);
        Ladder?.Draw(spriteBatch);
        QuadTree.Draw(spriteBatch, Camera2d.mVisibleArea);
        DrawSummonRange(spriteBatch);
        FogOfWar.Draw(spriteBatch, Camera2d.mVisibleMap);
    }

    public void UpdateGameObjects(float deltaTime)
    {
        Camera2d.UpdateCamera();
        EventDispatcher.SendPositionToSoundManger(new CommunicationEvent(Camera2d.Position));
        HandleSpellCoolDown(deltaTime);
        UpdateProjectiles(deltaTime);
        var toBeAdded = new List<GameObject>();
        foreach (var item in MutableUsable.Values)
        {
            item.Update(deltaTime);
            if (item.State == InstanceState.LimitReached)
            {
                if (item is TreasureChest)
                {
                    toBeAdded.Add(new EnvironmentalItem(item.Position,
                        EnvironmentalAnimations.OpenChest, EnvironmentalMode.AnimationWithStop));
                }
                MutableUsable.Remove(item.Id);
            }
        }
        foreach (var item in toBeAdded)
        {
            MutableUsable.Add(item.Id, item);
        }
        toBeAdded.Clear();
        ActionForCharacters(character =>
        {
            character.Update(deltaTime);
            HandleCharacterDeath(character);
        });
        Summoner.RegenerateMana();
        Summoner.CheckEnoughManaSpell();
        if (!mBossMusicFlag && ArchEnemy != null && (ArchEnemy.GetState() == CharacterState.Attacking || ArchEnemy.GetState() == CharacterState.Fleeing))
        {
            EventDispatcher.SendAudioRequest(new SongEvent(Songs.BossFight, true, true, 2f, 1f));
            mBossMusicFlag = true;
            mRaomingMusicFlag = true;
        }
        else if (mRaomingMusicFlag && ArchEnemy != null && (ArchEnemy.GetState() == CharacterState.Idle || ArchEnemy.GetState() == CharacterState.Patrolling))
        {
            EventDispatcher.SendAudioRequest(new SongEvent(Songs.Roaming, true, true, 10f, 3f));
            mBossMusicFlag = false;
            mRaomingMusicFlag = false;
        }
        else if (ArchEnemy == null && mBossMusicFlag)
        {
            // Archenemy has been defeated, reset flags and fade to Roaming
            EventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.ArchEnemyDeath, null));
            EventDispatcher.SendAudioRequest(new SongEvent(Songs.Roaming, true, true, 6f, 1f));
            mBossMusicFlag = false;
            mRaomingMusicFlag = false; // Otherwise, a fade would happen when the level is changed
        }
    }

    public void UpdateQuadTree()
    {
        QuadTree.Clear();
        ActionForCharacters((character) => { QuadTree.Insert(character); });

        foreach (var potion in MutableUsable)
        {
            QuadTree.Insert(potion.Value);
        }

        foreach (var proj in MutableProjectiles)
        {
            QuadTree.Insert(proj);
        }
    }

    public void UpdateFogOfWar()
    {
        FogOfWar.Update(Summoner.Position, 6);
        foreach (var character in this.MutableFriendlySummons.Values)
        {
            FogOfWar.Update(character.Position, 6);
        }
    }
    private void FillMapTree()
    {
        for (var y = 0; y < GameMap.DungeonDimension; y++)
        {
            for (var x = 0; x < GameMap.DungeonDimension; x++)
            {
                if (GameMap.Grid.GetCellType(x, y) == CellType.WallCell)
                {
                    MapTree.Insert(new WallObject(new Vector2(x * Game1.sScaledPixelSize + Game1.sScaledPixelSize / 2, y * Game1.sScaledPixelSize + Game1.sScaledPixelSize / 2), -1));
                }
                if (GameMap.Grid.GetCellType(x, y) == CellType.DestructAbleWallCell)
                {
                    MapTree.Insert(new DestructableWall(new Vector2(x * Game1.sScaledPixelSize + Game1.sScaledPixelSize / 2, y * Game1.sScaledPixelSize + Game1.sScaledPixelSize / 2), 2));
                }
            }
        }
        var bloodShrineTop = new EnvironmentalItem(BloodShrine.Position - new Vector2(0, 1) * Game1.sScaledPixelSize, EnvironmentalAnimations.BloodFountainTop, EnvironmentalMode.FullAnimation);
        var bloodShrineBottom = new EnvironmentalItem(BloodShrine.Position, EnvironmentalAnimations.BloodFountainBottom, EnvironmentalMode.FullAnimation);
        var shrineHint = new EnvironmentalItem(BloodShrine.Position + new Vector2(0, 1) * Game1.sScaledPixelSize, EnvironmentalAnimations.ShrineHintMap, EnvironmentalMode.SingleSprite);
        AddToMutableUseAble(bloodShrineTop);
        AddToMutableUseAble(bloodShrineBottom);
        AddToMutableUseAble(shrineHint);
        MapTree.Insert(BloodShrine);
    }
    
    public bool IsSummonLimitReached()
    {
        return MutableFriendlySummons.Count > 4;
    }

    public bool IsSpaceFree(Vector2 position)
    {
        var objects = QuadTree.PointSearchCharacters(position);
        return objects.Count <= 0;
    }

    public bool OnGround(Vector2 position)
    {
        var mapPos = position.ToGrid();
        return !(this.GameMap.Collidable[(int)mapPos.Y, (int)mapPos.X]);
    }
    private void HandleCharacterDeath(Character character)
    {
        // If character is still alive, do nothing
        if (character.CurrentLifePoints > 0)
        {
            return;
        }

        // Remove target from set of attackers of its target
        // then remove itself from the AttackerToTarget mapping
        if (AttackerToTarget.ContainsKey(character.Id))
        {
            var targetId = AttackerToTarget[character.Id];
            Attackers[targetId].Remove(character.Id);
            if (Attackers[AttackerToTarget[character.Id]].Count == 0)
            {
                Attackers.Remove(targetId);
                var target = GetCharacterWithId(targetId);
                if (target is { IsFriendly: false })
                {
                    target.Selected = false;
                }
            }

            AttackerToTarget.Remove(character.Id);
        }

        // Remove target from all the units that were
        // attacking it before its death
        if (Attackers.ContainsKey(character.Id))
        {
            foreach (var attackerId in Attackers[character.Id])
            {
                AttackerToTarget.Remove(attackerId);
                // Change player summons back to PlayerControl state
                var attacker = GetCharacterWithId(attackerId);
                if (attacker is { IsFriendly: true })
                {
                    attacker.CurrentState = CharacterState.PlayerControl;
                }
            }

            Attackers.Remove(character.Id);
        }

        // Remove killed summon from list
        if (character is Summon summon)
        {
            // Logic for friendly summon death
            if (summon.IsFriendly)
            {
                MutableFriendlySummons.Remove(summon.Id);
            }
            // Logic for enemy summon death
            else
            {
                // Enemy summon kill == 10 points
                mCurrentPoints += 10;
                if (Summoner.AddExp(30 + LevelCount * 30)) // XP - Adjustment
                {
                    EventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.LevelUp, null));
                }
                if (!InTechDemo)
                {
                    EventDispatcher.SendAchievementEvent(AchievementType.JackTheRipper);
                    EventDispatcher.SendStatisticEvent(StatisticType.DefeatedEnemies);
                    EventDispatcher.SendStatisticEvent(StatisticType.GainedExp, 20 + LevelCount * 10);
                }

                var newItem = summon.DropItem();
                if (newItem != null)
                {
                    MutableUsable.Add(newItem.Id, newItem);
                }
                MutableHostileSummons.Remove(summon.Id);
            }

            var testEnvironItem = new EnvironmentalItem(summon.Position, EnvironmentalAnimations.BloodStain, EnvironmentalMode.SingleSprite);
            MutableUsable.Add(testEnvironItem.Id, testEnvironItem);
            EventDispatcher.SendAudioRequest(new SoundEvent(summon.mDeathSound, summon.Position));
            return;
        }

        // Dead character was summoner, lose game
        if (character.Id == Summoner.Id)
        { 
            LoseGame();
            return;
        }

        // ArchEnemy death
        if (ArchEnemy != null)
        {
            // ArchEnemy defeat == 100 points
            mCurrentPoints += 100;

            if (!InTechDemo)
            {
                EventDispatcher.SendStatisticEvent(StatisticType.DefeatedBosses);
            }
            LevelCount++;
            if (LevelCount < 6)
            {
                EventDispatcher.SendPopupEvent(new IPopupEvent.NotificationPopup("Der Erzfeind flieht eine Ebene hoeher! Ihm nach!", Color.Red));
                Ladder = new Ladder(Camera.TileCenterToWorld(ArchEnemy.Position.ToGrid()));
                ArchEnemy = null;
                return;
            }

            // Killed the ArchEnemy a fifth time, the game is won
            WinGame();
        }
        else
        {
            ArchEnemy = null;
        }
    }


    private void LoseGame()
    {
        EventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        EventDispatcher.SendScreenRequest(new INavigationEvent.GameOverScreen());
        EventDispatcher.StopSound();
        EventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.GameLost, null));
        if (InTechDemo)
        {
            return;
        }
        EventDispatcher.SendPopupEvent(new IPopupEvent.NotificationPopup("Erreichte Gesamtpunktzahl: " + mCurrentPoints, Color.Red));
        EventDispatcher.SendStatisticEvent(StatisticType.AccumulatedPoints, mCurrentPoints);
    }

    private void WinGame()
    {
        EventDispatcher.SendScreenRequest(new INavigationEvent.PopScreen());
        EventDispatcher.SendScreenRequest(new INavigationEvent.GameWonScreen());
        EventDispatcher.StopSound();
        EventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.GameWon, null));
        if (InTechDemo)
        {
            return;
        }
        EventDispatcher.SendPopupEvent(new IPopupEvent.NotificationPopup("Erreichte Gesamtpunktzahl: " + mCurrentPoints, Color.Red));
        EventDispatcher.SendStatisticEvent(StatisticType.AccumulatedPoints, mCurrentPoints);
        EventDispatcher.SendAchievementEvent(AchievementType.EndlichFrei);
    }

    /// <summary>
    /// Temporary solution to let all characters from all lists
    /// perform a single action, like draw and update or input
    /// handling if needed. Used to remove clutter
    /// </summary>
    /// <param name="action"></param>
    public void ActionForCharacters(Action<Character> action)
    {
        ActionForFriendlyCharacters(action);
        ActionForHostileCharacters(action);
    }

    public void ActionForFriendlyCharacters(Action<Character> action)
    {
        action(Summoner);
        MutableFriendlySummons.Values.ForEachReversed(action);
    }

    private void ActionForHostileCharacters(Action<Character> action)
    {
        if (ArchEnemy != null)
        {
            action(ArchEnemy);
        }
        MutableHostileSummons.Values.ForEachReversed(action);
    }

    public void AddToMutableUseAble(GameObject newItem)
    {
        MutableUsable.Add(newItem.Id, newItem);
    }

    public void AddToMutableUseAble(List<GameObject> content)
    {
        foreach (var item in content)
        {
            MutableUsable.Add(item.Id, item);
        }
    }

    private void UpdateProjectiles(float deltaTime)
    {
        MutableProjectiles.ForEachReversed(projectile =>
            {
                projectile.Update(deltaTime);
                if (projectile.State == InstanceState.LimitReached)
                {
                    MutableProjectiles.Remove(projectile);
                }

                // 14 = Lamp off
                // 15 = lamp on

                var collidingWalls = MapTree.Search(projectile.Hitbox).OfType<WallObject>().ToList();
                foreach (var wall in collidingWalls)
                {
                    if (GameMap.DungeonMidGround[(int)wall.Position.ToGrid().Y, (int)wall.Position.ToGrid().X] == 14)
                    {
                        GameMap.DungeonMidGround[(int)wall.Position.ToGrid().Y, (int)wall.Position.ToGrid().X] = 15;
                        EventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.FireBallImpact, wall.Position));
                        continue;
                    }

                    if (wall is DestructableWall && projectile.Damage > 0)
                    {
                        if (wall.HitsLeft-- == 0)
                        {
                            EventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.DestructibleBreaks, wall.Position));
                            this.GameMap.Grid.SetCell((int)wall.Position.ToGrid().X,
                                (int)wall.Position.ToGrid().Y,
                                CellType.GroundCell);
                            this.GameMap.DungeonMidGround[(int)wall.Position.ToGrid().Y,
                                (int)wall.Position.ToGrid().X] = 20;
                            this.GameMap.DungeonBackGround[(int)wall.Position.ToGrid().Y,
                                (int)wall.Position.ToGrid().X] = 1;
                            this.GameMap.Collidable[(int)wall.Position.ToGrid().Y, (int)wall.Position.ToGrid().X] =
                                false;
                            this.MapTree.Clear();
                            FillMapTree();
                        }
                        else
                        {
                            EventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.DestructibleHit, wall.Position));
                            wall.HitsLeft--;
                            this.GameMap.DungeonMidGround[(int)wall.Position.ToGrid().Y,
                                (int)wall.Position.ToGrid().X] = 19;
                        }
                        continue;
                    }
                    EventDispatcher.SendAudioRequest(new SoundEvent(SoundEffects.HitWall, wall.Position));
                }
                if (collidingWalls.Count > 0)
                {
                    RemoveProjectile(projectile);
                }
            }
        );
    }
    public void AddProjectile(Projectile projectile)
    {
        MutableProjectiles.Add(projectile);
    }

    public void ConsumeItem(Item item)
    {
        if (!InTechDemo)
        {
            EventDispatcher.SendAchievementEvent(AchievementType.Nimmersatt);
        }
        item.Use(Summoner);
        MutableUsable.Remove(item.Id);
    }

    public void PickUpSoul(Item soul)
    {
        // 1 Soul == 1 point
        mCurrentPoints += 1;
        Summoner.Souls += 1;
        MutableUsable.Remove(soul.Id);
    }

    private void RemoveProjectile(Projectile projectile)
    {
        MutableProjectiles.Remove(projectile);
    }

    public void AddFriendlySummon(Summon summon)
    {
        if (!InTechDemo)
        {
            EventDispatcher.SendAchievementEvent(AchievementType.KarnickelAusDemHut);
            EventDispatcher.SendStatisticEvent(StatisticType.SummonedMonsters);
        }
        MutableFriendlySummons.Add(summon.Id, summon);
    }

    public void AddHostileSummon(Summon summon)
    {
        MutableHostileSummons.Add(summon.Id, summon);
    }

    public void AddItem(Item item)
    {
        MutableUsable.Add(item.Id, item);
    }

    public Summon? GetSummonWithId(string id)
    {
        return MutableFriendlySummons!.GetValueOrDefault(id, MutableHostileSummons!.GetValueOrDefault(id, null));
    }

    public Character? GetCharacterWithId(string id)
    {
        var summon = GetSummonWithId(id);
        if (summon != null)
        {
            return summon;
        }

        if (Summoner.Id == id)
        {
            return Summoner;
        }

        return ArchEnemy?.Id == id ? ArchEnemy : null;
    }

    public Character? GetEnemyWithId(string id)
    {
        var summon = MutableHostileSummons!.GetValueOrDefault(id, null);
        if (summon != null)
        {
            return summon;
        }

        return ArchEnemy?.Id == id ? ArchEnemy : null;
    }

    public void ChangeLevel()
    {
        // 1 lvl change == 200 points
        mCurrentPoints += 200;

        AttackerToTarget.Clear();
        Attackers.Clear();
        QuadTree.Clear();
        MapTree.Clear();
        GameMap.GenerateNewMap();
        FogOfWar = FogOfWar.CreateFogOfWar(GameMap, true);
        MapTree = SafeStaticGameObjectQuadTree.CreateQuadTree(
            rect: new Rect(new Vector2(0, 0),
                size: new Vector2((GameMap.DungeonDimension + 10) * Game1.sScaledPixelSize,
                    y: (GameMap.DungeonDimension + 10) * Game1.sScaledPixelSize))
        );
        FillMapTree();
        var spawnPoint = GameMap.GetSpawnPoint();
        Summoner.SetPosition(spawnPoint);
        Ladder = null;
        BloodShrine.Position = new Vector2(spawnPoint.X, spawnPoint.Y - Game1.sScaledPixelSize);
        Camera2d.SetCameraPosition(spawnPoint);
        ArchEnemy = new Archenemy(GameMap.GetBossSpawnPoint(), LevelCount, null);
        var horizontalOffset = -2 * Game1.sScaledPixelSize;
        foreach (var friendly in MutableFriendlySummons)
        {
            friendly.Value.SetPosition(spawnPoint + new Vector2(horizontalOffset, Game1.sScaledPixelSize));
            horizontalOffset += Game1.sScaledPixelSize;
        }
        
        MutableHostileSummons = ObjectBuilder.PopulateDungeon(GameMap.RoomList,
            new List<RoomType>()
            {
                RoomType.PillarRoom, RoomType.EmptyRoom, RoomType.EmptyRoom, RoomType.PillarRoom,
                RoomType.LabyrinthRoom, RoomType.LayerRoom
            },
            LevelCount);
        MutableUsable = ObjectBuilder.SpawnLoot(GameMap.RoomList, LevelCount);
        var bloodShrineTop =
            new EnvironmentalItem( BloodShrine.Position - new Vector2(0, 1) * Game1.sScaledPixelSize, EnvironmentalAnimations.BloodFountainTop, EnvironmentalMode.FullAnimation);
        var bloodShrineBottom =
            new EnvironmentalItem(BloodShrine.Position, EnvironmentalAnimations.BloodFountainBottom, EnvironmentalMode.FullAnimation);
        MutableUsable.Add(bloodShrineTop.Id, bloodShrineTop);
        MutableUsable.Add(bloodShrineBottom.Id, bloodShrineBottom);
        MutableProjectiles.Clear();
        EventDispatcher.SendPopupEvent(new IPopupEvent.NotificationPopup("Ebene " + LevelCount + "/5", Color.White, 2f));
    }
    public static LevelState CreateDefaultLevelState()
    {
        var map = Map.Map.CreateMap();
        var fog = FogOfWar.CreateFogOfWar(map, true);
        var spawnPoint = map.GetSpawnPoint();
        var bossPoint = map.GetBossSpawnPoint();

        var levelState = new LevelState
        {
            Camera2d = new Camera(new Vector2(spawnPoint.X - 2 * Game1.sScaledPixelSize, spawnPoint.Y + Game1.sScaledPixelSize)),
            MutableFriendlySummons = new Dictionary<string, Summon>(), // Will be filled through summons
            BloodShrine = new BloodShrine(new Vector2(spawnPoint.X, spawnPoint.Y - Game1.sScaledPixelSize)),
            Ladder = null,
            MutableHostileSummons = ObjectBuilder.PopulateDungeon(map.RoomList, new List<RoomType>() { RoomType.PillarRoom, RoomType.EmptyRoom, RoomType.EmptyRoom, RoomType.PillarRoom, RoomType.LabyrinthRoom, RoomType.LayerRoom }, 1),
            MutableUsable = ObjectBuilder.CreateDefaultItems(map.RoomList),
            Summoner = new Summoner(new Vector2(spawnPoint.X - 2 * Game1.sScaledPixelSize, spawnPoint.Y + Game1.sScaledPixelSize)),
            ArchEnemy = new Archenemy(bossPoint, 1, null),
            MutableProjectiles = new List<Projectile>(), // Initially empty
            GameMap = map,
            FogOfWar = fog,
            QuadTree = SafeStaticGameObjectQuadTree.CreateQuadTree(
                rect: new Rect(new Vector2(-1000, -1000),
                    size: new Vector2((map.DungeonDimension + 10) * Game1.sScaledPixelSize,
                        y: (map.DungeonDimension + 10) * Game1.sScaledPixelSize))
            ),
            MapTree = SafeStaticGameObjectQuadTree.CreateQuadTree(
                rect: new Rect(new Vector2(0, 0),
                    size: new Vector2((map.DungeonDimension + 10) * Game1.sScaledPixelSize,
                        y: (map.DungeonDimension + 10) * Game1.sScaledPixelSize))
            ),
            Attackers = new Dictionary<string, HashSet<string>>(),
            AttackerToTarget = new Dictionary<string, string>(),
            InTechDemo = false,
            InAiDemo = false
        };
        levelState.FillMapTree();
        return levelState;
    }

    public static LevelState CreateTechDemoLevelState()
    {
        var map = Map.Map.TechDemoMap();
        var fog = FogOfWar.CreateFogOfWar(map, false);
        var spawnPoint = map.GetSpawnPoint();
        var bossPoint = new Vector2(map.BossPos.X * Game1.sScaledPixelSize + Game1.sScaledPixelSize / 2f,
            (map.BossPos.Y + 5) * Game1.sScaledPixelSize + Game1.sScaledPixelSize / 2f);

        var levelState = new LevelState
        {
            Camera2d = new Camera(spawnPoint),
            MutableHostileSummons = ObjectBuilder.CreateTechDemoHostiles(map.RoomTopLeftCornerList[1]),
            MutableFriendlySummons = ObjectBuilder.CreateTechDemoFriendlies(map.RoomTopLeftCornerList[0]),
            MutableUsable = new Dictionary<string, GameObject>(),
            Summoner = new Summoner(spawnPoint),
            BloodShrine = new BloodShrine(new Vector2(spawnPoint.X - 16 * Game1.sScaledPixelSize, spawnPoint.Y)),
            Ladder = null,
            ArchEnemy = new Archenemy(bossPoint, 1, null),
            MutableProjectiles = new List<Projectile>(), // Initially empty
            GameMap = map,
            FogOfWar = fog,
            QuadTree = SafeStaticGameObjectQuadTree.CreateQuadTree(
                rect: new Rect(new Vector2(0, 0),
                    size: new Vector2((map.DungeonDimension + 10) * Game1.sScaledPixelSize,
                        y: (map.DungeonDimension + 10) * Game1.sScaledPixelSize))
            ),
            MapTree = SafeStaticGameObjectQuadTree.CreateQuadTree(
                rect: new Rect(new Vector2(0, 0),
                    size: new Vector2((map.DungeonDimension + 10) * Game1.sScaledPixelSize,
                        y: (map.DungeonDimension + 10) * Game1.sScaledPixelSize))
            ),
            Attackers = new Dictionary<string, HashSet<string>>(),
            AttackerToTarget = new Dictionary<string, string>(),
            InTechDemo = true,
            InAiDemo = false
        };
        levelState.Summoner.Souls = 10000;
        levelState.FillMapTree();
        return levelState;
    }

    public static LevelState CreateAiTestLevelState(EnemyType testEnemy, int level)
    {
        var map = Map.Map.AiMap();
        var fog = FogOfWar.CreateFogOfWar(map, false);
        var spawnPoint = map.GetSpawnPoint();
        var bossPoint = map.GetBossSpawnPoint();
        var enemy = new List<Summon>();
        var archEnemy = new Archenemy(bossPoint, 5, null);
        if (testEnemy is not EnemyType.ArchEnemy)
        {
            enemy.Add(GetEnemyByType(testEnemy, level, bossPoint));
            archEnemy = new Archenemy(bossPoint + new Vector2(2, 0) * Game1.sScaledPixelSize, level, new NoneBehaviour());
        }

        var levelState = new LevelState
        {
            Camera2d = new Camera(spawnPoint),
            MutableFriendlySummons = new Dictionary<string, Summon>(),
            MutableHostileSummons = enemy.ToDictionary(summon => summon.Id, summon => summon),
            MutableUsable = new Dictionary<string, GameObject>(),
            Summoner = new Summoner(spawnPoint),
            BloodShrine = new BloodShrine(new Vector2((map.RoomTopLeftCornerList[0].X + 1.5f) * Game1.sScaledPixelSize, (map.RoomTopLeftCornerList[0].Y + 7.5f) * Game1.sScaledPixelSize)),
            Ladder = null,
            ArchEnemy = archEnemy,
            MutableProjectiles = new List<Projectile>(), // Initially empty
            GameMap = map,
            FogOfWar = fog,
            QuadTree = SafeStaticGameObjectQuadTree.CreateQuadTree(
                rect: new Rect(new Vector2(0, 0),
                    size: new Vector2((map.DungeonDimension + 10) * Game1.sScaledPixelSize,
                        y: (map.DungeonDimension + 10) * Game1.sScaledPixelSize))
            ),
            MapTree = SafeStaticGameObjectQuadTree.CreateQuadTree(
                rect: new Rect(new Vector2(0, 0),
                    size: new Vector2((map.DungeonDimension + 10) * Game1.sScaledPixelSize,
                        y: (map.DungeonDimension + 10) * Game1.sScaledPixelSize))
            ),
            Attackers = new Dictionary<string, HashSet<string>>(),
            AttackerToTarget = new Dictionary<string, string>(),
            InTechDemo = true,
            InAiDemo = true
        };
        levelState.Summoner.Souls = 10000;
        levelState.Summoner.SkillPoints = 20;
        levelState.FillMapTree();
        return levelState;
    }

    private static Summon GetEnemyByType(EnemyType enemyType, int level, Vector2 position)
    {
        switch (enemyType)
        {
            case EnemyType.ConanTheBarbarian:
                return new ConanTheBarbarian(position, level);
            case EnemyType.FrontPensioner:
                return new FrontPensioner(position, level);
            case EnemyType.Paladin:
                return new Paladin(position, level);
            case EnemyType.BombMagician:
                return new BombMagician(position, level);
            case EnemyType.Pirate:
                return new Pirate(position, level);
            default:
                throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null);
        }
    }
    private void DrawSummonRange(SpriteBatch spriteBatch)
    {
        if (Summoner.SelectedSummonType == null)
        {
            return;
        }
        for (var y = (int)-Summoner.SummonRange; y <= (int)Summoner.SummonRange; y++)
        {
            for (var x = (int)-Summoner.SummonRange; x <= (int)Summoner.SummonRange; x++)
            {
                if (GameMap.DungeonBackGround[(int)Summoner.Position.ToGrid().Y + y, (int)Summoner.Position.ToGrid().X + x] == 1)
                {
                    spriteBatch.Draw(AssetManager.mSpriteSheet,
                        new Vector2((int)Summoner.Position.ToGrid().X + x + 0.5f, (int)Summoner.Position.ToGrid().Y + y + 0.5f) * Game1.sScaledPixelSize,
                        (Summoner.CanSummon() && MutableFriendlySummons.Count < 5) ? AssetManager.GetRectangleFromId16(61) : AssetManager.GetRectangleFromId16(62),
                        Color.White * 0.2f,
                        0f,
                        Game1.mOrigin,
                        Game1.mScale,
                        SpriteEffects.None,
                        0.8f);
                }
            }
        }
    }
    private void HandleSpellCoolDown(float deltaTime)
    {
        if (Summoner.FireBallCoolDown < Summoner.CoolDownLimitFire)
        {
            Summoner.FireBallCoolDown += deltaTime;
        }
        if (Summoner.HealingSpellCoolDown < Summoner.CoolDownLimitHeal)
        {
            Summoner.HealingSpellCoolDown += deltaTime;
        }
        if (Summoner.SpeedSpellCoolDown < Summoner.CoolDownLimitSpeed)
        {
            Summoner.SpeedSpellCoolDown += deltaTime;
        }
    }

    public void RefreshQuadTree()
    {
        QuadTree = SafeStaticGameObjectQuadTree.CreateQuadTree(
            rect: new Rect(new Vector2(-1000, -1000),
                size: new Vector2((GameMap.DungeonDimension + 10) * Game1.sScaledPixelSize,
                    y: (GameMap.DungeonDimension + 10) * Game1.sScaledPixelSize))
        );
        MapTree = SafeStaticGameObjectQuadTree.CreateQuadTree(
            rect: new Rect(new Vector2(0, 0),
                size: new Vector2((GameMap.DungeonDimension + 10) * Game1.sScaledPixelSize,
                    y: (GameMap.DungeonDimension + 10) * Game1.sScaledPixelSize))
        );
        UpdateQuadTree();
        FillMapTree();
    }
}