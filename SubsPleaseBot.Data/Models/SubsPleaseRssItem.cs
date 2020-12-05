using System;

namespace SubsPleaseBot.Data.Models
{
    public class SubsPleaseRssItem
    {
        public SubsPleaseRssItem(string title, int season, int episode, Resolution resolution, string link, DateTimeOffset publishDate, string size, string id)
        {
            Title = title;
            Season = season;
            Episode = episode;
            Resolution = resolution;
            Link = link;
            PublishDate = publishDate;
            Size = size;
            Id = id;
        }

        public string Title { get; }
        public int Season { get; }
        public int Episode { get; }
        public Resolution Resolution { get; }

        public string Link { get; }
        public DateTimeOffset PublishDate { get; }
        public string Size { get; }
        public string Id { get; }
    }

    public enum Resolution
    {
        FullHD,
        HD,
        SD
    }
}