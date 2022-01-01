using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using React_ASPNETCORE_JWT_Auth.Models;

namespace React_ASPNETCORE_JWT_Auth.Filters
{
    /// <summary>
    /// Filter used to format the ModelState errors in a custom format and send them back in the API response if ModelState is invalid.
    /// Applied globally but will only work for the Controllers which have [ApiController] attribute on them.
    /// </summary>
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var isApiController = context.ActionDescriptor.EndpointMetadata.OfType<ApiControllerAttribute>().Any();
           
            if (isApiController)
            {
                if (!context.ModelState.IsValid)
                {
                    var errorsInModelState = context.ModelState.Where(x => x.Value?.Errors.Count > 0)
                                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage))
                                                    .ToArray();

                    var errorResponse = new BaseResponseModel()
                    {
                        IsSuccess = false,
                        Errors = new List<string>(),
                        UserFriendlyErrors = new List<ErrorModel>()
                    };

                    foreach (var error in errorsInModelState)
                    {
                        if(error.Value != null)
                        {
                            foreach (var subError in error.Value)
                            {
                                var errorModel = new ErrorModel()
                                {
                                    Property = error.Key,
                                    Error = subError
                                };

                                errorResponse.UserFriendlyErrors.Add(errorModel);
                            }
                        }
                    }

                    context.Result = new BadRequestObjectResult(errorResponse);

                    return;  //return from here as we don't want to execute the Action Method if ModelState is Invalid
                }
            }

            //before controller
            await next();
            //after controller

        }
    }
}
