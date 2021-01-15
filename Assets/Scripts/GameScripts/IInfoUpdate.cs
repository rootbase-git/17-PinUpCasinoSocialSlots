namespace GameScripts
{
    public interface IInfoUpdate<in T>
    {
        void UpdateHash(T centerSpinElement);
    }
}
