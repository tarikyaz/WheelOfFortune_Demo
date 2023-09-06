using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    protected override bool isDontDestroyOnload => true;

    public CoinsManager CoinsManager;
    public MainMenu MainMenu;
    public Minigame Minigame;

    private void OnEnable()
    {
        BaseEvents.OnGetExtraCoin += OnGetExtraCoinsHandler;
        BaseEvents.OnClaimFreeCoin += OnClaimFreeCoinHandler;
        BaseEvents.OnSpendOneCoin += OnSpendOneCoinHandler;

    }
    private void OnDisable()
    {
        BaseEvents.OnGetExtraCoin -= OnGetExtraCoinsHandler;
        BaseEvents.OnClaimFreeCoin -= OnClaimFreeCoinHandler;
        BaseEvents.OnSpendOneCoin -= OnSpendOneCoinHandler;
    }

    private void OnSpendOneCoinHandler()
    {
        BaseEvents.CallAddCoins(-1);
    }
    private void Start()
    {
        MainMenu.gameObject.SetActive(true);
        Minigame.gameObject.SetActive(false);
    }
    private void OnClaimFreeCoinHandler()
    {
        if (CoinsManager.TryGetDailyBonus())
        {
            BaseEvents.CallAddCoins(1);
        }
    }


    private void FixedUpdate()
    {
        if (CoinsManager.MaxCoinsAmount == CoinsManager.NumOfCoins)
        {
            MainMenu.GetExtraCoins_Button.interactable = false;
            MainMenu.GetDailyBonus_Button.interactable = false;
            MainMenu.GetExtraCoins_Text.text = "Get extra coin\n(Wallet is full !)";
            MainMenu.GetDailyBonus_Text.text = "Claim free coin\n(Wallet is full !)";
        }
        else
        {
            MainMenu.GetExtraCoins_Button.interactable = CoinsManager.TryGetAds(true, out var timeRemaingForExtraCoins);
            MainMenu.GetDailyBonus_Button.interactable = CoinsManager.TryGetDailyBonus(true, out var timeRemaingForDailyBonus);

            if (MainMenu.GetExtraCoins_Button.interactable)
            {
                MainMenu.GetExtraCoins_Text.text = "Get extra coin";
            }
            else
            {
                MainMenu.GetExtraCoins_Text.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeRemaingForExtraCoins.Hours, timeRemaingForExtraCoins.Minutes, timeRemaingForExtraCoins.Seconds);
            }

            if (MainMenu.GetDailyBonus_Button.interactable)
            {
                MainMenu.GetDailyBonus_Text.text = "Claim free coin";
            }
            else
            {
                MainMenu.GetDailyBonus_Text.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeRemaingForDailyBonus.Hours, timeRemaingForDailyBonus.Minutes, timeRemaingForDailyBonus.Seconds);
            }
        }
    }
    private void OnGetExtraCoinsHandler()
    {
        if (CoinsManager.TryGetAds())
        {
            MainMenu.ShowAds();
        }
        MainMenu.GetExtraCoins_Button.interactable = CoinsManager.TryGetAds(true);
    }
    public void OpenLinkedin()
    {
        Application.OpenURL("https://www.linkedin.com/in/trk90/");
    }
}
