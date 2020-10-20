using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poppin.Interfaces;
using Poppin.Models;

namespace Poppin.Controllers
{
				[Route("api/[controller]")]
				[ApiController]
				public class CategoriesController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public CategoriesController(ILocationService locationService)
								{
            _locationService = locationService;
								}

        [HttpGet]
        public async Task<IActionResult> GetCategoryList()
        {
            var categories = await _locationService.GetCategories();

            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            await _locationService.AddCategory(category);
            return Ok(category);
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            await _locationService.UpdateCategory(category.Slug, category);
            return Ok(category);
        }

        [HttpPut("{slug}")]
        public async Task<IActionResult> UpdateCategory(string slug, Category category)
        {
            await _locationService.UpdateCategory(slug, category);
            return Ok(category);
        }

        [HttpDelete("{slug}")]
        public IActionResult DeleteCategory(string slug)
        {
            return Ok(_locationService.DeleteCategory(slug));
        }
    }
}
