using System;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Revanche.Managers;
using System.Linq;
using Revanche.Core;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Revanche.Sound;
/// <summary>
/// Implements all logic for the game sound & music to work. See the individual methods for more details.
/// </summary>
internal sealed class SoundManager
{
    // Only two SoundEffectInstances are needed, because the Play function overrides them at each call (There can still be
    // several soundeffects at the same time (can be different, but don't have to)).
    private static SoundEffectInstance sSCurrentMusicInstance;
    private SoundEffectInstance mNextMusicInstance;

    // Takes the name of a SoundEffectInstance as key, and the SoundEffectInstance itself as value
    private readonly Dictionary<SoundEffects, SoundEffect> mSoundEffects = new();
    // Similar to sSoundEffects. Stores the music of the game instead. This is in an own dictionary, because the music will often be
    // handled differently from the sound effects; eg. looping, fading into each other etc.
    private readonly Dictionary<Songs, SoundEffect> mMusic = new();

    private readonly List<Tuple<SoundEffects, float>> mWillBePlayed = new ();
    private readonly List<RevancheSoundEffect> mActiveSounds = new();

    // the master volume for all sound in the game. Ranges from 0f to 1f
    private float mSoundVolume;
    private float mMusicVolume;


    private Vector2 mSummonerPosition;

    // These variables are stored by the ChangeMusic(string) method, so ChangeMusic(gameTime) can access them. These variables are
    // marked as such with the word "Fade" at the beginning of their names.
    private float mFadeTime;
    private Songs mFadeNextMusic = Songs.NoFade;
    private bool mFadeFlag;
    private float mFadeTimespan;
    private float mFadeSongStart;

