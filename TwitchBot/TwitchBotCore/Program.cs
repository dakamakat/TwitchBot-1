﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
using System.Net.Http;

using Autofac;

using TwitchBotCore.Config;
using TwitchBotCore.Modules;
using TwitchBotCore.Models;
using TwitchBotCore.Libraries;

namespace TwitchBotCore
{
    class Program
    {
        public static List<DelayedMessage> DelayedMessages = new List<DelayedMessage>(); // used to handle delayed msgs
        public static List<RouletteUser> RouletteUsers = new List<RouletteUser>(); // used to handle russian roulette
        public static readonly HttpClient HttpClient = new HttpClient();

        static void Main(string[] args)
        {
            try
            {
                var appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var botConfigSection = appConfig.GetSection("TwitchBotConfiguration") as TwitchBotConfigurationSection;

                //Create a container builder and register all classes that will be composed for the application
                var builder = new ContainerBuilder();

                // Password from www.twitchapps.com/tmi/
                // include the "oauth:" portion
                // Use chat bot's oauth
                /* main server: irc.chat.twitch.tv, 6667 */
                builder.RegisterModule(new TwitchBotModule()
                {
                    AppConfig = appConfig,
                    TwitchBotApiLink = new NamedParameter("twitchBotApiLink", botConfigSection.TwitchBotApiLink),
                    TwitchBotConfigurationSection = botConfigSection,
                    Irc = new IrcClient(botConfigSection.BotName.ToLower(), botConfigSection.TwitchOAuth, botConfigSection.Broadcaster.ToLower())
                });

                var container = builder.Build();

                //Define main lifetime scope
                //Get an instance of TwitchBotApplication and execute main loop
                using (var scope = container.BeginLifetimeScope())
                {
                    var app = scope.Resolve<TwitchBotApplication>();
                    Task.WaitAll(app.RunAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Local error found: " + ex.Message);
                Thread.Sleep(3000);
                Environment.Exit(1);
            }
        }
    }
}
