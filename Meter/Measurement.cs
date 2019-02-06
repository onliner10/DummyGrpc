using System;

namespace Meter
{
    public struct Measurement
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
