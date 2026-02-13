using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Core.Filters;

namespace SimpleAPI.Core.Attributes;

public class ApiKeyAttribute : ServiceFilterAttribute
{
    public ApiKeyAttribute()
        : base(typeof(ApiKeyAuthorizationFilter))
    {
    }
}