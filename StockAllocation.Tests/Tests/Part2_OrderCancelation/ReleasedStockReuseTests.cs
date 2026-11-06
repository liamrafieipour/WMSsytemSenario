
using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part2_OrderCancellation
{
    public class ReleasedStockReuseTests
    {
        [Fact]
        public void CancelOrder_ReleasesStockForAnotherOrder()
        {
            var location = new WarehouseLocation("LOC-001");

            var sku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                10,
                location);

            var firstLine = new OrderLine(
                "LINE-001",
                "JUICE",
                8);

            var firstOrder = new CustomerOrder(
                "ORDER-001",
                new DateTime(2026, 1, 1),
                OrderPriority.Normal,
                false,
                new[] { firstLine });

            var secondLine = new OrderLine(
                "LINE-002",
                "JUICE",
                8);

            var secondOrder = new CustomerOrder(
                "ORDER-002",
                new DateTime(2026, 1, 2),
                OrderPriority.Normal,
                false,
                new[] { secondLine });

            var service = new StockAllocationService();

            // First order allocates 8 units.
            service.Allocate(
                new[] { firstOrder },
                new[] { sku });

            Assert.Equal(8, sku.AllocatedQuantity);
            Assert.Equal(2, sku.AvailableQuantity);

            // cancel the first order.
            service.CancelOrder(
                firstOrder,
                new[] { sku });

            // The released stock is now available.
            Assert.Equal(0, sku.AllocatedQuantity);
            Assert.Equal(10, sku.AvailableQuantity);

            // Allocate stock to the second order.
            service.Allocate(
                new[] { secondOrder },
                new[] { sku });

            Assert.Equal(8, sku.AllocatedQuantity);
            Assert.Equal(2, sku.AvailableQuantity);

            Assert.Equal(8, secondLine.AllocatedQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                secondOrder.Status);
        }
    }
}