    internal SoundManager(AssetManager assetManager, EventDispatcher eventDispatcher)
    {
        mSoundVolume = 1;
        mMusicVolume = 1;

        // Store SoundEvents
        mSoundEffects.Add(SoundEffects.Movement, assetManager.mSoundMovement);
        mSoundEffects.Add(SoundEffects.PowerUpSound, assetManager.mSoundPowerUp);
        mSoundEffects.Add(SoundEffects.BombSound, assetManager.mSoundBomb);
        mSoundEffects.Add(SoundEffects.GameStart, assetManager.mSoundGameStart);
        mSoundEffects.Add(SoundEffects.MeleeAlliedHit, assetManager.mSoundMeleeAlliedHit);
        mSoundEffects.Add(SoundEffects.MeleeEnemyHit, assetManager.mSoundMeleeEnemyHit);
        mSoundEffects.Add(SoundEffects.BarClick, assetManager.mSoundBarClick);
        mSoundEffects.Add(SoundEffects.ButtonClick, assetManager.mSoundButtonClick);
        mSoundEffects.Add(SoundEffects.InvalidAction, assetManager.mSoundInvalidMovement);
        mSoundEffects.Add(SoundEffects.ChestOpening, assetManager.mSoundChestOpening);
        mSoundEffects.Add(SoundEffects.GameLost, assetManager.mSoundGameLost);
        mSoundEffects.Add(SoundEffects.GameWon, assetManager.mSoundGameWon);
        mSoundEffects.Add(SoundEffects.PickupSoul, assetManager.mSoundPickupSoul);
        mSoundEffects.Add(SoundEffects.GenericSummoning, assetManager.mSoundGenericSummoning);
        mSoundEffects.Add(SoundEffects.UpgradesConfirmed, assetManager.mSoundUpgradeConfirmed);
        mSoundEffects.Add(SoundEffects.UsePotion, assetManager.mSoundUsePotion);
        mSoundEffects.Add(SoundEffects.LevelUp, assetManager.mSoundLevelUp);
        mSoundEffects.Add(SoundEffects.HitWall, assetManager.mSoundWallHit);
        mSoundEffects.Add(SoundEffects.DestructibleHit, assetManager.mSoundDestructibleWallHit);
        mSoundEffects.Add(SoundEffects.DestructibleBreaks, assetManager.mSoundDestructibleWallBreaks);
        mSoundEffects.Add(SoundEffects.SummonerHit, assetManager.mSoundSummonerGetsHit);
        mSoundEffects.Add(SoundEffects.ConanSpecial, assetManager.mSoundConanSpecial);

        mSoundEffects.Add(SoundEffects.DemonDeath, assetManager.mSoundDemonDeath);
        mSoundEffects.Add(SoundEffects.SkeletonDeath, assetManager.mSoundSkeletonDeath);
        mSoundEffects.Add(SoundEffects.StormCloudDeath, assetManager.mSoundStormCloudDeath);
        mSoundEffects.Add(SoundEffects.WaterElementalDeath, assetManager.mSoundWaterElementalDeath);
        mSoundEffects.Add(SoundEffects.SeedlingDeath, assetManager.mSoundSeedlingDeath);
        mSoundEffects.Add(SoundEffects.SummonerDeath, assetManager.mSoundSummonerDeath);

        mSoundEffects.Add(SoundEffects.ConanDeath, assetManager.mSoundConanDeath);
        mSoundEffects.Add(SoundEffects.PensionerDeath, assetManager.mSoundPensionerDeath);
        mSoundEffects.Add(SoundEffects.PaladinDeath, assetManager.mSoundPaladinDeath);
        mSoundEffects.Add(SoundEffects.PirateDeath, assetManager.mSoundPirateDeath);
        mSoundEffects.Add(SoundEffects.BombMagicianDeath, assetManager.mSoundBombMagicianDeath);
        mSoundEffects.Add(SoundEffects.ArchEnemyDeath, assetManager.mSoundArchEnemyDeath);

        mSoundEffects.Add(SoundEffects.FireBallSpawn, assetManager.mSoundFireBallSpawn);
        mSoundEffects.Add(SoundEffects.SpeedSpellSpawn, assetManager.mSoundSpeedSpellSpawn);
        mSoundEffects.Add(SoundEffects.HealingSpellSpawn, assetManager.mSoundHealingSpellSpawn);
        mSoundEffects.Add(SoundEffects.SulfurChunkSpawn, assetManager.mSoundSulfurChunkSpawn);
        mSoundEffects.Add(SoundEffects.WaterDropletSpawn, assetManager.mSoundWaterDropletSpawn);
        mSoundEffects.Add(SoundEffects.StickSpawn, assetManager.mSoundStickSpawn);
        mSoundEffects.Add(SoundEffects.GunShotSpawn, assetManager.mSoundGunShotSpawn);
        mSoundEffects.Add(SoundEffects.BearTrapSpawn, assetManager.mSoundBearTrapSpawn);

        mSoundEffects.Add(SoundEffects.FireBallImpact, assetManager.mSoundFireBallImpact);
        mSoundEffects.Add(SoundEffects.SpeedSpellImpact, assetManager.mSoundSpeedSpellImpact);
        mSoundEffects.Add(SoundEffects.HealingSpellImpact, assetManager.mSoundHealingSpellImpact);
        mSoundEffects.Add(SoundEffects.SulfurChunkImpact, assetManager.mSoundSulfurChunkImpact);
        mSoundEffects.Add(SoundEffects.WaterDropletImpact, assetManager.mSoundWaterDropletImpact);
        mSoundEffects.Add(SoundEffects.StickImpact, assetManager.mSoundStickImpact);
        mSoundEffects.Add(SoundEffects.GunShotImpact, assetManager.mSoundGunShotImpact);
        mSoundEffects.Add(SoundEffects.BearTrapImpact, assetManager.mSoundBearTrapImpact);

        // Store Music
        mMusic.Add(Songs.Roaming, assetManager.mMusicDungeonRoaming);
        mMusic.Add(Songs.BossFight, assetManager.mMusicBossFight);
        mMusic.Add(Songs.BarClick, assetManager.mSoundBarClick);

        // Create initial dummy instance to prevent exceptions
        sSCurrentMusicInstance = mMusic[Songs.Roaming].CreateInstance();
        mNextMusicInstance = mMusic[Songs.Roaming].CreateInstance();

        // Events
        eventDispatcher.OnAudioRequest += HandleAudioEvent;
        eventDispatcher.OnSoundPauseRequest += PauseAllSound;
        eventDispatcher.OnSoundResumeRequest += ResumeAllSound;
        eventDispatcher.OnSoundStopRequest += StopAllSound;
        eventDispatcher.OnSoundVolumeRequest += GetSoundVolume;
        eventDispatcher.OnMusicVolumeRequest += GetMusicVolume;
        eventDispatcher.OnCommunicationRequest += UpdateSummonerPosition;
    }

    /// <summary>
    ///  starts playing the sound at the current volume. Will not override currently playing sounds.
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="soundSource">position on the map. If null, play sound at current volume</param>
    private void PlaySound(SoundEffects sound, Vector2? soundSource)
    {
        var newVolume = 1f;
        if (soundSource.HasValue)
        {
            newVolume = CalculateVolume(soundSource.Value);
            if (newVolume < 0f)
            {
                return;
            }
        }
        mWillBePlayed.Add(new Tuple<SoundEffects, float>(sound, newVolume * mSoundVolume));
    }

