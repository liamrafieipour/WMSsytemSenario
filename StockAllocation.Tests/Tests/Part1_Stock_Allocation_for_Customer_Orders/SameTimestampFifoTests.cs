using StockAllocation.Domain;
using StockAllocation.Services;

namespace StockAllocation.Tests.Tests.Part1_Stock_Allocation_for_Customer_Orders
{
    public class SameTimestampFifoTests
    {
        [Fact]

        // FIFO (First In, First Out) when two orders have the same priority and exactly the same creation date.
        public void Allocate_WhenPriorityAndCreationDateAreEqual_PreservesInputOrder()
        {
            var location = new WarehouseLocation("LOC-001");

            var sku = new StockKeepingUnit(
                "SKU-001",
                "JUICE",
                10,
                location);

            var sameDate = new DateTime(2026, 1, 1);

            // First order 
            var firstLine = new OrderLine(
                "LINE-001",
                "JUICE",
                10);

            var firstOrder = new CustomerOrder(
                "ORDER-002",
                sameDate,
                OrderPriority.Normal,
                false,
                new[] { firstLine });

            // Second order 
            var secondLine = new OrderLine(
                "LINE-002",
                "JUICE",
                10);

            var secondOrder = new CustomerOrder(
                "ORDER-001",
                sameDate,
                OrderPriority.Normal,
                false,
                new[] { secondLine });

            var service = new StockAllocationService();


            service.Allocate(
                new[] { firstOrder, secondOrder },
                new[] { sku });


            Assert.Equal(10, firstLine.AllocatedQuantity);
            Assert.Equal(0, secondLine.AllocatedQuantity);
            Assert.Equal(
                OrderStatus.ReadyForDelivery,
                firstOrder.Status);
            Assert.Equal(
                OrderStatus.Released,
                secondOrder.Status);
        }
    }
}