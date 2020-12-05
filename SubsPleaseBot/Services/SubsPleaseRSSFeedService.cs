using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SubsPleaseBot.Services
{
    public class SubsPleaseRSSFeedService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SubsPleaseRSSFeedService> _logger;
        private readonly Timer _timer;

        private const string rssUrl1080 = "https://subsplease.org/rss/?t&r=1080";
        private const string rssUrl720 = "https://subsplease.org/rss/?t&r=720";
        private const string rssUrlSd = "https://subsplease.org/rss/?t&r=sd";

        public SubsPleaseRSSFeedService(IConfiguration config, ILogger<SubsPleaseRSSFeedService> logger)
        {
            _config = config;
            _logger = logger;
            _timer = new Timer(TimeSpan.FromMinutes(_config.GetValue<int>("TimerInterval")).TotalMilliseconds);
            _timer.Elapsed += OnElapsed;
            _timer.Start();
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}