using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class SharedSkuAllocationTests
    {
        [Fact]

        // The same SKU can be shared across multiple customer orders.
        public void Allocate_WhenMultipleOrdersUseSameSku_SharesAvailableStock()
        {
            var location = new WarehouseLocation("LOC-001");

            var sku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                15,
                location);

            var firstOrderLine = new OrderLine(
                "LINE-001",
                "JUICE",
                5);

            var secondOrderLine = new OrderLine(
                "LINE-002",
                "JUICE",
                7);

            var firstOrder = new CustomerOrder(
                "ORDER-001",
                new DateTime(2026, 1, 1),
                OrderPriority.Normal,
                false,
                new[] { firstOrderLine });

            var secondOrder = new CustomerOrder(
                "ORDER-002",
                new DateTime(2026, 1, 2),
                OrderPriority.Normal,
                false,
                new[] { secondOrderLine });

            var service = new StockAllocationService();


            service.Allocate(
                new[] { firstOrder, secondOrder },
                new[] { sku });


            Assert.Equal(12, sku.AllocatedQuantity);
            Assert.Equal(3, sku.AvailableQuantity);
            Assert.Equal(5, firstOrderLine.AllocatedQuantity);
            Assert.Equal(7, secondOrderLine.AllocatedQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                firstOrder.Status);

            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                secondOrder.Status);
        }
    }
}