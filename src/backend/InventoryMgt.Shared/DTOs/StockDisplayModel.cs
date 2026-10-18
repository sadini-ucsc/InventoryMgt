namespace InventoryMgt.Shared.DTOs;

using InventoryMgt.Shared.Constants;

public class StockDisplayModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    // computed from Quantity, not stored in the database
    public bool LowStock => Quantity <= StockThresholds.LowStock;
}

public class PaginatedStock
{
    public IEnumerable<StockDisplayModel> Stocks { get; set; } = [];
    public PaginationBase Pagination { get; set; } = null!;
}
