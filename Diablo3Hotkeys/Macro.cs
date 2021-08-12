using System;

namespace DiabloIIIHotkeys
{
    internal class Macro : IEquatable<Macro>
    {
        public Skill Skill { get; set; }
        public int Interval { get; set; }

        public Macro(Skill skill, int interval)
        {
            Skill = skill;
            Interval = interval;
        }

        public override string ToString()
        {
            return $"{Skill}: {Interval}";
        }

        public override int GetHashCode()
        {
            return Skill.GetHashCode() ^ Interval.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Macro macro))
            {
                return false;
            }

            return Equals(macro);
        }

        public bool Equals(Macro other)
        {
            if (other == null)
            {
                return false;
            }

            return Skill == other.Skill && Interval == other.Interval;
        }
    }
}
