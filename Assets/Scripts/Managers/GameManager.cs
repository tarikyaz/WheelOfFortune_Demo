using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    protected override bool isDontDestroyOnload => true;

    public int NumOfCoins => coinsManager.NumOfCoins;

    [SerializeField] CoinsManager coinsManager;
    [SerializeField] MainMenu mainMenu;
    [SerializeField] Minigame minigame;

    private void OnEnable()
    {
        BaseEvents.OnGetExtraCoin += OnGetExtraCoinsHandler;
        BaseEvents.OnClaimFreeCoin += OnClaimFreeCoinHandler;
        BaseEvents.OnSpendOneCoin += OnSpendOneCoinHandler;
        BaseEvents.OnStartMinigame += OnStartMinigameHandler;

    }
    private void OnDisable()
    {
        BaseEvents.OnGetExtraCoin -= OnGetExtraCoinsHandler;
        BaseEvents.OnClaimFreeCoin -= OnClaimFreeCoinHandler;
        BaseEvents.OnSpendOneCoin -= OnSpendOneCoinHandler;
        BaseEvents.OnStartMinigame -= OnStartMinigameHandler;

    }

    private void OnStartMinigameHandler(bool on)
    {
        minigame.gameObject.SetActive(on);
        mainMenu.gameObject.SetActive(!on);
    }

    private void OnSpendOneCoinHandler()
    {
        BaseEvents.CallAddCoins(-1);
    }
    private void Start()
    {
        mainMenu.gameObject.SetActive(true);
        minigame.gameObject.SetActive(false);
    }
    private void OnClaimFreeCoinHandler()
    {
        if (coinsManager.TryGetDailyBonus())
        {
            BaseEvents.CallAddCoins(1);
        }
    }


    private void FixedUpdate()
    {
        // checking if still can get coins or showing remaining time
        mainMenu.GetExtraCoins_Button.interactable = coinsManager.TryGetAds(true, out var timeRemaingForExtraCoins);
        mainMenu.GetDailyBonus_Button.interactable = coinsManager.TryGetDailyBonus(true, out var timeRemaingForDailyBonus);

        if (mainMenu.GetExtraCoins_Button.interactable)
        {
            mainMenu.GetExtraCoins_Text.text = "Get extra coin";
        }
        else
        {
            mainMenu.GetExtraCoins_Text.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeRemaingForExtraCoins.Hours, timeRemaingForExtraCoins.Minutes, timeRemaingForExtraCoins.Seconds);
        }

        if (mainMenu.GetDailyBonus_Button.interactable)
        {
            mainMenu.GetDailyBonus_Text.text = "Claim free coin";
        }
        else
        {
            mainMenu.GetDailyBonus_Text.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeRemaingForDailyBonus.Hours, timeRemaingForDailyBonus.Minutes, timeRemaingForDailyBonus.Seconds);
        }
    }
    private void OnGetExtraCoinsHandler()
    {
        if (coinsManager.TryGetAds())
        {
            mainMenu.ShowAds();
        }
        mainMenu.GetExtraCoins_Button.interactable = coinsManager.TryGetAds(true);
    }
    public void OpenLinkedin()
    {
        Application.OpenURL("https://www.linkedin.com/in/trk90/");
    }
}
