using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Revanche.Managers;
using System;
using Revanche.Extensions;

namespace Revanche.Map
{
    public sealed class FogOfWar
    {
        private const int I16 = 16;

        private const float F04 = 0.4f;

        [JsonProperty] internal bool[,] FogMask { get; set; }
        [JsonProperty] private int Size { get; set; }
        [JsonProperty] internal bool Use { get; set; }

        private FogOfWar(){}

        public static FogOfWar CreateFogOfWar(Map map, bool use)
        {
            var fog = new FogOfWar();
            fog.Size = map.DungeonDimension;
            fog.FogMask = new bool[fog.Size, fog.Size];
            fog.ResetTo(true);
            fog.Use = use;
            return fog;
        }

        private void ResetTo(bool desired)
        {
            for (var y = 0; y < this.Size; y++)
            {
                for (var x = 0; x < this.Size; x++)
                {
                    this.FogMask[y, x] = desired;
                }
            }
        }

        internal void Update(Vector2 position, int updateRange)
        {
            if (!Use)
            {
                return;
            }

            var gridPos = position.ToGrid();

            for (var y = Math.Clamp(gridPos.Y - updateRange + 1, 0, Size); y < Math.Clamp(gridPos.Y + updateRange, 0, Size); y++)
            {
                for (var x = Math.Clamp(gridPos.X - updateRange + 1, 0, Size); x < Math.Clamp(gridPos.X + updateRange, 0, Size); x++)
                {
                    this.FogMask[(int)x, (int)y] = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle visibleMapArea)
        {
            if (Game1.mDebugMode || !this.Use)
            {
                return;
            }
            for (var y = Math.Max(0, visibleMapArea.Top); y < Math.Min(this.Size - 1, visibleMapArea.Bottom); y++)
            {
                for (var x = Math.Max(0, visibleMapArea.Left); x < Math.Min(this.Size - 1, visibleMapArea.Right); x++)
                {
                    if (this.FogMask[x, y])
                    {
                        spriteBatch.Draw(AssetManager.mSpriteSheet,
                            new Vector2(Game1.sScaledPixelSize * x, Game1.sScaledPixelSize * y),
                            AssetManager.GetRectangleFromId16(I16),
                            Color.White,
                            0f,
                            Vector2.Zero,
                            Game1.mScale,
                            SpriteEffects.None,
                            F04);
                    }

                }
            }
        }
    }
}
