using ECommerceAPI.Application.Abstractions;
using ECommerceAPI.Application.Features.Commands;
using ECommerceAPI.Application.Features.Commands.CreateProduct;
using ECommerceAPI.Application.Features.Queries.GetAllProduct;
using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Application.Repositories.File;
using ECommerceAPI.Application.RequestParameters;
using ECommerceAPI.Application.ViewModels.Products;
using ECommerceAPI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Net;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductWriteRepository _productWriteRepsoitory;
        private readonly IProductReadRepository _productReadRepsoitory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileWriteRepository _fileWriteRepository;
        private readonly IFileReadRepository _fileReadRepository;
        private readonly IProductImageFileReadRepository _productImageFileReadRepository;
        private readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        private readonly IInvoiceFileReadRepository _invoiceFileReadRepository;
        private readonly IInvoiceFileWriteRepository _invoiceFileWriteRepository;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;

        readonly IMediator _mediator;

        public ProductsController(IProductWriteRepository productWriteRepsoitory, IProductReadRepository productReadRepsoitory, IWebHostEnvironment webHostEnvironment, IFileWriteRepository fileWriteRepository, IFileReadRepository fileReadRepository, IProductImageFileReadRepository productImageFileReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IInvoiceFileReadRepository invoiceFileReadRepository, IInvoiceFileWriteRepository invoiceFileWriteRepository, IStorageService storageService, IConfiguration configuration, IMediator mediator)
        {
            _productWriteRepsoitory = productWriteRepsoitory;
            _productReadRepsoitory = productReadRepsoitory;
            _webHostEnvironment = webHostEnvironment;
            _fileWriteRepository = fileWriteRepository;
            _fileReadRepository = fileReadRepository;
            _productImageFileReadRepository = productImageFileReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _invoiceFileReadRepository = invoiceFileReadRepository;
            _invoiceFileWriteRepository = invoiceFileWriteRepository;
            _storageService = storageService;
            _configuration = configuration;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
        {
            GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _productReadRepsoitory.GetByIdAsync(id, false));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
        {

            CreateProductCommandResponse response = await _mediator.Send(createProductCommandRequest);

            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPut]
        public async Task<IActionResult> Put(VM_Update_Product model)
        {
            Product product = await _productReadRepsoitory.GetByIdAsync(model.Id);
            product.Stock = model.Stock;
            product.Name = model.Name;
            product.Price = model.Price;
            await _productWriteRepsoitory.SaveAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _productWriteRepsoitory.RemoveAsync(id);
            await _productWriteRepsoitory.SaveAsync();
            return Ok();
        }

        [HttpPost("[action]")] // api/products?id=123
        public async Task<IActionResult> Upload(string id)
        {
            List<(string fileName, string pathOrContainerName)> result = await _storageService.UploadAsync("photo-images", Request.Form.Files);

            Product product = await _productReadRepsoitory.GetByIdAsync(id);

            //foreach (var r in result)
            //{
            //    product.ProductImageFiles.Add(new())
            //        {
            //        FileName = r.fileName,
            //            Path = r.pathOrContainerName,
            //            Storage = _storageService.StorageName,
            //            Products = new List<Product>() { product }
            //        }).ToList());
            //}
            //}

            await _productImageFileWriteRepository.AddRangeAsync(result.Select(r => new ProductImageFile
            {
                FileName = r.fileName,
                Path = r.pathOrContainerName,
                Storage = _storageService.StorageName,
                Products = new List<Product>() { product }
            }).ToList());

            await _productImageFileWriteRepository.SaveAsync();

            return Ok();
        }

        [HttpGet("[action]/{id}")] // api/products/123
        public async Task<IActionResult> GetProductImages(string id)
        {
            Product? product = await _productReadRepsoitory
                .Table.Include(p => p.ProductImageFiles)
                .FirstOrDefaultAsync(P => P.Id == Guid.Parse(id));

            //return Ok(product.ProductImageFiles.Select(p => new
            //{
            //    p.Path,
            //    p.FileName
            //}));

            return Ok(product.ProductImageFiles.Select(p => new
            {
                Path = $"{_configuration["BaseStorageUrl"]}/{p.Path}",
                p.FileName,
                p.Id
            }));
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteProductImage(string id, string imageId)
        {
            Product? product = await _productReadRepsoitory
               .Table.Include(p => p.ProductImageFiles)
               .FirstOrDefaultAsync(P => P.Id == Guid.Parse(id));

            ProductImageFile productImageFile = product.ProductImageFiles.FirstOrDefault(p => p.Id == Guid.Parse(imageId));

            product.ProductImageFiles.Remove(productImageFile);
            await _productWriteRepsoitory.SaveAsync();

            return Ok();
        }
    }
}
