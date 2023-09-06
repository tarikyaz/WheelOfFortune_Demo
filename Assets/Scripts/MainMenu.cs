using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    BlockingWait blockingWaitPopup;
    public Button GetExtraCoins_Button;
    public Button GetDailyBonus_Button;
    public Button PlayMiniGame_Button;

    public TMP_Text GetExtraCoins_Text;
    public TMP_Text GetDailyBonus_Text;

    // Play Wheel of Fortune
    public void Action_SpendOneCoin()
    {
        Debug.Log("MainMenu:Action_SpendOneCoin");
        BaseEvents.CallOnSpendOneCoin();
    }

    // Ads Coin
    public void Action_GetExtraCoin()
    {
        Debug.Log("MainMenu:Action_GetExtraCoin");
        BaseEvents.CallOnGetExtraCoin();

    }
    // Daily Bonus
    public void Action_ClaimFreeCoin()
    {
        Debug.Log("MainMenu:Action_ClaimFreeCoin");
        BaseEvents.CallOnClaimFreeCoin();

    }

    // minigame
    public void Action_StartMinigame(bool on)
    {
        Debug.Log($"MainMenu:Action_StartMinigame {(on ? "on" : "off")}");
        BaseEvents.CallStartMinigame(on);
    }
    private void OnEnable()
    {
        BaseEvents.OnCoinsAmountUpdate += OnCoinsAmountUpdateHandler;
    }
    private void OnDisable()
    {
        BaseEvents.OnCoinsAmountUpdate -= OnCoinsAmountUpdateHandler;
    }

    private void OnCoinsAmountUpdateHandler(int newAmouont)
    {
        PlayMiniGame_Button.interactable = newAmouont > 0;
    }

    public void ShowAds()
    {
        blockingWaitPopup.gameObject.SetActive(true);
    }
}
