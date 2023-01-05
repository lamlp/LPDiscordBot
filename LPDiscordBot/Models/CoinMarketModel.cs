using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class CoinMarketModel
    {
        [JsonProperty("id")]
        public int id;
        [JsonProperty("marketPairs")]
        public List<WemixModel> marketPairs;
    }

    public class CoinMarketDataReceivedModel
    {
        [JsonProperty("data")]
        public CoinMarketModel data;
        [JsonProperty("marketPair")]
        public object status;
    }
}
