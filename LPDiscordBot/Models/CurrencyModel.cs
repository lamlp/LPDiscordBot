using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class CurrencyModel
    {
        [JsonProperty("vnd")]
        public decimal VND { get; set; }
    }
}
