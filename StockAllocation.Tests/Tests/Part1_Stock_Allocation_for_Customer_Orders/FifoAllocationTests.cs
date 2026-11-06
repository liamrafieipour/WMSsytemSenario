using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class FifoAllocationTests
    {
        [Fact]

        // FIFO (First In, First Out) for orders with the same priority.
        public void Allocate_WhenOrdersHaveSamePriority_OlderOrderIsAllocatedFirst()
        {
            var location = new WarehouseLocation("LOC-001");

            var sku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                10,
                location);

            // Older order
            var olderOrderLine = new OrderLine(
                "LINE-001",
                "JUICE",
                10);

            var olderOrder = new CustomerOrder(
                "ORDER-001",
                new DateTime(2026, 1, 1),
                OrderPriority.Normal,
                false,
                new[] { olderOrderLine });

            // Newer order
            var newerOrderLine = new OrderLine(
                "LINE-002",
                "JUICE",
                10);

            var newerOrder = new CustomerOrder(
                "ORDER-002",
                new DateTime(2026, 1, 2),
                OrderPriority.Normal,
                false,
                new[] { newerOrderLine });

            var service = new StockAllocationService();


            service.Allocate(
                new[] { newerOrder, olderOrder },
                new[] { sku });


            Assert.Equal(10, olderOrderLine.AllocatedQuantity);
            Assert.Equal(0, newerOrderLine.AllocatedQuantity);
            Assert.Equal(10, sku.AllocatedQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                olderOrder.Status);

            Assert.Equal(
                OrderStatus.Released,
                newerOrder.Status);
        }
    }
}