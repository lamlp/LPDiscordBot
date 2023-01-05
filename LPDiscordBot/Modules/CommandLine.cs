using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LPDiscordBot.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LPDiscordBot.Modules
{
    public class CommandLine: ModuleBase<ICommandContext>
    {
        private readonly Mir4Service _mir4Service;
        private readonly AudioService _audioService;
        private readonly DiscordSocketClient _client;
        private bool IsStarted = false;
        private bool CoinIsStarted = false;

        public CommandLine(DiscordSocketClient client, Mir4Service mir4Service, AudioService audioService)
        {
            _client = client;
            _mir4Service = mir4Service;
            _audioService = audioService;
        }

        [Command("start")]
        public async Task InitialService()
        {
            if (IsStarted == true)
            {
                await Task.CompletedTask;
            }
            IsStarted = true;
            var task1 = Task.Run(() => SetStatusPrice());
            var task2 = Task.Run(() => CheckNFTPrice());
        }

        private async void SetStatusPrice()
        {
            while (true)
            {
                Thread.Sleep(10000);
                var wePrice = await _mir4Service.GetPriceWemixLatest(true);
                var hyPrice = await _mir4Service.GetPriceHydraLatest(true);
                var dracoPrice = await _mir4Service.GetPriceDracoLatest(true);
                await _client.SetGameAsync(String.Format("Wemix (Gate.io): {0}$,\nHydra: {1}$,\nDraco: {2}$ \nLast Update: {3}", wePrice, hyPrice, dracoPrice, DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")), type: ActivityType.Playing);
            }
        }

        private async void CheckNFTPrice()
        {
            while (true)
            {
                var result = _mir4Service.GetNFTCharactersWithLowPrice();
                if (!string.IsNullOrEmpty(result))
                {
                    await ReplyAsync("<@&950231175975301230>");
                    await SendEmbedMessage(" " + result);
                }
                Thread.Sleep(1000);
            }
        }

        [Command("Hello")]
        public async Task Hello()
        {
            await ReplyAsync("Chào mừng " + Context.Message.Author.Mention + " đến với Dạ Hành Mir4. ");
        }

        [Command("draco")]
        public async Task PriceDraco()
        {
            var result = _mir4Service.GetPriceDracoLatest();
            await SendEmbedMessage(result.Result);
        }

        [Command("hydra")]
        public async Task PriceHydra()
        {
            var result = _mir4Service.GetPriceHydraLatest();
            await SendEmbedMessage(result.Result);
        }

        [Command("wemix")]
        public async Task PriceWemix()
        {
            var result = _mir4Service.GetPriceWemixLatest();
            await SendEmbedMessage(result.Result);
        }

        [Command("usd")]
        public async Task PriceUsd()
        {
            var result = _mir4Service.GetPriceUsdLatest();
            await SendEmbedMessage("Giá Usd là: " + result.Result + " VND");
        }

        [Command("rank")]
        public async Task GetRanking([Remainder] string parameters)
        {
            String[] input = parameters.Split(' ');
            var server = input[0];
            var count = int.Parse(input[1]);

            if (server.ToUpper() == "ALL")
            {
                var botAllMsg = await ReplyAsync("Xử lý dữ liệu rất lâu, vui lòng chờ ít phút!");
                var result = _mir4Service.GetRankingAllServer(count);
                await botAllMsg.DeleteAsync();
                await SendEmbedMessage("Bảng xếp hạng tất cả server là: \n" + result);
                return;
            }

            var serverId = _mir4Service.GetServerId(server);
            if (string.IsNullOrEmpty(serverId))
            {
                await ReplyAsync("Không tìm thấy server " + server);
                return;
            }

            if (count <= 50)
            {
                var result = _mir4Service.GetRankingServer(serverId, 1, count);
                await SendEmbedMessage("Bảng xếp hạng " + server + " là: \n" + result.Result);
            } 
            else if (count > 50)
            {
                var result = _mir4Service.GetRankingServer(serverId, 1, 50);
                await SendEmbedMessage("Bảng xếp hạng " + server + " là: \n" + result.Result);
                var result2 = _mir4Service.GetRankingServer(serverId, 51, count);
                await SendEmbedMessage(result2.Result);
            }
        }

        [Command("redmoon", RunMode = RunMode.Async)]
        public async Task RedmoonMineSpot([Remainder] string parameters)
        {
            await HandleMineSpot("redmoon", parameters);
        }

        [Command("snake", RunMode = RunMode.Async)]
        public async Task SnakeMineSpot([Remainder] string parameters)
        {
            await HandleMineSpot("snake", parameters);
        }

        [Command("bicheon", RunMode = RunMode.Async)]
        public async Task BicheonMineSpot([Remainder] string parameters)
        {
            await HandleMineSpot("bicheon", parameters);
        }

        [Command("coin")]
        public async Task InitialCoinChecking()
        {
            if (CoinIsStarted == true)
            {
                await Task.CompletedTask;
            }
            CoinIsStarted = true;
            var task1 = Task.Run(() => CoinChecking());
        }

        private async void CoinChecking()
        {
            while (true)
            {
                var result = await _mir4Service.GetPriceScaleCoinTop();
                if (!string.IsNullOrEmpty(result))
                {
                    await SendEmbedMessage(" " + result);
                }
                Thread.Sleep(10000);
            }
        }

        // You *MUST* mark these commands with 'RunMode.Async'
        // otherwise the bot will not respond until the Task times out.
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd()
        {
            await _audioService.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }

        // Remember to add preconditions to your commands,
        // this is merely the minimal amount necessary.
        // Adding more commands of your own is also encouraged.
        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
            await _audioService.LeaveAudio(Context.Guild);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string text)
        {
            await _audioService.SendAudioAsync(Context.Guild, Context.Channel, (Context.User as IVoiceState).VoiceChannel, text);
        }

        private async Task HandleMineSpot(string map, string parameters)
        {
            String[] input = parameters.Split(' ');
            var floor = input[0];
            var time = input[1];
            var area = (floor.ToLower() == "3f" || floor.ToLower() == "4f") ? (input.Length < 3 ? "random" : input[2]) : "";
            var result = _mir4Service.AlertMineSpot(map, floor, area, time);
            var finishedTime = _mir4Service.HandleFinishedTime(time);
            var endTime = finishedTime.Add(new TimeSpan(0, 05, 00));
            var deplayTime = finishedTime.Subtract(DateTime.Now.TimeOfDay);
            var botMsg = await ReplyAsync("Đã ghi nhận...");
            Thread.Sleep(2000);
            await botMsg.DeleteAsync();
            Thread.Sleep(deplayTime);
            await SendEmbedMessage(Context.Message.Author.Mention + " " + result.Item1 + " (" + endTime.ToString(@"hh\:mm") + ")");
            await _audioService.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await _audioService.SendAudioAsync(Context.Guild, Context.Channel, @$"audio\{result.Item2}");
            await _audioService.LeaveAudio(Context.Guild);
        }

        private async Task SendEmbedMessage(string message)
        {
            var eb = new EmbedBuilder();
            eb.WithDescription(message);
            await ReplyAsync("", false, eb.Build());
        }
    }
}
