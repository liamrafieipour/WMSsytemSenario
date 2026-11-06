
using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part2_OrderCancellation
{
    public class CancelOrderTests
    {
        [Fact]
        public void CancelOrder_ReleasesAllAllocatedStock()
        {
            var location = new WarehouseLocation("LOC-001");

            var sku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                10,
                location);

            var line1 = new OrderLine("LINE-001", "JUICE", 4);
            var line2 = new OrderLine("LINE-002", "JUICE", 3);

            var order = new CustomerOrder(
                "ORDER-001",
                new DateTime(2026, 1, 1),
                OrderPriority.Normal,
                false,
                new[] { line1, line2 });

            var service = new StockAllocationService();

            service.Allocate(
                new[] { order },
                new[] { sku });

            service.CancelOrder(order, new[] { sku });

            Assert.Equal(0, sku.AllocatedQuantity);
            Assert.Equal(10, sku.AvailableQuantity);

            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.True(line1.IsCancelled);
            Assert.True(line2.IsCancelled);
            Assert.Empty(line1.Allocations);
            Assert.Empty(line2.Allocations);
        }
    }
}
