using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    [SerializeField] Button[] buttonsArray = new Button[8];
    const string TIMES_SPINING_KEY_Str = "TIMESSPINING";
    int timesSpining
    {
        get => PlayerPrefs.GetInt(TIMES_SPINING_KEY_Str, 0);
        set => PlayerPrefs.SetInt(TIMES_SPINING_KEY_Str, value);
    }
    [SerializeField] PickerWheel pickerWheel;
    int currentBet = -1;
    private void Start()
    {
        for (int i = 0; i < buttonsArray.Length; i++)
        {
            int index = i;
            Button button = buttonsArray[index];
            button.onClick.AddListener(() => ButtonClicked(index));
            button.interactable = currentBet > 0 && currentBet <= GameManager.Instance.CoinsManager.NumOfCoins;

        }
    }

    void ButtonClicked(int btnIndex)
    {
        pickerWheel.Spin(btnIndex);
    }
    public void OnBetChange(string value)
    {
        if (int.TryParse(value, out int valueInt))
        {
            currentBet = valueInt;
        }
        else
        {
            currentBet = -1;
        }
        for (int i = 0; i < buttonsArray.Length; i++)
        {
            buttonsArray[i].interactable = currentBet > 0 && currentBet <= GameManager.Instance.CoinsManager.NumOfCoins;
        }
    }
}
