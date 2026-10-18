using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Shared.Services;

public interface ICsvService
{
    /// <summary>
    /// Generates a CSV file for supplier records
    /// </summary>
    /// <param name="suppliers">List of supplier records to include in the file</param>
    /// <returns>UTF-8 encoded CSV (with BOM, so Excel detects the encoding) as byte array</returns>
    byte[] GenerateSupplierCsv(IEnumerable<SupplierReadDto> suppliers);
}
