using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class DuplicateSkuTests
    {
        [Fact]

        // Duplicated SKU IDs must not be accepted by the allocation service.
        public void Allocate_WhenDuplicateSkuIdsExist_ThrowsArgumentException()
        {
            
            var location = new WarehouseLocation("LOC-001");

            var firstSku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                10,
                location);

            var secondSku = new StockKeepingUnit(
                "SKU-001",
                "WATER",
                10,
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


            Assert.Throws<ArgumentException>(() =>
                service.Allocate(
                    new[] { order },
                    new[] { firstSku, secondSku }));
        }
    }
}