using System;

namespace DiabloIIIHotkeys
{
    internal class KeypressParameters : IEquatable<KeypressParameters>
    {
        public string Key { get; }
        public int Interval { get; }
        public DateTime NextRunTime { get; private set; }

        public KeypressParameters(string key, int interval)
        {
            Key = key;
            Interval = interval;
        }

        public void UpdateNextRunTime()
        {
            NextRunTime = DateTime.Now.AddMilliseconds(Interval);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Interval.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is KeypressParameters parameters))
            {
                return false;
            }

            return Equals(parameters);
        }

        public bool Equals(KeypressParameters other)
        {
            if (other == null)
            {
                return false;
            }

            return String.Compare(Key, other.Key, true) == 0 && Interval == other.Interval;
        }
    }
}
