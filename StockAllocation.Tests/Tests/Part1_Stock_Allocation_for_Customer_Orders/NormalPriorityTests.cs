using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class NormalPriorityTests
    {
        [Fact]

        // Normal priority orders must be processed before Low priority orders.
        public void Allocate_WhenNormalAndLowPriorityOrdersCompete_NormalPriorityIsAllocatedFirst()
        {            
            var location = new WarehouseLocation("LOC-001");

            var sku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                10,
                location);

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

            var normalPriorityLine = new OrderLine(
                "LINE-002",
                "JUICE",
                10);

            var normalPriorityOrder = new CustomerOrder(
                "ORDER-002",
                new DateTime(2026, 1, 2),
                OrderPriority.Normal,
                false,
                new[] { normalPriorityLine });

            var service = new StockAllocationService();

            service.Allocate(
                new[] { lowPriorityOrder, normalPriorityOrder },
                new[] { sku });

            Assert.Equal(10, normalPriorityLine.AllocatedQuantity);
            Assert.Equal(0, lowPriorityLine.AllocatedQuantity);
            Assert.Equal(10, sku.AllocatedQuantity);
            Assert.Equal(0, sku.AvailableQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                normalPriorityOrder.Status);
            Assert.Equal(
                OrderStatus.Released,
                lowPriorityOrder.Status);
        }
    }
}