    private void PlaySound(SoundEffects sound, float volume)
    {
        mWillBePlayed.Add(new Tuple<SoundEffects, float>(sound, volume));
    }
    private float CalculateVolume(Vector2 soundPosition)
    {
        var soundVector = new Vector2(Math.Max(Math.Abs(mSummonerPosition.X), Math.Abs(soundPosition.X)) - Math.Min(Math.Abs(mSummonerPosition.X), Math.Abs(soundPosition.X)), Math.Max(Math.Abs(mSummonerPosition.Y), Math.Abs(soundPosition.Y)) - Math.Min(Math.Abs(mSummonerPosition.Y), Math.Abs(soundPosition.Y)));
        if (soundVector.Length() != 0)
        {
            return (1f - soundVector.Length() / 1900) * mSoundVolume;
        }
        return 1f * mSoundVolume;
    }

    /// <summary>
    /// Takes the name of a music and starts playing it at the current volume. Stops the currently playing music.
    /// </summary>
    /// <param name="song">New music to be played.</param>
    /// <param name="looping">Determine if the music should be looped or not.</param>
    private void PlayMusic(Songs song, bool looping = true)
    {
        if (song == Songs.BarClick)
        {
            PlaySound(SoundEffects.BarClick, mMusicVolume);
            return;
        }
        sSCurrentMusicInstance.Stop();
        sSCurrentMusicInstance = mMusic[song].CreateInstance();
        sSCurrentMusicInstance.IsLooped = looping;
        sSCurrentMusicInstance.Volume = mMusicVolume;
        sSCurrentMusicInstance.Play();
    }

    private void PauseAllSound()
    {
        foreach (var sound in mActiveSounds.ToList())
        {
            sound.Pause();
            sound.mState = InstanceState.Paused;
        }
        if (!sSCurrentMusicInstance.IsDisposed)
        {
            sSCurrentMusicInstance.Pause();
        }
    }

    private void ResumeAllSound()
    {
        foreach (var sound in mActiveSounds.ToList())
        {
            sound.Resume();
            sound.mState = InstanceState.Pending;
        }
        if (!sSCurrentMusicInstance.IsDisposed)
        {
            sSCurrentMusicInstance.Resume();
        }
    }

    /// <summary>
    /// Kills all sound. Can not be resumed.
    /// </summary>
    private void StopAllSound()
    {
        foreach (var sound in mActiveSounds.ToList())
        {
            sound.Stop();
        }
        sSCurrentMusicInstance.Stop();
        sSCurrentMusicInstance.Dispose();
        mNextMusicInstance.Stop();
        mNextMusicInstance.Dispose();
        mActiveSounds.Clear();
        mFadeNextMusic = Songs.NoFade;
    }

    /// <summary>
    /// Changes the master volume of the class and all currently running sounds and music.
    /// </summary>
    /// <param name="newVolume"></param>
    /// <param name="mode"></param>
    private void ChangeVolume(float newVolume, VolumeMode mode)
    {
        if (mode == VolumeMode.ChangeSoundVolume)
        {
            foreach (var sound in mActiveSounds.ToList())
            {
                sound.ChangeVolume(newVolume);
            }
            mSoundVolume = newVolume;
        }
        else if (mode == VolumeMode.ChangeMusicVolume)
        {
            sSCurrentMusicInstance.Volume = newVolume;
            mMusicVolume = newVolume;

        }
        else
        {
            foreach (var sound in mActiveSounds.ToList())
            {
                sound.ChangeVolume(newVolume);
            }
            mSoundVolume = newVolume;
            mMusicVolume = newVolume;
            sSCurrentMusicInstance.Volume = newVolume;
        }
    }

    private float GetSoundVolume()
    {
        return mSoundVolume;
    }
    private float GetMusicVolume()
    {
        return mMusicVolume;
    }

    public void Update(float gameTime)
    {
        ChangeMusic(gameTime);
        // ToList should create a new List, so no interfering with removing at this point
        // The right instances are updating here, I checked it. 
        foreach (var sound in mActiveSounds.ToList())
        {
            sound.Update(gameTime);
            if (sound.mState == InstanceState.LimitReached)
            {
                mActiveSounds.Remove(sound);
            }
        }
        var canBePlayed = new List<SoundEffects>();
        foreach (var sound in mWillBePlayed)
        {
            if (canBePlayed.Contains(sound.Item1))
            {
                continue;
            }
            canBePlayed.Add(sound.Item1);
            var newSound = new RevancheSoundEffect(mSoundEffects[sound.Item1]);
            mActiveSounds.Add(newSound);
            newSound.Play(sound.Item2);
        }
        mWillBePlayed.Clear();
    }

