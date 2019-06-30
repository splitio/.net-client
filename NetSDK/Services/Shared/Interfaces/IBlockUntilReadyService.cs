namespace Splitio.Services.Shared.Interfaces
{
    public interface IBlockUntilReadyService
    {
        void BlockUntilReady();
        bool IsSdkReady();
    }
}
