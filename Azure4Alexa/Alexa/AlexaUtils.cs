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
            // in production, you'd probably want to do the check as follows:

            //if (session.Application.Id != AlexaConstants.AppId)

            // In development, you might have a single instance of this service and multiple different Alexa skills pointing 
            // at it.  Structuring the AppId check in this manner lets you have a single string (AlexaConstants.AppId) containing all your
            // valid AppIds.  Lazy, but it works.

            if (!AlexaConstants.AppId.Contains(session.Application.Id))
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
            public string smallImage { get; set; } = "";
            public string largeImage { get; set; } = "";
            public string musicUrl { get; set; } = "";
            public string msgId { get; set; } = "";
            public string musicAction { get; set; } = "";
        };

        public static string AddSpeakTagsAndClean(string spokenText)
        {
            // remove characters that will cause SSML to break.
            // probably a whole lot of other characters to remove or sanitize.  This is just a lazy start.
            return "<speak> " + spokenText.Replace("&", "and") + " </speak>";

        }

        public static SpeechletResponse BuildSpeechletResponse(SimpleIntentResponse simpleIntentResponse, bool shouldEndSession)
        {

            SpeechletResponse response = new SpeechletResponse();
            response.ShouldEndSession = shouldEndSession;

            List<Directives> directives = new List<Directives>();

            // if there's a value present for musicUrl, this then generate the directives
            // required for Echo to play the requested track 

            if (simpleIntentResponse.musicUrl != "")
            {
                // figure out if we are dealing with a multi-track album
                // the token we send Echo is in this format if so:
                // <MusicId>?<Index>

                string[] items = simpleIntentResponse.msgId.Split('?');
                int msgAlbumIndex = -1;
                string msgAlbumId = "";

                // if there are two members of the array item[] then
                // we are dealing with a multi-track album

                if (items.Length == 2)
                {
                    msgAlbumId = items[0];
                    msgAlbumIndex = int.Parse(items[1]);
                }

                if (msgAlbumIndex < 1 || simpleIntentResponse.musicAction == "AMAZON.NextIntent")
                {
                    directives = new List<Directives>()
                    {
                        new Directives()
                        {
                            type = "AudioPlayer.Play",
                            playBehavior = "REPLACE_ALL",
                            audioItem = new Directives.AudioItem()
                            {
                                stream = new Directives.Stream()
                                        {
                                            offsetInMilliseconds = 0,
                                            token = simpleIntentResponse.msgId,
                                            url = simpleIntentResponse.musicUrl,
                                        }
                            },
                        }
                    };

                    // the next intent doesn't require a card to be generated

                    if (simpleIntentResponse.musicAction == "AMAZON.NextIntent")
                    {
                        response.Directives = directives;
                        return response;
                    }
                }
                else
                {
                    int previousIndex = msgAlbumIndex - 1;
                    string previousToken = msgAlbumId + "?" + previousIndex;

                    directives = new List<Directives>()
                    {
                        new Directives()
                        {
                            type = "AudioPlayer.Play",
                            playBehavior = "ENQUEUE",
                            audioItem = new Directives.AudioItem()
                            {
                                stream = new Directives.Stream()
                                {
                                    offsetInMilliseconds = 0,
                                    token = simpleIntentResponse.msgId,
                                    url = simpleIntentResponse.musicUrl,
                                    expectedPreviousToken = previousToken,
                                }
                            },
                        }
                    };

                    // the enqueue intent can not have a card associated with it
                    // otherwise, Alexa will return an error

                    response.Directives = directives;
                    return response;
                }
                response.Directives = directives;
            }



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


            // if images are passed, then assume a standard card is wanted
            // images should be stored in the ~/Images/ folder and follow these requirements

            // JPEG or PNG supported, no larger than 2MB
            // 720x480 - small size recommendation
            // 1200x800 - large size recommendation

            if (simpleIntentResponse.smallImage != "" && simpleIntentResponse.largeImage != "")
            {
                StandardCard card = new StandardCard();
                card.Title = AlexaConstants.AppName;
                card.Text = simpleIntentResponse.cardText;

                // The trailing slash after the image name is required because we're serving off the image through a Web API controller and
                // don't want to change the default web project settings

                card.Image = new Image()
                {
                    LargeImageUrl = "https://" + System.Web.HttpContext.Current.Request.Url.Host + "/api/alexaimages/" + simpleIntentResponse.largeImage + "/",
                    SmallImageUrl = "https://" + System.Web.HttpContext.Current.Request.Url.Host + "/api/alexaimages/" + simpleIntentResponse.smallImage + "/",
                };

                response.Card = card;

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