using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azure4Alexa.Alexa
{
    public class AlexaConstants
    {
        // Inbound requests from Amazon include the Voice Skills AppId, assigned to you when registering your skill
        // Validate the value against what you registered, to ensure that someone else isn't calling your service.  

        // It's bad practice to include the actual AppId in code,
        // but we'll do so here so you can learn how things work

        public static string AppId = "amzn1.ask.skill.8040e55b-03a8-4a4f-a102-bdc13b5acd1a";

        // the value of AppName has no correspondence to what you have registered in Amazon
        // we just store it here because it's useful

        public static string AppName = "Azure4Alexa";

        // standard error message

        public static string AppErrorMessage = "Sorry, something went wrong.  Please try again.";


    }
}