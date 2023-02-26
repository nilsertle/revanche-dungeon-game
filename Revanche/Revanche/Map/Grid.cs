using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Revanche.Map
{
    /// <summary>
    /// Contains all possible cell types.
    /// </summary>
    public enum CellType
    {
        EmptyCell,
        WallCell,
        GroundCell,
        ShrineTopCell,
        ShrineMidCell,
        ShrineBotCell,
        PillarTopCell,
        PillarMidCell,
        PillarBotCell,
        DestructAbleWallCell
    }

    /// <summary>
    /// Class Representation of a single Cell of the Dungeon.
    /// </summary>
    public sealed class Cell
    {
        [JsonProperty] public CellType mCellType;

        public Cell(CellType ct)
        {
            mCellType = ct;
        }
    }

    /// <summary>
    /// Class Representation of an entire Floor. Is a matrix storing Cells.
    /// </summary>
    public sealed class Grid
    {
        private const int BloodShrineTopSpriteId = 2;
        private const int BloodShrineBottomSpriteId = 3;

        private const int I5 = 5;

        private const int BloodShrineTopId = 5;
        private const int PillarTopSpriteId = 6;
        private const int PillarMiddleSpriteId = 7;
        private const int PillarBottomSpriteId = 8;
        private const int HorizontalCrackSpriteId = 9;
        private const int SkullAndBoneSpriteId = 10;
        private const int VerticalCrackSpriteId = 11;
        private const int OldBloodStainSpriteId = 12;
        private const int OldBloodStainOnWallSpriteId = 13;
        private const int SkullOnWallSpriteId = 14;
        private const int DestructAbleWallPhase1SpriteId = 18;
        private const int EmptySprite = 31;

        private const int I20 = 20;
        private const int I30 = 30;
        
        private const int I40 = 40;

        private const int I35 = 35;
        private const int I60 = 60;
        private const int I70 = 70;

        private const int I1000 = 1000;

        [JsonProperty] public Cell[,] mCellGrid;
        [JsonProperty] public int mWidth;
        [JsonProperty] public int mHeight;


        public Grid(int width, int height)
        {
            this.mCellGrid = new Cell[height, width];
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    this.mCellGrid[i, j] = new Cell(CellType.EmptyCell);
                }
            }
            this.mWidth = width;
            this.mHeight = height;
        }

        internal void SetCell(int x, int y, CellType c)
        {
            this.mCellGrid[y, x].mCellType = c;
        }

        internal CellType GetCellType(int x, int y)
        {
            return this.mCellGrid[y, x].mCellType;
        }

        /// <summary>
        /// Gives boolean representation weather or not a cell has collisions with characters.
        /// </summary>
        /// <returns>Boolean Matrix, true if wall or empty, false if Ground</returns>
        internal bool[,] CollisionMatrix()
        {
            var outMatrix = new bool[this.mHeight, this.mWidth];
            for (var i = 0; i < this.mHeight; i++)
            {
                for (var j = 0; j < this.mWidth; j++)
                {
                    if (this.mCellGrid[i, j].mCellType == CellType.GroundCell || this.mCellGrid[i, j].mCellType == CellType.ShrineBotCell)
                    {
                        outMatrix[i, j] = false;
                    }
                    else
                    {
                        outMatrix[i, j] = true;
                    }
                }
            }
            return outMatrix;
        }

        /// <summary>
        /// Gives the Matrix containing the fitting spriteIDs for the Background, so the Floor and Walls.
        /// </summary>
        /// <returns> Matrix of spriteIDs of the Background </returns>
        internal int[,] BackgroundSpriteMatrix()
        {
            var outMatrix = new int[this.mHeight, this.mWidth];
            for (var i = 0; i < this.mHeight; i++)
            {
                for (var j = 0; j < this.mWidth; j++)
                {
                    outMatrix[i, j] = BackgroundSprite(this.mCellGrid[i, j].mCellType);
                }
            }
            return outMatrix;
        }

        private static int BackgroundSprite(CellType cellType)
        {
            var val = cellType switch
            {
                CellType.EmptyCell => EmptySprite,
                CellType.WallCell => 0,
                CellType.GroundCell => 1,
                CellType.ShrineTopCell => 0,
                CellType.ShrineMidCell => 0,
                CellType.ShrineBotCell => 1,
                CellType.PillarTopCell => 0,
                CellType.PillarMidCell => 0,
                CellType.PillarBotCell => 0,
                CellType.DestructAbleWallCell => 1,
                _ => EmptySprite
            };
            return val;
        }

        private static int WallSkin()
        {
            return RandomNumberGenerator.GetInt32(0, I1000) switch
            {
                < I35 => PillarMiddleSpriteId,
                < I60 => OldBloodStainOnWallSpriteId,
                < I70 => SkullOnWallSpriteId,
                _ => 0
            };
        }

        private static int GroundSkin()
        {
            return RandomNumberGenerator.GetInt32(0, I1000) switch
            {
                < I5 => VerticalCrackSpriteId,
                < I20 => SkullAndBoneSpriteId,
                < I30 => HorizontalCrackSpriteId,
                < I40 => OldBloodStainSpriteId,
                _ => 1
            };
        }

        /// <summary>
        /// Gives the Matrix containing the fitting spriteIDs for the Midground, so shrines (and possibly other things).
        /// </summary>
        /// <returns> Matrix of spriteIDs of the Background </returns>
        internal int[,] MidGroundSpriteMatrix()
        {
            var outMatrix = new int[this.mHeight, this.mWidth];
            for (var i = 0; i < this.mHeight; i++)
            {
                for (var j = 0; j < this.mWidth; j++)
                {
                    outMatrix[i, j] = MidGroundSprite(this.mCellGrid[i, j].mCellType);
                }
            }
            return outMatrix;
        }

        private static int MidGroundSprite(CellType cellType)
        {
            var val = cellType switch
            {
                CellType.WallCell => WallSkin(),
                CellType.GroundCell => GroundSkin(),
                CellType.ShrineTopCell => BloodShrineTopId,
                CellType.ShrineMidCell => BloodShrineTopSpriteId,
                CellType.ShrineBotCell => BloodShrineBottomSpriteId,
                CellType.PillarTopCell => PillarTopSpriteId,
                CellType.PillarMidCell => PillarMiddleSpriteId,
                CellType.PillarBotCell => PillarBottomSpriteId,
                CellType.DestructAbleWallCell => DestructAbleWallPhase1SpriteId,
                CellType.EmptyCell => EmptySprite,
                _ => EmptySprite
            };
            return val;
        }

    }
}
