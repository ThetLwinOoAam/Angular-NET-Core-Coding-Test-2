using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using WebApi.Helpers;
using WebApi.Models.Tokens;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppBaseController : Controller
    {
        public TokenData TokenData { get; internal set; }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            StringValues stringValues = base.Request.Headers["Authorization"];
            AccessTokenConfig tokenConfig = context.HttpContext.RequestServices.GetRequiredService<AccessTokenConfig>();
            var token = stringValues.ToString().Replace("Bearer ","");

            var tokenData = AccessTokenHelper.GetTokenData(token, tokenConfig);
            if (tokenData == null)
            {
                throw new UnauthorizedAccessException();
            }

            TokenData = tokenData;
        }
    }
}
