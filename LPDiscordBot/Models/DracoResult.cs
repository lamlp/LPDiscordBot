using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class DracoResult
    {
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Data")]
        public DracoModel Data { get; set; }
    }
}
