using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IQVIA.ClientRequestor;
namespace IQVIA.Controllers
{
    public class SwaggersController : ApiController
    {
        [Authorize]
        [HttpGet]
        public async System.Threading.Tasks.Task<IEnumerable<Swagger>> GetSwaggerAsync(string startDate, string endDate)
        {
            Client.SetURI("https://badapi.iqvia.io/api/v1/Tweets?");
            return await Client.GetSwaggerAsync(startDate, endDate);
        }
    }
}
