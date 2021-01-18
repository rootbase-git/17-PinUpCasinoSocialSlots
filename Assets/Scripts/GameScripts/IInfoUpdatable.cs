namespace GameScripts
{
    public interface IInfoUpdatable<in T>
    {
        void UpdateElement(T centerSpinElement);
    }
}
