//  New file created by Tam Huynh (tamhinsf)
//  Original AlexaSkillsLib Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using AlexaSkillsKit.UI;

namespace AlexaSkillsKit.UI
{
    public class Directives
    {
        public string type { get; set; }
        public string playBehavior { get; set; }
        public AudioItem audioItem { get; set; }

        public class AudioItem
        {
            public Stream stream { get; set; }
        }

        public class Stream
        {
            public string token { get; set; }
            public string expectedPreviousToken { get; set; }
            public string url { get; set; }
            public int offsetInMilliseconds { get; set; }
        }


    }
}
