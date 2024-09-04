using SeaHats.Leveling;
using SeaHats.Levels;
using System.Diagnostics;

namespace SeaHats.UnitTest.Levelling
{
    [TestClass]
    public class LevelingObject
    {
        [TestMethod]
        public void AddExperienceLevelObject()
        {
            LevelService _levelService = new LevelService().CreateLevelConfiguration(maxLevel: 100);

            for (int i = _levelService.MinLevel; i <= _levelService.MaxLevel; i++)
            {
                _levelService.CreateLevelsExp(i, 10 * (i - 1));
            }

            int levelToCalc = 6;
            double exp = _levelService.GetTotalExperience(levelToCalc);

            LevelObject pm = new LevelObject(_levelService);
            Console.WriteLine($"EXP ADDED: {exp} ");
            var result = pm.AddExperience(exp);

            if (pm.Level != levelToCalc)
            {
                Assert.Fail("Wrong level calculus");
            }
            else
                Console.WriteLine($"S: {result} LEVEL: {pm.Level} TOTAL EXPERIENCE: {pm.TotaleExperience} LEVEL EXP: {pm.LevelExperience}");

            pm.AddExperience(exp);

            double exp2 = exp + exp;
            int newLevel = _levelService.GetLevel(exp2);
            Console.WriteLine($"EXP ADDED: {exp} ");

            if (pm.Level != newLevel)
            {
                Assert.Fail("Wrong level calculus");
            }
            else
                Console.WriteLine($"S: {result} LEVEL: {pm.Level} TOTAL EXPERIENCE: {pm.TotaleExperience} LEVEL EXP: {pm.LevelExperience}");
        }

        [TestMethod]
        public void RemoveExperienceLevelObject()
        {
            LevelService _levelService = new LevelService().CreateLevelConfiguration(maxLevel: 100);

            for (int i = _levelService.MinLevel; i <= _levelService.MaxLevel; i++)
            {
                _levelService.CreateLevelsExp(i, 10 * (i - 1));
            }

            int levelToCalc = 50;

            double exp = _levelService.GetTotalExperience(levelToCalc);

            LevelObject pm = new LevelObject(_levelService);
            Console.WriteLine($"EXP ADDED: {exp} ");
            var result = pm.AddExperience(exp);

            if (pm.Level != levelToCalc)
            {
                Assert.Fail("Wrong level calculus add");
            }
            else
                Console.WriteLine($"S: {result} LEVEL: {pm.Level} TOTAL EXPERIENCE: {pm.TotaleExperience} LEVEL EXP: {pm.LevelExperience}");

            double exp2 = -_levelService.LevelExperiences[levelToCalc];

            result = pm.AddExperience(exp2);
            double newExp = exp + exp2;
            int newLevel = _levelService.GetLevel(newExp);
            Console.WriteLine($"EXP ADDED: {exp2} ");

            if (pm.Level != newLevel)
            {
                Assert.Fail("Wrong level calculus remove");
            }
            else
                Console.WriteLine($"S: {result} LEVEL: {pm.Level} TOTAL EXPERIENCE: {pm.TotaleExperience} LEVEL EXP: {pm.LevelExperience}");
        }

        [TestMethod]
        public void IntensiveAddExperience()
        {
            LevelService _levelService = new LevelService().CreateLevelConfiguration(maxLevel: 100);

            for (int i = _levelService.MinLevel; i <= _levelService.MaxLevel; i++)
            {
                _levelService.CreateLevelsExp(i, 100 * (i - 1));
            }

            LevelObject pm = new LevelObject(_levelService);

            for (int i = 0; i < 50000; i++)
            {
                if (pm.Level == _levelService.MaxLevel - 1)
                {

                }

               var result = pm.AddExperience(10);

                if (result == ExperienceResult.NewLevelUP)
                {
                    Console.WriteLine($"GOOD, NEW LEVEL {pm.Level}");
                }

                if(result == ExperienceResult.MaxLevel)
                {
                    Console.WriteLine($"MAX LEVEL REACHED -> {pm.Level}");
                    break;
                }
            }

            Console.WriteLine($"LEVEL:{pm.Level}, now exp: {pm.LevelExperience}, to reach: {pm.ExperienceToReach}");
        }

        [TestMethod]
        public void SetLevelCapAndRemove()
        {
            LevelService _levelService = new LevelService().CreateLevelConfiguration(maxLevel: 100);

            for (int i = _levelService.MinLevel; i <= _levelService.MaxLevel; i++)
            {
                _levelService.CreateLevelsExp(i, 100 * (i - 1));
            }

            LevelObject pm = new LevelObject(_levelService);
            pm.SetLevel(_levelService.MaxLevel);

            Console.WriteLine($"LEVEL:{pm.Level}, now exp: {pm.LevelExperience}, to reach: {pm.ExperienceToReach}");

            double expToRemove = -100;
            Console.WriteLine($"Removing {expToRemove} experience");
            pm.AddExperience(expToRemove);

            Console.WriteLine($"LEVEL:{pm.Level}, now exp: {pm.LevelExperience}, to reach: {pm.ExperienceToReach}");

            double expToAdd = -expToRemove;
            Console.WriteLine($"Add {expToAdd} experience");
            pm.AddExperience(expToAdd);

            Console.WriteLine($"LEVEL:{pm.Level}, now exp: {pm.LevelExperience}, to reach: {pm.ExperienceToReach}");
        }
    }
}
