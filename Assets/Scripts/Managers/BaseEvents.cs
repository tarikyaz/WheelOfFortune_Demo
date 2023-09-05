using System;

public static class BaseEvents
{
    internal static Action OnClaimFreeCoin;
    internal static Action OnGetExtraCoin;
    internal static Action OnSpendOneCoin;
    internal static Action<int> OnAdsRewardCoin;
    internal static Action<int> OnCoinsAmountUpdate;

    internal static void CallCoinsAmountUpdate(int newValue)
    {
        OnCoinsAmountUpdate?.Invoke(newValue);
    }

    internal static void CallOnClaimFreeCoin()
    {
        OnClaimFreeCoin?.Invoke();
    }

    internal static void CallOnGetExtraCoin()
    {
        OnGetExtraCoin?.Invoke();
    }

    internal static void CallOnSpendOneCoin()
    {
        OnSpendOneCoin?.Invoke();
    }

    internal static void CallRewardCoin(int amount)
    {
        OnAdsRewardCoin?.Invoke(amount);
    }
}
