using Statistics.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics.Models
{
    public class MovieQuality
    {
        public List<Movie> Movies { get; set; }
        public VideoQuality Quality { get; set; }
    }
}
