using System;

namespace Meter
{
    public struct Measurement
    {
        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }
        public bool IsSuccessfull { get; }

        public string ErrorReason { get; }
        public TimeSpan Duration => End - Start;

        public Measurement(DateTimeOffset start, DateTimeOffset end)
        {
            Start = start;
            End = end;
            IsSuccessfull = true;
            ErrorReason = null;
        }
            
        public Measurement(DateTimeOffset start, DateTimeOffset end, string errorReason)
        {
            Start = start;
            End = end;
            IsSuccessfull = false;
            ErrorReason = errorReason;
        }
    }
}
