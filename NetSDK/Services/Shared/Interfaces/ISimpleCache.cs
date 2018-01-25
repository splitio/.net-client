namespace Splitio.Services.Shared.Interfaces
{
    public interface ISimpleCache<T>
    {
        void AddItem(T item);
    }
}
