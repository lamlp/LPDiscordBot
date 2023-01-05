using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class DracoModel
    {
        [JsonProperty("CreatedDT")]
        public DateTime CreatedDT { get; set; }
        [JsonProperty("DracoPrice")]
        public decimal DracoPrice { get; set; }
        [JsonProperty("DracoAmount")]
        public decimal DracoAmount { get; set; }
        [JsonProperty("DracoPricePrev")]
        public decimal DracoPricePrev { get; set; }
        [JsonProperty("DracoAmountPrev")]
        public decimal DracoAmountPrev { get; set; }
        [JsonProperty("USDWemixRate")]
        public decimal USDWemixRate { get; set; }
        [JsonProperty("USDKLAYRate")]
        public decimal USDKLAYRate { get; set; }
        [JsonProperty("USDWemixRatePrev")]
        public decimal USDWemixRatePrev { get; set; }
        [JsonProperty("USDDracoRate")]
        public decimal USDDracoRate { get; set; }
        [JsonProperty("DracoPriceWemix")]
        public decimal DracoPriceWemix { get; set; }
        [JsonProperty("DracoPriceKlay")]
        public decimal DracoPriceKlay { get; set; }
        [JsonProperty("USDDracoRatePrev")]
        public decimal USDDracoRatePrev { get; set; }
    }
}
