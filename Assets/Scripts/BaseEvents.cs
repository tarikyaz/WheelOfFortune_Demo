using System;


public static class BaseEvents
{
    internal static Action OnClaimFreeCoin;
    internal static Action OnGetExtraCoin;
    internal static Action OnSpendOneCoin;
    internal static Action<int> OnRewardCoin;

    internal static void CallOnClaimFreeCoin()
    {
        OnClaimFreeCoin?.Invoke();
    }

    internal static void CallOnGetExtraCoin()
    {
        OnGetExtraCoin.Invoke();
    }

    internal static void CallOnSpendOneCoin()
    {
        OnSpendOneCoin?.Invoke();
    }

    internal static void CallRewardCoin(int amount)
    {
        OnRewardCoin?.Invoke(amount);
    }
}
