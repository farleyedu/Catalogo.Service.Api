using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Catalogo.Service.Api;

/// <summary>
/// Exception Handler.
/// </summary>
public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    /// <summary>
    /// Ordem.
    /// </summary>
    public int Order => int.MaxValue - 10;

    /// <summary>
    /// Ao executar.
    /// </summary>
    /// <param name="context">Contexto.</param>
    public void OnActionExecuting(ActionExecutingContext context) { }

    /// <summary>
    /// Ao finalizar a ação.
    /// </summary>
    /// <param name="context">Contexto.</param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null)
        {
            context.Result = new BadRequestObjectResult(Util.ControllerError(context.Exception));
            context.ExceptionHandled = true;
        }
    }
}
