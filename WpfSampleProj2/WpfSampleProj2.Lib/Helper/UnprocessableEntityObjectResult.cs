using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using WpfSampleProj2.Lib.Extensions;

namespace WpfSampleProj2.Lib.Helper
{
    public class UnprocessableEntityObjectResult:ObjectResult
    {
        public UnprocessableEntityObjectResult(ModelStateDictionary modelstate):base(new SerializableError(modelstate))
        {
            if(modelstate.IsNull())
            {
                throw new ArgumentNullException(nameof(modelstate));
            }

            StatusCode = 422;
        }
    }
}
