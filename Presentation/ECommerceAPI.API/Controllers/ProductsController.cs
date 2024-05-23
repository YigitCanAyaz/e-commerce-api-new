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
        private readonly IOrderWriteRepository _orderWriteRepsoitory;
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly ICustomerWriteRepository _customerWriteRepository;

        public ProductsController(IProductWriteRepository productWriteRepsoitory, IProductReadRepository productReadRepsoitory, IOrderWriteRepository orderWriteRepsoitory, IOrderReadRepository orderReadRepository, ICustomerWriteRepository customerWriteRepository)
        {
            _productWriteRepsoitory = productWriteRepsoitory;
            _productReadRepsoitory = productReadRepsoitory;
            _orderWriteRepsoitory = orderWriteRepsoitory;
            _orderReadRepository = orderReadRepository;
            _customerWriteRepository = customerWriteRepository;
        }

        [HttpGet]
        public async Task Get()
        {
            Order order = await _orderReadRepository.GetByIdAsync("c4b549f7-b1b0-4458-b52f-d4ba2006cab9");
            order.Address = "İstanbul";
            await _orderWriteRepsoitory.SaveAsync();
        }
    }
}
