using TMPro;
using UnityEngine;

namespace GameScripts
{
    public class Balance : MonoBehaviour, IRefreshable, IWallet
    {
        public TMP_Text currentBalanceUI;
        public float CurrentBalance { get; private set;}
        private const string BalanceKey = "Balance";
        private const int DefaultBalance = 200;

        private void Start()
        {
            //store default balance data or load existing
            CurrentBalance = PlayerPrefs.HasKey(BalanceKey) ? PlayerPrefs.GetFloat(BalanceKey) : DefaultBalance;
            CurrentBalance = 200;
            RefreshUi(CurrentBalance);
        }

        private void SaveBalance()
        {
            PlayerPrefs.SetFloat(BalanceKey, CurrentBalance);
        }

        public void OnIncreaseBalance(float value)
        {
            CurrentBalance += value;
            SaveBalance();
            RefreshUi(CurrentBalance);
        }

        public void OnDecreaseBalance(float value)
        {
            CurrentBalance -= value;
            SaveBalance();
            RefreshUi(CurrentBalance);
        }

        public void RefreshUi(float value)
        {
            currentBalanceUI.SetText(value.ToString());
        }

        private void OnEnable()
        {
            BetHandler.OnBetMade += OnDecreaseBalance;
            PointsCalculator.OnPointCalculated += OnIncreaseBalance;
        }
        private void OnDisable()
        {
            BetHandler.OnBetMade -= OnDecreaseBalance;
            PointsCalculator.OnPointCalculated -= OnIncreaseBalance;
        }
    }
}
