using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OA.API.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace OA.API.Filter
{
    public class ApiResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objresult)
            {
                if (objresult.Value is ApiResponseEntity)
                {
                    return;
                }
                objresult.Value = new ApiResponseEntity()
                {
                    Data = objresult.Value
                };
            }
        }
    }

}
