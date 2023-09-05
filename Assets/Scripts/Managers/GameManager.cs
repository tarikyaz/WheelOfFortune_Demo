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
    }
    private void OnDisable()
    {
        BaseEvents.OnGetExtraCoin -= OnGetExtraCoinsHandler;
    }
    private void Start()
    {
        MainMenu.GetExtraCoins_Button.interactable = DataManager.TryGetAds(true);

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
