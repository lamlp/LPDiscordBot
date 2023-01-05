using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;
using LPDiscordBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LPDiscordBot.Services
{
    public class Mir4Service
    {
        private const string Mir4URL = "https://api.mir4global.com/";
        private string priceDracoLatest = "wallet/prices/draco/lastest";
        private string priceHydraLatest = "wallet/prices/hydra/lastest";
        private string formatDraco = "Giá Draco ({0}):\nUSD: {1}\nUSD hôm qua: {2}\nUSD {3}\nVND: {4}\nVND hôm qua: {5}\nVND {6}\nWemix: {7}\nKlay: {8}\nTổng lượng tăng: {9}\n Tổng lượng tăng hôm qua: {10}";
        private string formatHydra = "Giá Hydra ({0}):\nUSD: {1}\nUSD hôm qua: {2}\nUSD {3}\nVND: {4}\nVND hôm qua: {5}\nVND {6}\nWemix: {7}\nKlay: {8}\nTổng lượng tăng: {9}\n Tổng lượng tăng hôm qua: {10}";
        private string formatWemix = "Sàn: {0}\nURL: {1}\nUSD: {2}$\nVND: {3} Vnd\n\n";
        private string formatCoin = "Pair: {0}; Rate: {1}; Max Price: {2}; URL: {3}; Price: {4}$\n\n";
        private List<CoinModel> coinList = new List<CoinModel>
        { 
            new CoinModel { name = "bitcoin", maxPrice = (decimal)68789.63 },
            new CoinModel { name = "ethereum", maxPrice = (decimal)4891.70 },
            new CoinModel { name = "bnb", maxPrice = (decimal)669.35 },
            new CoinModel { name = "xrp", maxPrice = (decimal)1.35 },
            new CoinModel { name = "cardano", maxPrice = (decimal)2.80 },
            new CoinModel { name = "solana", maxPrice = (decimal)260.06 },
            new CoinModel { name = "Dogecoin", maxPrice = (decimal)0.3388 },
            new CoinModel { name = "Polkadot", maxPrice = (decimal)55.00 },
            new CoinModel { name = "Polygon", maxPrice = (decimal)2.92 },
            new CoinModel { name = "Dai", maxPrice = (decimal)3.67 },
            new CoinModel { name = "Shiba-Inu", maxPrice = (decimal)0.00008845 },
            new CoinModel { name = "TRON", maxPrice = (decimal)0.1291 },
            new CoinModel { name = "Avalanche", maxPrice = (decimal)146.22 },
            new CoinModel { name = "Ethereum-Classic", maxPrice = (decimal)64.94 },
            new CoinModel { name = "Uniswap", maxPrice = (decimal)28.43 },
            new CoinModel { name = "Litecoin", maxPrice = (decimal)294.56 },
            new CoinModel { name = "Cosmos", maxPrice = (decimal)44.70 },
            new CoinModel { name = "Chainlink", maxPrice = (decimal)38.16 },
            new CoinModel { name = "Cronos", maxPrice = (decimal)0.9698 },
            new CoinModel { name = "Monero", maxPrice = (decimal)296.74 },
            new CoinModel { name = "Stellar", maxPrice = (decimal)0.4403 },
            new CoinModel { name = "ApeCoin", maxPrice = (decimal)39.40 },
            new CoinModel { name = "Tezos", maxPrice = (decimal)9.18 },
            new CoinModel { name = "The-Sandbox", maxPrice = (decimal)8.44 },
            new CoinModel { name = "Hedera", maxPrice = (decimal)0.5701 },
            new CoinModel { name = "Aave", maxPrice = (decimal)454.27 },
            new CoinModel { name = "elrond-egld", maxPrice = (decimal)542.58 },
            new CoinModel { name = "Chiliz", maxPrice = (decimal)0.643 }
        };
        private List<int> CheckedNftSeqs = new List<int>();

        public async Task<string> GetPriceDracoLatest(bool isOnlyPrice = false)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Mir4URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.PostAsync(priceDracoLatest, null).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                string contents = await response.Content.ReadAsStringAsync();
                DracoResult dracoResult = JsonConvert.DeserializeObject<DracoResult>(contents);
                client.Dispose();
                CultureInfo viVn = new CultureInfo("vi-VN");
                var giaUsd = GetPriceUsdLatest().Result;
                var date = dracoResult.Data.CreatedDT.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss", viVn);
                var priceDracoUsd = RoundNumber(dracoResult.Data.USDDracoRate).ToString();
                if (isOnlyPrice)
                {
                    return priceDracoUsd.ToString();
                }
                var priceDracoUsdToday = RoundNumber(dracoResult.Data.USDDracoRatePrev).ToString();
                var priceDracoUsdDifference = PriceDifferenceRate(dracoResult.Data.USDDracoRatePrev, dracoResult.Data.USDDracoRate);
                var priceDracoVnd = RoundNumber(dracoResult.Data.USDDracoRate * giaUsd).ToString();
                var priceDracoVndToday = RoundNumber(dracoResult.Data.USDDracoRatePrev * giaUsd).ToString();
                var priceDracoVndDifference = PriceDifferenceRate(RoundNumber(dracoResult.Data.USDDracoRatePrev * giaUsd), RoundNumber(dracoResult.Data.USDDracoRate * giaUsd));
                var priceDracoWemix = RoundNumber(dracoResult.Data.DracoPriceWemix).ToString();
                var priceKlay = RoundNumber(dracoResult.Data.DracoPriceKlay).ToString();
                var amountDraco = RoundNumber(dracoResult.Data.DracoAmount).ToString();
                var amountDracoToday = RoundNumber(dracoResult.Data.DracoAmountPrev).ToString();
                return string.Format(formatDraco, date, priceDracoUsd, priceDracoUsdToday, priceDracoUsdDifference, priceDracoVnd, priceDracoVndToday, priceDracoVndDifference, priceDracoWemix, priceKlay, amountDraco, amountDracoToday);
            }

            client.Dispose();
            return string.Empty;
        }

        public async Task<string> GetPriceHydraLatest(bool isOnlyPrice = false)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Mir4URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.PostAsync(priceHydraLatest, null).Result;
            if (response.IsSuccessStatusCode)
            {
                string contents = await response.Content.ReadAsStringAsync();
                HydraResult hydraResult = JsonConvert.DeserializeObject<HydraResult>(contents);
                client.Dispose();
                CultureInfo viVn = new CultureInfo("vi-VN");
                var giaUsd = GetPriceUsdLatest().Result;
                var date = hydraResult.Data.CreatedDT.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss", viVn);
                var priceHydraUsd = RoundNumber(hydraResult.Data.USDHydraRate).ToString();
                if (isOnlyPrice)
                {
                    return priceHydraUsd.ToString();
                }
                var priceHydraUsdToday = RoundNumber(hydraResult.Data.USDHydraRatePrev).ToString();
                var priceHydraUsdDifference = PriceDifferenceRate(hydraResult.Data.USDHydraRatePrev, hydraResult.Data.USDHydraRate);
                var priceHydraVnd = RoundNumber(hydraResult.Data.USDHydraRate * giaUsd).ToString();
                var priceHydraVndToday = RoundNumber(hydraResult.Data.USDHydraRatePrev * giaUsd).ToString();
                var priceHydraVndDifference = PriceDifferenceRate(RoundNumber(hydraResult.Data.USDHydraRatePrev * giaUsd), RoundNumber(hydraResult.Data.USDHydraRate * giaUsd));
                var priceHydraWemix = RoundNumber(hydraResult.Data.HydraPriceWemix).ToString();
                var priceKlay = RoundNumber(hydraResult.Data.HydraPriceKlay).ToString();
                var amountHydra = RoundNumber(hydraResult.Data.HydraAmount).ToString();
                var amountHydraToday = RoundNumber(hydraResult.Data.HydraAmountPrev).ToString();
                return string.Format(formatHydra, date, priceHydraUsd, priceHydraUsdToday, priceHydraUsdDifference, priceHydraVnd, priceHydraVndToday, priceHydraVndDifference, priceHydraWemix, priceKlay, amountHydra, amountHydraToday);
            }

            client.Dispose();
            return string.Empty;
        }

        public async Task<string> GetPriceWemixLatest(bool isOnlyPrice = false)
        {
            string result = string.Empty;
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://api.coinmarketcap.com/");

            client.DefaultRequestHeaders.Clear();

            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "PostmanRuntime/7.28.2");

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("data-api/v3/cryptocurrency/market-pairs/latest?slug=wemix&start=1&limit=100&category=spot&sort=cmc_rank_advanced").Result;
            if (response.IsSuccessStatusCode)
            {
                string contents = await response.Content.ReadAsStringAsync();
                CoinMarketDataReceivedModel coinMarket = JsonConvert.DeserializeObject<CoinMarketDataReceivedModel>(contents);
                var wemixResults = coinMarket.data.marketPairs;
                client.Dispose();
                var giaUsd = GetPriceUsdLatest().Result;
                result = "Giá Wemix:\n";
                foreach (var wemixResult in wemixResults)
                {
                    var marketName = wemixResult.exchangeName;
                    var marketUrl = wemixResult.marketUrl;
                    var priceUsd = RoundNumber(wemixResult.price).ToString();
                    if (isOnlyPrice)
                    {
                        if (marketName.ToUpper() == "GATE.IO")
                        return priceUsd.ToString();
                    }
                    var priceVnd = RoundNumber(wemixResult.price * giaUsd).ToString();
                    string stringFormatResult = string.Format(formatWemix, marketName, marketUrl, priceUsd, priceVnd);
                    result = result + stringFormatResult;
                }
                return result;
            }

            client.Dispose();
            return string.Empty;
        }

        private string PriceDifferenceRate(decimal before, decimal after)
        {
            if(before == 0 || after == 0)
            {
                return string.Empty;
            }
            var rate = (after / before);
            if (rate > 1)
            {
                return $"tăng {RoundNumber((rate * 100) - 100)}%";
            }
            else if (rate < 1)
            {
                return $"giảm {RoundNumber(100 - (rate * 100))}%";
            }
            else
            {
                return "giữ nguyên";
            }
        }

        public async Task<decimal> GetPriceUsdLatest()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/usd.json");

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
            {
                string contents = await response.Content.ReadAsStringAsync();
                UsdModel usdResult = JsonConvert.DeserializeObject<UsdModel>(contents);
                client.Dispose();
                return usdResult.Currencies.VND;
            }

            client.Dispose();
            return 0;
        }

        public async Task<string> GetRankingServer(string serverId, int from, int to)
        {
            string result = string.Empty;
            using (var client = new WebClient())
            {
                string html = client.DownloadString($"https://forum.mir4global.com/rank?ranktype=1&worldgroupid=13&worldid=" + serverId + "&classtype=&searchname=");

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                //Selecting all the nodes with tagname `span` having "id=ctl00_ContentBody_CacheName".
                var divRank = doc.DocumentNode.SelectNodes("//div").Where(d => d.Attributes.Contains("class")).FirstOrDefault(d => d.Attributes["class"].Value == "rank_section common_box");
                var tbodyrank = divRank.SelectNodes("//tbody").Where(d => d.Attributes.Contains("id")).FirstOrDefault(d => d.Attributes["id"].Value == "lists");
                var players = tbodyrank.SelectNodes("//tr").Where(d => d.Attributes.Contains("class")).Where(d => d.Attributes["class"].Value.Contains("list_article"));

                foreach (HtmlNode player in players)
                {
                    var modelPlayer = ConvertToData(player);
                    if (modelPlayer.Rank < from || modelPlayer.Rank > to)
                    {
                        continue;
                    }
                    result = result + $"Rank {modelPlayer.Rank}: {modelPlayer.Name}, Class: {modelPlayer.ClassName}, Clan: {modelPlayer.Clan}, Power Score: {modelPlayer.PowerScore} " + "\n";
                }
            }
            return result;
        }

        public string GetServerId(string serverName)
        {
            try
            {
                EnumServerMir4 objs = new EnumServerMir4();
                return typeof(EnumServerMir4).GetField(serverName.ToUpper()).GetValue(objs).ToString();
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }

        public Dictionary<string, string> GetAllServer()
        {
            try
            {
                EnumServerMir4 objs = new EnumServerMir4();
                var servers = objs.GetType()
                     .GetFields()
                     .Select(field => field).ToDictionary(x => x.Name.ToString(), x => x.GetValue(objs).ToString());
                return servers;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }

        public string GetRankingAllServer(int to)
        {
            string result = string.Empty;
            var players = new List<PlayerModel>();
            EnumServerMir4 objs = new EnumServerMir4();
            var servers = objs.GetType()
                 .GetFields()
                 .Select(field => field).ToDictionary(x => x.Name.ToString(), x => x.GetValue(objs).ToString());
            foreach(var server in servers)
            {
                using (var client = new WebClient())
                {
                    client.Proxy = null;
                    string html = client.DownloadString($"https://forum.mir4global.com/rank?ranktype=1&worldgroupid=13&worldid=" + server.Value + "&classtype=&searchname=");

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    //Selecting all the nodes with tagname `span` having "id=ctl00_ContentBody_CacheName".
                    var divRank = doc.DocumentNode.SelectNodes("//div").Where(d => d.Attributes.Contains("class")).FirstOrDefault(d => d.Attributes["class"].Value == "rank_section common_box");
                    var tbodyrank = divRank.SelectNodes("//tbody").Where(d => d.Attributes.Contains("id")).FirstOrDefault(d => d.Attributes["id"].Value == "lists");
                    var nodePlayers = tbodyrank.SelectNodes("//tr").Where(d => d.Attributes.Contains("class")).Where(d => d.Attributes["class"].Value.Contains("list_article"));

                    foreach (HtmlNode nodePlayer in nodePlayers)
                    {
                        var modelPlayer = ConvertToData(nodePlayer);
                        modelPlayer.Server = server.Key;
                        players.Add(modelPlayer);
                    }
                }
            }

            var orderPlayers = players.OrderByDescending(x => int.Parse(x.PowerScore.Replace(",","")));
            var rankCount = 1;
            foreach(var modelPlayer in orderPlayers)
            {
                if (rankCount > to)
                {
                    return result;
                }
                result = result + $"Rank {rankCount}: {modelPlayer.Name}, Class: {modelPlayer.ClassName}, Clan: {modelPlayer.Clan}, Power Score: {modelPlayer.PowerScore}, Server: {modelPlayer.Server} " + "\n";
                rankCount++;
            }
            return result;
        }

        public Tuple<string, string> AlertMineSpot(string map, string floor,string area, string time)
        {
            string message = string.Empty;
            string audioPath = string.Empty;
            switch (map.ToLower())
            {
                case nameof(EnumMapMir4.redmoon):
                    message = HandleMessageMineSpot(EnumMapMir4.redmoon, floor, area);
                    audioPath = HandleAudioMineSpot(nameof(EnumMapMir4.redmoon), floor, area);
                    break;
                case nameof(EnumMapMir4.snake):
                    message = HandleMessageMineSpot(EnumMapMir4.snake, floor, area);
                    audioPath = HandleAudioMineSpot(nameof(EnumMapMir4.snake), floor, area);
                    break;
                case nameof(EnumMapMir4.bicheon):
                    message = HandleMessageMineSpot(EnumMapMir4.bicheon, floor, area);
                    audioPath = HandleAudioMineSpot(nameof(EnumMapMir4.bicheon), floor, area);
                    break;
            }
            return Tuple.Create(message, audioPath);
        }

        public TimeSpan HandleFinishedTime(string time)
        {
            DateTime finishedTime = DateTime.Parse(time);
            TimeSpan duration = new TimeSpan(0, 55, 00);
            var newHour = finishedTime.Add(duration);
            return newHour.TimeOfDay;
            //var nowHour = DateTime.Now;
            //return newHour.Subtract(nowHour);
        }

        private string HandleAudioMineSpot(string map, string floor, string area)
        {
            return map.ToLower() + "-" + floor + (string.IsNullOrEmpty(area) ? "" : "-" + area) + ".wav";
        }

        private string HandleMessageMineSpot(string map, string floor, string area)
        {
            var floorMessage = string.Empty;
            switch(ReverseString(floor.ToUpper()))
            {
                case nameof(EnumFloorMir4.F1):
                    floorMessage = EnumFloorMir4.F1;
                    break;
                case nameof(EnumFloorMir4.F2):
                    floorMessage = EnumFloorMir4.F2;
                    break;
                case nameof(EnumFloorMir4.F3):
                    floorMessage = EnumFloorMir4.F3;
                    break;
                case nameof(EnumFloorMir4.F4):
                    floorMessage = EnumFloorMir4.F4;
                    break;
            }
            return "Mỏ vàng sẽ xuất hiện tại " + map + " " + floorMessage + (string.IsNullOrEmpty(area) ? "" : " " + GetAreaMessage(area)) + " sau 5 phút nữa";
        }

        public async Task<NFTCharacterResult> GetNFTCharacters(int page)
        {
            HttpClient client = new HttpClient();
            string nftCharactersUrl = @"https://webapi.mir4global.com/nft/lists?listType=sale&class=0&levMin=0&levMax=0&powerMin=0&powerMax=0&priceMin=0&priceMax=0&sort=latest&page=" + page + @"&languageCode=en";
            client.BaseAddress = new Uri(nftCharactersUrl);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
            {
                string contents = await response.Content.ReadAsStringAsync();
                NFTCharacterApiResult apiResult = JsonConvert.DeserializeObject<NFTCharacterApiResult>(contents);
                client.Dispose();
                return apiResult.Data;
            }

            client.Dispose();
            return null;
        }

        public string GetNFTCharactersWithLowPrice()
        {
            string result = string.Empty;
            List<NFTCharacterModel> nftCharacters = new List<NFTCharacterModel>();
            var nftResult = GetNFTCharacters(1);
            if (nftResult.Result == null)
            {
                return string.Empty;
            }

            nftCharacters.AddRange(nftResult.Result.Lists);

            foreach (var nftCharacter in nftCharacters)
            {
                if (CheckedNftSeqs.Exists(x => x == nftCharacter.SEQ))
                {
                    continue;
                }
                CheckedNftSeqs.Add(nftCharacter.SEQ);
                if (!CalculatePriceBaseOnPowerScore(nftCharacter))
                {
                    continue;
                }
                result = result + $"Name: {nftCharacter.CharacterName}, Class: {nftCharacter.Class.ToString()}, Power Score: {nftCharacter.PowerScore}, Level: {nftCharacter.LV}, Price: {nftCharacter.Price}, Link: https://www.xdraco.com/nft/trade/{nftCharacter.SEQ}" + "\n";
            }
            return result;
        }

        private bool CalculatePriceBaseOnPowerScore(NFTCharacterModel character)
        {
            if (character.Price <= 10)
            {
                return true;
            }

            if (character.PowerScore > 100000 && character.PowerScore <= 120000 && character.Price <= 15)
            {
                return true;
            }
            if (character.PowerScore > 120000 && character.PowerScore <= 130000 && character.Price <= 20)
            {
                return true;
            }
            if (character.PowerScore > 130000 && character.PowerScore <= 140000 && character.Price <= 30)
            {
                return true;
            }
            if (character.PowerScore > 140000 && character.PowerScore <= 150000 && character.Price <= 50)
            {
                return true;
            }
            if (character.PowerScore > 150000 && character.PowerScore <= 160000 && character.Price <= 100)
            {
                return true;
            }
            if (character.PowerScore > 160000 && character.PowerScore <= 170000 && character.Price <= 200)
            {
                return true;
            }
            if (character.PowerScore > 170000 && character.PowerScore <= 180000 && character.Price <= 500)
            {
                return true;
            }
            if (character.PowerScore > 180000 && character.Price <= 1000)
            {
                return true;
            }
            return false;
        }

        private string GetAreaMessage(string area)
        {
            if (area.ToLower() == "center")
            {
                return "khu vực tập trung";
            }
            return "khu vực ngẫu nhiên";
        }

        private PlayerModel ConvertToData(HtmlNode trNode)
        {
            var player = new PlayerModel();
            var tdNodes = trNode.ChildNodes.Where(d => d.Name == "td").ToList();
            var rank = tdNodes[0].SelectNodes("em/span").Where(d => d.Attributes.Contains("class")).FirstOrDefault(d => d.Attributes["class"].Value == "num").InnerText;
            var name = tdNodes[1].SelectNodes("span/span").Where(d => d.Attributes.Contains("class")).FirstOrDefault(d => d.Attributes["class"].Value == "user_name").InnerText;
            var className = GetClassName(tdNodes[1].SelectNodes("span/span").Where(d => d.Attributes.Contains("class")).FirstOrDefault(d => d.Attributes["class"].Value == "user_icon").Attributes["style"].Value);
            var clanName = tdNodes[2].SelectNodes("span").FirstOrDefault().InnerText;
            var powerScore = tdNodes[3].SelectNodes("span").FirstOrDefault().InnerText;
            player.Rank = int.Parse(rank);
            player.ClassName = className;
            player.Name = name;
            player.Clan = clanName;
            player.PowerScore = powerScore;
            return player;
        }

        private static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray(); // chuỗi thành mảng ký tự
            Array.Reverse(arr); // đảo ngược mảng
            return new string(arr); // trả về chuỗi mới sau khi đảo mảng
        }

        private string GetClassName(string classData)
        {
            switch(classData)
            {
                case EnumCharacterMir4.Warrior:
                    return "Warrior";
                case EnumCharacterMir4.Sorcerer:
                    return "Sorcerer";
                case EnumCharacterMir4.Taoist:
                    return "Taoist";
                case EnumCharacterMir4.Arbalist:
                    return "Arbalist";
                case EnumCharacterMir4.Lancer:
                    return "Lancer";
            }
            return string.Empty;
        }

        private decimal RoundNumber(decimal number)
        {
            return Math.Round(number, 2);
        }

        public async Task<string> GetPriceScaleCoinTop()
        {
            string result = "Coin scale rate: \n";
            List<CoinModel> coinListResult = new List<CoinModel>();

            foreach (var coin in coinList)
            {
                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri("https://api.coinmarketcap.com/");

                client.DefaultRequestHeaders.Clear();

                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "PostmanRuntime/7.28.2");

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync($"data-api/v3/cryptocurrency/market-pairs/latest?slug={coin.name}&start=1&limit=100&category=spot&sort=cmc_rank_advanced").Result;
                if (response.IsSuccessStatusCode)
                {
                    string contents = await response.Content.ReadAsStringAsync();
                    CoinMarketDataReceivedModel coinMarket = JsonConvert.DeserializeObject<CoinMarketDataReceivedModel>(contents);
                    var coinResults = coinMarket.data.marketPairs;
                    client.Dispose();
                    foreach (var coinResult in coinResults)
                    {
                        if (coinResult.exchangeName.ToLower() == "binance" && coinResult.quoteSymbol.ToLower() == "usdt")
                        {
                            var coinItem = new CoinModel();
                            coinItem.pair = coinResult.marketPair;
                            coinItem.url = coinResult.marketUrl;
                            coinItem.price = RoundNumber(coinResult.price);
                            coinItem.maxPrice = coin.maxPrice;
                            coinItem.rate = coin.maxPrice / coinResult.price;
                            coinListResult.Add(coinItem);
                        }
                    }
                }
                client.Dispose();
            }

            coinListResult = coinListResult.OrderByDescending(x => x.rate).ToList();

            foreach (var coinResult in coinListResult)
            {
                string stringFormatResult = string.Format(formatCoin, coinResult.pair, RoundNumber(coinResult.rate), coinResult.maxPrice, coinResult.url, coinResult.price);
                result = result + stringFormatResult;
            }

            return result;
        }
    }
}
