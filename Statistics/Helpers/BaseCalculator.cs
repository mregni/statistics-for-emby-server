using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;

namespace Statistics.Helpers
{
    public abstract class BaseCalculator : IDisposable
    {
        private IEnumerable<Movie> _movieCache;
        private IEnumerable<Series> _seriesCache;
        private IEnumerable<Episode> _episodeCache;
        private IEnumerable<BoxSet> _boxsetCache;

        private IEnumerable<Movie> _viewedMovieCache;
        private IEnumerable<Episode> _ownedEpisodeCache;
        private IEnumerable<Episode> _viewedEpisodeCache;



        public readonly IUserManager UserManager;
        public readonly ILibraryManager LibraryManager;
        public readonly IUserDataManager UserDataManager;
        public User User;


        protected BaseCalculator(IUserManager userManager, ILibraryManager libraryManager,
            IUserDataManager userDataManager)
        {
            UserManager = userManager;
            LibraryManager = libraryManager;
            UserDataManager = userDataManager;
        }

        #region Helpers

        protected IEnumerable<Movie> GetAllMovies()
        {
            return _movieCache ?? (_movieCache = GetItems<Movie>());
        }

        protected IEnumerable<Series> GetAllSeries()
        {
            return _seriesCache ?? (_seriesCache = GetItems<Series>());
        }

        protected IEnumerable<Episode> GetAllEpisodes()
        {
            return _episodeCache ?? (_episodeCache = GetItems<Episode>());
        }

        protected IEnumerable<Episode> GetAllOwnedEpisodes()
        {
            return _ownedEpisodeCache ?? (_ownedEpisodeCache = GetOwnedItems<Episode>());
        }

        protected IEnumerable<Episode> GetAllViewedEpisodesByUser()
        {
            return _viewedEpisodeCache ?? (_viewedEpisodeCache = GetOwnedItems<Episode>(true));
        }

        protected IEnumerable<Movie> GetAllViewedMoviesByUser()
        {
            return _viewedMovieCache ?? (_viewedMovieCache = GetOwnedItems<Movie>(true));
        }

        protected List<BaseItem> GetAllBaseItems()
        {
            return GetAllMovies().Union(GetAllEpisodes().Cast<BaseItem>()).ToList();
        }

        protected IEnumerable<User> GetAllUser()
        {
            return UserManager.Users;
        }

        protected int GetOwnedCount(Type type)
        {
            var query = new InternalItemsQuery(User)
            {
                IncludeItemTypes = new[] { type.Name },
                Limit = 0,
                Recursive = true,
                IsVirtualItem = false,
                SourceTypes = new[] { SourceType.Library }
            };

            return LibraryManager.GetCount(query);
        }

        protected IEnumerable<BoxSet> GetBoxsets()
        {
            return _boxsetCache ?? (_boxsetCache = GetItems<BoxSet>());
        }


        #region Queries
        protected int GetOwnedEpisodesCount(Series show)
        {
            var query = new InternalItemsQuery(User)
            {
                IncludeItemTypes = new[] { typeof(Episode).Name },
                Recursive = true,
                ParentId = show.Id,
                IsSpecialSeason = false,
                SourceTypes = new[] { SourceType.Library },
                IsVirtualItem = false
            };

            var episodes = LibraryManager.GetItemList(query).OfType<Episode>().Where(e => e.PremiereDate <= DateTime.Now || e.PremiereDate == null);
            return episodes.Sum(r => (r.IndexNumberEnd == null || r.IndexNumberEnd < r.IndexNumber ? r.IndexNumber : r.IndexNumberEnd) - r.IndexNumber + 1) ?? 0;
        }

        protected int GetPlayedEpisodeCount(Series show)
        {
            var query = new InternalItemsQuery(User)
            {
                IncludeItemTypes = new[] { typeof(Episode).Name },
                Recursive = true,
                ParentId = show.Id,
                IsSpecialSeason = false,
                SourceTypes = new[] { SourceType.Library },
                IsVirtualItem = false,
                IsPlayed = true
            };

            var episodes = LibraryManager.GetItemList(query).OfType<Episode>().Where(e => e.PremiereDate <= DateTime.Now || e.PremiereDate == null);
            return episodes.Sum(r => (r.IndexNumberEnd ?? r.IndexNumber) - r.IndexNumber + 1) ?? 0;
        }

        protected int GetOwnedSpecials(Series show)
        {
            var query = new InternalItemsQuery(User)
            {
                IncludeItemTypes = new[] { typeof(Season).Name },
                Recursive = true,
                ParentId = show.Id,
                IsSpecialSeason = true,
                IsVirtualItem = false,
                SourceTypes = new[] { SourceType.Library }
            };

            var seasons = LibraryManager.GetItemList(query).OfType<Season>();
            return seasons.Sum(x => x.Children.Count(e => e.PremiereDate <= DateTime.Now || e.PremiereDate == null));
        }

        protected int GetPlayedSpecials(Series show)
        {
            var query = new InternalItemsQuery(User)
            {
                IncludeItemTypes = new[] { typeof(Season).Name },
                Recursive = true,
                ParentId = show.Id,
                IsSpecialSeason = true,
                MaxPremiereDate = DateTime.Now,
                IsVirtualItem = false,
                SourceTypes = new[] { SourceType.Library }
            };

            var seasons = LibraryManager.GetItemList(query).OfType<Season>();
            return seasons.Sum(x => x.Children.Count(e => (e.PremiereDate <= DateTime.Now || e.PremiereDate == null) && e.IsPlayed(User)));
        }

        #endregion

        private IEnumerable<T> GetItems<T>()
        {
            var query = new InternalItemsQuery(User)
            {
                IncludeItemTypes = new[] { typeof(T).Name },
                Recursive = true,
                SourceTypes = new[] { SourceType.Library },
                IsVirtualItem = false
            };
            
            return LibraryManager.GetItemList(query).OfType<T>();
        }

        private IEnumerable<T> GetOwnedItems<T>(bool? isPLayed = null)
        {
            var query = new InternalItemsQuery(User)
            {
                IncludeItemTypes = new[] { typeof(T).Name },
                IsPlayed = isPLayed,
                Recursive = true,
                SourceTypes = new[] { SourceType.Library },
                IsVirtualItem = false
            };

            return LibraryManager.GetItemsResult(query).Items.OfType<T>().ToList();
        }

        #endregion

        public void Dispose()
        {
            ClearCache();
        }

        public void SetUser(User user)
        {
            User = user;
        }

        public void ClearCache()
        {
            User = null;
            _episodeCache = null;
            _movieCache = null;
            _boxsetCache = null;
            _ownedEpisodeCache = null;
            _viewedEpisodeCache = null;
            _viewedMovieCache = null;
            _seriesCache = null;
        }
    }
}