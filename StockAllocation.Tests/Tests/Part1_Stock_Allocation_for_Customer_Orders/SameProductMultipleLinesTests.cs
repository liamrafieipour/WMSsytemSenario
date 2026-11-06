using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class SameProductMultipleLinesTests
    {

        [Fact]

        // The same product appearing on multiple order lines within one customer order.
        public void Allocate_WhenMultipleLinesRequestSameProduct_SharesStockCorrectly()
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
                4);

            var secondLine = new OrderLine(
                "LINE-002",
                "JUICE",
                6);

            var order = new CustomerOrder(
                "ORDER-001",
                new DateTime(2026, 1, 1),
                OrderPriority.Normal,
                false,
                new[] { firstLine, secondLine });

            var service = new StockAllocationService();


            service.Allocate(
                new[] { order },
                new[] { sku });


            Assert.Equal(4, firstLine.AllocatedQuantity);
            Assert.Equal(6, secondLine.AllocatedQuantity);
            Assert.Equal(10, sku.AllocatedQuantity);
            Assert.Equal(0, sku.AvailableQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                order.Status);
        }
    }
}