using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SubsPleaseBot.Data.Models;

namespace SubsPleaseBot.Services
{
    public class SubsPleaseRSSFeedService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SubsPleaseRSSFeedService> _logger;
        private readonly Timer _timer;

        private readonly HashSet<string> unique = new HashSet<string>(50);
        private readonly object uniqueLock = new object();

        private const string rssUrl1080 = "https://subsplease.org/rss/?t&r=1080";
        private const string rssUrl720 = "https://subsplease.org/rss/?t&r=720";
        private const string rssUrlSd = "https://subsplease.org/rss/?t&r=sd";

        public SubsPleaseRSSFeedService(IConfiguration config, ILogger<SubsPleaseRSSFeedService> logger)
        {
            _config = config;
            _logger = logger;
            _timer = new Timer(TimeSpan.FromSeconds(_config.GetValue<int>("TimerInterval")).TotalMilliseconds);
            _timer.Elapsed += OnElapsed;
            _timer.Start();
        }

        private async void OnElapsed(object sender, ElapsedEventArgs e)
        {
            await ReadRssAsync(Resolution.FullHD);
        }

        public async Task AddToUniqueAsync()
        {
            var x = ReadRssAsync(Resolution.FullHD);
            var y = ReadRssAsync(Resolution.HD);
            var z = ReadRssAsync(Resolution.SD);

            await Task.WhenAll(x, y, z);
        }

        private async Task<IReadOnlyCollection<SubsPleaseRssItem>> ReadRssAsync(Resolution resolution)
        {
            string url = resolution switch
            {
                Resolution.FullHD => rssUrl1080,
                Resolution.HD => rssUrl720,
                Resolution.SD => rssUrlSd,
                _ => throw new NotImplementedException(),
            };
            var xmlReader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(xmlReader);
            xmlReader.Close();

            var list = new List<SubsPleaseRssItem>(50);

            foreach (var item in feed.Items)
            {
                string[] x = item.Title.Text.Split(" - ");
                string[] y = string.Join(" - ", x.Take(x.Length - 1)).Split(' ');
                int episode = int.Parse(x[^1].Split()[0]);
                int season = 1;
                string z = y[^1];
                string title;
                if (z.Length == 2 && z[0] == 'S' && char.IsDigit(z[1]))
                {
                    season = int.Parse(z[1].ToString());
                    title = string.Join(' ', y.Skip(1).Take(y.Length - 2));
                }
                else
                {
                    title = string.Join(' ', y.Skip(1).Take(y.Length - 1));
                }
                string size = item.ElementExtensions.FirstOrDefault(ext => ext.OuterName == "size").GetObject<string>();
                list.Add(new(title, season, episode, resolution, item.Links.FirstOrDefault()?.Uri.ToString(), item.PublishDate, size, item.Id));
            }
            List<SubsPleaseRssItem> result;
            lock (uniqueLock)
            {
                result = list.Where(x => !unique.Contains(x.Id)).ToList();
                result.ForEach((e) =>
                {
                    unique.Add(e.Id);
                });
            }
            return await Task.FromResult(result.AsReadOnly());
        }
    }
}