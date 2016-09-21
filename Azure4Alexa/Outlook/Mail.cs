using AlexaSkillsKit.Speechlet;
using Azure4Alexa.Alexa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Azure4Alexa.Outlook
{

    public class Mail
    {
        public static string unreadOutlookEmailsInInbox =
            "https://outlook.office.com/api/v2.0/me/mailfolders/inbox/messages?$count=true&$filter=isread%20eq%20false";

        public static string mailboxNotUpgradedMessage = "Sorry, your Outlook inbox hasn't been upgraded to work with this skill.  " +
            "Want to try this skill?  You can easily sign up for a new Outlook.com email address and then try again with your new details.";

        public static SpeechletResponse GetUnreadEmailCount(Session session, HttpClient httpClient)
        {

            int unreadCount = 0;
            int maxEmailsToRead = 5;
            string httpResultString = "";

            // Connect to Outlook

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.User.AccessToken);
            var httpResponseMessage =
                httpClient.GetAsync(unreadOutlookEmailsInInbox).Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                dynamic resultObject = JObject.Parse(httpResultString);
                unreadCount = resultObject.value.Count;
            }
            else if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                dynamic resultObject = JObject.Parse(httpResultString);
                var errorString = resultObject.error.code;
                httpResponseMessage.Dispose();
                if (errorString == "MailboxNotEnabledForRESTAPI" || errorString == "MailboxNotSupportedForRESTAPI")
                {
                    return AlexaUtils.BuildSpeechletResponse(new AlexaUtils.SimpleIntentResponse() { cardText = mailboxNotUpgradedMessage }, true);
                }
                else
                {
                    return AlexaUtils.BuildSpeechletResponse(new AlexaUtils.SimpleIntentResponse() { cardText = AlexaConstants.AppErrorMessage }, true);
                }
            }
            else
            {
                httpResponseMessage.Dispose();
                return AlexaUtils.BuildSpeechletResponse(new AlexaUtils.SimpleIntentResponse() { cardText = AlexaConstants.AppErrorMessage }, true);
            }

            string cardBody = "";
            string spokenEmailString = "";

            string emailMaximumNotice = "We'll read the first " + maxEmailsToRead + " unread emails. ";

            switch (unreadCount)
            {
                case 0:
                    cardBody = "You have no unread email in your Outlook Inbox.";
                    spokenEmailString = cardBody;
                    break;
                case 1:
                    cardBody = "You have 1 unread email in your Outlook Inbox. ";
                    spokenEmailString = cardBody + CreateUnreadEmailSpeechString(httpResultString);
                    break;
                default:
                    cardBody = "You have " + unreadCount + " unread emails in your Outlook Inbox. ";
                    if (unreadCount <= maxEmailsToRead)
                    {
                        spokenEmailString = cardBody + CreateUnreadEmailSpeechString(httpResultString, unreadCount);
                    }
                    else
                    {
                        spokenEmailString = cardBody + emailMaximumNotice + CreateUnreadEmailSpeechString(httpResultString, maxEmailsToRead);
                    }
                    
                    break;
            }

            httpResponseMessage.Dispose();

            AlexaUtils.SimpleIntentResponse simpleIntentResponse = 
                new AlexaUtils.SimpleIntentResponse() {
                    cardText = cardBody,
                    ssmlString = AlexaUtils.AddSpeakTagsAndClean(spokenEmailString),
                    largeImage = "outlook.png",
                    smallImage = "outlook.png",
                };
            return AlexaUtils.BuildSpeechletResponse(simpleIntentResponse, true);
        }

        private static string CreateUnreadEmailSpeechString(string resultString, int numEmailsToRead = 1)
        {
            string emailToRead = String.Empty;
            dynamic resultObject = JObject.Parse(resultString);

            int emailAddedToStringCount = 0;

            for (int i = 0; i < numEmailsToRead; i++)
            {
                emailToRead += " <break time=\"2s\"/> An email from " +
                  resultObject.value[i].From.EmailAddress.Name + " with subject " + resultObject.value[i].Subject + "  ";
                emailAddedToStringCount++;
            }

            return emailToRead;
        }

    }
}