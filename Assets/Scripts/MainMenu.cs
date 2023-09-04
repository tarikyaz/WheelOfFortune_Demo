using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    BlockingWait blockingWaitPopup;

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
        blockingWaitPopup.gameObject.SetActive(true);
        BaseEvents.CallOnGetExtraCoin();

    }
    // Daily Bonus
    public void Action_ClaimFreeCoin()
    {
        Debug.Log("MainMenu:Action_ClaimFreeCoin");
        BaseEvents.CallOnClaimFreeCoin();

    }
}
