using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Azure4Alexa.Alexa
{
    public class AlexaConstants
    {
        // Inbound requests from Amazon include the Voice Skills AppId, assigned to you when registering your skill
        // Validate the value against what you registered, to ensure that someone else isn't calling your service.  

        // In previous versions of this project, we stored the AppId as a code variable (below)
        // Now, we'll move the location of this setting into Web.Config

        public static string AppId = WebConfigurationManager.AppSettings["AppId"];

        // the value of AppName has no correspondence to what you have registered in Amazon
        // we just store it here because it's useful

        // However, it will appear in the card shown to the user in the Alexa Companion app for iOS or Android
        // You might want to change it

        // In previous versions of this project, we stored the AppName as a code variable (below)
        // Now, we'll move the location of this setting into Web.Config

        public static string AppName = WebConfigurationManager.AppSettings["AppName"];

        // standard error message

        public static string AppErrorMessage = "Sorry, something went wrong.  Please try again.";


    }
}