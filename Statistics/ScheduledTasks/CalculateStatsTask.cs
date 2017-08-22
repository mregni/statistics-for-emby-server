using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Tasks;
using Statistics.Api;
using Statistics.Configuration;
using Statistics.Helpers;
using Statistics.Models.Chart;
using Statistics.ViewModel;

namespace Statistics.ScheduledTasks
{
    public class CalculateStatsTask : IScheduledTask
    {
        private readonly IFileSystem _fileSystem;
        private readonly IHttpClient _httpClient;
        private readonly ILibraryManager _libraryManager;
        private readonly ILogger _logger;
        private readonly IMemoryStreamFactory _memoryStreamProvider;
        private readonly IServerApplicationPaths _serverApplicationPaths;
        private readonly IUserDataManager _userDataManager;
        private readonly IUserManager _userManager;
        private readonly IZipClient _zipClient;

        public CalculateStatsTask(ILogManager logger,
            IUserManager userManager,
            IUserDataManager userDataManager,
            ILibraryManager libraryManager, IZipClient zipClient, IHttpClient httpClient, IFileSystem fileSystem,
            IMemoryStreamFactory memoryStreamProvider, IServerApplicationPaths serverApplicationPaths)
        {
            _logger = logger.GetLogger("Statistics");
            _libraryManager = libraryManager;
            _userManager = userManager;
            _userDataManager = userDataManager;
            _zipClient = zipClient;
            _httpClient = httpClient;
            _fileSystem = fileSystem;
            _memoryStreamProvider = memoryStreamProvider;
            _serverApplicationPaths = serverApplicationPaths;
        }

        private static PluginConfiguration PluginConfiguration => Plugin.Instance.Configuration;

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            var users = _userManager.Users.ToList();

            // No users found, so stop the task
            if (users.Count == 0)
            { 
                _logger.Info("No Users returned");
                return;
            }

            // clear all previously saved stats
            PluginConfiguration.UserStats = new List<UserStat>();
            Plugin.Instance.SaveConfiguration();

            var ver = System.Reflection.Assembly.GetAssembly(typeof(CalculateStatsTask)).GetName().Version;
            PluginConfiguration.Version = ver.ToString();

            // purely for progress reporting
            var percentPerUser = 100 / (users.Count + 3);
            var numComplete = 0;

            PluginConfiguration.LastUpdated = DateTime.Now.ToString("g", Thread.CurrentThread.CurrentCulture);

            numComplete++;
            progress.Report(percentPerUser * numComplete);

            CalculateTotalEpisodes(cancellationToken);

            numComplete++;
            progress.Report(percentPerUser * numComplete);

            var activeUsers = new Dictionary<string, RunTime>();

            foreach (var user in users)
            {
                await Task.Run(() =>
                {
                    using (var calculator = new Calculator(user, _userManager, _libraryManager, _userDataManager))
                    {
                        var overallTime = calculator.CalculateOverallTime();
                        activeUsers.Add(user.Name, new RunTime(overallTime.Raw));
                        var stat = new UserStat
                        {
                            UserName = user.Name,
                            OverallStats = new List<ValueGroup>
                            {
                                overallTime,
                                calculator.CalculateOverallTime(false)
                            },
                            MovieStats = new List<ValueGroup>
                            {
                                calculator.CalculateTotalMovies(),
                                calculator.CalculateTotalBoxsets(),
                                calculator.CalculateTotalMoviesWatched(),
                                calculator.CalculateFavoriteYears(),
                                calculator.CalculateFavoriteMovieGenres(),
                                calculator.CalculateMovieTime(),
                                calculator.CalculateMovieTime(false),
                                calculator.CalculateLastSeenMovies()
                            },
                            ShowStats = new List<ValueGroup>
                            {
                                calculator.CalculateTotalShows(),
                                calculator.CalculateTotalOwnedEpisodes(),
                                calculator.CalculateTotalEpiosodesWatched(),
                                calculator.CalculateTotalFinishedShows(PluginConfiguration.TotalEpisodeCounts),
                                calculator.CalculateFavoriteShowGenres(),
                                calculator.CalculateShowTime(),
                                calculator.CalculateShowTime(false),
                                calculator.CalculateLastSeenShows()
                            },
                            ShowProgresses =
                                new ShowProgressCalculator(_userManager, _libraryManager, _userDataManager, _zipClient, _httpClient, _fileSystem, _memoryStreamProvider, _serverApplicationPaths, user)
                                    .CalculateShowProgress(PluginConfiguration.TotalEpisodeCounts)
                        };
                        PluginConfiguration.UserStats.Add(stat);
                    }
                }, cancellationToken);

                numComplete++;
                progress.Report(percentPerUser * numComplete);
            }

