using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace hausaufgabe1;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _backgroundSprite;
    private Texture2D _logoSprite;
    private SoundEffect _hit;
    private SoundEffect _miss;

    // 892 / 8 = 111.5 (bc of the 0.125f scaling), radius = 111.5 / 2 = 55.75
    private float _logoRadius = 55.75f;
    private Vector2 _logoPos;
    private Vector2 _logoPosMid;
    private float _x;
    private Vector2 _origin = new Vector2(0, 0);

    public float RotationVelocity = 3f;
    public float LinearVelocity = 4f;

    private MouseState _mouse;
    private bool _mouseReleased = true;
    
    

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _backgroundSprite = Content.Load<Texture2D>("background");
        _logoSprite = Content.Load<Texture2D>("logo");
        _hit = Content.Load<SoundEffect>("Logo_hit");
        _miss = Content.Load<SoundEffect>("Logo_miss");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        
        // monogmame window has the 800x480
        // radius = 55.75, 400 - 55.75 = 344.25; 20 + 100 + 100 - 55.75 = 164.25, bc the spritedraw coordinates e.g. (0,0) are not the middle of the Sprite
        _logoPos = new Vector2(100 * Convert.ToSingle(Math.Sin(_x)) + 344.25f, 100 * Convert.ToSingle(Math.Cos(_x)) + 164.25f);
        // the real Middle of the logo on the screen
        _logoPosMid = new Vector2(100 * Convert.ToSingle(Math.Sin(_x)) + 400f, 100 * Convert.ToSingle(Math.Cos(_x)) + 220f);
        _x += 0.1f;
        if (Math.Abs(_x - 2 * 3.1417f) < 0)
        {
            _x = 0;
        }
        
        _mouse = Mouse.GetState();
        if (_mouse.LeftButton == ButtonState.Pressed && _mouseReleased && _mouse.Position.X >= 0 && _mouse.Position.X < 800 &&
            _mouse.Position.Y >= 0 && _mouse.Position.Y < 480)
        {
            float distance = Vector2.Distance(_logoPosMid, _mouse.Position.ToVector2());
            if (distance < _logoRadius)
            {
                _hit.Play();
            }
            else
            {
                _miss.Play();
            }

            _mouseReleased = false;
        }

        if (_mouse.LeftButton == ButtonState.Released)
        {
            _mouseReleased = true;
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        
        _spriteBatch.Begin();
        _spriteBatch.Draw(_backgroundSprite, new Vector2(0, 0), null, Color.White, 0, _origin, new Vector2(2.75f, 2.1f), SpriteEffects.None, 0f);
        _spriteBatch.Draw(_logoSprite, _logoPos, null, Color.White * 0.5f, 0, _origin, 0.125f, SpriteEffects.None, 0f);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}