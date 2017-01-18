//  New file created by Tam Huynh (tamhinsf)
//  Original AlexaSkillsLib Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using Newtonsoft.Json.Linq;

namespace AlexaSkillsKit.Speechlet
{
    public class AudioPlayer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static AudioPlayer FromJson(JObject json)
        {
            return new AudioPlayer
            {
                Token = json.Value<string>("token")
            };
        }

        public virtual string Token
        {
            get;
            set;
        }
    }
}