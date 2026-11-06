using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class StockAllocationServiceTests
    {
        [Fact]

        // If enough stock is available, the order should receive the requested quantity.
        public void Allocate_WhenEnoughStockIsAvailable_AllocatesStock()
        {
            // Create warehouse location
            var location = new WarehouseLocation("LOC-001");

            // Create SKU with 20 units
            var sku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                20,
                location);

            // Create customer order for 10 units
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

            // Act: Allocate stock
            service.Allocate(
                new[] { order },
                new[] { sku });

            // Check allocation
            Assert.Equal(10, sku.AllocatedQuantity);
            Assert.Equal(10, sku.AvailableQuantity);
            Assert.Equal(10, orderLine.AllocatedQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                order.Status);
        }
    }
}