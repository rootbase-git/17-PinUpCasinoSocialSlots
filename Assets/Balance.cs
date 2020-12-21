using TMPro;
using UnityEngine;

public class Balance : MonoBehaviour, IRefreshable, IWallet
{
    public TMP_Text currentBalanceUI;
    public int CurrentBalance { get; private set;}
    private const string BalanceKey = "Balance";
    private const int DefaultBalance = 20;

    private void Start()
    {
        //store default balance data or load existing
        CurrentBalance = PlayerPrefs.HasKey(BalanceKey) ? PlayerPrefs.GetInt(BalanceKey) : DefaultBalance;
        //CurrentBalance = 20;
        RefreshUi(CurrentBalance);
    }

    private void SaveBalance()
    {
        PlayerPrefs.SetInt(BalanceKey, CurrentBalance);
    }

    public void OnIncreaseBalance(int value)
    {
        CurrentBalance += value;
        SaveBalance();
        RefreshUi(CurrentBalance);
    }

    public void OnDecreaseBalance(int value)
    {
        CurrentBalance -= value;
        SaveBalance();
        RefreshUi(CurrentBalance);
    }

    public void RefreshUi(int value)
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
