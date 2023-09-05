using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    protected override bool isDontDestroyOnload => true;

    public DataManager DataManager;
    public MainMenu MainMenu;
    private void OnEnable()
    {
        BaseEvents.OnGetExtraCoin += OnGetExtraCoinsHandler;
        BaseEvents.OnClaimFreeCoin += OnClaimFreeCoinHandler;
    }
    private void OnDisable()
    {
        BaseEvents.OnGetExtraCoin -= OnGetExtraCoinsHandler;
        BaseEvents.OnClaimFreeCoin -= OnClaimFreeCoinHandler;

    }

    private void OnClaimFreeCoinHandler()
    {
        if (DataManager.TryGetDailyBonus())
        {
            BaseEvents.CallRewardCoin(1);
        }
        MainMenu.GetDailyBonus_Button.interactable = DataManager.TryGetDailyBonus(true);
    }

    private void Start()
    {
        MainMenu.GetExtraCoins_Button.interactable = DataManager.TryGetAds(true);
        MainMenu.GetDailyBonus_Button.interactable = DataManager.TryGetDailyBonus(true);
    }
    private void OnGetExtraCoinsHandler()
    {
        if (DataManager.TryGetAds())
        {
            MainMenu.ShowAds();
        }
        MainMenu.GetExtraCoins_Button.interactable = DataManager.TryGetAds(true);
    }
}
