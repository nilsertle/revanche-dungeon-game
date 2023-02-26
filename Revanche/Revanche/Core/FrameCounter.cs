using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Revanche.Core
{
    internal sealed class FrameCounter
    {
        private const int BufferLength = 8;
        private const int WantedAverage = 60;

        private readonly Stopwatch mStopwatch;
        private readonly Queue<long> mQueue;
        private readonly int mLen;

        internal FrameCounter()
        {
            mStopwatch = new Stopwatch();
            mLen = BufferLength;
            mQueue = new Queue<long>(mLen);
            for (var i = 0; i < mLen; i++)
            {
                mQueue.Enqueue(WantedAverage);
            }

            mStopwatch.Start();
        }

        internal void Update()
        {
            mQueue.Dequeue();
            var sec = mStopwatch.Elapsed.TotalSeconds;
            var frames = (long)(1 / sec);
            mQueue.Enqueue(frames < 0 ? 0 : frames);
            mStopwatch.Restart();
        }

        private int FramesPerSecond()
        {
            return (int)mQueue.Sum() / mLen;
        }

        internal void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont,
                $"FPS: {FramesPerSecond().ToString()}",
                new Vector2(1, 1),
                Color.Green);
            spriteBatch.End();
        }
    }
}