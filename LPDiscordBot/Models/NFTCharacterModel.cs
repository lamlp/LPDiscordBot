using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class NFTCharacterModel
    {
        [JsonProperty("characterName")]
        public string CharacterName { get; set; }
        [JsonProperty("class")]
        public EnumNftCharacterMir4 Class { get; set; }
        [JsonProperty("lv")]
        public int LV { get; set; }
        [JsonProperty("nftID")]
        public int NftID { get; set; }
        [JsonProperty("powerScore")]
        public long PowerScore { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("seq")]
        public int SEQ { get; set; }
    }
}
