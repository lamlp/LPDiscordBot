using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class WemixModel
    {
        [JsonProperty("exchangeName")]
        public string exchangeName; // : "Bithumb";
        [JsonProperty("marketPair")]
        public string marketPair; // "WEMIX/KRW"
        [JsonProperty("marketUrl")]
        public string marketUrl; // "https://www.bithumb.com/trade/order/WEMIX_KRW"
        [JsonProperty("price")]
        public decimal price; // 5.6176263445098
        [JsonProperty("volumeBase")]
        public decimal volumeBase; // 3328137.15731093
        [JsonProperty("volumeUsd")]
        public decimal volumeUsd; // 18708638.32833589
        [JsonProperty("lastUpdated")]
        public DateTime lastUpdated;
        [JsonProperty("quoteSymbol")]
        public string quoteSymbol; // USDT
    }
}
