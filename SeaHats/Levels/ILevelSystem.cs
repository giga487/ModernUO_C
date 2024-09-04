using SeaHats.Leveling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHats.Levels
{
    public enum ExperienceResult
    {
        NewLevelUP = 0,
        NewLevelDOWN = 5,
        AddOK = 1,
        RemoveOk = 2,
        Bad = 3,
        NotNeeded = 4
    }

    public interface ILevelSystem
    {
        public IReadOnlyDictionary<int, double> LevelExperiences { get; }
        public int MaxLevel { get;}
        public int MinLevel { get;}
        public double GetTotalExperience(int level);
        public int GetLevel(double totalExperience);
    }
}
