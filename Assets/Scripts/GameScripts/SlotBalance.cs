using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameScripts
{
    public class SlotBalance : MonoBehaviour, IRefreshable, IMoneys
    {
        [FormerlySerializedAs("currentBalance")] [FormerlySerializedAs("currentBalanceUI")] public TMP_Text thisBalance;
        public float CurrentMone { get; private set;}
        private const string Wallet = "Wallet";
        private const int DefaultValue = 200;

        private void Start()
        {
            //store default balance data or load existing
            CurrentMone = PlayerPrefs.HasKey(Wallet) ? PlayerPrefs.GetFloat(Wallet) : DefaultValue;
            //CurrentMone = 200;
            Refreshh(CurrentMone);
        }

        private void Save()
        {
            PlayerPrefs.SetFloat(Wallet, CurrentMone);
        }

        public void IncreaseMoneys(float value)
        {
            CurrentMone += value;
            Save();
            Refreshh(CurrentMone);
        }

        public void DecreaseMoneys(float value)
        {
            CurrentMone -= value;
            Save();
            Refreshh(CurrentMone);
        }

        public void Refreshh(float value)
        {
            thisBalance.SetText(value.ToString());
        }

        private void OnEnable()
        {
            BetMoney.OnBetMade += DecreaseMoneys;
            BetCalculate.OnCalculate += IncreaseMoneys;
        }
        private void OnDisable()
        {
            BetMoney.OnBetMade -= DecreaseMoneys;
            BetCalculate.OnCalculate -= IncreaseMoneys;
        }
    }
}
