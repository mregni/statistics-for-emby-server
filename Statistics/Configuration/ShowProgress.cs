﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;

namespace Statistics.Configuration
{
    public class ShowProgress
    {
        public string Name { get; set; }
        public string SortName { get; set; }
        public string StartYear { get; set; }
        public decimal Watched { get; set; }
        public float? Score { get; set; }
        public SeriesStatus? Status { get; set; }
        public int Episodes { get; set; }
        public int SeenEpisodes { get; set; }
        public int Specials { get; set; }
        public int SeenSpecials { get; set; }
        public decimal Collected { get; set; }
        public decimal Total { get; set; }
    }
}
