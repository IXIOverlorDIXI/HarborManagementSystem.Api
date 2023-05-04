using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Extensions
{
    public static class IdentityResultExtensions
    {
        public static void ToModelState(this IdentityResult validationResult, ModelStateDictionary modelState)
        {
            foreach (var error in validationResult.Errors)
            {
                modelState.AddModelError("password", error.Description);
            }
        }
    }
}