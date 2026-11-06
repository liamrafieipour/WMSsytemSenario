using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class LockedAndUnlockedLocationTests
    {
        [Fact]

        // When the same product exists in both locked and unlocked locations, the system must use stock only from the unlocked location.
        public void Allocate_WhenLockedAndUnlockedStockExists_UsesOnlyUnlockedStock()
        {
            var lockedLocation = new WarehouseLocation("LOC-001", true);
            var unlockedLocation = new WarehouseLocation("LOC-002");

            var lockedSku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                10,
                lockedLocation);

            var unlockedSku = new StockKeepingUnit(
                "SKU-002",
                "JUICE",
                10,
                unlockedLocation);

            var orderLine = new OrderLine(
                "LINE-001",
                "JUICE",
                8);

            var order = new CustomerOrder(
                "ORDER-001",
                new DateTime(2026, 1, 1),
                OrderPriority.Normal,
                false,
                new[] { orderLine });

            var service = new StockAllocationService();


            service.Allocate(
                new[] { order },
                new[] { lockedSku, unlockedSku });


            Assert.Equal(0, lockedSku.AllocatedQuantity);
            Assert.Equal(10, lockedSku.AvailableQuantity);
            Assert.Equal(8, unlockedSku.AllocatedQuantity);
            Assert.Equal(2, unlockedSku.AvailableQuantity);
            Assert.Equal(8, orderLine.AllocatedQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                order.Status);
        }
    }
}