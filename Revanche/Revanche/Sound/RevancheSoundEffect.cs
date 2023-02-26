using Microsoft.Xna.Framework.Audio;
using Revanche.Core;

namespace Revanche.Sound
{
    internal sealed class RevancheSoundEffect
    {
        public InstanceState mState = InstanceState.Pending;
        private readonly SoundEffectInstance mSoundInstance;
        private float mTimer;
        private readonly float mMaxTime;

        public RevancheSoundEffect(SoundEffect sound)
        {
            // + 1, because some sound have the length of "0" seconds, and wouldn't play otherwise
            mMaxTime = sound.Duration.Seconds + 1;
            mSoundInstance = sound.CreateInstance();
        }

        public void Update(float deltaTime)
        {
            if (mState == InstanceState.Paused)
            {
                return;
            } 
            mTimer += deltaTime;
            if (mTimer >= mMaxTime)
            {
                Stop();
                mState = InstanceState.LimitReached;
            }
        }

        public void Play(float volume)
        {
            mSoundInstance.Volume = volume;
            mSoundInstance.Play();
        }

        public void Pause()
        {
            mSoundInstance.Pause();
        }

        public void Resume()
        {
            mSoundInstance.Resume();
        }

        public void Stop()
        {
            mSoundInstance.Stop();
            mSoundInstance.Dispose();
        }

        public void ChangeVolume(float volume)
        {
            mSoundInstance.Volume = volume;
        }
    }
}
