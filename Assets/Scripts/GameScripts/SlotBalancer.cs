using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameScripts
{
    public class SlotBalancer : MonoBehaviour, IRefresh, IMoney
    {
        [FormerlySerializedAs("currentBalanceUI")] public TMP_Text currentBalance;
        public float CurrentMoney { get; private set;}
        private const string Balance = "Balance";
        private const int DefaultMoney = 200;

        private void Start()
        {
            //store default balance data or load existing
            CurrentMoney = PlayerPrefs.HasKey(Balance) ? PlayerPrefs.GetFloat(Balance) : DefaultMoney;
            //CurrentMoney = 200;
            Refresh(CurrentMoney);
        }

        private void SaveMoney()
        {
            PlayerPrefs.SetFloat(Balance, CurrentMoney);
        }

        public void IncreaseMoney(float value)
        {
            CurrentMoney += value;
            SaveMoney();
            Refresh(CurrentMoney);
        }

        public void DecreaseMoney(float value)
        {
            CurrentMoney -= value;
            SaveMoney();
            Refresh(CurrentMoney);
        }

        public void Refresh(float value)
        {
            currentBalance.SetText(value.ToString());
        }

        private void OnEnable()
        {
            Betting.OnBet += DecreaseMoney;
            BetCalculator.OnPointCalculate += IncreaseMoney;
        }
        private void OnDisable()
        {
            Betting.OnBet -= DecreaseMoney;
            BetCalculator.OnPointCalculate -= IncreaseMoney;
        }
    }
}
