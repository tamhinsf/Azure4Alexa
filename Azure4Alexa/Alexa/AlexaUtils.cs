using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Azure4Alexa.Alexa
{
    public class AlexaUtils
    {


        // In a debug environment, you might find inbound request validation to be a nuisance, especially if you're
        // manually generating requests using cURL, Postman or another utility.
        // The #if #endif directives disable validation in DEBUG builds.


        public static bool IsRequestInvalid(Session session)
        {
#if DEBUG
            return false;
#endif

            if (session.Application.Id != AlexaConstants.AppId)
            {
                return true;
            }

            return false;
        }
  
        // a convienence class to pass the multiple components used to build
        // an Alexa response in one object

        public class SimpleIntentResponse
        {
            public string cardText { get; set; } = "";
            public string ssmlString { get; set; } = "";
            public string smallImageUrl { get; set; } = "";
            public string largeImageUrl { get; set; } = "";
        };

        public static string AddSpeakTagsAndClean(string spokenText)
        {
            // remove characters that will cause SSML to break.
            // probably a whole lot of other characters to remove or sanitize.  This is just a lazy start.
            return   "<speak> " + spokenText.Replace("&", "and") + " </speak>";

        }

        public static SpeechletResponse BuildSpeechletResponse(SimpleIntentResponse simpleIntentResponse, bool shouldEndSession)
        {

            SpeechletResponse response = new SpeechletResponse();
            response.ShouldEndSession = shouldEndSession;

            // Create the speechlet response from SimpleIntentResponse.
            // If there's an ssmlString use that as the spoken reply
            // If ssmlString is empty, speak cardText

            if (simpleIntentResponse.ssmlString != "")  
            {
                SsmlOutputSpeech speech = new SsmlOutputSpeech();
                speech.Ssml = simpleIntentResponse.ssmlString;
                response.OutputSpeech = speech;
            }
            else
            {
                PlainTextOutputSpeech speech = new PlainTextOutputSpeech();
                speech.Text = simpleIntentResponse.cardText;
                response.OutputSpeech = speech;
            }


            // if HTTPS images are passed, then assume a standard card is wanted
            // nag: remember the images have to be CORS-accessible and served via HTTP
            
            // JPEG or PNG supported, no larger than 2MB
            // 720x480 - small size recommendation
            // 1200x800 - large size recommendation

            if (simpleIntentResponse.smallImageUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) &&
                simpleIntentResponse.largeImageUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                StandardCard card = new StandardCard();
                card.Title = AlexaConstants.AppName;
                card.Text = simpleIntentResponse.cardText;
                response.Card = card;

                card.Image.SmallImageUrl = simpleIntentResponse.smallImageUrl;
                card.Image.LargeImageUrl = simpleIntentResponse.largeImageUrl;
            }
            else
            {
                SimpleCard card = new SimpleCard();
                card.Title = AlexaConstants.AppName;
                card.Content = simpleIntentResponse.cardText;
                response.Card = card;
            }

            return response;
        }

    }
}