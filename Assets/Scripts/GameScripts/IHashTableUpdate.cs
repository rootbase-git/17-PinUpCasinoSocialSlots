namespace GameScripts
{
    public interface IHashTableUpdate<in T>
    {
        void UpdateHashTableInfo(T centerSpinElement);
    }
}
