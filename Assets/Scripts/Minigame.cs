using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    [SerializeField] Button[] buttonsArray = new Button[0];
    [SerializeField] TMP_InputField bet_Input;
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
            button.interactable = false;
        }
        bet_Input.onValueChanged.AddListener((value) => {
            OnBetChange(value);
        });
    }
    private void OnEnable()
    {
        BaseEvents.OnCoinsAmountUpdate += OnCoinsUpdateAmountHandler;
        bet_Input.text = "";


    }
    private void OnDisable()
    {
        BaseEvents.OnCoinsAmountUpdate -= OnCoinsUpdateAmountHandler;
    }

    private void OnCoinsUpdateAmountHandler(int newValue)
    {
        for (int i = 0; i < buttonsArray.Length; i++)
        {
            Button button = buttonsArray[i];
            button.interactable = currentBet > 0 && currentBet <= newValue;
        }
    }

    void ButtonClicked(int btnIndex)
    {
        GetResult(ref btnIndex, out var isWin);
        for (int i = 0; i < buttonsArray.Length; i++)
        {
            Button button = buttonsArray[i];
            button.interactable = false;
        }
        pickerWheel.Spin(btnIndex, () =>
        {
            BaseEvents.CallAddCoins(isWin ? currentBet * 2 : -currentBet);
        });
    }

    private void GetResult(ref int btnIndex, out bool isWin)
    {
        timesSpining++;
        Debug.Log("timesSpining " + timesSpining);
        switch (timesSpining)
        {
            case 1:
                isWin = false;
                break;
            case 2:
                isWin = true;
                break;
            case var n when (n > 2 && n < 7):
                isWin = false;
                break;
            case var n when (n == 7 || n == 8):
                isWin = true;
                break;
            case 9:
                isWin = false;
                break;
            default:
                isWin = UnityEngine.Random.value <= .05f;
                break;
        }
        Debug.Log("is win " + isWin);
        if (!isWin)
        {
            List<int> newVlaues = new List<int>();
            for (int i = 0; i < buttonsArray.Length - 1; i++)
            {
                if (i != btnIndex)
                {
                    newVlaues.Add(i);
                }
            }
            btnIndex = newVlaues[UnityEngine.Random.Range(0, newVlaues.Count)];
        }
    }

     void OnBetChange(string value)
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
            buttonsArray[i].interactable = currentBet > 0 && currentBet <= GameManager.Instance.NumOfCoins;
        }
    }
}
