﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

using CefSharp;

using Newtonsoft.Json;

using TwitchBotDb.Temp;

namespace TwitchBotWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Browser.Dispose();
            Cef.Shutdown();

            // ToDo: More thorough testing
            Process[] processes = Process.GetProcessesByName("CefSharp.BrowserSubprocess");
            foreach (Process process in processes)
            {
                if (!process.HasExited)
                {
                    // Kill if subprocesses didn't close gracefully
                    process.Kill();
                }
            }

            Application.Current.Shutdown();
        }

        private void ChromiumWebBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Browser.TitleChanged += Browser_TitleChanged;
            Browser.LoadingStateChanged += Browser_LoadingStateChanged;
            Browser.ConsoleMessage += Browser_ConsoleMessage;

            Browser.ShowDevTools(); // debugging only
        }

        private void Browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            Dispatcher.BeginInvoke((Action) (() => 
            {
                int index = Title.IndexOf("<<Playing>>") > 1 ? Title.IndexOf("<<Playing>>") : Title.IndexOf("<<Paused>>");

                if (index > -1)
                {
                    if (e.Message == "Video is playing")
                        Title = Title.Replace(Title.Substring(index), "<<Playing>>");
                    else if (e.Message == "Video is not playing")
                        Title = Title.Replace(Title.Substring(index), "<<Paused>>");
                }
                else
                {
                    if (e.Message == "Video is playing")
                        Title += " <<Playing>>";
                    else if (e.Message == "Video is not playing")
                        Title += " <<Paused>>";
                }
            }));
        }

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Browser.ExecuteScriptAsync(@"
                var youtubeMoviePlayer = document.getElementById('movie_player');

                var observer = new MutationObserver(function (event) {
                    twitchBotPlaybackStatus(event[0].target.className)   
                })

                observer.observe(youtubeMoviePlayer, {
                    attributes: true, 
                    attributeFilter: ['class'],
                    childList: false, 
                    characterData: false,
                    subtree: false
                })

                function twitchBotPlaybackStatus(mpClassAttr) {
                    if (mpClassAttr.includes('playing-mode')) {
                        console.log('Video is playing');
                    } else if (mpClassAttr.includes('paused-mode') || mpClassAttr.includes('ended-mode')) {
                        console.log('Video is not playing');
                    } else {
                        console.log('Cannot find video player');
                    }
                }
            ");
        }

        private void Browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string playlistId = "PLps6TngL_FGtk-aXhNcMyMbf-PeVvY8Tk"; // ToDo: Make the playlist link dynamic
            Browser.Load($"https://www.youtube.com/playlist?list={playlistId}");
        }

        private void Browser_TitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Title = Browser.Title.Replace("- YouTube", "");

            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (Browser.IsLoaded)
                {
                    CefSharpCache csCache = new CefSharpCache
                    {
                        Url = Browser.Address
                    };

                    // ToDo: Store file name and path into config file
                    string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TwitchBot");
                    string filename = "CefSharpCache.json";

                    Directory.CreateDirectory(filepath);

                    using (StreamWriter file = File.CreateText($"{filepath}\\{filename}"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(file, csCache);
                    }
                }
            }));
        }
    }
}