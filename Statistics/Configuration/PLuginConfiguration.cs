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
            GeneralStat = new List<ValueGroup>();
            MovieStat = new List<ValueGroup>();
            ShowStat = new List<ValueGroup>();
            Charts = new List<ChartModel>();
            TotalEpisodeCounts = new UpdateModel();
        }
        public List<UserStat> UserStats { get; set; }
        public List<ValueGroup> GeneralStat { get; set; }
        public List<ValueGroup> MovieStat { get; set; }
        public List<ValueGroup> ShowStat { get; set; }
        public List<ChartModel> Charts { get; set; }
        public string LastUpdated { get; set; }

        public UpdateModel TotalEpisodeCounts { get; set; }
        public bool IsTheTvdbCallFailed { get; set; }
    }
}
