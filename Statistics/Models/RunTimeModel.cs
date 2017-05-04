﻿using System;
using Statistics.Enum;

namespace Statistics.ViewModel
{
    public class RunTime : IComparable<RunTime>
    {
        private TimeSpan _timeSpan;
        public int Days => _timeSpan.Days;
        public int Hours => _timeSpan.Hours;
        public int Minutes => _timeSpan.Minutes;
        public int Seconds => _timeSpan.Seconds;
        public long Ticks => _timeSpan.Ticks;

        public RunTime(TimeSpan timeSpan = new TimeSpan())
        {
            _timeSpan = timeSpan;
        }

        public RunTime(object ticks)
        {
            _timeSpan = new TimeSpan(Convert.ToInt64(ticks));
        }

        public void Add(TimeSpan timespan)
        {
            _timeSpan = _timeSpan.Add(timespan);
        }

        public void Add(long? ticks)
        {
            _timeSpan = _timeSpan.Add(new TimeSpan(ticks ?? 0));
        }

        public override string ToString()
        {
            return $"<td>{Days}</td><td>{Hours}</td><td>{Minutes}</td>";
        }

        public int CompareTo(RunTime other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return _timeSpan.CompareTo(other._timeSpan);
        }
    }
}
