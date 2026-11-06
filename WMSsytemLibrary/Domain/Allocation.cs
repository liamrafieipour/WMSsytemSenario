namespace StockAllocation.Domain
{
    public class Allocation
    {
        public string SkuId { get; }
        public int Quantity { get; }

        public Allocation(string skuId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(skuId))
                throw new ArgumentException("SKU ID is required.");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            SkuId = skuId;
            Quantity = quantity;
        }
    }
}