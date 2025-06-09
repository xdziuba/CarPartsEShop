using Microsoft.AspNetCore.Mvc;
using CarPartsEShop.Repositories;
using System;
using CarPartsEShop.Services;
using CarPartsEShop.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarPartsEShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/<ProductController>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _productService.GetAllAsync();
            return Ok(result);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var result = await _productService.GetAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProductDto productDto)
        {
            var category = await _productService.GetCategoryByIdAsync(productDto.CategoryId);
            if (category == null)
            {
                return BadRequest("Category not found");
            }
            else if (category.Deleted)
            {
                return BadRequest("Category is deleted");
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Ean = productDto.Ean,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryId = category.Id,
                Category = category,
                Deleted = productDto.Deleted
            };

            var result = await _productService.Add(product);

            return Ok(result);
        }

        // PUT api/<ProductController>/5 COS TU NIE BANGLA XD
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Product product)
        {
            var result = await _productService.Update(product);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _productService.GetAsync(id);
            product.Deleted = true;
            var result = await _productService.Update(product);

            return Ok(result);
        }
    }
}
