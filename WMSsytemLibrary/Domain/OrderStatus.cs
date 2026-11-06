namespace StockAllocation.Domain
{
    public enum OrderStatus
    {
        Released,
        PartiallyAllocated,
        ReadyForDelivery,
        Cancelled
    }
}