function showInfo(text, title) {
	Dashboard.alert({ message: text, title: title });
}

(function () {
	var UserBasedStatsPage = {
		pluginId: '291d866f-baad-464a-aed6-a4a8b95a8fd7',
		loadStats: function (page, user) {
			Dashboard.showLoadingMsg();
			ApiClient.getPluginConfiguration(UserBasedStatsPage.pluginId).then(function (config) {
				$("#UserTitle", page).html("User statistics for " + user);
				$("#OverallTitle", page).html("Overall statistics");
				$("#MovieTitle", page).html("Movie statistics");
				$("#ShowTitle", page).html("Show statistics");
				$("#overallStat", page).html("");
				$("#movieStat", page).html("");
				$("#showStat", page).html("");
				const userStat = $.grep(config.UserStats, function (v) { return v.UserName === user; })[0];
				$.each(userStat.OverallStats, function (i, v) { UserBasedStatsPage.createStatDiv(v, "#overallStat", page); });
				$.each(userStat.MovieStats, function (i, v) { UserBasedStatsPage.createStatDiv(v, "#movieStat", page); });
				$.each(userStat.ShowStats, function (i, v) { UserBasedStatsPage.createStatDiv(v, "#showStat", page); });

				Dashboard.hideLoadingMsg();
			});
		},
		createStatDiv: function (v, div, page) {
			var html = '<div class="col ' + v.Size + '"><div class="statCard"><div class="statCard-content">';

			if (v.ExtraInformation !== undefined)
				html += "<div class=\"infoBlock\" onclick=\"showInfo('" + v.ExtraInformation + "', '" + v.Title + "');\"><i class=\"md-icon\">info_outline</i></div>";

			html += '<div class="statCard-stats-title">' + v.Title + '</div><div class="statCard-stats-number">' + v.ValueLineOne + '</div></div></div></div>';

			$(div, page).append(html);
		}
	};

	$('.UserBasedStatisticsPage').on('pageinit', function (event) {
		var page = this;

		$('#selectUser', page).on('change', function () {
			$("#userStat", page).empty();
			const user = $(this).find("option:selected").text();
			UserBasedStatsPage.loadStats(page, user);
		});
	});

	$('.UserBasedStatisticsPage').on('pageshow', function (event) {
		var page = this;

		ApiClient.getUsers().then(function (users) {
			$('#selectUser', page).html(users.map(function (user) {
				return '<option value="' + user.Id + '">' + user.Name + '</option>';
			})).selectmenu('refresh').trigger('change');
		});
	});
})();