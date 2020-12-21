using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BetHandler : MonoBehaviour, IRefreshable
{
    public static Action<int> OnBetMade;

    public Button playButton;
    public Button increaseBetButton;
    public Button decreaseBetButton;

    public TMP_Text currentBetText;
    public TMP_Text winValueText;

    private int _currentBetValue;
    private const int DefaultBetStep = 10;

    private Balance _currentBalance;

    private void Awake()
    {
        _currentBalance = FindObjectOfType<Balance>();
    }

    private void Start()
    {
        _currentBetValue = 0;
        
        if (_currentBetValue == 0) playButton.interactable = false;
    }

    public void IncreaseBet()
    {
        if (_currentBetValue + DefaultBetStep <= _currentBalance.CurrentBalance)
        {
            _currentBetValue += DefaultBetStep;
            RefreshUi(_currentBetValue);
            
            if (_currentBalance.CurrentBalance - DefaultBetStep  >= 0)
            {
                decreaseBetButton.interactable = true;
            }
        }
        else
        {
            increaseBetButton.interactable = false;
        }
        
        if (_currentBetValue > 0) playButton.interactable = true;
    }
    
    public void DecreaseBet()
    {
        if (_currentBalance.CurrentBalance >= DefaultBetStep && _currentBetValue - DefaultBetStep >= 0)
        {
            _currentBetValue -= DefaultBetStep;
            RefreshUi(_currentBetValue);
            
            if (_currentBalance.CurrentBalance > _currentBetValue)
            {
                increaseBetButton.interactable = true;
            }
        }
        else
        {
            decreaseBetButton.interactable = false;
        }

        if (_currentBetValue == 0) playButton.interactable = false;
    }

    public void DecreaseBalanceAfterBet()
    {
        OnBetMade?.Invoke(_currentBetValue);
    }

    private void SetWinTextValue(int winValue)
    {
        winValueText.SetText(winValue.ToString());
    }
    
    public void RefreshUi(int value)
    {
        currentBetText.SetText(value.ToString());
    }
    
    private void OnEnable()
    {
        PointsCalculator.OnPointCalculated += SetWinTextValue;
    }
    
    private void OnDisable()
    {
        PointsCalculator.OnPointCalculated -= SetWinTextValue;
    }
}
