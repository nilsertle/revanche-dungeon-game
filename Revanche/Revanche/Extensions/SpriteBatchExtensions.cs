using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Revanche.Extensions;

public static class SpriteBatchExtensions
{
    private static Texture2D sBlankTexture;

    private static Texture2D BlankTexture(this GraphicsResource s)
    {
        if (sBlankTexture != null)
        {
            return sBlankTexture;
        }

        sBlankTexture = new Texture2D(s.GraphicsDevice, 1, 1);
        sBlankTexture.SetData(new[] { Color.White });
        return sBlankTexture;
    }

    internal static void DrawRectangleOutline(this SpriteBatch s, Rectangle rectangle, int linewidth, Color color, float layer=0f)
    {
        s.Draw(s.BlankTexture(), new Rectangle(rectangle.X, rectangle.Y, linewidth, rectangle.Height + linewidth), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        s.Draw(s.BlankTexture(), new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + linewidth, linewidth), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        s.Draw(s.BlankTexture(), new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, linewidth, rectangle.Height + linewidth), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        s.Draw(s.BlankTexture(), new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + linewidth, linewidth), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
    }
}