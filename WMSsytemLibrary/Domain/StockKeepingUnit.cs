namespace StockAllocation.Domain
{
    public class StockKeepingUnit
    {
        public string Id { get; }
        public string ProductNumber { get; }
        public WarehouseLocation Location { get; }

        public int Quantity { get; private set; }

        public int AllocatedQuantity { get; private set; }

        public int AvailableQuantity => Quantity - AllocatedQuantity;

        public StockKeepingUnit(
            string id,
            string productNumber,
            int quantity,
            WarehouseLocation location)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("SKU ID is required.");

            if (string.IsNullOrWhiteSpace(productNumber))
                throw new ArgumentException("Product number is required.");

            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative.");

            Id = id;
            ProductNumber = productNumber;
            Quantity = quantity;
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public bool CanAllocate(int quantity)
        {
            return quantity > 0
                && !Location.IsLocked
                && AvailableQuantity >= quantity;
        }

        public void Allocate(int quantity)
        {
            if (!CanAllocate(quantity))
            {
                throw new InvalidOperationException(
                    "Insufficient available stock or location is locked.");
            }

            AllocatedQuantity += quantity;
        }
        public void Deallocate(int quantity)
        {
            if (quantity <= 0 || quantity > AllocatedQuantity)
            {
                throw new InvalidOperationException(
                    "Invalid deallocation quantity.");
            }

            AllocatedQuantity -= quantity;
        }
    }
}