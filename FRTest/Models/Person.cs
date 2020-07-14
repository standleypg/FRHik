using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRTest.Models
{
    public class Person
    {
        //employeeNo = "4", userType = "normal", Valid = "2021-08-01T17:30:08", enable = false, beginTime = "2019-08-01T17:30:08", endTime = "2019 - 08 - 01T17:30:08"
        
        public UserInfo userInfo { get; set; }
        public class UserInfo
        {
            [JsonProperty("employeeNo")]
            public string employeeNo { get; set; }
            [JsonProperty("name")]
            public string name { get; set; }
            [JsonProperty("userType")]
            public string userType { get; set; }
            [JsonProperty("valid")]
            public Valid valid { get; set; }
            [JsonProperty("checkUser")]
            public Boolean checkUser { get; set; }
            [JsonProperty("addUser")]
            public Boolean addUser { get; set; }
        }

        public class Valid
        {
            [JsonProperty("enable")]
            public Boolean enable { get; set; }
            [JsonProperty("beginTime")]
            public string beginTime { get; set; }
            [JsonProperty("endTime")]
            public string endTime { get; set; }
            [JsonProperty("timeType")]
            public string timeType { get; set; }
        }
    }

 
}
