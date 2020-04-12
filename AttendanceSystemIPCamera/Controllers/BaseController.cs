using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AttendanceSystemIPCamera.Framework;
using AttendanceSystemIPCamera.Framework.ExeptionHandler;
using Microsoft.Extensions.Logging;

namespace AttendanceSystemIPCamera.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController: ControllerBase
    {
        protected ILogger logger;
        public BaseController(ILogger<BaseController> logger)
        {
            this.logger = logger;
        }
      
        protected BaseResponse<T> ExecuteInMonitoring<T>(Func<T> function)
        {
            try
            {
                dynamic result = function();
                return BaseResponse<T>.GetSuccessResponse(result);
            }
            catch (BaseException ex)
            {
                logger.LogError(ex.ToString());
                var err = new Dictionary<string, IEnumerable<string>>
                {
                    { "General", new List<string> { ex.Message } }
                };
                return BaseResponse<T>.GetErrorResponse(err);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var err = new Dictionary<string, IEnumerable<string>>
                {
                    { "General", new List<string> { ex.ToString() } }
                };
                return BaseResponse<T>.GetErrorResponse(err);
            }
        }

        protected async Task<BaseResponse<T>> ExecuteInMonitoring<T>(Func<Task<T>> function)
        {
            dynamic result;
            try
            {
                result = await function();
            }
            catch (BaseException ex)
            {
                logger.LogError(ex.ToString());
                var err = new Dictionary<string, IEnumerable<string>>
                {
                    { "General", new List<string> { ex.Message } }
                };
                return BaseResponse<T>.GetErrorResponse(err);
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                var err = new Dictionary<string, IEnumerable<string>>
                {
                    { "General", new List<string> { ex.Message } }
                };
                return BaseResponse<T>.GetErrorResponseWithStatusCode(err, ex.StatusCode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                var err = new Dictionary<string, IEnumerable<string>>
                {
                    { "General", new List<string> { ex.ToString() } }
                };
                return BaseResponse<T>.GetErrorResponse(err);
            }
            return BaseResponse<T>.GetSuccessResponse(result);
        }
    }
}
