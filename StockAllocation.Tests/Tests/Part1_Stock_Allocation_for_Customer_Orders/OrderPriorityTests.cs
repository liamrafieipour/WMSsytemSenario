using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class OrderPriorityTests
    {
        [Fact]

        // High priority orders must be processed before Normal and Low priority orders, even when a lower-priority order was received earlier
        public void Allocate_WhenStockIsLimited_HighPriorityOrderIsAllocatedFirst()
        {
            var location = new WarehouseLocation("LOC-001");

            var sku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                10,
                location);

            // Low priority order arrives first
            var lowPriorityLine = new OrderLine(
                "LINE-001",
                "JUICE",
                10);

            var lowPriorityOrder = new CustomerOrder(
                "ORDER-001",
                new DateTime(2026, 1, 1),
                OrderPriority.Low,
                false,
                new[] { lowPriorityLine });

            // High priority order arrives later
            var highPriorityLine = new OrderLine(
                "LINE-002",
                "JUICE",
                10);

            var highPriorityOrder = new CustomerOrder(
                "ORDER-002",
                new DateTime(2026, 1, 2),
                OrderPriority.High,
                false,
                new[] { highPriorityLine });

            var service = new StockAllocationService();

            service.Allocate(
                new[] { lowPriorityOrder, highPriorityOrder },
                new[] { sku });


            Assert.Equal(10, highPriorityLine.AllocatedQuantity);
            Assert.Equal(0, lowPriorityLine.AllocatedQuantity);
            Assert.Equal(10, sku.AllocatedQuantity);
            Assert.Equal(0, sku.AvailableQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                highPriorityOrder.Status);
            Assert.Equal(
                OrderStatus.Released,
                lowPriorityOrder.Status);
        }
    }
}