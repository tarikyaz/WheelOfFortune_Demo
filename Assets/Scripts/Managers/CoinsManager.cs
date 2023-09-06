using System;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    // The maximum number of coins a player can have
    public int MaxCoinsAmount = 10;

    // The current number of coins the player has
    public int NumOfCoins
    {
        get => PlayerPrefs.GetInt(NUMBER_OF_COINS_KEY_Str, startingCoinsAmount);
        private set
        {
            PlayerPrefs.SetInt(NUMBER_OF_COINS_KEY_Str, value);
            BaseEvents.CallCoinsAmountUpdate(value);
        }
    }

    // Serializable struct to store dates-related data
    [Serializable]
    public struct DatesDataStruc
    {
        public int NumOfClicks;
        public double FirstClickedAdsTime_Timestamp;
        public double DailyBonusClaimDate_Timestamp;

        // Add an ad click and save the data
        public void AddAdsClick()
        {
            Debug.Log("Add Ads Click");
            NumOfClicks++;
            PlayerPrefs.SetString(DATES_DATA_KEY_Str, JsonUtility.ToJson(this));
        }

        // Reset ad clicks and save the data
        public void RestAdsClicks()
        {
            Debug.Log("Rest Ads Clicks");
            FirstClickedAdsTime_Timestamp = TimerUtility.ConvertDateTimeToTimestamp(TimerUtility.CurrentTime);
            NumOfClicks = 0;
            PlayerPrefs.SetString(DATES_DATA_KEY_Str, JsonUtility.ToJson(this));
        }

        // Reset daily bonus data and save the data
        public void RestDailyBonus()
        {
            Debug.Log("Rest Daily Bonus");
            DailyBonusClaimDate_Timestamp = TimerUtility.ConvertDateTimeToTimestamp(TimerUtility.GetRestTime(TimerUtility.CurrentTime));
            PlayerPrefs.SetString(DATES_DATA_KEY_Str, JsonUtility.ToJson(this));
        }

        // Default value for the struct
        public static string DefaultValueStr
        {
            get
            {
                double currentTime = TimerUtility.ConvertDateTimeToTimestamp(TimerUtility.CurrentTime);
                var data = new DatesDataStruc
                {
                    DailyBonusClaimDate_Timestamp = 0,
                    FirstClickedAdsTime_Timestamp = currentTime,
                    NumOfClicks = 0
                };
                return JsonUtility.ToJson(data);
            }
        }
    }

    // The maximum number of ads a player can view per day
    [SerializeField] int maxNumOfAdsPerDay = 5;

    // The starting number of coins
    [SerializeField] int startingCoinsAmount = 5;

    // Retrieve ads-related data from PlayerPrefs
    DatesDataStruc adsData => JsonUtility.FromJson<DatesDataStruc>(PlayerPrefs.GetString(DATES_DATA_KEY_Str, DatesDataStruc.DefaultValueStr));

    const string DATES_DATA_KEY_Str = "DATESDATA";
    const string NUMBER_OF_COINS_KEY_Str = "NUMBEROFCOINS";

    /// <summary>
    /// Check if the player can get ads.
    /// </summary>
    /// <param name="onlyCheck">Check without changing values.</param>
    /// <param name="timeRemaining">Time left for the next ads.</param>
    /// <returns>Returns true if the player can get ads.</returns>
    public bool TryGetAds(bool onlyCheck, out TimeSpan timeRemaining)
    {
        DateTime currentTime = TimerUtility.CurrentTime;
        DateTime restTime = TimerUtility.GetRestTime(TimerUtility.ConvertTimestampToDateTime(adsData.FirstClickedAdsTime_Timestamp));

        // If the player passed the rest time of the first click (13:00 UTC), reset the ads data
        if (restTime <= currentTime)
        {
            if (!onlyCheck)
            {
                adsData.RestAdsClicks();
            }
            timeRemaining = new TimeSpan(0);
            return true;
        }
        else
        {
            // If the player still has fewer ad clicks than the limit for a day
            if (adsData.NumOfClicks < maxNumOfAdsPerDay)
            {
                if (!onlyCheck)
                {
                    adsData.AddAdsClick();
                }
                timeRemaining = new TimeSpan(0);
                return true;
            }
            else
            {
                // Otherwise, the player cannot view more ads until the next day
                timeRemaining = restTime - currentTime;
                return false;
            }
        }
    }

    /// <summary>
    /// Check if the player can get ads without changing values.
    /// </summary>
    /// <returns>Returns true if the player can get ads.</returns>
    public bool TryGetAds(bool onlyCheck = false)
    {
        return TryGetAds(onlyCheck, out var timeRemaining);
    }

    /// <summary>
    /// Check if the player can get a daily bonus.
    /// </summary>
    /// <param name="onlyCheck">Check without changing values.</param>
    /// <param name="timeRemaining">Time remaining for the next claim.</param>
    /// <returns>Returns true if the player can get a daily bonus.</returns>
    public bool TryGetDailyBonus(bool onlyCheck, out TimeSpan timeRemaining)
    {
        DateTime currentTime = TimerUtility.CurrentTime;
        DateTime claimTime = TimerUtility.ConvertTimestampToDateTime(adsData.DailyBonusClaimDate_Timestamp);

        // If the player passed the daily bonus time, reset the daily bonus data
        if (claimTime <= currentTime)
        {
            if (!onlyCheck)
            {
                adsData.RestDailyBonus();
            }
            timeRemaining = new TimeSpan(0);
            return true;
        }
        else
        {
            // Otherwise, calculate the time remaining for the next daily bonus claim
            timeRemaining = claimTime - currentTime;
            return false;
        }
    }

    /// <summary>
    /// Check if the player can get a daily bonus without changing values.
    /// </summary>
    /// <returns>Returns true if the player can get a daily bonus.</returns>
    public bool TryGetDailyBonus(bool onlyCheck = false)
    {
        return TryGetDailyBonus(onlyCheck, out var timeRemaining);
    }

    private void OnEnable()
    {
        BaseEvents.OnAddCoins += OnAddCoinsHandler;
    }

    private void OnDisable()
    {
        BaseEvents.OnAddCoins -= OnAddCoinsHandler;
    }

    private void Start()
    {
        // Update the UI with the current number of coins
        BaseEvents.CallCoinsAmountUpdate(NumOfCoins);
    }

    private void OnAddCoinsHandler(int toAdd)
    {
        // Add coins to the player's balance and ensure it stays within the maximum limit
        NumOfCoins = Mathf.Clamp(NumOfCoins + toAdd, 0, MaxCoinsAmount);
    }
}
