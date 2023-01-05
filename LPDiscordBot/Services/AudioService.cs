using Discord;
using Discord.Audio;
using LPDiscordBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace LPDiscordBot.Services
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private AudioOutStream _globalStream = null;
        private IGuild _guild;
        private IMessageChannel _channel;
        private IVoiceChannel _target;
        private string _part;

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            try
            {
                IAudioClient client;
                if (ConnectedChannels.TryGetValue(guild.Id, out client))
                {
                    return;
                }
                if (target.Guild.Id != guild.Id)
                {
                    return;
                }

                var audioClient = await target.ConnectAsync();

                if (ConnectedChannels.TryAdd(guild.Id, audioClient))
                {
                    // If you add a method to log happenings from this service,
                    // you can uncomment these commented lines to make use of that.
                    await Log($"Connected to voice on {guild.Name}.");
                }
            }
            catch (Exception ex)
            {
                await Log($"Connected to voice on {ex.Message}.");
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
                await Log($"Disconnected from voice on {guild.Name}.");
            }
        }

        private Task Log(string msg)
        {
            Console.ForegroundColor = System.ConsoleColor.Yellow;
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                using (var ffmpeg = CreateProcess(path))
                using (var stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, IVoiceChannel target, string text)
        {
            var dict = new Dictionary<string, string>();
            string zaloApiUrl = "https://api.zalo.ai/v1/tts/synthesize";
            dict.Add("input", text);
            dict.Add("speed", "0.8");
            dict.Add("encode_type", "0");
            dict.Add("speaker_id", "1");
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("apikey", "r8LeY5p3qmBLjUdn9xoTcrqkbE2Qcy1z");

            HttpResponseMessage response = httpClient.PostAsync(zaloApiUrl, new FormUrlEncodedContent(dict)).Result;
            if (response.IsSuccessStatusCode)
            {
                var contents = response.Content.ReadAsStringAsync().Result;
                ZaloModel zaloData = JsonConvert.DeserializeObject<ZaloModel>(contents);
                if (zaloData != null && zaloData.error_code == 0)
                {
                    string audioUrl = zaloData.data.url;
                    string id = Guid.NewGuid().ToString("N");
                    string path = @$"D:\Temp\{id}.wav";
                    await DownloadAudioAsync(audioUrl, path, guild, channel, target);
                }
            }
            httpClient.Dispose();
        }

        private async Task FinishedDownload()
        {
            await PlayVoice(_guild, _channel, _target, _part);
        }

        private async Task PlayVoice(IGuild guild, IMessageChannel channel, IVoiceChannel target, string path)
        {
            //path = @$"D:\Temp\test.mp3";
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }

            IAudioClient client;

            if (!ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                await JoinAudio(guild, target);
                if (ConnectedChannels.TryGetValue(guild.Id, out client))
                {
                    using (var ffmpeg = CreateProcess(path))
                    {
                        if (_globalStream == null)
                        {
                            _globalStream = client.CreatePCMStream(AudioApplication.Music);
                        }
                        await ffmpeg.StandardOutput.BaseStream.CopyToAsync(_globalStream);
                        await _globalStream.FlushAsync();
                        //var localStream = client.CreatePCMStream(AudioApplication.Voice);
                        //await ffmpeg.StandardOutput.BaseStream.CopyToAsync(localStream);
                        //await localStream.FlushAsync();
                    }
                }
            }
            else
            {
                using (var ffmpeg = CreateProcess(path))
                {
                    if (_globalStream == null)
                    {
                        _globalStream = client.CreatePCMStream(AudioApplication.Music);
                    }
                    await ffmpeg.StandardOutput.BaseStream.CopyToAsync(_globalStream);
                    await _globalStream.FlushAsync();
                    //var localStream = client.CreatePCMStream(AudioApplication.Voice);
                    //await ffmpeg.StandardOutput.BaseStream.CopyToAsync(localStream);
                    //await localStream.FlushAsync();
                }
            }
        }

        private Process CreateProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        private async Task DownloadAudioAsync(string url, string filePath, IGuild guild, IMessageChannel channel, IVoiceChannel target)
        {
            var webClient = new WebClient();
            _guild = guild;
            _channel = channel;
            _target = target;
            _part = filePath;
            var task = webClient.DownloadFileTaskAsync(new Uri(url), filePath);

            task.Wait();

            await FinishedDownload();
            webClient.Dispose();
        }

        public async Task PlayYoutube()
        {
            YoutubeClient youtube = new YoutubeClient();
            var StreamManifest = await youtube.Videos.Streams.GetManifestAsync("u_yIGGhubZs");
            var StreamInfo = StreamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            var stream = youtube.Videos.Streams.GetAsync(StreamInfo);
            var memoryStream = new MemoryStream();

            //await Cli.Wrap("ffmpeg")
            //    .WithArguments(" -hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
            //    .WithStandardInputPipe(PipeSource.FromStream(stream))
            //    .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
            //    .ExecuteAsync();
        }
    }
}
