using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.AchievementSystem;
using Revanche.Core;
using Revanche.Input;
using Revanche.Managers;
using Revanche.Screens;
using Revanche.Screens.Game;
using Revanche.Sound;
using Revanche.Stats;

namespace Revanche
{
    internal sealed class Game1 : Game
    {
        // HD
        public static int mScreenWidth = 1024;
        public static int mScreenHeight = 768;
        public static Vector2 mCenter; 
        public static Vector2 mScale = new(4f, 4f);
        private const float SPixelSize = 16; // was public
        public static readonly float sScaledPixelSize = (int)(SPixelSize * mScale.X);
        public static Vector2 mOrigin = new(SPixelSize / 2, SPixelSize / 2);
        public static bool mDebugMode;
        public static bool mFullScreen;
        public static Language mLanguage = Language.English; // TODO: Change to English once all textures are ready

        // Graphics
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private SpriteFont mSpriteFont;

        // Managers and Handlers
        private ScreenManager mScreenManager;
        private InputMapper mInputMapper;
        private SoundManager mSoundManager;
        private SaveManager mSaveManager;
        private EventDispatcher mEventDispatcher;

        private AchievementManager mAchievementManager;
        private StatisticManager mStatManager;
        private AssetManager mAssetManager;
        private PopupManager mPopupManager;
        private FrameCounter mFrameCounter;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Resize window
            mCenter = new Vector2(mScreenWidth / 2f, mScreenHeight / 2f);
            mGraphics.PreferredBackBufferWidth = mScreenWidth;
            mGraphics.PreferredBackBufferHeight = mScreenHeight;
            mGraphics.ApplyChanges();

            // Initialize managers
            mEventDispatcher = new EventDispatcher();
            mInputMapper = new InputMapper();
            mSaveManager = new SaveManager();
            mAchievementManager = new AchievementManager(mEventDispatcher);
            mStatManager = new StatisticManager(mEventDispatcher);
            mAssetManager = new AssetManager(Content);
            mPopupManager = new PopupManager(mEventDispatcher, mAssetManager);
            mSoundManager = new SoundManager(mAssetManager, mEventDispatcher);
            mScreenManager = new ScreenManager(new ScreenFactory(mAssetManager, mEventDispatcher, mSaveManager, mAchievementManager, mStatManager), mEventDispatcher);
            mFrameCounter = new FrameCounter();

            // Events
            mEventDispatcher.OnExit += Exit;
            mEventDispatcher.OnResolutionRequest += HandleResize;
            mEventDispatcher.OnFullScreenRequest += HandleFullScreen;
            Exiting += OnGameExiting;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            mSpriteFont = Content.Load<SpriteFont>("FramecounterFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            // Get abstract input
            var input = mInputMapper.Update();

            // Updates the game logic
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            mPopupManager.Update(deltaTime);
            mScreenManager.Update(deltaTime, input);
            mSoundManager.Update(deltaTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            mScreenManager.Draw(mSpriteBatch);
            mPopupManager.Draw(mSpriteBatch);
            if (mDebugMode)
            {
                mFrameCounter.Draw(mSpriteBatch, mSpriteFont);
                mFrameCounter.Update();
            }

            base.Draw(gameTime);
        }

        private void OnGameExiting(object sender, EventArgs e)
        {
            mAchievementManager.SaveAchievements();
            mStatManager.SaveStats();
        }

        private void HandleResize(ResolutionEvent resEvent)
        {
            mGraphics.IsFullScreen = false;
            mFullScreen = false;
            mGraphics.PreferredBackBufferWidth = resEvent.Width;
            mGraphics.PreferredBackBufferHeight = resEvent.Height;
            mScreenWidth = resEvent.Width;
            mScreenHeight = resEvent.Height;
            mCenter = new Vector2(mGraphics.PreferredBackBufferWidth/2f, mGraphics.PreferredBackBufferHeight/2f);
            mGraphics.ApplyChanges();
            mScreenManager.RebuildScreens();
        }

        private void HandleFullScreen()
        {
            mScreenWidth = Math.Min(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, 1920);
            mScreenHeight = Math.Min(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, 1080);
            mGraphics.PreferredBackBufferWidth = mScreenWidth;
            mGraphics.PreferredBackBufferHeight = mScreenHeight;
            mCenter = new Vector2(mGraphics.PreferredBackBufferWidth / 2f, mGraphics.PreferredBackBufferHeight / 2f);
            mGraphics.IsFullScreen = true;
            mFullScreen = true;
            mGraphics.HardwareModeSwitch = false;
            mGraphics.ApplyChanges();
            mScreenManager.RebuildScreens();
        }
    }

}