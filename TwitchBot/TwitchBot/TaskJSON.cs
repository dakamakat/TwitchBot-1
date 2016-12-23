﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Configuration;

namespace TwitchBot
{
    public class TaskJSON
    {
        public static async Task<ChannelJSON> GetChannel(string broadcasterName, string clientID)
        {
            using (HttpClient client = new HttpClient())
            {
                string body = await client.GetStringAsync("https://api.twitch.tv/kraken/channels/" + broadcasterName + "?client_id=" + clientID);
                ChannelJSON response = JsonConvert.DeserializeObject<ChannelJSON>(body);
                return response;
            }
        }

        public static async Task<RootStreamJSON> GetStream(string broadcasterName, string clientID)
        {
            using (HttpClient client = new HttpClient())
            {
                string body = await client.GetStringAsync("https://api.twitch.tv/kraken/streams/" + broadcasterName + "?client_id=" + clientID);
                RootStreamJSON response = JsonConvert.DeserializeObject<RootStreamJSON>(body);
                return response;
            }
        }

        public static async Task<FollowerInfo> GetFollowerInfo(string broadcasterName, string clientID, int followers)
        {
            using (HttpClient client = new HttpClient())
            {
                string body = await client.GetStringAsync("https://api.twitch.tv/kraken/channels/" + broadcasterName + "/follows?limit=" + followers + "&client_id=" + clientID);
                FollowerInfo response = JsonConvert.DeserializeObject<FollowerInfo>(body);
                return response;
            }
        }

        public static async Task<ChatterInfo> GetChatters(string broadcasterName, string clientID)
        {
            using (HttpClient client = new HttpClient())
            {
                string body = await client.GetStringAsync("https://tmi.twitch.tv/group/user/" + broadcasterName + "/chatters" + "?client_id=" + clientID);
                ChatterInfo response = JsonConvert.DeserializeObject<ChatterInfo>(body);
                return response;
            }
        }
    }
}