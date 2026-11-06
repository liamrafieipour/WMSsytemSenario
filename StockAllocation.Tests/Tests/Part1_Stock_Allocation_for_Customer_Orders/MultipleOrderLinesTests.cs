using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class MultipleOrderLinesTests
    {
        [Fact]
        // A customer order can contain multiple order lines, and each line must receive the correct product and quantity.
        public void Allocate_WhenOrderContainsMultipleProducts_AllocatesEachLineCorrectly()
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
                15,
                location);

            var milkSku = new StockKeepingUnit(
                "SKU-003",
                "MILK",
                8,
                location);

            var juiceLine = new OrderLine(
                "LINE-001",
                "JUICE",
                5);

            var waterLine = new OrderLine(
                "LINE-002",
                "WATER",
                8);

            var milkLine = new OrderLine(
                "LINE-003",
                "MILK",
                3);

            var order = new CustomerOrder(
                "ORDER-001",
                new DateTime(2026, 1, 1),
                OrderPriority.Normal,
                false,
                new[] { juiceLine, waterLine, milkLine });

            var service = new StockAllocationService();

            service.Allocate(
                new[] { order },
                new[] { juiceSku, waterSku, milkSku });

            Assert.Equal(5, juiceLine.AllocatedQuantity);
            Assert.Equal(8, waterLine.AllocatedQuantity);
            Assert.Equal(3, milkLine.AllocatedQuantity);
            Assert.Equal(5, juiceSku.AllocatedQuantity);
            Assert.Equal(8, waterSku.AllocatedQuantity);
            Assert.Equal(3, milkSku.AllocatedQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                order.Status);
        }
    }
}