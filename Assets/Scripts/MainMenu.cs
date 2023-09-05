using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    BlockingWait blockingWaitPopup;
    public Button GetExtraCoins_Button;
    public Button GetDailyBonus_Button;

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
    public void ShowAds()
    {
        blockingWaitPopup.gameObject.SetActive(true);
    }
}
