using Revanche.Core;

namespace Revanche.Sound;

internal sealed class SongEvent : IAudioEvent
{
    internal Songs Song { get; }
    internal bool Change { get; }
    internal bool Loop { get; }

    internal float FadeTime { get; }
    internal float BeginNextSong { get; }


    /// <summary>
    /// Event to play a new song or change
    /// the song that is currently running
    /// </summary>
    /// <param name="song">The song that should be played</param>
    /// <param name="loop">Determines if the song shall loop indefinitely</param>
    /// <param name="change">If true the current song will be swapped out</param>
    /// <param name="fadeTime">Determines how long the overall duration of the Fade (in seconds)</param>
    /// <param name="beginNextSong">Determines when the next music will start to fade in</param>
    public SongEvent(Songs song, bool loop = true, bool change=false, float fadeTime = 4f, float beginNextSong = 2f)
    {
        Song = song;
        Change = change;
        Loop = loop;
        FadeTime = fadeTime;
        BeginNextSong = beginNextSong;
    }
}