using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class MultipleSkuAllocationTests
    {
        [Fact]
        // Multiple SKUs containing the same product can be combined to fulfil one order line.
        public void Allocate_WhenProductExistsInMultipleSkus_CombinesStock()
        {
            var location1 = new WarehouseLocation("LOC-001");
            var location2 = new WarehouseLocation("LOC-002");

            var sku1 = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                5,
                location1);

            var sku2 = new StockKeepingUnit(
                "SKU-002",
                "JUICE",
                10,
                location2);

            var orderLine = new OrderLine(
                "LINE-001",
                "JUICE",
                12);

            var order = new CustomerOrder(
                "ORDER-001",
                new DateTime(2026, 1, 1),
                OrderPriority.Normal,
                false,
                new[] { orderLine });

            var service = new StockAllocationService();

            service.Allocate(
                new[] { order },
                new[] { sku1, sku2 });

            Assert.Equal(5, sku1.AllocatedQuantity);
            Assert.Equal(7, sku2.AllocatedQuantity);
            Assert.Equal(12, orderLine.AllocatedQuantity);
            Assert.Equal(0, orderLine.RemainingQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                order.Status);
        }
    }
}