using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace ContabilidadeApi.Scalar
{
    public class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationScheProvider) : IOpenApiDocumentTransformer
    {
       public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,CancellationToken cancellationToken)
        {
            var scheme = await authenticationScheProvider.GetAllSchemesAsync();
            var requirements = new Dictionary<string, OpenApiSecurityScheme>();
            if (scheme.Any(authScheme => authScheme.Name == "Bearer"))
            {
                
                {
                requirements["Bearer"] = new()
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT",
                };
                }; 
                
            }

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;

            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
            {
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    }] = Array.Empty<string>()
                });
                    
            }
        }
    }
    
}
