using System;
using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    public int MaxCoinsAmount = 10;
    public int NumOfCoins
    {
        get => PlayerPrefs.GetInt(NUMBER_OF_COINS_KEY_Str, startingCoinsAmount);
        private set
        {
            PlayerPrefs.SetInt(NUMBER_OF_COINS_KEY_Str, value);
            BaseEvents.CallCoinsAmountUpdate(value);
        }
    }

    [Serializable]
    public struct DatesDataStruc
    {
        public int NumOfClicks;
        public double FirstClickedAdsTime_Timestamp;
        public double DailyBonusClaimDate_Timestamp;
        public void AddAdsClick()
        {
            Debug.Log("Add Ads Click");
            NumOfClicks++;
            PlayerPrefs.SetString(DATES_DATA_KEY_Str, JsonUtility.ToJson(this));
        }
        public void RestAdsClicks()
        {
            Debug.Log("Rest Ads Clicks");
            FirstClickedAdsTime_Timestamp = TimerUtility.ConvertDateTimeToTimestamp(TimerUtility.CurrentTime);
            NumOfClicks = 0;
            PlayerPrefs.SetString(DATES_DATA_KEY_Str, JsonUtility.ToJson(this));
        }
        public void RestDailyBonus()
        {
            Debug.Log("Rest Daily Bonus");

            DailyBonusClaimDate_Timestamp = TimerUtility.ConvertDateTimeToTimestamp(TimerUtility.GetRestTime(TimerUtility.CurrentTime));
            PlayerPrefs.SetString(DATES_DATA_KEY_Str, JsonUtility.ToJson(this));

        }

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


    [SerializeField] int maxNumOfAdsPerDay = 5;
    [SerializeField] int startingCoinsAmount = 5;

    DatesDataStruc adsData => JsonUtility.FromJson<DatesDataStruc>(PlayerPrefs.GetString(DATES_DATA_KEY_Str, DatesDataStruc.DefaultValueStr));

    const string DATES_DATA_KEY_Str = "DATESDATA";
    const string NUMBER_OF_COINS_KEY_Str = "NUMBEROFCOINS";

    public bool TryGetAds(bool onlyCheck, out TimeSpan timeRemaining)
    {
        DateTime currentTime = TimerUtility.CurrentTime;
        DateTime restTime = TimerUtility.GetRestTime(TimerUtility.ConvertTimestampToDateTime(adsData.FirstClickedAdsTime_Timestamp));

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
                timeRemaining = restTime - currentTime;
                return false;
            }

        }
    }
    public bool TryGetAds(bool onlyCheck = false)
    {
        return TryGetAds(onlyCheck, out var timeRemaining);
    }


    public bool TryGetDailyBonus(bool onlyCheck, out TimeSpan timeRemaining)
    {
        DateTime currentTime = TimerUtility.CurrentTime;
        DateTime claimTime = TimerUtility.ConvertTimestampToDateTime(adsData.DailyBonusClaimDate_Timestamp);
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
            timeRemaining = claimTime - currentTime;
            return false;
        }
    }
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
        BaseEvents.CallCoinsAmountUpdate(NumOfCoins);
    }
    private void OnAddCoinsHandler(int toAdd)
    {
        NumOfCoins = Mathf.Clamp(NumOfCoins + toAdd, 0, MaxCoinsAmount);
    }
}
