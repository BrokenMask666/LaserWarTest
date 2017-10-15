using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserwarTest.Presentation.Games.Comparers
{
    public class PlayerComparer : IComparer<Player>
    {
        public bool SortByDesc { get; }

        public PlayerComparer(bool desc = false)
        {
            SortByDesc = desc;
        }

        public virtual int Compare(Player x, Player y)
        {
            Player first = (SortByDesc) ? y : x;
            Player second = (SortByDesc) ? x : y;

            return first.Name.CompareTo(second.Name);
        }
    }

    public class PlayerByRatingComparer : PlayerComparer
    {
        public PlayerByRatingComparer(bool desc = false) : base(desc)
        {
        }

        public override int Compare(Player x, Player y)
        {
            Player first = (SortByDesc) ? y : x;
            Player second = (SortByDesc) ? x : y;

            int result = first.Rating.CompareTo(second.Rating);
            if (result == 0) return base.Compare(x, y);

            return result;
        }
    }

    public class PlayerByAccuracComparer : PlayerComparer
    {
        public PlayerByAccuracComparer(bool desc = false) : base(desc)
        {
        }

        public override int Compare(Player x, Player y)
        {
            Player first = (SortByDesc) ? y : x;
            Player second = (SortByDesc) ? x : y;

            int result = first.AccuracyPercentage.CompareTo(second.AccuracyPercentage);
            if (result == 0) return base.Compare(x, y);

            return result;
        }
    }

    public class PlayerByShotsComparer : PlayerComparer
    {
        public PlayerByShotsComparer(bool desc = false) : base(desc) { }

        public override int Compare(Player x, Player y)
        {
            Player first = (SortByDesc) ? y : x;
            Player second = (SortByDesc) ? x : y;

            int result = first.Shots.CompareTo(second.Shots);
            if (result == 0) return base.Compare(x, y);

            return result;
        }
    }
}
