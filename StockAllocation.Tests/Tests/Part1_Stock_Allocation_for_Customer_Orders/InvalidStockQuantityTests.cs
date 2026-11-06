using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class InvalidStockQuantityTests
    {
        [Fact]

        // A SKU with zero stock can be valid warehouse data, but a SKU with a negative quantity should be rejected.
        public void CreateSku_WhenQuantityIsNegative_ThrowsArgumentException()
        {
            var location = new WarehouseLocation("LOC-001");

            Assert.Throws<ArgumentException>(() =>
                new StockKeepingUnit(
                    "SKU-001",
                    "JUICE",
                    -5,
                    location));
        }

        [Fact]
        public void Allocate_WhenSkuQuantityIsZero_DoesNotAllocateStock()
        {
            var location = new WarehouseLocation("LOC-001");

            var sku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                0,
                location);

            var orderLine = new OrderLine(
                "LINE-001",
                "JUICE",
                5);

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
            Assert.Equal(0, sku.AvailableQuantity);
            Assert.Equal(0, orderLine.AllocatedQuantity);
            Assert.Equal(
                OrderStatus.Released,
                order.Status);
        }
    }
}