using SeaHats.Leveling;
using System.Diagnostics;

namespace SeaHats.UnitTest.Levelling
{
    [TestClass]
    public class LevelingServiceTest
    {
        [TestMethod]
        public void CreateLevelService()
        {
            LevelService levels = new LevelService();

            for (int i = 1; i <= 100; i++)
            {
                levels.CreateLevelsExp(i, 10 * (i - 1));
            }

            foreach (var vKV in levels.LevelExperiences)
            {
                Console.WriteLine($"L[{vKV.Key.ToString("000")}] = {vKV.Value} ");
            }
        }

        [TestMethod]
        public void GetTotalExperiences()
        {
            LevelService _levelService = new LevelService();

            for (int i = 1; i <= 100; i++)
            {
                _levelService.CreateLevelsExp(i, 10 * (i - 1));
            }

            int levelToCalc = 5; //fare il livello 2 3 4 5
            double exp = _levelService.GetTotalExperience(levelToCalc);

            Console.WriteLine($"EXP LEVEL: {levelToCalc} => {exp}");
        }

        [TestMethod]
        public void IntensiveGetTotalExperience()
        {
            LevelService _levelService = new LevelService().CreateLevelConfiguration(maxLevel: 100);

            for (int i = 1; i <= 100; i++)
            {
                _levelService.CreateLevelsExp(i, 10 * (i - 1));
            }

            int levelToCalc = 99; //fare il livello 2 3 4 5

            Stopwatch t = Stopwatch.StartNew();

            int maxTry = 10000;

            for (int i = 0; i < maxTry; i++)
            {
                double exp = _levelService.GetTotalExperience(levelToCalc);
            }

            t.Stop();

            Console.WriteLine($"MAX TRY: {maxTry} in {t.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void GetLevelByExperiences()
        {
            LevelService _levelService = new LevelService();

            for (int i = 1; i <= 100; i++)
            {
                _levelService.CreateLevelsExp(i, 10 * i);
            }

            int levelToCalc = 6;

            int newLevel = levelToCalc + 1;
            double expNew = _levelService.LevelExperiences[newLevel];
            double exp = _levelService.GetTotalExperience(levelToCalc);

            int level = _levelService.GetLevel(exp);

            Console.WriteLine($"EXP {exp} => LEVEL: {level}");

            if (level != levelToCalc)
            {
                Assert.Fail("Calculus is wrong");
            }

            double expLess1 = exp - 1;
            level = _levelService.GetLevel(expLess1);
            Console.WriteLine($"EXP {expLess1} => LEVEL: {level}");

            if (level != levelToCalc - 1)
            {
                Assert.Fail("Calculus is wrong");
            }

            double FINALXP = exp + expNew;
            level = _levelService.GetLevel(FINALXP);
            if (level != newLevel)
            {
                Assert.Fail("Calculus is wrong");
            }

            Console.WriteLine($"EXP {FINALXP} => LEVEL: {level}");
        }
    }
}
