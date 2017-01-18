//  New file created by Tam Huynh (tamhinsf)
//  Original AlexaSkillsLib Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using System.Diagnostics;
using AlexaSkillsKit.Slu;

namespace AlexaSkillsKit.Speechlet
{
    public class AudioPlayerRequest : SpeechletRequest
    {
        public AudioPlayerRequest(string requestId, DateTime timestamp)
            : base(requestId, timestamp)
        {

        }

        public virtual Context Context
        {
            get;
            private set;
        }

    }
}