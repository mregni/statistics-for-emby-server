using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics.Models.Chart;
using Statistics.ViewModel;

namespace Statistics.Configuration
{
    public class UserStat
    {
        public string UserName { get; set; }
        public List<ValueGroup> OverallStats { get; set; }
        public List<ValueGroup> MovieStats { get; set; }
        public List<ValueGroup> ShowStats { get; set; }
        public List<ShowProgress> ShowProgresses { get; set; }
		public ChartModel DayOfWeekChart { get; set; }
        public ChartModel HourOfDayChart { get; set; }

        public UserStat()
        {
            OverallStats = new List<ValueGroup>();
            MovieStats = new List<ValueGroup>();
            ShowStats = new List<ValueGroup>();
            ShowProgresses = new List<ShowProgress>();
        }

       
    }
}
