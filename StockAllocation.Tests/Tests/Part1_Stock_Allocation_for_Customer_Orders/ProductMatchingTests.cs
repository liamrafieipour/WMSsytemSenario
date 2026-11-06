using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class ProductMatchingTests
    {
        [Fact]

        // Stock must only be allocated to an order line with the matching product number.
        public void Allocate_WhenProductNumbersDoNotMatch_DoesNotAllocateStock()
        {
            var location = new WarehouseLocation("LOC-001");

            var sku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                20,
                location);

            var orderLine = new OrderLine(
                "LINE-001",
                "WATER",
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


            Assert.Equal(0, sku.AllocatedQuantity);
            Assert.Equal(20, sku.AvailableQuantity);
            Assert.Equal(0, orderLine.AllocatedQuantity);
            Assert.Equal(10, orderLine.RemainingQuantity);
            Assert.Equal(
                OrderStatus.Released,
                order.Status);
        }
    }
}