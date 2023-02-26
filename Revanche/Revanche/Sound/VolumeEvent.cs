using Revanche.Core;

namespace Revanche.Sound;

internal sealed class VolumeEvent : IAudioEvent
{
    internal float Volume { get;}
    internal readonly VolumeMode mMode;

    /// <summary>
    /// Event to change the current volume level
    /// </summary>
    /// <param name="volume">The volume as value between 0f and 1f</param>
    /// <param name="mode">Mode from VolumeMode</param>
    internal VolumeEvent(float volume, VolumeMode mode)
    {
        Volume = volume is >= 0f and <= 1f ? volume : 0f;
        mMode = mode;
    }
}