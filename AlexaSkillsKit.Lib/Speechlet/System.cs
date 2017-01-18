//  New file created by Tam Huynh (tamhinsf)
//  Original AlexaSkillsLib Copyright 2015 Stefan Negritoiu (FreeBusy). See LICENSE file for more information.

using System;
using Newtonsoft.Json.Linq;

namespace AlexaSkillsKit.Speechlet
{
    public class System
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static System FromJson(JObject json)
        {
            return new System
            {
                Application = Application.FromJson(json.Value<JObject>("application")),
                User = User.FromJson(json.Value<JObject>("user")),
            };
        }

        public virtual Application Application
        {
            get;
            set;
        }

        public virtual User User
        {
            get;
            set;
        }
    }
}