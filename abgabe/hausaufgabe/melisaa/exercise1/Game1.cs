﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;


namespace exercise1
{
    internal sealed class Game1 : Game
    {
        private Texture2D mUniLogo;
        private Texture2D mBackground;
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private Rectangle mUniLogoRectangle;
        private Rectangle mBackgroundRectangle;
        private int mScreenWidth;
        private int mScreenHeight;
        private Vector2 mRotationOrigin;
        private int mRotationRadius;
        private SoundEffect mAudioHit;
        private SoundEffect mAudioMiss;
        private bool mButtonPressed;
        private int mUniLogoCircleRadius;


        internal Game1()
        {
            mGraphics = new GraphicsDeviceManager(game: this);
            this.IsFixedTimeStep = true;//false;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 30d); //60);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            mBackgroundRectangle.X = 0;
            mBackgroundRectangle.Y = 0;
            mScreenWidth = GraphicsDevice.Viewport.Width;
            mScreenHeight = GraphicsDevice.Viewport.Height;
            mUniLogoRectangle.X =  mScreenWidth / 2 - mUniLogoRectangle.Width/2;
            mUniLogoRectangle.Y = mScreenHeight / 2 - mUniLogoRectangle.Height/2;
            mRotationOrigin.X = mScreenWidth / 2 - mUniLogoRectangle.Width /2;
            mRotationOrigin.Y = mScreenHeight / 2  - mUniLogoRectangle.Height/2;
            mRotationRadius = 100;
            mBackgroundRectangle.Width = mScreenWidth;
            mBackgroundRectangle.Height = mScreenHeight;
            mGraphics.PreferMultiSampling = true;
        }

        private void Rotate(GameTime gameTime)
        {
            Vector2 uniLogoCenter;
            uniLogoCenter.X =  (float) (Math.Cos(gameTime.TotalGameTime.TotalSeconds*2) - Math.Sin(gameTime.TotalGameTime.TotalSeconds*2)) *mRotationRadius + mUniLogoRectangle.Width/2;
            uniLogoCenter.Y =  (float) (Math.Sin(gameTime.TotalGameTime.TotalSeconds*2) + Math.Cos(gameTime.TotalGameTime.TotalSeconds*2)) *mRotationRadius + mUniLogoRectangle.Height/2;
            mUniLogoRectangle.X = (int) mRotationOrigin.X + (int) uniLogoCenter.X - mUniLogoRectangle.Width / 2;
            mUniLogoRectangle.Y = (int)mRotationOrigin.Y + (int)uniLogoCenter.Y - mUniLogoRectangle.Height / 2;
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            mUniLogo = Content.Load<Texture2D>(assetName: "unilogo");
            mBackground = Content.Load<Texture2D>(assetName: "background");
            mAudioHit = Content.Load<SoundEffect>(assetName: "Logo_hit");
            mAudioMiss = Content.Load<SoundEffect>(assetName: "Logo_miss");
            mUniLogoRectangle.Width = 150;
            mUniLogoRectangle.Height = 150;
            mUniLogoCircleRadius = mUniLogoRectangle.Width / 2;
        }
        
        private void PlayHit()
        {
            mAudioHit.Play();
        }
        
        private void PlayMiss()
        {
            mAudioMiss.Play();
        }

        private bool Hit()
        {
            return mUniLogoCircleRadius >= Math.Sqrt( Math.Pow( Mouse.GetState().X - (mUniLogoRectangle.X + mUniLogoRectangle.Width/2), 2 ) +
                                                       Math.Pow(Mouse.GetState().Y - (mUniLogoRectangle.Y + mUniLogoRectangle.Height/2), 2));
        }

        private void HandleMouseClick()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !mButtonPressed)
            {
                mButtonPressed = true;
                if (Hit())
                {
                    PlayHit();
                }
                else
                {
                    PlayMiss();
                }
            }
            if (Mouse.GetState().LeftButton == ButtonState.Released && mButtonPressed)
            {
                mButtonPressed = false;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            HandleMouseClick(); 
            Rotate(gameTime);
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackground, mBackgroundRectangle, Color.White);
            mSpriteBatch.Draw(mUniLogo, mUniLogoRectangle, 0.65f * Color.White);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}