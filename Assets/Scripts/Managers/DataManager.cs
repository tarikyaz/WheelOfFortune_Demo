using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [Serializable]
    public struct DatesDataStruc
    {
        public int NumOfClicks;
        public long FirstClickedAdsTime_Binary;
        public long DailyBonusClaimDate_Binary;
        public void AddAdsClick()
        {
            Debug.Log("Add Ads Click");
            NumOfClicks++;
            PlayerPrefs.SetString(DATES_DATA_KEY_Str, JsonUtility.ToJson(this));
        }
        public void RestAdsClicks()
        {
            Debug.Log("Rest Ads Clicks");
            FirstClickedAdsTime_Binary = TimerUtility.CurrentTime.ToBinary();
            NumOfClicks = 0;
            PlayerPrefs.SetString(DATES_DATA_KEY_Str, JsonUtility.ToJson(this));
        }
        public void RestDailyBonus()
        {
            Debug.Log("Rest Daily Bonus");

            DailyBonusClaimDate_Binary = TimerUtility.GetRestTime(TimerUtility.CurrentTime).ToBinary();
            PlayerPrefs.SetString(DATES_DATA_KEY_Str, JsonUtility.ToJson(this));

        }

        public static string DefaultValueStr
        {
            get
            {
                long currentTime = TimerUtility.CurrentTime.ToBinary();
                var data = new DatesDataStruc
                {
                    DailyBonusClaimDate_Binary = 0,
                    FirstClickedAdsTime_Binary = currentTime,
                    NumOfClicks = 0
                };
                return JsonUtility.ToJson(data);
            }
        }
    }
    static string DATES_DATA_KEY_Str => "DATESDATA";
    static  string NUMBER_OF_COINS_KEY_Str => "NUMBEROFCOINS";
    public static int NumOfCoins
    {
        get => PlayerPrefs.GetInt(NUMBER_OF_COINS_KEY_Str, 0);
        private set
        {
            PlayerPrefs.SetInt(NUMBER_OF_COINS_KEY_Str, value);
            BaseEvents.CallCoinsAmountUpdate(value);
        }
    }


    DatesDataStruc adsData => JsonUtility.FromJson<DatesDataStruc>(PlayerPrefs.GetString(DATES_DATA_KEY_Str, DatesDataStruc.DefaultValueStr));
    [SerializeField] int maxNumOfAdsPerDay = 5;



    public bool TryGetAds(bool onlyCheck = false)
    {
        Debug.Log("TryGetAds " + PlayerPrefs.GetString(DATES_DATA_KEY_Str));
        DateTime currentTime = TimerUtility.CurrentTime;
        Debug.Log("currentTime " + currentTime);
        DateTime restTime = TimerUtility.GetRestTime(DateTime.FromBinary(adsData.FirstClickedAdsTime_Binary));
        Debug.Log("restTime " + restTime);

        if (restTime <= currentTime)
        {
            if (!onlyCheck)
            {
                adsData.RestAdsClicks();
            }
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
                return true;
            }
            else
            {
                return false;
            }

        }
    }

    public bool TryGetDailyBonus(bool onlyCheck = false)
    {
        Debug.Log("TryGetDailyBonus " + PlayerPrefs.GetString(DATES_DATA_KEY_Str));
        DateTime currentTime = TimerUtility.CurrentTime;
        Debug.Log("currentTime " + currentTime);
        DateTime claimTime = DateTime.FromBinary(adsData.DailyBonusClaimDate_Binary);
        Debug.Log("restTime " + claimTime);

        if (claimTime <= currentTime)
        {
            if (!onlyCheck)
            {
                adsData.RestDailyBonus();
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    private void OnEnable()
    {
        BaseEvents.OnAdsRewardCoin += OnAdsRewardCoinsHandler;
    }
    private void OnDisable()
    {
        BaseEvents.OnAdsRewardCoin -= OnAdsRewardCoinsHandler;
    }

    private void OnAdsRewardCoinsHandler(int toAdd)
    {
        NumOfCoins += toAdd;
    }
}