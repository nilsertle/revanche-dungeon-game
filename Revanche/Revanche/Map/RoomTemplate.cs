using System;

namespace Revanche.Map
{
    public enum RoomType
    {
        SpawnRoom,
        EmptyRoom,
        BossRoom,
        LabyrinthRoom,
        LayerRoom,
        PillarRoom,
        TechDemoRoom,
        TechDemoBig,
        AiRoom
    }

    internal sealed class RoomTemplate
    {
        public readonly Grid mRoomGrid;
        private const int I0 = 0;
        private const int I1 = 1;
        private const int I2 = 2;
        private const int I3 = 3;
        private const int I4 = 4;
        private const int I5 = 5;
        private const int I6 = 6;
        private const int I7 = 7;
        private const int I8 = 8;
        private const int I9 = 9;
        private const int I10 = 10;
        private const int I11 = 11;
        private const int I12 = 12;
        private const int I13 = 13;
        private const int I14 = 14;
        private const int I15 = 15;
        private const int I16 = 16;
        private const int I17 = 17;
        private const int I18 = 18;
        private const int I19 = 19;
        private const int I20 = 20;
        private const int I21 = 21;
        private const int I22 = 22;
        private const int I23 = 23;
        private const int I24 = 24;
        private const int I25 = 25;
        private const int I26 = 26;
        private const int I27 = 27;
        private const int I28 = 28;
        private const int I29 = 29;
        private const int I30 = 30;
        private const int I31 = 31;
        private const int I32 = 32;
        private const int I33 = 33;
        private const int I34 = 34;

        private const int I36 = 36;


        private const int I39 = 39;

        private const int I41 = 41;
        private const int I42 = 42;
        private const int I43 = 43;
        private const int I44 = 44;
        private const int I45 = 45;


