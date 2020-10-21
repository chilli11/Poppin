using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poppin.Contracts.Requests;
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
            var updated = new List<Category>();

            if (!string.IsNullOrEmpty(category.Parent))
												{
                var parent = await _locationService.GetCategoryBySlug(category.Parent);
                if (parent != null)
																{
                    parent.Children.Add(category.Slug);
																}
                updated.Add(parent);
												}

            if (category.Children != null && category.Children.Count > 0)
            {
                var children = await _locationService.GetCategoriesBySlug(category.Children);
                if (children != null)
																{
                    children.ForEach(c => c.Parent = category.Slug);
                    updated.AddRange(children);
																}
            }

            _locationService.UpdateCategories(updated);
            await _locationService.AddCategory(category);
            return Ok(new { Category = category, Updated = updated });
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryRequest request)
        {
            var updated = new List<Category>();
            if (request.Original.Parent != request.Category.Parent)
												{
                var parent = await _locationService.GetCategoryBySlug(request.Category.Parent);
                if (parent != null)
                {
                    parent.Children.Add(request.Category.Slug);
                }
                updated.Add(parent);
            }
            if (request.Original.Children != request.Category.Children)
												{
                var oC = request.Original.Children;
                var nC = request.Category.Children;
                var removed = oC;
                var added = nC;
                var allChildren = new List<Category>();

                if (oC != null && oC.Count > 0 && nC != null && nC.Count > 0)
                {
                    removed = oC.Except(nC).ToHashSet();
                    added = nC.Except(oC).ToHashSet();
                }
                if (removed != null && removed.Count > 0)
                {
                    var removedChildren = (removed != null && removed.Count > 0) ? await _locationService.GetCategoriesBySlug(removed) : null;
                    if (removedChildren != null)
                    {
                        removedChildren.ForEach(c => c.Parent = null);
                        allChildren.AddRange(removedChildren);
                    }

                }
                if (added != null && added.Count > 0)
                {
                    var addedChildren = (added != null && added.Count > 0) ? await _locationService.GetCategoriesBySlug(added) : null;
                    if (addedChildren != null)
                    {
                        addedChildren.ForEach(c => c.Parent = request.Category.Parent);
                        allChildren.AddRange(addedChildren);
                    }
                }
                updated.AddRange(allChildren);
            }
            
            _locationService.UpdateCategories(updated);
            await _locationService.UpdateCategory(request.Original.Slug, request.Category);
            return Ok(request.Category);
        }

        [HttpDelete("{slug}")]
        public IActionResult DeleteCategory(string slug)
        {
            return Ok(_locationService.DeleteCategory(slug));
        }
    }
}
