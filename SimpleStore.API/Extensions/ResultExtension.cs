using Microsoft.AspNetCore.Mvc;
using SimpleStore.Application.Common;
using SimpleStore.Application.Errors;

namespace SimpleStore.API.Extensions
{
    public static class ResultExtension
    {
        public static ActionResult<T> ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            return result.Error!.Type switch
            {
                ErrorType.BadRequest => new BadRequestObjectResult(result.Error),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(result.Error),
                ErrorType.NotFound => new NotFoundObjectResult(result.Error),
                ErrorType.Validation => new ObjectResult(result.Error) { StatusCode = 422 },
                ErrorType.Conflict => new ConflictObjectResult(result.Error),
                _ => new ObjectResult(result.Error) { StatusCode = 500 }
            };
        }

        public static ActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
                return new OkResult();

            return result.Error!.Type switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(result.Error),
                ErrorType.Validation => new BadRequestObjectResult(result.Error),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(result.Error),
                _ => new ObjectResult(result.Error) { StatusCode = 500 }
            };
        }
    }
}
