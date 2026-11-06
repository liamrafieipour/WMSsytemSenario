using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class PartialAllocationRetryTests
    {
        [Fact]

        // Partially allocated order can receive its remaining stock when new stock becomes available.
        public void Allocate_WhenMoreStockBecomesAvailable_AllocatesRemainingQuantity()
        {
            var location = new WarehouseLocation("LOC-001");

            var firstSku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                5,
                location);

            var orderLine = new OrderLine(
                "LINE-001",
                "JUICE",
                10);

            var order = new CustomerOrder(
                "ORDER-001",
                new DateTime(2026, 1, 1),
                OrderPriority.Normal,
                false,
                new[] { orderLine });

            var service = new StockAllocationService();

            // First allocation 
            service.Allocate(
                new[] { order },
                new[] { firstSku });


            Assert.Equal(5, orderLine.AllocatedQuantity);
            Assert.Equal(
                OrderStatus.PartiallyAllocated,
                order.Status);

            // New stock arrives
            var secondSku = new StockKeepingUnit(
                "SKU-002",
                "JUICE",
                5,
                location);

            // Second allocation 
            service.Allocate(
                new[] { order },
                new[] { firstSku, secondSku });

            // Final allocation
            Assert.Equal(10, orderLine.AllocatedQuantity);
            Assert.Equal(0, orderLine.RemainingQuantity);
            Assert.Equal(5, firstSku.AllocatedQuantity);
            Assert.Equal(5, secondSku.AllocatedQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                order.Status);
        }
    }
}