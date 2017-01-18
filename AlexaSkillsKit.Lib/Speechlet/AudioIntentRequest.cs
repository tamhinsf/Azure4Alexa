//  Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using AlexaSkillsKit.Slu;

namespace AlexaSkillsKit.Speechlet
{
    public class AudioIntentRequest : SpeechletRequest
    {
        public AudioIntentRequest(string requestId, DateTime timestamp, Intent intent)
            : base(requestId, timestamp)
        {

            Intent = intent;
            Context = Context;
        }

        public virtual Intent Intent
        {
            get;
            private set;
        }

        public virtual Context Context
        {
            get;
            private set;
        }
    }
}