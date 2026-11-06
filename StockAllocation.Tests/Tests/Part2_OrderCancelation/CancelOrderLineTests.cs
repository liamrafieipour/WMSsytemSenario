
using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part2_OrderCancellation
{
    public class CancelOrderLineTests
    {
        [Fact]

        public void CancelOrderLine_ReleasesOnlySelectedLineStock()
        {
            var location = new WarehouseLocation("LOC-001");

            var sku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                20,
                location);

            var line1 = new OrderLine(
                "LINE-001",
                "JUICE",
                5);

            var line2 = new OrderLine(
                "LINE-002",
                "JUICE",
                7);

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

            // Allocation
            Assert.Equal(12, sku.AllocatedQuantity);
            Assert.Equal(8, sku.AvailableQuantity);

            // Cancel only the first line
            service.CancelOrderLine(
                order,
                "LINE-001",
                new[] { sku });

            // Only 5 units are released
            Assert.Equal(7, sku.AllocatedQuantity);
            Assert.Equal(13, sku.AvailableQuantity);

            Assert.True(line1.IsCancelled);
            Assert.False(line2.IsCancelled);

            Assert.Empty(line1.Allocations);
            Assert.Equal(7, line2.AllocatedQuantity);

            Assert.Equal(
            OrderStatus.ReadyForDelivery,
            order.Status);
        }
    }
}
