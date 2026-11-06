using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class PartialDeliveryTests
    {
        [Fact]

        // Partial delivery is allowed when complete delivery is not required.
        public void Allocate_WhenStockIsInsufficientAndCompleteDeliveryIsNotRequired_AllocatesAvailableStock()
        {

            var location = new WarehouseLocation("LOC-001");

            var sku = new StockKeepingUnit(
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

            service.Allocate(
                new[] { order },
                new[] { sku });


            Assert.Equal(5, sku.AllocatedQuantity);
            Assert.Equal(0, sku.AvailableQuantity);
            Assert.Equal(5, orderLine.AllocatedQuantity);
            Assert.Equal(5, orderLine.RemainingQuantity);
            Assert.Equal(
                OrderStatus.PartiallyAllocated,
                order.Status);
        }
    }
}