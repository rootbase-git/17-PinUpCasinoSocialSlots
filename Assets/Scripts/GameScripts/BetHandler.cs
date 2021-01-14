using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameScripts
{
    public class BetHandler : MonoBehaviour, IRefreshable
    {
        public static Action<float> OnBetMade;

        public Button playButton;
        public Button increaseBetButton;
        public Button decreaseBetButton;

        public TMP_Text currentBetText;
        public TMP_Text winValueText;

        private int _currentBetValue;
        private const int DefaultBetStep = 10;

        private Balance _currentBalance;
        private SlotsLogic _slotsLogic;

        private void Awake()
        {
            _currentBalance = FindObjectOfType<Balance>();
            _slotsLogic = FindObjectOfType<SlotsLogic>();
        }

        private void Start()
        {
            IncreaseBet();
        }

        public void IncreaseBet()
        {
            if (_currentBetValue + DefaultBetStep < _currentBalance.CurrentBalance)
            {
                _currentBetValue += DefaultBetStep;
                RefreshUi(_currentBetValue);
                
                RefreshPlusButton();
            }
            else if(_currentBetValue + DefaultBetStep == _currentBalance.CurrentBalance)
            {
                _currentBetValue += DefaultBetStep;
                RefreshUi(_currentBetValue);

                RefreshPlusButton();
                
                increaseBetButton.interactable = false;
            }
            else
            {
                _currentBetValue = Mathf.RoundToInt(_currentBalance.CurrentBalance);
                RefreshUi(_currentBetValue);

                RefreshPlusButton();
                
                increaseBetButton.interactable = false;
            }

            if (_currentBetValue > 0) playButton.interactable = true;
        }
    
        public void DecreaseBet()
        {
            if (_currentBalance.CurrentBalance >= DefaultBetStep && _currentBetValue - DefaultBetStep > 0)
            {
                _currentBetValue -= DefaultBetStep;
                RefreshUi(_currentBetValue);

                RefreshMinusButton();
            }
            else if (_currentBetValue - DefaultBetStep == 0)
            {
                _currentBetValue -= DefaultBetStep;
                RefreshUi(_currentBetValue);

                RefreshMinusButton();
                
                decreaseBetButton.interactable = false;
            }
            else
            {
                _currentBetValue = 0;
                RefreshUi(_currentBetValue);

                RefreshMinusButton();
                
                decreaseBetButton.interactable = false;
            }

            if (_currentBetValue == 0) playButton.interactable = false;
        }

        public void DecreaseBalanceAfterBet()
        {
            if(_slotsLogic.IsPlaying) return;
            OnBetMade?.Invoke(_currentBetValue);
        }

        private void SetWinTextValue(float winValue)
        {
            winValueText.SetText(winValue.ToString());
            ButtonsRefresh();
        }
    
        public void RefreshUi(float value)
        {
            currentBetText.SetText(value.ToString());
        }

        private void ButtonsRefresh()
        {
            RefreshMinusButton();
            RefreshPlusButton();
        }

        private void RefreshMinusButton()
        {
            if (_currentBalance.CurrentBalance > _currentBetValue)
            {
                increaseBetButton.interactable = true;
            }
        }
        private void RefreshPlusButton()
        {
            if (_currentBalance.CurrentBalance - DefaultBetStep  >= 0)
            {
                decreaseBetButton.interactable = true;
            }
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
}
