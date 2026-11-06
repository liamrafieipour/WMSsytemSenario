using StockAllocation.Domain;

namespace StockAllocation.Services
{
    public class StockAllocationService
    {
        public void Allocate(
            IEnumerable<CustomerOrder> orders,
            IEnumerable<StockKeepingUnit> stock)
        {
            if (orders == null)
                throw new ArgumentNullException(nameof(orders));

            if (stock == null)
                throw new ArgumentNullException(nameof(stock));

            var stockList = stock.ToList();

            ValidateStock(stockList);

            var orderedOrders = orders
                .Where(order =>
                    order.Status == OrderStatus.Released ||
                    order.Status == OrderStatus.PartiallyAllocated)
                .OrderByDescending(order => order.Priority)
                .ThenBy(order => order.CreatedAt)
                .ToList();

            foreach (var order in orderedOrders)
            {
                AllocateOrder(order, stockList);
            }
        }

        public void CancelOrder(
            CustomerOrder order,
            IEnumerable<StockKeepingUnit> stock)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (stock == null)
                throw new ArgumentNullException(nameof(stock));

            if (order.Status == OrderStatus.Cancelled)
                return;

            var stockList = stock.ToList();
            ValidateStock(stockList);

            ReleaseAllocations(order.Lines, stockList);

            order.Cancel();
        }

        public void CancelOrderLine(
            CustomerOrder order,
            string lineId,
            IEnumerable<StockKeepingUnit> stock)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (string.IsNullOrWhiteSpace(lineId))
                throw new ArgumentException("Line ID is required.");

            if (stock == null)
                throw new ArgumentNullException(nameof(stock));

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException(
                    "Cannot cancel a line from an already cancelled order.");

            var line = order.Lines.SingleOrDefault(
                item => item.Id == lineId);

            if (line == null)
                throw new ArgumentException(
                    "The specified order line was not found.",
                    nameof(lineId));

            if (line.IsCancelled)
                return;

            var stockList = stock.ToList();
            ValidateStock(stockList);

            ReleaseAllocations(new[] { line }, stockList);

            line.Cancel();

            order.UpdateAllocationStatus();
        }

        private void ReleaseAllocations(
            IEnumerable<OrderLine> lines,
            List<StockKeepingUnit> stock)
        {
            var stockById = stock.ToDictionary(
                sku => sku.Id,
                StringComparer.Ordinal);

            var quantitiesToRelease = new Dictionary<string, int>(
                StringComparer.Ordinal);

            foreach (var line in lines)
            {
                foreach (var allocation in line.Allocations)
                {
                    if (!stockById.ContainsKey(allocation.SkuId))
                    {
                        throw new InvalidOperationException(
                            $"SKU '{allocation.SkuId}' was not found.");
                    }

                    quantitiesToRelease.TryGetValue(
                        allocation.SkuId,
                        out var currentQuantity);

                    quantitiesToRelease[allocation.SkuId] =
                        currentQuantity + allocation.Quantity;
                }
            }

            // Validate all releases before changing any stock.
            foreach (var item in quantitiesToRelease)
            {
                if (stockById[item.Key].AllocatedQuantity < item.Value)
                {
                    throw new InvalidOperationException(
                        $"Cannot release more stock than allocated for SKU '{item.Key}'.");
                }
            }

            // Release the quantities from the SKUs.
            foreach (var item in quantitiesToRelease)
            {
                stockById[item.Key].Deallocate(item.Value);
            }
        }

        private void ValidateStock(List<StockKeepingUnit> stock)
        {
            if (stock.Any(sku => sku == null))
                throw new ArgumentException(
                    "Stock cannot contain null SKUs.");

            if (stock.Select(sku => sku.Id)
                .Distinct(StringComparer.Ordinal).Count() != stock.Count)
            {
                throw new ArgumentException(
                    "SKU IDs must be unique.");
            }
        }

        private void AllocateOrder(
            CustomerOrder order,
            List<StockKeepingUnit> stock)
        {
            if (order.CompleteDeliveryRequired)
            {
                AllocateCompleteOrder(order, stock);
            }
            else
            {
                AllocatePartialOrder(order, stock);
            }

            order.UpdateAllocationStatus();
        }

        private void AllocateCompleteOrder(
            CustomerOrder order,
            List<StockKeepingUnit> stock)
        {
            var plan = CreateAllocationPlan(order, stock);

            if (plan == null)
                return;

            ApplyAllocationPlan(plan);
        }

        private void AllocatePartialOrder(
            CustomerOrder order,
            List<StockKeepingUnit> stock)
        {
            var plan = CreateAllocationPlan(
                order,
                stock,
                allowPartial: true);

            if (plan == null)
                return;

            ApplyAllocationPlan(plan);
        }

        private List<PlannedAllocation>? CreateAllocationPlan(
            CustomerOrder order,
            List<StockKeepingUnit> stock,
            bool allowPartial = false)
        {
            var availableStock = stock.ToDictionary(
                sku => sku.Id,
                sku => sku.AvailableQuantity,
                StringComparer.Ordinal);

            var plan = new List<PlannedAllocation>();

            foreach (var line in order.Lines.Where(
                line => !line.IsCancelled))
            {
                var remainingQuantity = line.RemainingQuantity;

                var matchingSkus = stock
                    .Where(sku =>
                        sku.ProductNumber == line.ProductNumber &&
                        !sku.Location.IsLocked &&
                        availableStock[sku.Id] > 0)
                    .OrderBy(sku => sku.Id, StringComparer.Ordinal);

                foreach (var sku in matchingSkus)
                {
                    if (remainingQuantity == 0)
                        break;

                    var quantity = Math.Min(
                        remainingQuantity,
                        availableStock[sku.Id]);

                    plan.Add(new PlannedAllocation(
                        line,
                        sku,
                        quantity));

                    availableStock[sku.Id] -= quantity;
                    remainingQuantity -= quantity;
                }

                if (remainingQuantity > 0 && !allowPartial)
                    return null;
            }

            return plan;
        }

        private void ApplyAllocationPlan(
            List<PlannedAllocation> plan)
        {
            foreach (var allocation in plan)
            {
                allocation.Sku.Allocate(allocation.Quantity);

                allocation.Line.AddAllocation(
                    allocation.Sku.Id,
                    allocation.Quantity);
            }
        }

        private sealed record PlannedAllocation(
            OrderLine Line,
            StockKeepingUnit Sku,
            int Quantity);
    }
}