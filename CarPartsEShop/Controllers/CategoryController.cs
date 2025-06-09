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
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var result = await _categoryService.GetAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST api/<CategoryController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Category category)
        {
            var result = await _categoryService.Add(category);

            return Ok(result);
        }

        // PUT api/<CategoryController>/5 COS TU NIE BANGLA XD
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Category category)
        {
            var result = await _categoryService.Update(category);

            return Ok(result);
        }
    }
}
