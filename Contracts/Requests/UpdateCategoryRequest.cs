using Poppin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests
{
				public class UpdateCategoryRequest
				{
								public Category Category { get; set; }
								public Category Original { get; set; }
				}
}
