function showInfo(text, title) {
	Dashboard.alert({ message: text, title: title });
}

(function (c) {
	c.fn.stupidtable = function (b) { return this.each(function () { var a = c(this); b = b || {}; b = c.extend({}, c.fn.stupidtable.default_sort_fns, b); a.data("sortFns", b); a.on("click.stupidtable", "thead th span", function () { c(this).parent().stupidsort() }) }) }; c.fn.stupidsort = function (b) {
		var a = c(this), g = 0, f = c.fn.stupidtable.dir, e = a.closest("table"), k = a.data("sort") || null; if (null !== k) {
			a.parents("tr").find("th").slice(0, c(this).index()).each(function () { var a = c(this).attr("colspan") || 1; g += parseInt(a, 10) }); var d; 1 == arguments.length ?
d = b : (d = b || a.data("sort-default") || f.ASC, a.data("sort-dir") && (d = a.data("sort-dir") === f.ASC ? f.DESC : f.ASC)); if (a.data("sort-dir") !== d) return a.data("sort-dir", d), e.trigger("beforetablesort", { column: g, direction: d }), e.css("display"), setTimeout(function () {
	var b = [], l = e.data("sortFns")[k], h = e.children("tbody").children("tr"); h.each(function (a, d) { var e = c(d).children().eq(g), f = e.data("sort-value"); "undefined" === typeof f && (f = e.text(), e.data("sort-value", f)); b.push([f, d]) }); b.sort(function (a, b) {
		return l(a[0],
		b[0])
	}); d != f.ASC && b.reverse(); h = c.map(b, function (a) { return a[1] }); e.children("tbody").append(h); e.find("th").data("sort-dir", null).removeClass("sorting-desc sorting-asc"); a.data("sort-dir", d).addClass("sorting-" + d); e.trigger("aftertablesort", { column: g, direction: d }); e.css("display")
}, 10), a
		}
	}; c.fn.updateSortVal = function (b) { var a = c(this); a.is("[data-sort-value]") && a.attr("data-sort-value", b); a.data("sort-value", b); return a }; c.fn.stupidtable.dir = { ASC: "asc", DESC: "desc" }; c.fn.stupidtable.default_sort_fns =
	{ "int": function (b, a) { return parseInt(b, 10) - parseInt(a, 10) }, "float": function (b, a) { return parseFloat(b) - parseFloat(a) }, string: function (b, a) { return b.toString().localeCompare(a.toString()) }, "string-ins": function (b, a) { b = b.toString().toLocaleLowerCase(); a = a.toString().toLocaleLowerCase(); return b.localeCompare(a) } }
})(jQuery);


var ShowProgressPage = {
	pluginId: '291d866f-baad-464a-aed6-a4a8b95a8fd7',
	loadStats: function (page, user) {
		Dashboard.showLoadingMsg();
		ApiClient.getPluginConfiguration(ShowProgressPage.pluginId).then(function (config) {
			if (config.LastUpdated === undefined) {
				Dashboard.alert({ message: 'No configuration found, please run the statistics task on the Scheduled Tasks page and come back for the results.' });
				Dashboard.hideLoadingMsg();
			} else {
				$("#ShowsTable tbody").html("");
				const userStat = $.grep(config.UserStats, function (v) { return v.UserName === user; })[0];
				$.each(userStat.ShowProgresses, function (i, v) {
					var html = '<tr>';
					html += '<td data-sort-value="' + v.SortName + '">' + v.Name + '</td>';
					html += '<td>' + v.StartYear + '</td>';
					html += '<td data-sort-value="' + v.Watched + '" class="center ' + ShowProgressPage.calculateProgressClass(v.Watched) + '">' + v.SeenEpisodes + ' / ' + v.Episodes + ' (' + v.Watched + '%)';
					if (v.SeenSpecials > 0)
						html += ' +' + v.SeenSpecials + ' sp';
					html += '</td>';
					html += '<td data-sort-value="' + v.Collected + '" class="center ' + ShowProgressPage.calculateProgressClass(v.Collected) + '">' + v.Episodes + ' / ' + v.Total + ' (' + v.Collected + '%)';
					if (v.Specials > 0)
						html += ' +' + v.Specials + ' sp';
					html+= '</td>';
					html += '<td>' + v.Score + '</td>';
					html += '<td>' + v.Status + '</td>';
					html += '</tr>';
					$("#ShowsTable tbody").append(html);
				});

				Dashboard.hideLoadingMsg();
			}
		});
	},
	calculateProgressClass: function (value) {
		if (value == 0)
			return "";
		else if (value < 40)
			return "progress-20";
		else if (value < 60)
			return "progress-40";
		else if (value < 80)
			return "progress-60";
		else if (value < 100)
			return "progress-80";
		else
			return "progress-100";
	}
};

var table;

$('.ShowProgressUserBasedPage').on('pageinit', function (event) {
	var page = this;
	$('#selectUserShowProgress', page).on('change', function () {
		const user = $(this).find("option:selected").text();
		ShowProgressPage.loadStats(page, user);


		$("#defaultColumn span").parent().stupidsort('asc');
	});
});

$('.ShowProgressUserBasedPage').on('pageshow', function (event) {
	var page = this;

	table = $("#ShowsTable").stupidtable();

	table.on("aftertablesort", function (event, data) {
		console.log("SORTED");
		var th = $(this).find("th");
		th.removeClass("selectLabelFocused");
		th.eq(data.column).addClass('selectLabelFocused');
	});

	$("#defaultColumn span").parent().stupidsort('asc');

	ApiClient.getUsers().then(function (users) {
		$('#selectUserShowProgress', page).html(users.map(function (user) {
			return '<option value="' + user.Id + '">' + user.Name + '</option>';
		})).selectmenu('refresh').trigger('change');
	});
});