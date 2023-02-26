using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

// Sopra Hausaufgabe Gruppe 07 - Pascal Walter

namespace Hausaufgabe
{
    public class Game1 : Game
    {
        // Graphics
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;

        // Mouse and Input
        private MouseState mOldMouseState; // Used to register a mouse release

        //------------Contents--------------
        //--Textures--
        private Texture2D mBackgroundImage;
        private Texture2D mUniLogo;
        //--Sound effects--
        private SoundEffect mSoundHit;
        private SoundEffect mSoundMiss;
        //--Content attributes--
        private Vector2 mUniLogoPosition;
        private float mAngularVelocity;
        //----------------------------------

        // Auxiliary
        private Vector2 mCenter; // Center of the scaled window

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            mOldMouseState = Mouse.GetState();

            // Resize window to fit image (rather than fitting the image to the window)
            mGraphics.PreferredBackBufferWidth = 1280;
            mGraphics.PreferredBackBufferHeight = 1024;
            mGraphics.ApplyChanges();
            mCenter = new Vector2(mGraphics.PreferredBackBufferWidth / 2f, mGraphics.PreferredBackBufferHeight / 2f);

            // Initialize content attributes
            mUniLogoPosition = mCenter + new Vector2(250f, 0f);
            mAngularVelocity = MathHelper.PiOver2;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            // Load both images and sound-effects
            mBackgroundImage = Content.Load<Texture2D>("Background"); // 1280 x 1024
            mUniLogo = Content.Load<Texture2D>("Unilogo"); // 893 x 892
            mSoundHit = Content.Load<SoundEffect>("Logo_hit");
            mSoundMiss = Content.Load<SoundEffect>("Logo_miss");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Handle sound effect on mouse-click
            var newMouseState = Mouse.GetState();
            if (mOldMouseState.LeftButton == ButtonState.Pressed && newMouseState.LeftButton == ButtonState.Released && this.IsActive)
            {
                // Use circle inequality to check if mouse click was in Logo bounds
                // Divide by 8f since logo is scaled down to 25% of its size
                if (Math.Pow((newMouseState.X - mUniLogoPosition.X), 2) + Math.Pow((newMouseState.Y - mUniLogoPosition.Y), 2) <= Math.Pow(mUniLogo.Width / 8f, 2))
                {
                    mSoundHit.Play();
                }
                else
                {
                    mSoundMiss.Play();
                }
            }
            mOldMouseState = newMouseState;


            // Apply linear transformation to rotate the position vector of Unilogo around the screen's center
            // ElapsedGameTime is used to make the movement of the logo frame independent
            var m = Matrix.CreateRotationZ(-mAngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            mUniLogoPosition = Vector2.TransformNormal(mUniLogoPosition - mCenter, m) + mCenter;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackgroundImage, Vector2.Zero, Color.White);
            mSpriteBatch.Draw(mUniLogo,
                mUniLogoPosition,
                null,
                Color.White,
                0f,
                new Vector2(mUniLogo.Width / 2f, mUniLogo.Height / 2f),
                new Vector2(0.25f, 0.25f),
                SpriteEffects.None,
                0f);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}