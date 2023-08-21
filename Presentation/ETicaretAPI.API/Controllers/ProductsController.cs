using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        public ProductsController(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
        }

        [HttpGet]
        public async Task Get()
        {
            await _productWriteRepository.AddRangeAsync(new()
            {
                new() { Id = 1, Name = "Product 1", Price = 100, Stock = 10, CreatedDate = DateTime.UtcNow},
                new() { Id = 2, Name = "Product 2", Price = 200, Stock = 20, CreatedDate = DateTime.UtcNow},
                new() { Id = 3, Name = "Product 3", Price = 300, Stock = 130, CreatedDate = DateTime.UtcNow},
            });
            await _productWriteRepository.SaveAsync();
        }

        [HttpGet("id")]
        public async Task<IActionResult> Get(int id)
        {
            Product product = await _productReadRepository.GetByIdAsync(id);
            return Ok(product);
        }
    }
}
