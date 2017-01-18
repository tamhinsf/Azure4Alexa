//  New file created by Tam Huynh (tamhinsf)
//  Original AlexaSkillsLib Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AlexaSkillsKit.Speechlet
{
    public class Context
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Context FromJson(JObject json)
        {
            return new Context
            {
                System = System.FromJson(json.Value<JObject>("System")),
                AudioPlayer = AudioPlayer.FromJson(json.Value<JObject>("AudioPlayer")),
            };
        }

        public virtual System System
        {
            get;
            set;
        }

        public virtual AudioPlayer AudioPlayer
        {
            get;
            set;
        }

    }
}