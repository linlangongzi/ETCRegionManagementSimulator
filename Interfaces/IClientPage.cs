namespace ETCRegionManagementSimulator.Interfaces
{
    public interface IClientPage
    {
        string ClientPageId { get; }
        void UpdateMessageView(string message);
    }
}
