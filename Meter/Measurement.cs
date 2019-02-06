using System;
using System.Collections.Generic;
using System.Text;

namespace Meter
{
    public class Measurement
    {
        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }
        public bool IsSuccessfull { get; }

        public TimeSpan Duration => End - Start;
        public Measurement(DateTimeOffset start, DateTimeOffset end, bool isSuccessfull)
        {
            Start = start;
            End = end;
            IsSuccessfull = isSuccessfull;
        }
    }
}
