using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.Repositories.File;
using ETicaretAPI.Application.Repositories.InvoiceFile;
using ETicaretAPI.Application.Repositories.ProductImageFile;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public ProductsController(IProductReadRepository productReadRepository,
                                  IProductWriteRepository productWriteRepository,
                                  IWebHostEnvironment webHostEnvironment,
                                  IFileReadRepository fileReadRepository,
                                  IFileWriteRepository fileWriteRepository,
                                  IInvoiceFileReadRepository invoiceFileReadRepository,
                                  IInvoiceFileWriteRepository invoiceFileWriteRepository,
                                  IProductImageFileReadRepository productImageFileReadRepository,
                                  IProductImageFileWriteRepository productImageFileWriteRepository,
                                  IStorageService storageService)
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
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Pagination pagination)
        {
            var totalCount = _productReadRepository.GetAll(false).Count();
            var products = _productReadRepository.GetAll(false).Skip(pagination.Page * pagination.Size).Take(pagination.Size).Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.Stock,
                p.CreatedDate,
                p.UpdatedDate
            }).ToList();
            return Ok(new { totalCount, products });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _productReadRepository.GetByIdAsync(id, false));
        }

        [HttpPost]
        public async Task<IActionResult> Post(VM_Create_Product model)
        {
            if (ModelState.IsValid)
            {

            }
            await _productWriteRepository.AddAsync(new()
            {
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock,
            });
            await _productWriteRepository.SaveAsync();
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
        public async Task<IActionResult> Upload()
        {
            var data = await _storageService.UploadAsync("resource/files", Request.Form.Files);
            //var data = await _fileService.UploadAsync("resource/product-images", Request.Form.Files);
            await _productImageFileWriteRepository.AddRangeAsync(data.Select(d => new ProductImageFile()
            {
                FileName = d.fileName,
                Path = d.path,
                Storage = _storageService.StorageName
            }).ToList());
            await _productImageFileWriteRepository.SaveAsync();

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
    }
}
