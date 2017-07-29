using System.Collections.Generic;

namespace Statistics.Models.Chart
{
    public class ChartModel
    {
        public List<ChartValueModel> Values { get; set; }

        public ChartModel()
        {
            Values = new List<ChartValueModel>();
        }

        public ChartModel(IEnumerable<string> keys )
        {
            Values = new List<ChartValueModel>();

            foreach (var key in keys)
            {
                Values.Add(new ChartValueModel(key));
            }
        }
    }
}
