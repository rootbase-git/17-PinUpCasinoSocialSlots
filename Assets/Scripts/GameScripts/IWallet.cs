namespace GameScripts
{
    public interface IWallet
    {
        void OnIncreaseBalance(float value);
        void OnDecreaseBalance(float value);
    }
}
