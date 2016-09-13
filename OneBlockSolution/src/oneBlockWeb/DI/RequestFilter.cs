using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace oneBlockWeb.DI
{
    /// <summary>
    /// 启用或禁用某些功能
    /// </summary>
    public class RequestFilter
    {
        private readonly RequestDelegate _next;

        private readonly WebSetting _setting;

        public RequestFilter(RequestDelegate next, IOptions<WebSetting> settings)
        {
            _next = next;
            _setting = settings.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!_setting.Register && context.Request.Path == "/Account/Register")
            {
                return;
            }
            await _next.Invoke(context);
        }
    }
}
