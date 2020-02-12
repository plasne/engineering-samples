
using Microsoft.AspNetCore.Mvc;

namespace common
{

    public static class ActionResultExtensions
    {

        public static int StatusCode<T>(this ActionResult<T> actionResult, int dflt = 0)
        {
            var objectResult = actionResult.Result as ObjectResult;
            int? code = objectResult?.StatusCode;
            if (code == null) return dflt;
            return (int)code;
        }

        public static T Body<T>(this ActionResult<T> actionResult)
        {
            var objectResult = actionResult.Result as ObjectResult;
            return (T)objectResult?.Value;
        }

    }

}