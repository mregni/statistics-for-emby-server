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
            TotalEpisodeCounts = new UpdateModel();
        }
        public List<UserStat> UserStats { get; set; }

        public ValueGroup MovieQualities { get; set; }
        public ValueGroup MostActiveUsers { get; set; }
        public ValueGroup TotalUsers { get; set; }

        public ValueGroup TotalMovies { get; set; }
        public ValueGroup TotalBoxsets { get; set; }
        public ValueGroup TotalMovieStudios { get; set; }
        public ValueGroup BiggestMovie { get; set; }
        public ValueGroup LongestMovie { get; set; }
        public ValueGroup OldestMovie { get; set; }
        public ValueGroup NewestMovie { get; set; }
        public ValueGroup HighestRating { get; set; }
        public ValueGroup LowestRating { get; set; }
        public ValueGroup NewestAddedMovie { get; set; }

        public ValueGroup TotalShows { get; set; }
        public ValueGroup TotalOwnedEpisodes { get; set; }
        public ValueGroup TotalShowStudios { get; set; }
        public ValueGroup BiggestShow { get; set; }
        public ValueGroup LongestShow { get; set; }
        public ValueGroup NewestAddedEpisode { get; set; }


        public ChartModel DayOfWeekChart { get; set; }
        public ChartModel HourOfDayChart { get; set; }
        public string LastUpdated { get; set; }
        public string Version { get; set; }

        public UpdateModel TotalEpisodeCounts { get; set; }
        public bool IsTheTvdbCallFailed { get; set; }
    }
}
