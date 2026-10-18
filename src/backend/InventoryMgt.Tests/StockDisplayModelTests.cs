using System.Text.Json;
using InventoryMgt.Shared.Constants;
using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Tests;

public class StockDisplayModelTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(4.5)]
    [InlineData(5)]
    public void LowStock_IsTrue_WhenQuantityIsAtOrBelowThreshold(decimal quantity)
    {
        var stock = new StockDisplayModel { Quantity = quantity };

        Assert.True(stock.LowStock);
    }

    [Theory]
    [InlineData(5.01)]
    [InlineData(6)]
    [InlineData(100)]
    public void LowStock_IsFalse_WhenQuantityIsAboveThreshold(decimal quantity)
    {
        var stock = new StockDisplayModel { Quantity = quantity };

        Assert.False(stock.LowStock);
    }

    [Fact]
    public void LowStockThreshold_IsFive()
    {
        Assert.Equal(5m, StockThresholds.LowStock);
    }

    [Fact]
    public void LowStock_IsIncludedInSerializedApiResponse()
    {
        var stock = new StockDisplayModel { Id = 1, ProductId = 10, Quantity = 3, ProductName = "Book" };

        // same camelCase naming the API uses for JSON responses
        var json = JsonSerializer.Serialize(stock, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        using var doc = JsonDocument.Parse(json);

        Assert.True(doc.RootElement.GetProperty("lowStock").GetBoolean());
        Assert.Equal(3m, doc.RootElement.GetProperty("quantity").GetDecimal());
    }
}
