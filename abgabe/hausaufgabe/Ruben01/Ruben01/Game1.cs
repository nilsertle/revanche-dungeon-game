using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

// If the game doesn't run the first time, please try again. This happened to me, without changing anything in between the runs.
namespace Ruben01
{
    public sealed class Game1 : Game
    {
        private GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private readonly GraphicsAdapter mGraphicsAdapter = GraphicsAdapter.DefaultAdapter;

        private Texture2D mLogo;
        private Texture2D mBackground;
        private SoundEffect mSoundHit;
        private SoundEffect mSoundMiss;

        MouseState mMouseState;
        private Rectangle mScreenRect;
        private Rectangle mLogoRect;
        // The actual size of the image is 339x339, but using this size causes graphical glitches. 338 seems save to use.
        // The center of the logo. Used to determine the distance of the logo to a mouse click event.
        private Vector2 mLogoCenter = new (79, 79);
        // the left top corner of the logo, important for the draw method.
        private Vector2 mLogoLeftTop = new (0, 0);
        private Vector2 mRotationCenter;
        private readonly int mLogoRadius = 79;
        private bool mMouseClicked = true;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            mScreenRect = new Rectangle(0, 0, mGraphicsAdapter.CurrentDisplayMode.Width, mGraphicsAdapter.CurrentDisplayMode.Height);
            mLogoRect = new Rectangle(0, 0, 159, 159);
            mRotationCenter.X = mGraphicsAdapter.CurrentDisplayMode.Width / (float)4;
            mRotationCenter.Y = mGraphicsAdapter.CurrentDisplayMode.Height / (float)4;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            mLogo = Content.Load<Texture2D>("uni freiburg siegel sopra");
            mBackground = Content.Load<Texture2D>("uni freiburg hintergund sopra");
            mSoundHit = Content.Load<SoundEffect>(assetName: "Wilhelm Scream");
            mSoundMiss = Content.Load<SoundEffect>(assetName: "Medium_Whoosh_04");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            mMouseState = Mouse.GetState();
            if (mMouseState.LeftButton == ButtonState.Pressed && mMouseClicked)
            {
                // check if the left mouse button was pressed inside the mLogo
                float distance = Vector2.Distance(mLogoCenter, mMouseState.Position.ToVector2());
                if (distance < mLogoRadius)
                {
                    mSoundHit.Play();
                }
                else
                {
                    mSoundMiss.Play();
                }
                mMouseClicked = false;
            }

            if (mMouseState.LeftButton == ButtonState.Released)
            {
                mMouseClicked = true;
            }
            // Compute the rotation of the logo
            mLogoCenter.X = (float)(Math.Cos((float)(gameTime.TotalGameTime.TotalSeconds * Math.PI)) * (mLogoRadius * 1.5) + mRotationCenter.X);
            mLogoCenter.Y = (float)(Math.Sin((float)(gameTime.TotalGameTime.TotalSeconds * Math.PI)) * (mLogoRadius * 1.5) + mRotationCenter.Y);
            mLogoLeftTop.X = mLogoCenter.X - mLogoRadius;
            mLogoLeftTop.Y = mLogoCenter.Y - mLogoRadius;
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackground, new Vector2(0, 0), mScreenRect, Color.White);
            mSpriteBatch.Draw(mLogo, mLogoLeftTop, mLogoRect, Color.White);
            mSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}