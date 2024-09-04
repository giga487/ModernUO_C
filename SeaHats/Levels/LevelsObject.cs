using SeaHats.Leveling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHats.Levels
{
    public class LevelObject
    {
        ILevelSystem _service { get; set; } = null;
        public LevelObject(ILevelSystem service)
        {
            _service = service;

            if (_service.LevelExperiences.TryGetValue(Level + 1, out var experiencePerLevel))
            {
                ExperienceToReach = experiencePerLevel;
            }
        }
        public int Level { get; private set; } = 1;

        public double ExperienceToReach { get; private set; } = 0;
        /// <summary>
        /// experience should be positive or negative
        /// </summary>
        /// <param name="experience"></param>
        /// <returns></returns>
        public ExperienceResult AddExperience(double experience)
        {
            ExperienceResult result = ExperienceResult.Bad;

            if (_service.LevelExperiences.TryGetValue(Level, out var experiencePerLevel))
            {
                if (Level == _service.MaxLevel || experience == 0)
                {
                    return ExperienceResult.NotNeeded;
                }

                TotaleExperience += experience;

                int oldLevel = Level;
                Level = _service.GetLevel(TotaleExperience);
                double totalExp = _service.GetTotalExperience(Level);

                LevelExperience = TotaleExperience - totalExp;

                if (Level > oldLevel)
                {
                    result = ExperienceResult.NewLevelUP;
                }
                else if(Level < oldLevel)
                {
                    result = ExperienceResult.NewLevelDOWN;
                }
                else if (experience > 0)
                {
                    result = ExperienceResult.AddOK;
                }
                else if (experience < 0)
                {
                    result = ExperienceResult.RemoveOk;
                }
            }

            return result;
        }

        public double TotaleExperience { get; private set; } = 0;
        public double LevelExperience { get; private set; } = 0;
    }
}
