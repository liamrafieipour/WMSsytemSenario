namespace StockAllocation.Domain
{
    public class CustomerOrder
    {
        private readonly List<OrderLine> _lines;

        public string Id { get; }
        public DateTime CreatedAt { get; }
        public OrderPriority Priority { get; }
        public bool CompleteDeliveryRequired { get; }
        public OrderStatus Status { get; private set; }

        public IReadOnlyCollection<OrderLine> Lines => _lines;

        public bool IsFullyAllocated =>
            _lines.Any(line => !line.IsCancelled) &&
            _lines.Where(line => !line.IsCancelled)
                  .All(line => line.IsFullyAllocated);

        public bool HasAllocations =>
            _lines.Any(line =>
                !line.IsCancelled && line.AllocatedQuantity > 0);

        public CustomerOrder(
            string id,
            DateTime createdAt,
            OrderPriority priority,
            bool completeDeliveryRequired,
            IEnumerable<OrderLine> lines)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Order ID is required.");

            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            _lines = lines.ToList();

            if (_lines.Count == 0)
                throw new ArgumentException(
                    "An order must contain at least one line.");

            if (_lines.Any(line => line == null))
                throw new ArgumentException(
                    "Order lines cannot contain null values.");

            if (_lines.Select(line => line.Id).Distinct().Count() != _lines.Count)
                throw new ArgumentException(
                    "Order line IDs must be unique.");

            Id = id;
            CreatedAt = createdAt;
            Priority = priority;
            CompleteDeliveryRequired = completeDeliveryRequired;

            Status = OrderStatus.Released;
        }

        public void UpdateAllocationStatus()
        {
            if (Status == OrderStatus.Cancelled)
                return;

            var activeLines = _lines
                .Where(line => !line.IsCancelled)
                .ToList();

            if (activeLines.Count == 0)
            {
                Status = OrderStatus.Cancelled;
            }
            else if (activeLines.All(line => line.IsFullyAllocated))
            {
                Status = OrderStatus.ReadyForDelivery;
            }
            else if (HasAllocations)
            {
                Status = OrderStatus.PartiallyAllocated;
            }
            else
            {
                Status = OrderStatus.Released;
            }
        }

        public void Cancel()
        {
            foreach (var line in _lines)
            {
                line.Cancel();
            }

            Status = OrderStatus.Cancelled;
        }
    }
}