        public RoomTemplate(RoomType rt)
        {
            mRoomGrid = new Grid(I19, I15);
            switch (rt)
            {
                case RoomType.EmptyRoom:
                    EmptyRoomInterior();
                    break;
                case RoomType.LabyrinthRoom:
                    LabyrinthRoomInterior();
                    break;
                case RoomType.LayerRoom:
                    LayerRoomInterior();
                    break;
                case RoomType.PillarRoom:
                    PillarRoomInterior();
                    break;
                case RoomType.SpawnRoom:
                    SpawnRoomInterior();
                    break;
                case RoomType.BossRoom:
                    BossRoomInterior();
                    break;
                case RoomType.TechDemoRoom:
                    mRoomGrid = new Grid(I45, I25);
                    TechDemoRoomInterior();
                    break;
                case RoomType.TechDemoBig:
                    mRoomGrid = new Grid(I45, I45);
                    TechDemoBigInterior();
                    break;
                case RoomType.AiRoom:
                    mRoomGrid = new Grid(I25, I25);
                    AiRoomInterior();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rt), rt, null);
            }

        }

        private void SpawnRoomInterior()
        {
            mRoomGrid.SetCell(I8, I6, CellType.PillarTopCell);
            mRoomGrid.SetCell(I9, I6, CellType.ShrineTopCell);
            mRoomGrid.SetCell(I10, I6, CellType.PillarTopCell);

            mRoomGrid.SetCell(I8, I7, CellType.PillarMidCell);
            mRoomGrid.SetCell(I9, I7, CellType.ShrineMidCell);
            mRoomGrid.SetCell(I10, I7, CellType.PillarMidCell);

            mRoomGrid.SetCell(I8, I8, CellType.PillarBotCell);
            mRoomGrid.SetCell(I9, I8, CellType.ShrineBotCell);
            mRoomGrid.SetCell(I10, I8, CellType.PillarBotCell);
        }

        private void BossRoomInterior()
        {
            //Possible to add interior later. Left empty for better maneuverability.
        }

        private void EmptyRoomInterior()
        {
            //The EMPTY room is supposed to be EMPTY.
        }

        private void LabyrinthRoomInterior()
        {
            for (var i = 0; i < I2; i++)
            {
                mRoomGrid.SetCell(i, I9, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I18 - i, I5, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I7, i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I11, I14 - i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I8, I5 + i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I10, I9 - i, CellType.DestructAbleWallCell);
            }

            for (var i = 0; i < I4; i++)
            {
                mRoomGrid.SetCell(I9 + i, I5, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I6 + i, I9, CellType.DestructAbleWallCell);
            }

            for (var i = 0; i < I8; i++)
            {
                mRoomGrid.SetCell(I2, I2 + i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I16, I5 + i, CellType.DestructAbleWallCell);
            }

            for (var i = 0; i < I10; i++)
            {
                mRoomGrid.SetCell(I5, i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I13, I14 - i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I7 + i, I2, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I2 + i, I12, CellType.DestructAbleWallCell);
            }
        }

        private void LayerRoomInterior()
        {
            for (var i = 0; i < I11; i++)
            {
                mRoomGrid.SetCell(I4 + i, I2, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I4 + i, I12, CellType.DestructAbleWallCell);
            }
            for (var i = 0; i < I3; i++)
            {
                mRoomGrid.SetCell(I8 + i, I5, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I8 + i, I9, CellType.DestructAbleWallCell);
            }
            for (var i = 0; i < I5; i++)
            {
                mRoomGrid.SetCell(I2, I5 + i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I5, I5 + i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I13, I5 + i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I16, I5 + i, CellType.DestructAbleWallCell);
            }
        }

        private void PillarRoomInterior()
        {
            for (var y = -1; y <= 1; y++)
            {
                for (var x = -1; x <= 1; x++)
                {
                    mRoomGrid.SetCell(I3 + x, I3 + y, CellType.DestructAbleWallCell);
                    mRoomGrid.SetCell(I15 + x, I11 + y, CellType.DestructAbleWallCell);
                    mRoomGrid.SetCell(I9 + x, I7 + y, CellType.DestructAbleWallCell);
                    mRoomGrid.SetCell(I3 + x, I11 + y, CellType.DestructAbleWallCell);
                    mRoomGrid.SetCell(I15 + x, I3 + y, CellType.DestructAbleWallCell);
                }
            }
        }

        private void TechDemoBigInterior()
        {
            mRoomGrid.SetCell(I0, I25, CellType.PillarTopCell);
            mRoomGrid.SetCell(I1, I25, CellType.ShrineTopCell);
            mRoomGrid.SetCell(I2, I25, CellType.PillarTopCell);

            mRoomGrid.SetCell(I0, I26, CellType.PillarMidCell);
            mRoomGrid.SetCell(I1, I26, CellType.ShrineMidCell);
            mRoomGrid.SetCell(I2, I26, CellType.PillarMidCell);

            mRoomGrid.SetCell(I0, I27, CellType.PillarBotCell);
            mRoomGrid.SetCell(I1, I27, CellType.ShrineBotCell);
            mRoomGrid.SetCell(I2, I27, CellType.PillarBotCell);
        }

        private void TechDemoRoomInterior()
        {
            TechDemoDividers();
            TechDemoTopLeft();
            TechDemoTopRight();
            TechDemoBotLeft();
            TechDemoBotRight();
        }

        private void TechDemoDividers()
        {
            for (var i = 0; i <= I16; i++)
            {
                this.mRoomGrid.SetCell(i, I10, CellType.WallCell);
                this.mRoomGrid.SetCell(I44 - i, I10, CellType.WallCell);
            }

            for (var i = 0; i <= I20; i++)
            {
                this.mRoomGrid.SetCell(i, I14, CellType.WallCell);
                this.mRoomGrid.SetCell(I44 - i, I14, CellType.WallCell);
            }

            for (var i = 0; i <= I10; i++)
            {
                this.mRoomGrid.SetCell(I20, i, CellType.WallCell);
                this.mRoomGrid.SetCell(I24, i, CellType.WallCell);
            }

            for (var i = 0; i < I4; i++)
            {
                this.mRoomGrid.SetCell(I20, I18 + i, CellType.WallCell);
                this.mRoomGrid.SetCell(I24, I18 + i, CellType.WallCell);
            }
            this.mRoomGrid.SetCell(I20, I15, CellType.WallCell);
            this.mRoomGrid.SetCell(I20, I24, CellType.WallCell);
            this.mRoomGrid.SetCell(I24, I15, CellType.WallCell);
            this.mRoomGrid.SetCell(I24, I24, CellType.WallCell);

        }

        private void TechDemoTopLeft()
        {
            this.mRoomGrid.SetCell(I4, I5, CellType.WallCell);
            for (var i = 0; i <= I10; i++)
            {
                this.mRoomGrid.SetCell(I3 + i, I7, CellType.WallCell);
            }
            for (var i = 0; i <= I9; i++)
            {
                this.mRoomGrid.SetCell(I4 + i, I4, CellType.WallCell);
            }
            for (var i = 0; i <= I4; i++)
            {
                this.mRoomGrid.SetCell(I2, I3 + i, CellType.WallCell);
            }
            for (var i = 0; i <= I2; i++)
            {
                this.mRoomGrid.SetCell(I13, I5 + i, CellType.WallCell);
            }
            for (var i = 0; i <= I7; i++)
            {
                this.mRoomGrid.SetCell(I16, I2 + i, CellType.WallCell);
            }
            for (var i = 0; i <= I13; i++)
            {
                this.mRoomGrid.SetCell(I2 + i, I2, CellType.WallCell);
            }
        }

        private void TechDemoTopRight()
        {
            for (var y = 0; y <= I3; y++)
            {
                for (var x = 0; x <= I3; x++)
                {
                    mRoomGrid.SetCell(I33 + x, I3 + y, CellType.WallCell);
                }
            }

            for (var i = 0; i < I6; i++)
            {
                mRoomGrid.SetCell(I27, I2 + i, CellType.WallCell);
                mRoomGrid.SetCell(I42, I2 + i, CellType.WallCell);
            }

            for (var i = 0; i < I2; i++)
            {
                mRoomGrid.SetCell(I30, I1 + i, CellType.WallCell);
                mRoomGrid.SetCell(I30, I4 + i, CellType.WallCell);
                mRoomGrid.SetCell(I30, I7 + i, CellType.WallCell);
                mRoomGrid.SetCell(I39, I1 + i, CellType.WallCell);
                mRoomGrid.SetCell(I39, I4 + i, CellType.WallCell);
                mRoomGrid.SetCell(I39, I7 + i, CellType.WallCell);
            }
        }

        private void TechDemoBotLeft()
        {
            for (var i = 0; i < I5; i++)
            {
                mRoomGrid.SetCell(I2 + i, I16, CellType.WallCell);
                mRoomGrid.SetCell(I2 + i, I23, CellType.WallCell);
                mRoomGrid.SetCell(I4 + i, I18, CellType.WallCell);
                mRoomGrid.SetCell(I4 + i, I21, CellType.WallCell);
            }
            for (var i = 0; i < I6; i++)
            {
                mRoomGrid.SetCell(I2, I17 + i, CellType.WallCell);
            }
            for (var i = 0; i < I4; i++)
            {
                mRoomGrid.SetCell(I9, I18 + i, CellType.WallCell);
            }

            for (var i = 0; i < I8; i++)
            {
                mRoomGrid.SetCell(I12 + i, I18, CellType.WallCell);
                mRoomGrid.SetCell(I12 + i, I21, CellType.WallCell);
            }

            for (var i = 0; i < I2; i++)
            {
                mRoomGrid.SetCell(I12, I15 + i, CellType.WallCell);
                mRoomGrid.SetCell(I14, I16 + i, CellType.WallCell);
                mRoomGrid.SetCell(I16, I15 + i, CellType.WallCell);
                mRoomGrid.SetCell(I18, I16 + i, CellType.WallCell);
                mRoomGrid.SetCell(I12, I22 + i, CellType.WallCell);
            }
        }

        private void TechDemoBotRight()
        {
            for (var i = 0; i < I6; i++)
            {
                mRoomGrid.SetCell(I29, I16 + i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I28 + i, I22, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I39 + i, I16, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I39 + i, I18, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I39 + i, I21, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I39 + i, I23, CellType.DestructAbleWallCell);
            }

            for (var i = 0; i < I8; i++)
            {
                mRoomGrid.SetCell(I27, I15 + i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I36, I17 + i, CellType.DestructAbleWallCell);
            }

            for (var i = 0; i < I3; i++)
            {
                mRoomGrid.SetCell(I34, I15 + i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I31 + i, I18, CellType.DestructAbleWallCell);
            }

            for (var i = 0; i < I2; i++)
            {
                mRoomGrid.SetCell(I33, I20 + i, CellType.DestructAbleWallCell);
                mRoomGrid.SetCell(I30 + i, I16, CellType.DestructAbleWallCell);
            }
            mRoomGrid.SetCell(I34, I18, CellType.DestructAbleWallCell);
            mRoomGrid.SetCell(I39, I19, CellType.DestructAbleWallCell);
            mRoomGrid.SetCell(I43, I19, CellType.DestructAbleWallCell);
            mRoomGrid.SetCell(I41, I20, CellType.DestructAbleWallCell);
            mRoomGrid.SetCell(I30, I23, CellType.DestructAbleWallCell);
            mRoomGrid.SetCell(I28, I24, CellType.DestructAbleWallCell);
            mRoomGrid.SetCell(I32, I24, CellType.DestructAbleWallCell);

        }
        private void AiRoomInterior()
        {
            mRoomGrid.SetCell(I0, I5, CellType.PillarTopCell);
            mRoomGrid.SetCell(I1, I5, CellType.ShrineTopCell);
            mRoomGrid.SetCell(I2, I5, CellType.PillarTopCell);

            mRoomGrid.SetCell(I0, I6, CellType.PillarMidCell);
            mRoomGrid.SetCell(I1, I6, CellType.ShrineMidCell);
            mRoomGrid.SetCell(I2, I6, CellType.PillarMidCell);

            mRoomGrid.SetCell(I0, I7, CellType.PillarBotCell);
            mRoomGrid.SetCell(I1, I7, CellType.ShrineBotCell);
            mRoomGrid.SetCell(I2, I7, CellType.PillarBotCell);
        }

    }
}
