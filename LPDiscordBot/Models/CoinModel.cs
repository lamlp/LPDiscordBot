using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPDiscordBot.Models
{
    public class CoinModel
    {
        public string name;
        public decimal maxPrice;
        public decimal rate;
        public string pair;
        public string url;
        public decimal price;
    }
}
