using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductWriteRepository _productWriteRepsoitory;
        private readonly IProductReadRepository _productReadRepsoitory;

        public ProductsController(IProductWriteRepository productWriteRepsoitory, IProductReadRepository productReadRepsoitory)
        {
            _productWriteRepsoitory = productWriteRepsoitory;
            _productReadRepsoitory = productReadRepsoitory;
        }

        [HttpGet]
        public async Task Get()
        {
            //await _productWriteRepsoitory.AddRangeAsync(new()
            // {
            //    new() {Id = Guid.NewGuid(), Name = "Product 1", Price = 100, CreatedDate = DateTime.UtcNow, Stock = 10},
            //    new() {Id = Guid.NewGuid(), Name = "Product 2", Price = 200, CreatedDate = DateTime.UtcNow, Stock = 20},
            //    new() {Id = Guid.NewGuid(), Name = "Product 3", Price = 300, CreatedDate = DateTime.UtcNow, Stock = 30},
            //});

            //await _productWriteRepsoitory.SaveAsync();

            Product p = await _productReadRepsoitory.GetByIdAsync("8151ab3c-2166-4c99-b78f-8c3beff54ec9", false);
            p.Name = "Mehmet";
            await _productWriteRepsoitory.SaveAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            Product product = await _productReadRepsoitory.GetByIdAsync(id);
            return Ok(product);
        }
    }
}
