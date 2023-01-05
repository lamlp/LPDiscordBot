using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class HydraModel
    {
        [JsonProperty("CreatedDT")]
        public DateTime CreatedDT { get; set; }
        [JsonProperty("HydraPrice")]
        public decimal HydraPrice { get; set; }
        [JsonProperty("HydraAmount")]
        public decimal HydraAmount { get; set; }
        [JsonProperty("HydraPricePrev")]
        public decimal HydraPricePrev { get; set; }
        [JsonProperty("HydraAmountPrev")]
        public decimal HydraAmountPrev { get; set; }
        [JsonProperty("USDWemixRate")]
        public decimal USDWemixRate { get; set; }
        [JsonProperty("USDKLAYRate")]
        public decimal USDKLAYRate { get; set; }
        [JsonProperty("USDWemixRatePrev")]
        public decimal USDWemixRatePrev { get; set; }
        [JsonProperty("USDHydraRate")]
        public decimal USDHydraRate { get; set; }
        [JsonProperty("HydraPriceWemix")]
        public decimal HydraPriceWemix { get; set; }
        [JsonProperty("HydraPriceKlay")]
        public decimal HydraPriceKlay { get; set; }
        [JsonProperty("USDHydraRatePrev")]
        public decimal USDHydraRatePrev { get; set; }
    }
}
