using System.Text;
using InventoryMgt.Api.Services;
using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Tests;

public class CsvServiceTests
{
    private readonly CsvService _service = new();

    private static string Decode(byte[] bytes)
    {
        var preamble = Encoding.UTF8.GetPreamble();
        Assert.Equal(preamble, bytes.Take(preamble.Length));
        return Encoding.UTF8.GetString(bytes, preamble.Length, bytes.Length - preamble.Length);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Acme", "Acme")]
    [InlineData("12 Main St, Suite 4", "\"12 Main St, Suite 4\"")]
    [InlineData("The \"Best\" Supplier", "\"The \"\"Best\"\" Supplier\"")]
    [InlineData("Line1\nLine2", "\"Line1\nLine2\"")]
    [InlineData("Line1\r\nLine2", "\"Line1\r\nLine2\"")]
    public void Escape_FollowsRfc4180(string? input, string expected)
    {
        Assert.Equal(expected, CsvService.Escape(input));
    }

    [Fact]
    public void GenerateSupplierCsv_WithNoSuppliers_ReturnsHeaderOnly()
    {
        var csv = Decode(_service.GenerateSupplierCsv([]));

        Assert.Equal(
            "Id,Supplier Name,Contact Person,Email,Phone,Address,City,State,Country,Postal Code,Tax Number,Payment Terms,Is Active\r\n",
            csv);
    }

    [Fact]
    public void GenerateSupplierCsv_WritesOneEscapedRowPerSupplier()
    {
        var suppliers = new[]
        {
            new SupplierReadDto
            {
                Id = 7,
                SupplierName = "Acme, Inc.",
                ContactPerson = "Jo \"JJ\" Doe",
                Email = "jo@acme.test",
                Phone = "+1-555-0100",
                Address = null,
                City = "Springfield",
                PaymentTerms = 30,
                IsActive = true
            },
            new SupplierReadDto { Id = 8, SupplierName = "Beta", PaymentTerms = 0, IsActive = false }
        };

        var lines = Decode(_service.GenerateSupplierCsv(suppliers))
            .Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(3, lines.Length);
        Assert.Equal("7,\"Acme, Inc.\",\"Jo \"\"JJ\"\" Doe\",jo@acme.test,+1-555-0100,,Springfield,,,,,30,Yes", lines[1]);
        Assert.Equal("8,Beta,,,,,,,,,,0,No", lines[2]);
    }
}
