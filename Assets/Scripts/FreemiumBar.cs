using TMPro;
using UnityEngine;

public class FreemiumBar : MonoBehaviour
{
    [SerializeField] TMP_Text coins_Text;

    private void OnEnable()
    {
        BaseEvents.OnCoinsAmountUpdate += OnCoinsAmountUpdate;
    }
    private void OnDisable()
    {
        BaseEvents.OnCoinsAmountUpdate -= OnCoinsAmountUpdate;

    }

    private void OnCoinsAmountUpdate(int newAmount)
    {
        coins_Text.text = $"Coins: {newAmount}/{GameManager.Instance.MaxCoinsAmount}";
    }
}
