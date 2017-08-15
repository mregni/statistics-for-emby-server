using System;
using System.Collections.Generic;
using MediaBrowser.Model.Plugins;
using Statistics.Api;
using Statistics.Models.Chart;
using Statistics.ViewModel;

namespace Statistics.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public PluginConfiguration()
        {
            UserStats = new List<UserStat>();
            MovieStat = new List<ValueGroup>();
            ShowStat = new List<ValueGroup>();
            TotalEpisodeCounts = new UpdateModel();
        }
        public List<UserStat> UserStats { get; set; }

        public ValueGroup MovieQualities { get; set; }
        public ValueGroup MostActiveUsers { get; set; }
        public ValueGroup TotalUsers { get; set; }

        public List<ValueGroup> MovieStat { get; set; }
        public List<ValueGroup> ShowStat { get; set; }
        public ChartModel DayOfWeekChart { get; set; }
        public ChartModel HourOfDayChart { get; set; }
        public string LastUpdated { get; set; }
        public string Version { get; set; }

        public UpdateModel TotalEpisodeCounts { get; set; }
        public bool IsTheTvdbCallFailed { get; set; }
    }
}
