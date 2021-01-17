using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameScripts
{
    public class Betting : MonoBehaviour, IRefresh
    {
        public static Action<float> OnBet;

        [FormerlySerializedAs("playButton")] public Button play;
        [FormerlySerializedAs("increaseBetButton")] public Button increaseBet;
        [FormerlySerializedAs("decreaseBetButton")] public Button decreaseBet;

        [FormerlySerializedAs("currentBetText")] public TMP_Text currentBet;
        [FormerlySerializedAs("winValueText")] public TMP_Text winValue;
        public ParticleSystem winParticleSystem;

        private int _currentBet;
        private const int DefaultStep = 10;

        private SlotBalancer _currentSlotBalance;
        private GameLogic _gameLogi;
        private AudioController _audioController;

        private void Awake()
        {
            _currentSlotBalance = FindObjectOfType<SlotBalancer>();
            _gameLogi = FindObjectOfType<GameLogic>();
            _audioController = FindObjectOfType<AudioController>();
        }

        private void Start()
        {
            IncreaseMoney();
        }

        public void IncreaseMoney()
        {
            if (_currentBet + DefaultStep < _currentSlotBalance.CurrentMoney)
            {
                _currentBet += DefaultStep;
                Refresh(_currentBet);
                
                RefreshPlus();
            }
            else if(_currentBet + DefaultStep == _currentSlotBalance.CurrentMoney)
            {
                _currentBet += DefaultStep;
                Refresh(_currentBet);

                RefreshPlus();
                
                increaseBet.interactable = false;
            }
            else
            {
                _currentBet = Mathf.RoundToInt(_currentSlotBalance.CurrentMoney);
                Refresh(_currentBet);

                RefreshPlus();
                
                increaseBet.interactable = false;
            }
            _audioController.PlayAudio(_audioController.click, false);
            if (_currentBet > 0) play.interactable = true;
        }
    
        public void DecreaseMoney()
        {
            if (_currentSlotBalance.CurrentMoney >= DefaultStep && _currentBet - DefaultStep > 0)
            {
                _currentBet -= DefaultStep;
                Refresh(_currentBet);

                RefreshMinus();
            }
            else if (_currentBet - DefaultStep == 0)
            {
                _currentBet -= DefaultStep;
                Refresh(_currentBet);

                RefreshMinus();
                
                decreaseBet.interactable = false;
            }
            else
            {
                _currentBet = 0;
                Refresh(_currentBet);

                RefreshMinus();
                
                decreaseBet.interactable = false;
            }
            _audioController.PlayAudio(_audioController.click, false);

            if (_currentBet == 0) play.interactable = false;
        }

        public void DecreaseBalanceAfterClick()
        {
            if(_gameLogi.IsPlaying) return;
            winParticleSystem.gameObject.SetActive(false);
            _audioController.PlayAudio(_audioController.buttonPress, false);
            OnBet?.Invoke(_currentBet);
        }

        private void SetWinText(float winValue)
        {
            winParticleSystem.gameObject.SetActive(true);
            this.winValue.SetText(winValue.ToString());
            ButtonRefresh();
        }
    
        public void Refresh(float value)
        {
            currentBet.SetText(value.ToString());
        }

        private void ButtonRefresh()
        {
            RefreshMinus();
            RefreshPlus();
        }

        private void RefreshMinus()
        {
            if (_currentSlotBalance.CurrentMoney > _currentBet)
            {
                increaseBet.interactable = true;
            }
        }
        private void RefreshPlus()
        {
            if (_currentSlotBalance.CurrentMoney - DefaultStep  >= 0)
            {
                decreaseBet.interactable = true;
            }
        }
    
        private void OnEnable()
        {
            BetCalculator.OnPointCalculate += SetWinText;
        }
    
        private void OnDisable()
        {
            BetCalculator.OnPointCalculate -= SetWinText;
        }
    }
}
