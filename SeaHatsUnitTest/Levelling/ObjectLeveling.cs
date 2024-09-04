using SeaHats.Leveling;
using SeaHats.Levels;

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
    }
}