    private void UpdateSummonerPosition(CommunicationEvent communicationEvent)
    {
        mSummonerPosition = communicationEvent.mSummonerPosition;
    }

    /// <summary>
    /// Called every frame. Implements the logic for the fading mechanic for the game music.
    /// Place SoundManager.ChangeMusic(gameTime) into the UpdateGameObjects function of the game for it to work.
    /// </summary>
    private void ChangeMusic(float gameTime)
    {
        if (mFadeNextMusic != Songs.NoFade)
        {
            mFadeTime += gameTime;

            // start to fade the first (current) music out, after ChangeMusic was called
            if (mFadeTime < mFadeSongStart)
            {
                // linear fading, in dependance to the frames of the game
                sSCurrentMusicInstance.Volume = (mFadeTimespan - mFadeTime) / mFadeTimespan * mMusicVolume;
            }
            // Reached the starting point for the next music (called one time only, hence the flag)
            else if (mFadeTime < mFadeTimespan && !mFadeFlag)
            {
                sSCurrentMusicInstance.Volume = (mFadeTimespan - mFadeTime) / mFadeTimespan * mMusicVolume;
                // start playing the next music, start with volume = 0f
                mNextMusicInstance = mMusic[mFadeNextMusic].CreateInstance();
                mNextMusicInstance.Volume = 0f;
                mNextMusicInstance.Play();
                mNextMusicInstance.IsLooped = true;
                mFadeFlag = true;
            }
            // after the starting point was reached, and until the end of the given fading time do:
            else if (mFadeTime < mFadeTimespan)
            {
                sSCurrentMusicInstance.Volume = (mFadeTimespan - mFadeTime) / mFadeTimespan * mMusicVolume;
                // Min(), to prevent the Volume getting higher than mMusicVolume
                mNextMusicInstance.Volume = Math.Min((mFadeTime - mFadeSongStart) / mFadeSongStart * mMusicVolume, mMusicVolume);
            }
            // The fading time is reached, the last actions for this function
            else
            {
                sSCurrentMusicInstance.Stop();
                // If there's an error in the future, it's here.
                sSCurrentMusicInstance.Dispose();
                // Keep sCurrentMusicInstance consistently as the instance of the currently playing SoundEffectInstance
                sSCurrentMusicInstance = mNextMusicInstance;
                sSCurrentMusicInstance.Volume = mMusicVolume;
                // Reset the variables for the fading. mFadeTimespan and mFadeSongStart are being auto-reset
                // on every new call of ChangeMusic(string)
                mFadeNextMusic = Songs.NoFade;
                mFadeFlag = false;
                mFadeTime = 0f;
            }
        }
    }

    /// <summary>
    /// Fades out of the current music and into the given new one. Can be called while a fade is already active: When called before the
    /// other music started to fade in, it will override that music and start playing. After this point, the new call will be ignored.
    /// </summary>
    /// <param name="newMusic">Sets the next music.</param>
    /// <param name="fadeTimespan">Sets the time when the fading will be finished.</param>
    /// <param name="fadeSongStart">Sets the time the next music will start to fade in.</param>
    /// <exception cref="ArgumentException">Starting point for the next music must be smaller than the overall given time in fadeTimespan.</exception>
    private void ChangeMusic(Songs newMusic, float fadeTimespan = 4f, float fadeSongStart = 2f)
    {
        if (fadeSongStart >= fadeTimespan)
        {
            throw new ArgumentException("The starting point for the new music must be smaller than the overall fading time.");
        }
        // Set the fade variables of this class, so ChangeMusic(gameTime) can access them
        mFadeTimespan = fadeTimespan;
        mFadeSongStart = fadeSongStart;
        mFadeNextMusic = newMusic;
    }

    // Event handling
    private void HandleAudioEvent(IAudioEvent audioEvent)
    {
        switch (audioEvent)
        {
            case SongEvent e:
                if (e.Change)
                {
                    ChangeMusic(e.Song, e.FadeTime, e.BeginNextSong);
                }
                else
                {
                    PlayMusic(e.Song, e.Loop);
                }
                break;
            case SoundEvent e:
                PlaySound(e.Sound, e.mSoundOrigin);
                break;
            case VolumeEvent e:
                ChangeVolume(e.Volume, e.mMode);
                break;
        }
    }
}
