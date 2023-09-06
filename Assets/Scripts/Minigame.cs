using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    [SerializeField] Button[] buttonsArray = new Button[0];
    [SerializeField] TMP_InputField bet_Input;
    const string TIMES_SPINNING_KEY_Str = "TIMESSPINING";
    int timesSpinning
    {
        get => PlayerPrefs.GetInt(TIMES_SPINNING_KEY_Str, 0);
        set => PlayerPrefs.SetInt(TIMES_SPINNING_KEY_Str, value);
    }
    [SerializeField] PickerWheel pickerWheel;
    int currentBet = -1;

    private void Start()
    {
        // Set button actions
        for (int i = 0; i < buttonsArray.Length; i++)
        {
            int index = i;
            Button button = buttonsArray[index];
            button.onClick.AddListener(() => ButtonClicked(index));
            button.interactable = false;
        }

        // Set bet input listener so the UI changes depending on the bet value if it's suitable to make a spin or not
        bet_Input.onValueChanged.AddListener((value) => {
            OnBetChange(value);
        });
    }
    private void OnEnable()
    {
        BaseEvents.OnCoinsAmountUpdate += OnCoinsUpdateAmountHandler;

        // Refreshing bet input 
        bet_Input.text = "";
    }
    private void OnDisable()
    {
        BaseEvents.OnCoinsAmountUpdate -= OnCoinsUpdateAmountHandler;
    }

    private void OnCoinsUpdateAmountHandler(int newValue)
    {
        // Checking if the bet value is suitable for spinning
        for (int i = 0; i < buttonsArray.Length; i++)
        {
            Button button = buttonsArray[i];
            button.interactable = currentBet > 0 && currentBet <= newValue;
        }
    }

    void ButtonClicked(int btnIndex)
    {
        // Changing the spinning target depending on the win logic 
        GetResult(ref btnIndex, out var isWin);
        for (int i = 0; i < buttonsArray.Length; i++)
        {
            Button button = buttonsArray[i];
            button.interactable = false;
        }

        // Call spin for the wheel
        pickerWheel.Spin(btnIndex, () =>
        {
            // After finishing spinning, show the result
            BaseEvents.CallAddCoins(isWin ? currentBet * 2 : -currentBet);
        });
    }

    private void GetResult(ref int btnIndex, out bool isWin)
    {
        timesSpinning++;
        Debug.Log("timesSpinning " + timesSpinning);

        /* Winning logic:
         * 
            -	1st "spin", user loses
            -	2nd "spin", user wins
            -	3rd-6th "spin", user loses
            -	7th, 8th "spin", user wins
            -	9th time user loses
            -	After the 10th "spin", user has a 5% chance of winning

         */

        switch (timesSpinning)
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
            // If there is no win, let it spin to a random value but not the one the player's bet
            List<int> newValues = new List<int>();
            for (int i = 0; i < buttonsArray.Length - 1; i++)
            {
                if (i != btnIndex)
                {
                    newValues.Add(i);
                }
            }
            btnIndex = newValues[UnityEngine.Random.Range(0, newValues.Count)];
        }
    }

    void OnBetChange(string value)
    {
        // If it's an int value, then check if the player can cover the loss
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
