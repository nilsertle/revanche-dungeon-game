using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project1
{
    internal sealed class Game1 : Game
    {
        private Texture2D mBackground;
        private Texture2D mUniLogoTexture;
        private Vector2 mLogoPosition;
        private float mLogoSpeed;
        private float mAngle;
        private SoundEffect mLogoHit;
        private SoundEffect mLogoMiss;
        private MouseState mOldMouseState;


        private readonly GraphicsDeviceManager mGraphics;    
        private SpriteBatch mSpriteBatch;


        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            mGraphics.PreferredBackBufferHeight = 1024;
            mGraphics.PreferredBackBufferWidth = 1280;
            mGraphics.ApplyChanges();
            mLogoPosition = new Vector2(mGraphics.PreferredBackBufferWidth / 2f, mGraphics.PreferredBackBufferHeight / 2f);
            mLogoSpeed = 0.05f;
            mAngle = 0f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            mUniLogoTexture = Content.Load<Texture2D>("Unilogo");
            mBackground = Content.Load<Texture2D>("Background");
            mLogoHit = Content.Load<SoundEffect>("Logo_hit");
            mLogoMiss = Content.Load<SoundEffect>("Logo_miss");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            mAngle += mLogoSpeed;
            mLogoPosition.X = 100 * (float)Math.Cos(mAngle) + mGraphics.PreferredBackBufferWidth / 2f;
            mLogoPosition.Y = 100 * (float)Math.Sin(mAngle) + mGraphics.PreferredBackBufferHeight / 2f;

            // sounds
            var state = Mouse.GetState();
            var mousePosition = new Point(state.X, state.Y);
            var screen = GraphicsDevice.PresentationParameters.Bounds;

            if (state.LeftButton == ButtonState.Pressed && screen.Contains(mousePosition) && mOldMouseState.LeftButton == ButtonState.Released && IsActive)
            {
                if ((mLogoPosition.X - mUniLogoTexture.Width*0.1f) < mousePosition.X && mousePosition.X < (mLogoPosition.X + mUniLogoTexture.Width * 0.1f)
                    && (mLogoPosition.Y - mUniLogoTexture.Height * 0.1f) < mousePosition.Y && mousePosition.Y < (mLogoPosition.Y + mUniLogoTexture.Height * 0.1f)
                    )
                {
                    mLogoHit.Play();
                }
                else
                {
                    mLogoMiss.Play();
                }
            }
            mOldMouseState = state;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(
                mBackground, 
                new Vector2(0,0), 
                null,
                Color.White
            );
            mSpriteBatch.Draw(
                mUniLogoTexture,
                mLogoPosition, 
                null,
                Color.White,
                0f,
                new Vector2(mUniLogoTexture.Width / 2, mUniLogoTexture.Height / 2),
                new Vector2(0.2f, 0.2f),
                SpriteEffects.None,
                0f
            );
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}