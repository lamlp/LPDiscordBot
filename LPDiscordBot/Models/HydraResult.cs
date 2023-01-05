using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class HydraResult
    {
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Data")]
        public HydraModel Data { get; set; }
    }
}
