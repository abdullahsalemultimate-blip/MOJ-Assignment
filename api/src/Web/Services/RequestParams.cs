using InventorySys.Application.Common.Interfaces;

namespace InventorySys.Web.Services;

public class RequestParams : IRequestParams
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestParams(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? RemoteIpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

}
