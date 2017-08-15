
function showInfo(text, title) {
	Dashboard.alert({ message: text, title: title });
}

(function () {
	require.config({
		paths: {
			chart: "https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.6.0/Chart.bundle"
		},
		waitSeconds: 2
	});

	var MainPage = {
		pluginId: '291d866f-baad-464a-aed6-a4a8b95a8fd7',
		tvdbCallFailed: false,
		createStat: function (i, v) {
			var html = '<div class="col ' +
				v.Size +
				'"><div class="statCard"><div class="statCard-content">';

			if (v.ExtraInformation !== undefined)
				html += "<div class=\"infoBlock\" onclick=\"showInfo('" +
					v.ExtraInformation +
					"', '" +
					v.Title +
					"');\"><i class=\"md-icon\">info_outline</i></div>";

			html += '<div class="statCard-stats-title">' +
				v.Title +
				'</div><div class="statCard-stats-number inside">' +
				v.ValueLineOne +
				'</div><div class="statCard-stats-number inside">' +
				v.ValueLineTwo +
				'</div></div></div></div>';

			return html;
		},
		loadStats: function (page) {
			Dashboard.showLoadingMsg();

			ApiClient.getPluginConfiguration(MainPage.pluginId).then(function (config) {
				if (config.LastUpdated === undefined) {
					Dashboard.alert({
						message:
						'No configuration found, please run the statistics task on the Scheduled Tasks page and come back for the results.'
					});
					$('#GoToUserStats', page).css("display", "none");
					$('#GoToShowProgress', page).css("display", "none");
					Dashboard.hideLoadingMsg();
				} else {
					console.log(config.IsTheTvdbCallFailed);
					MainPage.tvdbCallFailed = config.IsTheTvdbCallFailed;

					$("#statsIntro", page).append('<b>' + config.LastUpdated + '</b>');

					$.each(config.GeneralStat,
						function (i, v) {
							var html = MainPage.createStat(i, v);
							$("#generalStat", page).append(html);
						});

					$.each(config.MovieStat,
						function (i, v) {
							var html = MainPage.createStat(i, v);
							$("#movieStat", page).append(html);
						});


					$.each(config.ShowStat,
						function (i, v) {
							var html = MainPage.createStat(i, v);
							$("#showStat", page).append(html);
						});

					console.log(config);
					console.log(config.DayOfWeekChart);
					var keys = [];
					var movies = [];
					var episodes = [];
					$.each(config.DayOfWeekChart.Values, function (i, v) {
						keys.push(v.Key);
						movies.push(v.Movies);
						episodes.push(v.Episodes);
					});

					MainPage.buildChart(keys, movies, '# of views per day', 'weekMovieChart');
					MainPage.buildChart(keys, episodes, '# of views per day', 'weekEpisodesChart');

					keys = [];
					movies = [];
					episodes = [];
					$.each(config.HourOfDayChart.Values, function (i, v) {
						keys.push(v.Key);
						movies.push(v.Movies);
						episodes.push(v.Episodes);
					});

					MainPage.buildChart(keys, movies, '# of views per hour', 'dayMovieChart');
					MainPage.buildChart(keys, episodes, '# of views per hour', 'dayEpisodesChart');

					Dashboard.hideLoadingMsg();
				}
			});
		},
		buildChart(keys, data, title, div) {
			require(["chart"],
				function (Chart) {
					var colors = [];
					$.each(data, function (i, v) {
						colors.push('rgba(82, 181, 75, 1)');
					})
					var ctx = document.getElementById(div);
					var myBarChart = new Chart(ctx, {
						type: 'bar',
						data: {
							labels: keys,
							datasets: [{
								label: title,
								data: data,
								backgroundColor: colors,
								borderColor: colors,
								borderWidth: 1
							}]
						},
						options: {
							scales: {
								yAxes: [{
									ticks: {
										beginAtZero: true
									}
								}]
							}
						}
					});
				});
		}
	};

	$('.statisticsConfigurationPage').on('pageinit', function (event) {
		var page = this;

		$('#GoToUserStats', page).on('click',
			function () {
				var href = Dashboard.getConfigurationPageUrl("UserBasedStatistics");
				Dashboard.navigate(href);
			});

		$('#GoToMovieQuality', page).on('click',
			function () {
				var href = Dashboard.getConfigurationPageUrl("MovieQualityPage");
				Dashboard.navigate(href);
			});

		$('#GoToMovies', page).on('click',
			function () {
				var href = Dashboard.getConfigurationPageUrl("MoviePage");
				Dashboard.navigate(href);
			});


		$('#GoToShowProgress', page).on('click',
			function () {
				if (MainPage.tvdbCallFailed) {
					Dashboard.alert({
						message:
						'Last time the background calculation task ran it got an error from the TVDB API so no calculations could be made for the show progress. This is mostly because the TVDB server gave a 500 response. <br/><br/>You have to run the task again, if the problem persists wait a bit longer before running the task again.'
					});
				} else {
					var href = Dashboard.getConfigurationPageUrl("UserBasedShowOverview");
					Dashboard.navigate(href);
				}
			});

		MainPage.loadStats(page);
	});
})();