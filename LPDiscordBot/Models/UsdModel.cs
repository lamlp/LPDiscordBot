using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class UsdModel
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [JsonProperty("usd")]
        public CurrencyModel Currencies { get; set; }
    }
}
