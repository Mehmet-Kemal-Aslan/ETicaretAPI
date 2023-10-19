using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Features.Commands.CreateProduct;
using ETicaretAPI.Application.Features.Queries.GetAllProducts;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.Repositories.File;
using ETicaretAPI.Application.Repositories.InvoiceFile;
using ETicaretAPI.Application.Repositories.ProductImageFile;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        readonly IFileReadRepository _fileReadRepository;
        readonly IFileWriteRepository _fileWriteRepository;
        readonly IProductImageFileReadRepository _productImageFileReadRepository;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        readonly IInvoiceFileReadRepository _invoiceFileReadRepository;
        readonly IInvoiceFileWriteRepository _invoiceFileWriteRepository;
        readonly IStorageService _storageService;


        readonly IMediator _mediator;
        public ProductsController(IProductReadRepository productReadRepository,
                                  IProductWriteRepository productWriteRepository,
                                  IWebHostEnvironment webHostEnvironment,
                                  IFileReadRepository fileReadRepository,
                                  IFileWriteRepository fileWriteRepository,
                                  IInvoiceFileReadRepository invoiceFileReadRepository,
                                  IInvoiceFileWriteRepository invoiceFileWriteRepository,
                                  IProductImageFileReadRepository productImageFileReadRepository,
                                  IProductImageFileWriteRepository productImageFileWriteRepository,
                                  IStorageService storageService,
                                  IMediator mediator)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            this._webHostEnvironment = webHostEnvironment;
            _fileReadRepository = fileReadRepository;
            _fileWriteRepository = fileWriteRepository;
            _invoiceFileReadRepository = invoiceFileReadRepository;
            _invoiceFileWriteRepository = invoiceFileWriteRepository;
            _productImageFileReadRepository = productImageFileReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _storageService = storageService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllProductsQueryRequest getAllProductsQueryRequest)
        {
            GetAllProductsQueryResponse response = await _mediator.Send(getAllProductsQueryRequest);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _productReadRepository.GetByIdAsync(id, false));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
        {
            CreateProductCommandResponse response = await _mediator.Send(createProductCommandRequest);
            return Ok((int)HttpStatusCode.Created);
        }


        [HttpPut]
        public async Task<IActionResult> Put(VM_Update_Product model)
        {
            Product product = await _productReadRepository.GetByIdAsync(model.Id);
            product.Stock = model.Stock;
            product.Price = model.Price;
            product.Name = model.Name;
            await _productWriteRepository.SaveAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(int id)
        {
            List<(string fileName, string pathOrContainerName)> result = await _storageService.UploadAsync("photo-images", Request.Form.Files);

            Product product = await _productReadRepository.GetByIdAsync(id);
            
            await _productImageFileWriteRepository.AddRangeAsync(result.Select(r => new ProductImageFile
            {
                FileName = r.fileName,
                Path = r.pathOrContainerName,
                Storage = _storageService.StorageName,
                Products = new List<Product>() { product }
            }).ToList());

            await _productImageFileWriteRepository.SaveAsync();

            //var data = await _storageService.UploadAsync("files", Request.Form.Files);
            ////var data = await _fileService.UploadAsync("resource/product-images", Request.Form.Files);
            //await _productImageFileWriteRepository.AddRangeAsync(data.Select(d => new ProductImageFile()
            //{
            //    FileName = d.fileName,
            //    Path = d.pathOrContainerName,
            //    Storage = _storageService.StorageName
            //}).ToList());
            //await _productImageFileWriteRepository.SaveAsync();

            //var data = await _fileService.UploadAsync("resource/invoice", Request.Form.Files);
            //await _invoiceFileWriteRepository.AddRangeAsync(data.Select(d => new InvoiceFile()
            //{
            //    FileName = d.fileName,
            //    Path = d.path,
            //    Price = new Random().Next()
            //}).ToList()); ;
            //await _invoiceFileWriteRepository.SaveAsync();

            //var data = await _fileService.UploadAsync("resource/files", Request.Form.Files);
            //await _fileWriteRepository.AddRangeAsync(data.Select(d => new ETicaretAPI.Domain.Entities.File()
            //{
            //    FileName = d.fileName,
            //    Path = d.path
            //}).ToList()); ;
            //await _invoiceFileWriteRepository.SaveAsync();
            return Ok();
            //}
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetProductImages(int id)
        {
            Product ? product = await _productReadRepository.Table.Include(p => p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                Console.WriteLine($"Product Id: {product.Id}");
            }
            else
            {
                Console.WriteLine("Product not found");
            }
            return Ok(product.ProductImageFiles.Select(p => new
            {
                p.Path,
                p.FileName
            }));
        }



    }
}
