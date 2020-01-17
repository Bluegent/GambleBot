
namespace ConsoleTester
{
    using System.Collections.Generic;

    using SoulCollector.Entities;
    using SoulCollector.GameControllers;
    using SoulCollector.Output;
    using SoulCollector.SoulEffects;
    using SoulCollector.Utils;

    class Program
    {

        static bool IsInArr(int num, int[] nums)
        {
            foreach (int number in nums)
            {
                if (num == number)
                {
                    return true;
                }
            }
            return false;
        }

        static int GetDiffRan(int[] numbers, int max)
        {
            int result;
            do
            {
                result = Math.Random(0, max);
            }
            while (IsInArr(result, numbers));
            return result;
        }
        static void Main(string[] args)
        {
            string[] names = { "Joseph", "Jotaro", "Jonathan", "Giorno", "Jolyne", "Josuke", "Gappy"};
            ConsoleLogger log = new ConsoleLogger();
            Dependencies dep = new Dependencies(log);
            int playerNum = 2;
            int[] nameNums = new int[playerNum];
            Entity[] players = new Entity[playerNum];
            for (int i = 0; i < players.Length; ++i)
            {
                nameNums[i] = -1;
                players[i] = new BaseEntity(dep);
            }
            
            for (int i = 0; i < nameNums.Length; ++i)
            {
                nameNums[i] = GetDiffRan(nameNums, names.Length);
                players[i].Name = names[nameNums[i]];
            }

            List<Entity> p1 = new List<Entity>();
            List<Entity> p2 = new List<Entity>();
            for (int i = 0; i < playerNum; ++i)
            {
                if (i % 2 == 0)
                {
                    p1.Add(players[i]);
                }
                else
                {
                    p2.Add(players[i]);
                }
            }

            LifeStealEffect eff = new LifeStealEffect(dep,p1[0]);
            p1[0].AddPostHitEffect(eff);
            Party par1 = new Party(p1.ToArray());
            Party par2 = new Party(p2.ToArray());
            BattleController bc = new BattleController(par1, par2, dep);
            bc.BattleLoop();
        }
    }
}
