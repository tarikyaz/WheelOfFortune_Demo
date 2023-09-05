using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [Serializable]
    public struct AdsDataStruc {
        public int NumOfClicks;
        public long dayFirstAdsClickTime_Binary;

        public void AddClick()
        {
            if (dayFirstAdsClickTime_Binary <=0)
            {
                dayFirstAdsClickTime_Binary = TimerUtility.CurrentTime.ToBinary();
            }
            Debug.Log("add a click");
            NumOfClicks++;
            PlayerPrefs.SetString(CLICKED_ADS_TIMES_KEY, JsonUtility.ToJson(this));
        }
        public void RestClicks()
        {
            Debug.Log("rest clicks");
            dayFirstAdsClickTime_Binary = TimerUtility.CurrentTime.ToBinary();
            NumOfClicks =0;
            PlayerPrefs.SetString(CLICKED_ADS_TIMES_KEY, JsonUtility.ToJson(this));
        }
    }
    static string CLICKED_ADS_TIMES_KEY => "ClikedAdsTimes";
    static  string NUM_OF_COINS_KEY => "NumOfCOins";
    public static int NumOfCoins
    {
        get => PlayerPrefs.GetInt(NUM_OF_COINS_KEY, 0);
        private set
        {
            PlayerPrefs.SetInt(NUM_OF_COINS_KEY, value);
            BaseEvents.CallCoinsAmountUpdate(value);
        }
    }

    [SerializeField] AdsDataStruc adsData => JsonUtility.FromJson<AdsDataStruc>(PlayerPrefs.GetString(CLICKED_ADS_TIMES_KEY));
    [SerializeField] int maxNumOfAdsPerDay = 5;



    public bool TryGetAds(bool onlyCheck = false)
    {
        Debug.Log("TryGetAds " + PlayerPrefs.GetString(CLICKED_ADS_TIMES_KEY));
        DateTime currentTime = TimerUtility.CurrentTime;
        Debug.Log("currentTime " + currentTime);
        DateTime restTime = TimerUtility.GetRestTime(DateTime.FromBinary(adsData.dayFirstAdsClickTime_Binary));
        Debug.Log("restTime " + restTime);

        if (restTime <= currentTime)
        {
            if (!onlyCheck)
            {
                adsData.RestClicks();
            }
            return true;
        }
        else
        {
            if (adsData.NumOfClicks < maxNumOfAdsPerDay)
            {
                if (!onlyCheck)
                {
                    adsData.AddClick();
                }
                return true;
            }
            else
            {
                return false;
            }

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
