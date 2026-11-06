using StockAllocation.Domain;
using StockAllocation.Services;
using System.Numerics;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class PartialAllocationMultipleLinesTests
    {
        
        [Fact]

        // Partial allocation across multiple different product lines. This verifies that when complete delivery is not required, the system allocates available stock for each line independently, even if another line cannot be fully fulfilled.
        public void Allocate_WhenMultipleLinesHaveInsufficientStock_AllocatesAvailableStockForEachLine()
        {
            var location = new WarehouseLocation("LOC-001");

            var juiceSku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                10,
                location);

            var waterSku = new StockKeepingUnit(
                "SKU-002",
                "WATER",
                2,
                location);

            var juiceLine = new OrderLine(
                "LINE-001",
                "JUICE",
                5);

            var waterLine = new OrderLine(
                "LINE-002",
                "WATER",
                5);

            var order = new CustomerOrder(
                "ORDER-001",
                new DateTime(2026, 1, 1),
                OrderPriority.Normal,
                false,
                new[] { juiceLine, waterLine });

            var service = new StockAllocationService();


            service.Allocate(
                new[] { order },
                new[] { juiceSku, waterSku });


            Assert.Equal(5, juiceLine.AllocatedQuantity);
            Assert.Equal(2, waterLine.AllocatedQuantity);
            Assert.Equal(0, juiceLine.RemainingQuantity);
            Assert.Equal(3, waterLine.RemainingQuantity);
            Assert.Equal(5, juiceSku.AllocatedQuantity);
            Assert.Equal(2, waterSku.AllocatedQuantity);
            Assert.Equal(
                OrderStatus.PartiallyAllocated,
                order.Status);
        }
    }
}