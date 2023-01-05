using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class DataModel
    {
        [JsonProperty("url")]
        public string url { get; set; }
    }
}
