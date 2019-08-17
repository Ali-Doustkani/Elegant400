using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Elegant400.Validation
{
   internal class NullObjectModelValidator : IObjectModelValidator
   {
      public void Validate(ActionContext actionContext,
         ValidationStateDictionary validationState,
         string prefix,
          object model)
      { }
   }
}
