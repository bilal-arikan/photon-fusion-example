using System;
using Arikan;
using UnityEngine;

namespace Arikan
{
    /// <summary>
    /// Contains Events
    /// </summary>
    public static partial class E
    {
        public static Action AppStarting;
        public static Action AppStarted;
        public static Action ReceivedMessage;
        public static Action ReceivedToken;
        public static Action ConfigsFetched;
        public static Action<int> AvailableNewVersion;
        public static Action IAPInitialized;
        public static Action DatabaseReset;
        public static Action<string> LanguageChanged;
        public static Action<AudioClip> BackgroundMusicChanged;
        public static Action AdsRemoved;

        public static Action PlayerPhotoChanged;
        public static Action<int> PlayerLevelUp;

        public static Action<int> DailyGift;
        public static Action<int> RewardVideoGift;
        public static Action<int> InvitedSended;
        public static Action<string> InviteClicked;
        public static Action<string> ItemPurchasingStarted;
        public static Action<string> ItemPurchasingCanceled;
        public static Action<string> ItemPurchasingError;
        public static Action<string> ItemPurchased;
        public static Action<string> AchievementCompleted;

        public static Action<Vector2Int> SceneLoadingStarted;
        public static Action<float> SceneLoadingProgressed;
        public static Action<Vector2Int> SceneLoadingCompleted;
        public static Action<Vector2Int> LevelCompleted;
        public static Action<Vector2Int> LevelFailed;

        public static Action PlayerWon;
        public static Action PlayerLost;
        public static Action MainMenu;
        public static Action GameOnPreparing;
        public static Action GameStarted;
        public static Action GamePaused;
        public static Action GameUnPaused;
        public static Action GameFinished;
        public static Action PoolsFilled;
        public static Action PoolsEmpty;
    }

    //public static partial class E
    //{
    //    public static Action CubeChanged;
    //    public static Action<int> CubeStartFollowing;
    //    public static Action<int> CubeStopFollowing;
    //    public static Action<float> PassedTopAltitude;
    //    public static Action<float> DownedBottomAltitude;
    //}

    //public static partial class E
    //{
    //    public static Action<EmojiBlock> EmojiBlockDefeated;
    //    public static Action<EmojiBall> EmojiBallDestroyed;
    //    public static Action<EmojiBall> EmojiBallFired;
    //    public static Action<Lootable> LootableLooted;
    //    public static Action<Cannon> CannonTargeting;
    //}
}
