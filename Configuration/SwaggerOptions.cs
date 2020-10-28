using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Configuration
{
				public class SwaggerOptions
				{
								public string JsonRoute { get; set; }
								public string Description { get; set; }
								public string UIEndpoint { get; set; }
				}
    public class SwaggerOperationFilter : IOperationFilter
    {
        private readonly IEnumerable<string> objectIdIgnoreParameters = new[]
        {
            nameof(ObjectId.Timestamp),
            nameof(ObjectId.Machine),
            nameof(ObjectId.Pid),
            nameof(ObjectId.Increment),
            nameof(ObjectId.CreationTime)
        };

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters = operation.Parameters.Where(x =>
                x.In != ParameterLocation.Query || objectIdIgnoreParameters.Contains(x.Name) == false
            ).ToList();
        }
    }
}
