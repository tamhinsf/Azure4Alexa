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

    // 

    public class Calendar
    {
        private static string myOutlookTimeZone = "";

        private static TimeZoneInfo myZone; // = TimeZoneInfo.FindSystemTimeZoneById(myOutlookTimeZone);
        private static DateTime myTimeNow; // = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, myZone);

        private static string myTimeNowString = String.Empty;
        private static string myTimeNowEndOfDayString = String.Empty; // myTimeNowString with time at 23:59


        public static string outlookTimeZoneUri =
            "https://outlook.office.com/api/beta/me/mailboxSettings/timeZone";

        public static string outlookEventsUri =
            "https://outlook.office.com/api/beta/me/calendarview?$select=Subject,Start,End,IsAllDay&$Orderby=Start/DateTime";

        public static string mailboxNotUpgradedMessage = "Sorry, your Outlook inbox hasn't been upgraded to work with this skill.  " +
            "Want to try this skill?  You can easily sign up for a new Outlook.com email address and then try again with your new details.";

        public static async Task<string> GetOutlookTimezone(Session session, HttpClient httpClient)
        {
            string httpResultString = "";

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.User.AccessToken);
            //var httpResponseMessage =
            //    httpClient.GetAsync(unreadOutlookEmailsInInbox).Result;
            var httpResponseMessage = await httpClient.GetAsync(outlookTimeZoneUri);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                httpResultString = await httpResponseMessage.Content.ReadAsStringAsync();
                dynamic resultObject = JObject.Parse(httpResultString);
                myOutlookTimeZone = resultObject.value;
            }
            else if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                dynamic resultObject = JObject.Parse(httpResultString);
                var errorString = resultObject.error.code;
                httpResponseMessage.Dispose();
                if (errorString == "MailboxNotEnabledForRESTAPI" || errorString == "MailboxNotSupportedForRESTAPI")
                {
                    return myOutlookTimeZone;
                }
                else
                {
                    return myOutlookTimeZone;
                }
            }
            else
            {
                httpResponseMessage.Dispose();
                return myOutlookTimeZone;
            }
            return myOutlookTimeZone;
        }

        public static async Task<SpeechletResponse> GetOutlookEventCount(Session session, HttpClient httpClient)
        //public static SpeechletResponse GetUnreadEmailCount(Session session, HttpClient httpClient)
        {

            int unreadCount = 0;
            int maxToRead = 5;
            string httpResultString = "";
            string myOutlookTimeZone = "";

            // Connect to Outlook

            myOutlookTimeZone = await GetOutlookTimezone(session, httpClient);

            if (myOutlookTimeZone == "")
            {
                return AlexaUtils.BuildSpeechletResponse(new AlexaUtils.SimpleIntentResponse() { cardText = mailboxNotUpgradedMessage }, true);
            }

            // convert time zone string value returned from Outlook API (myOutlookTimeZone) into .NET standard

            myZone = TimeZoneInfo.FindSystemTimeZoneById(myOutlookTimeZone);

            // calculate the local time using myZone value to offset UTC
            myTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, myZone);

            // convert local time to a string to be used in the API query
            myTimeNowString = myTimeNow.ToString("yyyy-MM-ddTHH:mm");

            // make a string that's the end of the local date's day (23:59 / 11:59PM) based on myTimeNow year-month-date
            myTimeNowEndOfDayString = myTimeNow.ToString("yyyy-MM-ddT") + "23:59";

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.User.AccessToken);
            httpClient.DefaultRequestHeaders.Add("Prefer", "Outlook.timezone = \"" + myOutlookTimeZone + "\"");
            //var httpResponseMessage =
            //    httpClient.GetAsync(unreadOutlookEmailsInInbox).Result;

            var httpResponseMessage = await httpClient.GetAsync(outlookEventsUri + "&startDateTime=" + myTimeNowString + "&endDAteTime=" + myTimeNowEndOfDayString);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                //httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                httpResultString = await httpResponseMessage.Content.ReadAsStringAsync();
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
            string spokenString = "";

            string emailMaximumNotice = "We'll read the first " + maxToRead;

            switch (unreadCount)
            {
                case 0:
                    cardBody = "You have no events remaining on today's calendar";
                    spokenString = cardBody;
                    break;
                case 1:
                    cardBody = "You have 1 event remaining on today's calendar. ";
                    spokenString = cardBody + CreateSpeechString(httpResultString);
                    break;
                default:
                    cardBody = "You have " + unreadCount + " events remaining on today's calendar. ";
                    if (unreadCount <= maxToRead)
                    {
                        spokenString = cardBody + CreateSpeechString(httpResultString, unreadCount);
                    }
                    else
                    {
                        spokenString = cardBody + emailMaximumNotice + CreateSpeechString(httpResultString, maxToRead);
                    }

                    break;
            }

            httpResponseMessage.Dispose();

            AlexaUtils.SimpleIntentResponse simpleIntentResponse =
                new AlexaUtils.SimpleIntentResponse()
                {
                    cardText = cardBody,
                    ssmlString = AlexaUtils.AddSpeakTagsAndClean(spokenString),
                    largeImage = "outlook.png",
                    smallImage = "outlook.png",
                };
            return AlexaUtils.BuildSpeechletResponse(simpleIntentResponse, true);
        }

        private static string CreateSpeechString(string resultString, int numToRead = 1)
        {
            string stringToRead = String.Empty;
            dynamic resultObject = JObject.Parse(resultString);


            int itemAddedToStringCount = 0;

            for (int i = 0; i < numToRead; i++)
            {
                DateTime startDt = resultObject.value[i].Start.DateTime;
                DateTime endDt = resultObject.value[i].End.DateTime;

                string timeString = "";

                if (resultObject.value[i].IsAllDay == "true")
                {
                    timeString = "an all day event ";
                }
                else
                {
                    if (startDt.ToString("dd") == myTimeNow.ToString("dd"))
                    {
                        timeString += "starting at " + startDt.ToString("hh:mm tt ");
                    }
                    else
                    {
                        timeString += "started on " + startDt.ToString("MMMM dd ") + "at " + startDt.ToString("hh:mm tt ");
                    }
                    if (endDt.ToString("dd") == myTimeNow.ToString("dd"))
                    {
                        timeString += "and ending at " + endDt.ToString("hh:mm tt");
                    }
                    else
                    {
                        timeString += "and ending on " + endDt.ToString("MMMM dd ") +  "at " + endDt.ToString("hh:mm tt "); 

                    }

                }

                stringToRead += " <break time=\"2s\"/> " + resultObject.value[i].Subject +
                    " <break time=\"1s\"/> " + timeString;                    
                itemAddedToStringCount++;
            }
            return stringToRead;
        }



    }
}