using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class NFTCharacterApiResult
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("data")]
        public NFTCharacterResult Data { get; set; }
    }

    public class NFTCharacterResult
    {
        [JsonProperty("firstID")]
        public string FirstID { get; set; }
        [JsonProperty("lists")]
        public List<NFTCharacterModel> Lists { get; set; }
        [JsonProperty("more")]
        public int More { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }
}
