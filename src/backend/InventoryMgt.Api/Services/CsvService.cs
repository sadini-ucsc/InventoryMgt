using System.Globalization;
using System.Text;
using InventoryMgt.Shared.DTOs;
using InventoryMgt.Shared.Services;

namespace InventoryMgt.Api.Services;

public class CsvService : ICsvService
{
    private static readonly string[] SupplierHeaders =
    [
        "Id", "Supplier Name", "Contact Person", "Email", "Phone", "Address", "City",
        "State", "Country", "Postal Code", "Tax Number", "Payment Terms", "Is Active"
    ];

    public byte[] GenerateSupplierCsv(IEnumerable<SupplierReadDto> suppliers)
    {
        var sb = new StringBuilder();
        AppendRow(sb, SupplierHeaders);

        foreach (var s in suppliers)
        {
            AppendRow(sb,
            [
                s.Id.ToString(CultureInfo.InvariantCulture),
                s.SupplierName,
                s.ContactPerson,
                s.Email,
                s.Phone,
                s.Address,
                s.City,
                s.State,
                s.Country,
                s.PostalCode,
                s.TaxNumber,
                s.PaymentTerms.ToString(CultureInfo.InvariantCulture),
                s.IsActive ? "Yes" : "No"
            ]);
        }

        // BOM so Excel opens non-ASCII names (e.g. accented characters) correctly
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        return [.. encoding.GetPreamble(), .. encoding.GetBytes(sb.ToString())];
    }

    private static void AppendRow(StringBuilder sb, IEnumerable<string?> values)
    {
        sb.Append(string.Join(",", values.Select(Escape)));
        sb.Append("\r\n"); // RFC 4180 line ending
    }

    // RFC 4180: wrap in quotes when the value contains a delimiter, quote or line break; double inner quotes
    public static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (value.IndexOfAny([',', '"', '\r', '\n']) >= 0)
            return $"\"{value.Replace("\"", "\"\"")}\"";

        return value;
    }
}
