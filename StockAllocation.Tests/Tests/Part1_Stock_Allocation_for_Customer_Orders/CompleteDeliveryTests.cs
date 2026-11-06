using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class CompleteDeliveryTests
    {
        [Fact]

        // Complete delivery when stock is insufficient
        public void Allocate_WhenCompleteDeliveryRequiredAndStockIsInsufficient_AllocatesNothing()
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
                true,
                new[] { juiceLine, waterLine });

            var service = new StockAllocationService();

            // Act
            service.Allocate(
                new[] { order },
                new[] { juiceSku, waterSku });

            Assert.Equal(0, juiceSku.AllocatedQuantity);
            Assert.Equal(0, waterSku.AllocatedQuantity);
            Assert.Equal(0, juiceLine.AllocatedQuantity);
            Assert.Equal(0, waterLine.AllocatedQuantity);

            Assert.Equal(
                OrderStatus.Released,
                order.Status);
        }
    }
}