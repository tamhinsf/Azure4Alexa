using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Azure4Alexa.Controllers
{
    // Per Alexa requirements, all images need to be served off of a CORS-enabled server
    // and the only image types supported are PNG and JPG

    // This controller restricts CORS to the Alexa server and serves local images
    // stored in the ~/Images/ folder.  Then, the Alexa/AlexaUtils.cs BuildSpeechletResponse class constructs 
    // the full URL to the image for Alexa's consumption

    [EnableCors(origins: "http://ask-ifr-download.s3.amazonaws.com", headers: "*", methods: "get")]
    public class AlexaImagesController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage AlexaImagesSession(string id)
        {
            var imageType = String.Empty;

            if (id.EndsWith(".png"))
            {
                imageType = "image/png";
            }
            else if (id.EndsWith(".jpg") || id.EndsWith(".jpeg"))
            {
                imageType = "image/jpg";
            }
            else
            {
                return null;
            }


            // probably don't need to try catch here, but do it here just
            // in case a missing file causes a problem

            try
            {
                var imageData = File.ReadAllBytes(HostingEnvironment.MapPath("~/Images/") + id);
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(imageData);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(imageType);
                return result;
            }
            catch
            {
                return null;
            }
        }

    }
}
