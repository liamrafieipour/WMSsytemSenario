namespace StockAllocation.Domain
{
    public class WarehouseLocation
    {
        public string Id { get; }
        public bool IsLocked { get; private set; }

        public WarehouseLocation(string id, bool isLocked = false)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Location ID is required.");

            Id = id;
            IsLocked = isLocked;
        }

        public void Lock()
        {
            IsLocked = true;
        }

        public void Unlock()
        {
            IsLocked = false;
        }
    }
}