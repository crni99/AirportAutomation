using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AirportАutomation.Api.Helpers
{
	public class JsonPatchDocumentFilter : IDocumentFilter
	{
		public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
		{
			var schemas = swaggerDoc.Components.Schemas.ToList();
			foreach (var item in schemas)
			{
				if (item.Key.StartsWith("Operation") || item.Key.StartsWith("JsonPatchDocument"))
					swaggerDoc.Components.Schemas.Remove(item.Key);
			}

			swaggerDoc.Components.Schemas.Add("Operation", new OpenApiSchema
			{
				Type = "object",
				Properties = new System.Collections.Generic.Dictionary<string, OpenApiSchema>
				{
					{"op", new OpenApiSchema{ Type = "string" } },
					{"value", new OpenApiSchema{ Type = "string"} },
					{"path", new OpenApiSchema{ Type = "string" } }
				}
			});

			swaggerDoc.Components.Schemas.Add("JsonPatchDocument", new OpenApiSchema
			{
				Type = "array",
				Items = new OpenApiSchema
				{
					Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "Operation" }
				},
				Description = "Array of operations to perform"
			});

			// Loop through each path and operation for each API version
			foreach (var apiDescription in context.ApiDescriptions)
			{
				var relativePath = apiDescription.RelativePath;
				if (swaggerDoc.Paths.ContainsKey(relativePath))
				{
					var pathItem = swaggerDoc.Paths[relativePath];
					foreach (var operation in pathItem.Operations)
					{
						if (operation.Key == OperationType.Patch)
						{
							foreach (var item in operation.Value.RequestBody.Content.Where(c => c.Key != "application/json-patch+json").ToList())
								operation.Value.RequestBody.Content.Remove(item.Key);

							var response = operation.Value.RequestBody.Content.FirstOrDefault(c => c.Key == "application/json-patch+json");
							if (response.Value != null)
							{
								response.Value.Schema = new OpenApiSchema
								{
									Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "JsonPatchDocument" }
								};
							}
						}
					}
				}
			}
		}
	}
}