using SeaHats.Levels;
using Serilog;

namespace SeaHats.Leveling
{

    public class LevelService: ILevelSystem
    {
        public int MaxLevel { get; private set; } = 10;
        public int MinLevel { get; private set; } = 1;

        private Dictionary<int, double > _levelExperiences = new Dictionary<int, double>();
        public IReadOnlyDictionary<int, double> LevelExperiences => _levelExperiences;
        public LevelService()
        {
            Log.Information("Creating Level system");
        }

        /// <summary>
        /// This will create level system from configuration file
        /// </summary>
        /// <param name="levelConfigurationFile"></param>
        public LevelService(string levelConfigurationFile)
        {

        }
        public double GetTotalExperience(int level)
        {
            double totaleExperience = 0;

            for (int i = MinLevel; i <= level; i++)
            {
                totaleExperience += LevelExperiences[i];
            }

            return totaleExperience;
        }

        public int GetLevel(double totalExperience)
        {
            int level = MinLevel;
            int oldLevel = MinLevel;

            double tempTotal = totalExperience;

            for (int i = MinLevel; i < MaxLevel; i++)
            {
                tempTotal -= LevelExperiences[i];

                if (tempTotal < 0)
                {
                    return oldLevel;
                }
                if (tempTotal == 0)
                {
                    return i;
                }

                oldLevel = i;
            }

            return level;
        }

        public LevelService CreateLevelConfiguration(int maxLevel = 10, int minLevel = 1)
        {
            MaxLevel = maxLevel;
            MinLevel = minLevel;
            return this;
        }

        public LevelService CreateLevelsExp(int levelId, double experienceToReach)
        {
            if(_levelExperiences.TryAdd(levelId, experienceToReach))
            {
                _levelExperiences[levelId] = experienceToReach;
            }
            else
            {
                Log.Information("T");
            }
            return this;
        }
    }
}
