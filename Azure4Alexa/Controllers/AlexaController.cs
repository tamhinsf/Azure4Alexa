using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;

namespace Azure4Alexa.Controllers
{
    public class AlexaController : ApiController
    {
        [Route("alexa/alexa-session")]
        [HttpPost]
        public async Task<HttpResponseMessage> AlexaSession()
        {
            var alexaSpeechletAsync = new Alexa.AlexaSpeechletAsync();
            return await alexaSpeechletAsync.GetResponseAsync(Request);
        }
    }
}

