using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Abgabe_02
{
    public sealed class Game1 : Game
    {
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;


        private Texture2D mBackground;
        private Texture2D mLogo;
        private SoundEffect mHit;
        private SoundEffect mMiss;
        private Vector2 mLogoCorner;
        private Vector2 mRotationCenter;
        private float mRotationSpeed;
        private float mLogoScale;
        private uint mRadius;

        private MouseState mLastMouseState;
        private MouseState mCurrentMouseState;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            mRotationCenter = new Vector2(300, 150);
            mRadius = 120;
            mLogoCorner = new Vector2(mRotationCenter.X, mRotationCenter.Y + mRadius);
            mRotationSpeed = 1;
            mLogoScale = 0.2f;
            mLastMouseState = Mouse.GetState();
            mCurrentMouseState = Mouse.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(mGraphics.GraphicsDevice);

            mBackground = Content.Load<Texture2D>("Background");
            mLogo = Content.Load<Texture2D>("Unilogo");
            mHit = Content.Load<SoundEffect>("Logo_hit");
            mMiss = Content.Load<SoundEffect>("Logo_miss");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
                
            // Implement logic for rotation (DONE)

            mLogoCorner.X =
                (float)(Math.Cos((float)(gameTime.TotalGameTime.TotalSeconds * mRotationSpeed * Math.PI)) * mRadius + mRotationCenter.X);
            mLogoCorner.Y =
                (float)(Math.Sin((float)(gameTime.TotalGameTime.TotalSeconds * mRotationSpeed * Math.PI)) * mRadius +
                        mRotationCenter.Y);

            //Implement logic for click and collision (DONE)
            var mousePosition = new Point(Mouse.GetState().X, Mouse.GetState().Y);
            Rectangle area = new (new Point((int)mLogoCorner.X, (int)mLogoCorner.Y),
                new Point((int)(mLogoScale * mLogo.Width), (int)(mLogoScale * mLogo.Height)));

            mCurrentMouseState = Mouse.GetState();

            if (mLastMouseState.LeftButton == ButtonState.Released && mCurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (area.Contains(mousePosition))
                {
                    mHit.Play();
                }
                else
                {
                    mMiss.Play();
                }
            }

            mLastMouseState = mCurrentMouseState;
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackground, GraphicsDevice.Viewport.Bounds, Color.White);
            mSpriteBatch.Draw(mLogo, mLogoCorner, null, Color.White, 0, new Vector2(0, 0), mLogoScale, 0, 1);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}