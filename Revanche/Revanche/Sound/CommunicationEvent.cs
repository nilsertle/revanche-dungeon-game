using Microsoft.Xna.Framework;

namespace Revanche.Sound;

public sealed class CommunicationEvent : IAudioEvent
{
    internal Vector2 mSummonerPosition;

    /// <summary>
    /// Transfers the position of the summoner to the SoundManager
    /// </summary>
    /// <param name="summonerPos"></param>
    public CommunicationEvent(Vector2 summonerPos)
    {
        mSummonerPosition = summonerPos;
    }
}