            using (var calculator = new Calculator(null, _userManager, _libraryManager, _userDataManager))
            {
                PluginConfiguration.MovieQualities = calculator.CalculateMovieQualities();
                PluginConfiguration.MostActiveUsers = calculator.CalculateMostActiveUsers(activeUsers);
                PluginConfiguration.TotalUsers = calculator.CalculateTotalUsers();

                PluginConfiguration.DayOfWeekChart = calculator.CalculateDayOfWeekForAllUsersChart();
                PluginConfiguration.HourOfDayChart = calculator.CalculateHourForAllUsersChart();

                PluginConfiguration.TotalMovies = calculator.CalculateTotalMovies();
                PluginConfiguration.TotalBoxsets = calculator.CalculateTotalBoxsets();
                PluginConfiguration.TotalMovieStudios = calculator.CalculateTotalMovieStudios();
                PluginConfiguration.BiggestMovie = calculator.CalculateBiggestMovie();
                PluginConfiguration.LongestMovie = calculator.CalculateLongestMovie();
                PluginConfiguration.OldestMovie = calculator.CalculateOldestMovie();
                PluginConfiguration.NewestMovie = calculator.CalculateNewestMovie();
                PluginConfiguration.HighestRating = calculator.CalculateHighestRating();
                PluginConfiguration.LowestRating = calculator.CalculateLowestRating();
                PluginConfiguration.NewestAddedMovie = calculator.CalculateNewestAddedMovie();

                PluginConfiguration.TotalShows = calculator.CalculateTotalShows();
                PluginConfiguration.TotalOwnedEpisodes = calculator.CalculateTotalOwnedEpisodes();
                PluginConfiguration.TotalShowStudios = calculator.CalculateTotalShowStudios();
                PluginConfiguration.BiggestShow = calculator.CalculateBiggestShow();
                PluginConfiguration.LongestShow = calculator.CalculateLongestShow();
                PluginConfiguration.NewestAddedEpisode = calculator.CalculateNewestAddedEpisode();
            }

            numComplete++;
            progress.Report(percentPerUser * numComplete);

            Plugin.Instance.SaveConfiguration();
        }

        private void CalculateTotalEpisodes(CancellationToken cancellationToken)
        {
            var seriesList = _libraryManager.GetItemList(new InternalItemsQuery()
            {
                IncludeItemTypes = new[] { typeof(Series).Name },
                Recursive = true,
                GroupByPresentationUniqueKey = false

            }).Cast<Series>()
            .ToList();

            var seriesIdsInLibrary = seriesList
                .Where(i => !string.IsNullOrEmpty(i.GetProviderId(MetadataProviders.Tvdb)))
                .Select(i => i.GetProviderId(MetadataProviders.Tvdb));

            var calculator = new ShowProgressCalculator(_userManager, _libraryManager, _userDataManager, _zipClient, _httpClient, _fileSystem, _memoryStreamProvider, _serverApplicationPaths);

            var time = PluginConfiguration.TotalEpisodeCounts.LastUpdateTime;
            bool callFailed;
            //first run
            if (string.IsNullOrEmpty(time))
            {
                callFailed = FirstTvdbConnection(calculator, seriesIdsInLibrary, cancellationToken);
            }
            else
            {
                callFailed = UpdateTvdbConnection(calculator, time, seriesIdsInLibrary, cancellationToken);
            }

            PluginConfiguration.TotalEpisodeCounts.LastUpdateTime = calculator.GetServerTime(cancellationToken);
            PluginConfiguration.IsTheTvdbCallFailed = callFailed;
            Plugin.Instance.SaveConfiguration();
        }

        private bool FirstTvdbConnection(ShowProgressCalculator calculator, IEnumerable<string> seriesIdsInLibrary, CancellationToken cancellationToken )
        {
            var totals = calculator.CalculateTotalEpisodes(seriesIdsInLibrary, cancellationToken);
            PluginConfiguration.TotalEpisodeCounts.IdList = totals;
            return calculator.IsCalculationFailed;
        }

        private bool UpdateTvdbConnection(ShowProgressCalculator calculator, string time, IEnumerable<string> seriesIdsInLibrary, CancellationToken cancellationToken)
        {
            if (PluginConfiguration.TotalEpisodeCounts?.IdList == null)
                return true;

            var existingShows = PluginConfiguration.TotalEpisodeCounts.IdList.Select(x => x.ShowId).ToList();
            var updatedList = calculator.GetShowsToUpdate(existingShows, time, cancellationToken).ToList();
            if (calculator.IsCalculationFailed)
                return true;

            var newShows = seriesIdsInLibrary.Except(existingShows, StringComparer.OrdinalIgnoreCase).ToList();
            var updatedTotals = calculator.CalculateTotalEpisodes(updatedList, cancellationToken);
            if (calculator.IsCalculationFailed)
                return true;

            foreach (var showId in updatedList)
            {
                PluginConfiguration.TotalEpisodeCounts.IdList.First(x => x.ShowId == showId).Count = updatedTotals.First(x => x.ShowId == showId).Count;
            }

            var newTotals = calculator.CalculateTotalEpisodes(newShows, cancellationToken);
            if (calculator.IsCalculationFailed)
                return true;

            foreach (var showId in newShows)
            {
                PluginConfiguration.TotalEpisodeCounts.IdList.Add(new UpdateShowModel(showId, newTotals.First(x => x.ShowId == showId).Count));
            }

            return false;
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            var trigger = new TaskTriggerInfo
            {
                Type = TaskTriggerInfo.TriggerDaily,
                TimeOfDayTicks = TimeSpan.FromHours(0).Ticks
            }; //12am

            return new[] {trigger};
        }

        public string Name => "Calculate statistics for all users";
        public string Key => "StatisticsCalculateStatsTask";

        public string Description
            => "Task that will calculate statistics needed for the statistics plugin for all users.";

        public string Category => "Statistics";
    }
}