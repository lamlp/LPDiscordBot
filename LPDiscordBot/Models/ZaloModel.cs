using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class ZaloModel
    {
        [JsonProperty("data")]
        public DataModel data { get; set; }
        [JsonProperty("error_code")]
        public int error_code { get; set; }
        [JsonProperty("error_message")]
        public string error_message { get; set; }
    }
}
