using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameScripts
{
    public class BetMoney : MonoBehaviour, IRefreshable
    {
        public static Action<float> OnBetMade;

        public Button playButton;
        public Button increaseBetButton;
        public Button decreaseBetButton;

        public TMP_Text currentBetText;
        public TMP_Text winValueText;
        public ParticleSystem winParticleSystem;

        private int _currentBetVal;
        private const int DefaultStepVal = 10;

        private SlotBalance _currentBalance;
        private CoreLogic _coreLogic;
        private AudioController _audioController;

        private void Awake()
        {
            _currentBalance = FindObjectOfType<SlotBalance>();
            _coreLogic = FindObjectOfType<CoreLogic>();
            _audioController = FindObjectOfType<AudioController>();
        }

        private void Start()
        {
            AddMoney();
        }

        public void AddMoney()
        {
            if (_currentBetVal + DefaultStepVal < _currentBalance.CurrentMone)
            {
                _currentBetVal += DefaultStepVal;
                Refreshh(_currentBetVal);
                
                RefreshPlus();
            }
            else if(_currentBetVal + DefaultStepVal == _currentBalance.CurrentMone)
            {
                _currentBetVal += DefaultStepVal;
                Refreshh(_currentBetVal);

                RefreshPlus();
                
                increaseBetButton.interactable = false;
            }
            else
            {
                _currentBetVal = Mathf.RoundToInt(_currentBalance.CurrentMone);
                Refreshh(_currentBetVal);

                RefreshPlus();
                
                increaseBetButton.interactable = false;
            }
            _audioController.PlayAudio(_audioController.click, false);
            if (_currentBetVal > 0) playButton.interactable = true;
        }
    
        public void MinusMoney()
        {
            if (_currentBalance.CurrentMone >= DefaultStepVal && _currentBetVal - DefaultStepVal > 0)
            {
                _currentBetVal -= DefaultStepVal;
                Refreshh(_currentBetVal);

                RefreshMinus();
            }
            else if (_currentBetVal - DefaultStepVal == 0)
            {
                _currentBetVal -= DefaultStepVal;
                Refreshh(_currentBetVal);

                RefreshMinus();
                
                decreaseBetButton.interactable = false;
            }
            else
            {
                _currentBetVal = 0;
                Refreshh(_currentBetVal);

                RefreshMinus();
                
                decreaseBetButton.interactable = false;
            }
            _audioController.PlayAudio(_audioController.click, false);

            if (_currentBetVal == 0) playButton.interactable = false;
        }

        public void MinusBalanceAfterClick()
        {
            if(_coreLogic.IsPlay) return;
            winParticleSystem.gameObject.SetActive(false);
            _audioController.PlayAudio(_audioController.buttonPress, false);
            OnBetMade?.Invoke(_currentBetVal);
        }

        private void SetWin(float winValue)
        {
            winParticleSystem.gameObject.SetActive(true);
            this.winValueText.SetText(winValue.ToString());
            AllButtonRefresh();
        }
    
        public void Refreshh(float value)
        {
            currentBetText.SetText(value.ToString());
        }

        private void AllButtonRefresh()
        {
            RefreshMinus();
            RefreshPlus();
        }

        private void RefreshMinus()
        {
            if (_currentBalance.CurrentMone > _currentBetVal)
            {
                increaseBetButton.interactable = true;
            }
        }
        private void RefreshPlus()
        {
            if (_currentBalance.CurrentMone - DefaultStepVal  >= 0)
            {
                decreaseBetButton.interactable = true;
            }
        }
    
        private void OnEnable()
        {
            BetCalculate.OnCalculate += SetWin;
        }
    
        private void OnDisable()
        {
            BetCalculate.OnCalculate -= SetWin;
        }
    }
}
