using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Revanche.AchievementSystem;
using Revanche.Core;
using Revanche.GameObjects;
using Revanche.Screens.Game;
using Revanche.Screens.Menu;
using Revanche.Sound;
using Revanche.Stats;

namespace Revanche.Managers;

public sealed class EventDispatcher
{
    // Screen related events --------------------------------------------------------
    // Event to handle our screen requests made within the Screen creation
    // by calling the SendScreenRequest function
    public event Action<INavigationEvent> OnScreenRequest;
    public event Action<ResolutionEvent> OnResolutionRequest;
    public event Action OnFullScreenRequest;

    internal void SendScreenRequest(INavigationEvent screen)
    {
        OnScreenRequest?.Invoke(screen);
    }

    internal void SendResolutionRequest(ResolutionEvent resolutionEvent) 
    {
        OnResolutionRequest?.Invoke(resolutionEvent);
    }

    internal void SendFullScreenRequest()
    {
        OnFullScreenRequest?.Invoke();
    }

    // Game related events -----------------------------------------------------------
    public event Action OnExit;

    internal void CloseGame()
    {
        OnExit?.Invoke();
    }

    // Audio related events ----------------------------------------------------------
    public delegate float VolumeRequest();
    public event Action<IAudioEvent> OnAudioRequest;
    public event Action OnSoundPauseRequest;
    public event Action OnSoundResumeRequest;
    public event Action OnSoundStopRequest;
    public event VolumeRequest OnSoundVolumeRequest;
    public event VolumeRequest OnMusicVolumeRequest;
    public event Action<CommunicationEvent> OnCommunicationRequest;

    public void SendPositionToSoundManger(CommunicationEvent communicationEvent)
    {
        OnCommunicationRequest?.Invoke(communicationEvent);
    }
    public void SendAudioRequest(IAudioEvent audioEvent)
    {
        OnAudioRequest?.Invoke(audioEvent);
    }

    public void StopSound()
    {
        OnSoundStopRequest?.Invoke();
    }

    public void PauseSound()
    {
        OnSoundPauseRequest?.Invoke();
    }

    public void ResumeSound()
    {
        OnSoundResumeRequest?.Invoke();
    }

    public float SoundVolumeRequest => OnSoundVolumeRequest?.Invoke() ?? 1f;

    public float MusicVolumeRequest => OnMusicVolumeRequest?.Invoke() ?? 1f;

    // Saving
    public event Action OnSaveRequest;
    public void SendSaveRequest()
    {
        OnSaveRequest?.Invoke();
    }

    // Achievement related events ----------------------------------------------------------
    public event Action<AchievementType, int> OnAchievementEvent;

    public void SendAchievementEvent(AchievementType type, int increment=1)
    {
        OnAchievementEvent?.Invoke(type, increment);
    }

    // Statistics related events ----------------------------------------------------------
    public event Action<StatisticType, int> OnStatisticEvent;

    public void SendStatisticEvent(StatisticType type, int increment=1)
    {
        OnStatisticEvent?.Invoke(type, increment);
    }

    // Popup related events ----------------------------------------------------------
    public event Action<IPopupEvent> OnPopupEvent;

    public void SendPopupEvent(IPopupEvent popupEvent)
    {
        OnPopupEvent?.Invoke(popupEvent);
    }
}

public interface INavigationEvent
{
    public class NewGame : INavigationEvent
    {
        public readonly LevelState mLevelState;
        public NewGame(LevelState levelState)
        {
            mLevelState = levelState;
        }
    }

    public class MainMenu : INavigationEvent
    {
    }

    public class OptionMenu : INavigationEvent
    {
        internal MenuScreen mParent;
        internal OptionMenu(MenuScreen menu)
        {
            mParent = menu;
        }
    }

    public class CreditsScreen : INavigationEvent
    {
    }

    public class LoadMenu : INavigationEvent
    {
    }

    public class PauseMenu : INavigationEvent
    {
        internal bool CanSave { get; }

        internal PauseMenu(bool canSave)
        {
            CanSave = canSave;
        }
    }

    public class PopScreen : INavigationEvent
    {
    }

    public class GameOverScreen : INavigationEvent
    {
    }

    public class ControlsScreen : INavigationEvent
    {
    }

    public class ControlsImageScreen : INavigationEvent
    {
        public int BackgroundIndex { get; }

        public ControlsImageScreen(int backgroundIndex)
        {
            BackgroundIndex = backgroundIndex;
        }
    }

    public class GameWonScreen : INavigationEvent
    {

    }
    public class AchievementMenu : INavigationEvent
    {
    }

    public class StatisticsMenu : INavigationEvent
    {
    }

    public class TechDemoMenu : INavigationEvent
    {
    }

    public class PopAll : INavigationEvent 
    {
    }
    
    public class TalentMenu : INavigationEvent
    {
        public Dictionary<ElementType, int> CurrentSkills { get; }
        public readonly int mSkillPoints;
        public readonly Action<TalentTreeResult> mOnSkillPointsUpdate;

        public TalentMenu(Dictionary<ElementType, int> currentSkills, int skillPoints, Action<TalentTreeResult> onSkillPointsUpdate)
        {
            CurrentSkills = currentSkills;
            mSkillPoints = skillPoints;
            mOnSkillPointsUpdate = onSkillPointsUpdate;
        }
    }
}

public interface IPopupEvent
{
    public class AchievementPopup : IPopupEvent
    {
        public readonly Achievement mAchievement;
        public AchievementPopup(Achievement achievement)
        {
            mAchievement = achievement;
        }
    }

    public class NotificationPopup : IPopupEvent
    {
        public readonly string mNotification;
        public readonly Color mColor;
        public readonly float mDuration;

        public NotificationPopup(string notification, Color color, float duration=5f)
        {
            mNotification = notification;
            mColor = color;
            mDuration = duration;
        }
    }

    public class SavePopUp : IPopupEvent
    {
    }
}

public class ResolutionEvent
{
    public int Width { get; }
    public int Height { get; }
    
    public ResolutionEvent(int width, int height) 
    {
        Width = width;
        Height = height;
    }
}