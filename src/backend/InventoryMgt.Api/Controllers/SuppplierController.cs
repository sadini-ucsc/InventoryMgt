using System.Text.Json;
using InventoryMgt.Shared.CustomExceptions;
using InventoryMgt.Shared.DTOs;
using InventoryMgt.Shared.Repositories;
using InventoryMgt.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMgt.Api.Controllers;

[Authorize]
[ApiController]
[Route("/api/suppliers")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierRepository _supplierRepo;
    private readonly ICsvService _csvService;

    // GetSuppliersAsnc caps limit at 100, so the export fetches page by page
    private const int ExportPageSize = 100;

    public SupplierController(ISupplierRepository supplierRepo, ICsvService csvService)
    {
        _supplierRepo = supplierRepo;
        _csvService = csvService;
    }

    [HttpPost]
    public async Task<IActionResult> AddSupplier(SupplierDto supplier)
    {
        var createdSupplier = await _supplierRepo.AddSupplierAsync(supplier);
        return CreatedAtAction(nameof(GetSupplier), new { id = createdSupplier.Id }, createdSupplier);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] SupplierDto supplier)
    {
        if (id != supplier.Id)
            throw new BadRequestException("id in uri and body does not match");

        var existingSupplier = await _supplierRepo.GetSupplierByIdAsnc(id);
        if (existingSupplier == null)
            throw new NotFoundException($"Supplier with id : {id} does not found");

        await _supplierRepo.UpdateSupplierAsync(supplier);

        // Get updated supplier to return
        var updatedSupplier = await _supplierRepo.GetSupplierByIdAsnc(id);
        return Ok(updatedSupplier);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var existingSupplier = await _supplierRepo.GetSupplierByIdAsnc(id);
        if (existingSupplier == null)
            throw new NotFoundException($"Supplier with id : {id} does not found");

        await _supplierRepo.DeleteSupplierAsnc(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplier(int id)
    {
        var supplier = await _supplierRepo.GetSupplierByIdAsnc(id);
        if (supplier == null)
            throw new NotFoundException($"Supplier with id : {id} does not found");

        return Ok(supplier);
    }

    [HttpGet()]
    public async Task<IActionResult> GetSuppliers(int page = 1, int limit = 4, string? searchTerm = null, string? sortColumn = null, string? sortDirection = null)
    {
        var supplierResult = await _supplierRepo.GetSuppliersAsnc(page, limit, searchTerm, sortColumn, sortDirection);
        var suppliers = supplierResult.Suppliers;

        var paginationHeader = new
        {
            supplierResult.TotalRecords,
            supplierResult.TotalPages,
            supplierResult.Page,
            supplierResult.Limit,
        };

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));
        return Ok(suppliers);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportSuppliers(string? searchTerm = null, string? sortColumn = null, string? sortDirection = null)
    {
        // Export all matching suppliers (no pagination), honouring the current search and sort
        var suppliers = new List<SupplierReadDto>();
        var page = 1;
        PagedSupplier result;
        do
        {
            result = await _supplierRepo.GetSuppliersAsnc(page, ExportPageSize, searchTerm, sortColumn, sortDirection);
            suppliers.AddRange(result.Suppliers);
            page++;
        } while (page <= result.TotalPages);

        var csvBytes = _csvService.GenerateSupplierCsv(suppliers);
        var fileName = $"Suppliers_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        return File(csvBytes, "text/csv", fileName);
    }
}