using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRTest.Models
{
    public class Card
    {
        public CardInfo cardInfo { get; set; }

        public class CardInfo
        {
            [JsonProperty("employeeNo")]
            public string employeeNo { get; set; }
            [JsonProperty("cardNo")]
            public string cardNo { get; set; }
            [JsonProperty("cardType")]
            public string cardType { get; set; }
            [JsonProperty("deleteCard")]
            public Boolean deleteCard { get; set; }
        }
    }
}
