namespace StockAllocation.Domain
{
    public class OrderLine
    {
        private readonly List<Allocation> _allocations = new();

        public string Id { get; }
        public string ProductNumber { get; }
        public int RequestedQuantity { get; }

        public bool IsCancelled { get; private set; }

        public IReadOnlyCollection<Allocation> Allocations => _allocations;

        public int AllocatedQuantity =>
            _allocations.Sum(a => a.Quantity);

        public int RemainingQuantity =>
            RequestedQuantity - AllocatedQuantity;

        public bool IsFullyAllocated =>
            !IsCancelled && RemainingQuantity == 0;

        public OrderLine(
            string id,
            string productNumber,
            int requestedQuantity)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Line ID is required.");

            if (string.IsNullOrWhiteSpace(productNumber))
                throw new ArgumentException("Product number is required.");

            if (requestedQuantity <= 0)
                throw new ArgumentException(
                    "Requested quantity must be greater than zero.");

            Id = id;
            ProductNumber = productNumber;
            RequestedQuantity = requestedQuantity;
        }

        public void AddAllocation(string skuId, int quantity)
        {
            if (IsCancelled)
                throw new InvalidOperationException(
                    "Cannot allocate a cancelled order line.");

            if (quantity <= 0 || quantity > RemainingQuantity)
            {
                throw new InvalidOperationException(
                    "Invalid allocation quantity.");
            }

            _allocations.Add(new Allocation(skuId, quantity));
        }

        public void Cancel()
        {
            IsCancelled = true;
            _allocations.Clear();
        }
    }
}