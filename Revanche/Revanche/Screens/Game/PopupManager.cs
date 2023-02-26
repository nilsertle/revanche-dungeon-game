using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revanche.AchievementSystem;
using Revanche.Managers;

namespace Revanche.Screens.Game;

public sealed class PopupManager
{
    private const float F2 = 2f;

    private readonly Queue<Popup> mPopups = new();
    private readonly AssetManager mAssetManager;
    private bool mPopUpLock; //  = false; // by default
    public PopupManager(EventDispatcher eventDispatcher, AssetManager assetManager)
    {
        eventDispatcher.OnPopupEvent += HandlePopupEvent;
        mAssetManager = assetManager;
    }

    private void HandlePopupEvent(IPopupEvent popupEvent)
    {
        switch (popupEvent)
        {
            case IPopupEvent.AchievementPopup achievementPopup:
                HandleAchievementPopup(achievementPopup.mAchievement);
                break;
            case IPopupEvent.NotificationPopup notificationPopup:
                HandleNotificationPopup(notificationPopup.mNotification, notificationPopup.mColor, notificationPopup.mDuration);
                break;
            case IPopupEvent.SavePopUp:
                HandleSavePopup();
                break;
        }
    }

    private void HandleAchievementPopup(Achievement achievement)
    {
        var popup = new Popup("Errungenschaft freigeschaltet: " + achievement.Name + "\n \"" + achievement.Description + "\"",Color.White, Game1.mCenter, 5f);
        mPopups.Enqueue(popup);
    }

    private void HandleNotificationPopup(string notification, Color notificationColor, float duration)
    {
        var popup = new Popup(notification, notificationColor, Game1.mCenter, duration); 
        mPopups.Enqueue(popup);
    }

    private void HandleSavePopup()
    {
        if (mPopUpLock)
        {
            return;
        }

        var text = "Spiel gespeichert!";
        var popup = new Popup(text, Color.White, new Vector2(Game1.mScreenWidth/2f, 3*mAssetManager.mFont.MeasureString(text).Y), 3f, true);
        mPopups.Enqueue(popup);
        mPopUpLock = true;
    }

    public void Update(float deltaTime)
    {
        if (mPopups.Count == 0)
        {
            return;
        }
        var popup = mPopups.Peek();
        popup.Timer += deltaTime;
        if (popup.Timer >= popup.Duration)
        {
            var deq = mPopups.Dequeue();
            if (deq.Unlock)
            {
                mPopUpLock = false;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (mPopups.Count == 0)
        {
            return;
        }
        
        spriteBatch.Begin();
        spriteBatch.DrawString(mAssetManager.mFont,
            mPopups.Peek().Message,
            mPopups.Peek().Position,
            mPopups.Peek().TextColor,
            0f,
            mAssetManager.mFont.MeasureString(mPopups.Peek().Message) / F2,
            Vector2.One,
            SpriteEffects.None,
            0f);
        spriteBatch.End();
    }

}