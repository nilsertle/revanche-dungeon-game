using Revanche.Core;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Revanche.Sound;

internal sealed class SoundEvent : IAudioEvent
{
    internal SoundEffects Sound { get; set; }
    internal Vector2? mSoundOrigin;

/// <summary>
/// 
/// </summary>
/// <param name="sound"></param>
/// <param name="soundOrigin">Position of the sound. Use null to play the sound at current volume</param>
    public SoundEvent(SoundEffects sound, Vector2? soundOrigin)
    {
        Sound = sound;
        mSoundOrigin = soundOrigin;
    }
}