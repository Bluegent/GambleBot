namespace BotClient.Game
{
    using System;

    public class SlotMachine
    {
        public int LastWin { get; private set; }
        public string LastCombination { get; private set; }
        class WinCombination
        {
            public int Count { get; set; }
            public int Value { get; set; }
            public int Win { get; set; }
        }
        private string[] symbols =
            {
                "<:slot_bar:669201388944752640>",
                "<:slot_seven:669202282260332544>",
                "<:slot_cherry:669202297901023252>",
                "<:slot_orange:669202445657964554>",
                "<:slot_banana:669201857716944906>",
                "<:slot_lemon:669202433259339776>"
            };
        int[] slot1 = { 0, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5 };
        int[] slot2 = { 0, 1, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5 };
        int[] slot3 = { 0, 1, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5 };

        private WinCombination[] winCombinations =
            {
                new WinCombination(){Value = 0,Count = 3,Win = 60},
                new WinCombination(){Value = 1,Count = 3,Win = 40},
                new WinCombination(){Value = 2,Count = 3,Win = 20},
                new WinCombination(){Value = 3,Count = 3,Win = 10},
                new WinCombination(){Value = 4,Count = 3,Win = 10},
                new WinCombination(){Value = 5,Count = 3,Win = 10},
                new WinCombination(){Value = 2, Count = 2,Win = 5},
                new WinCombination(){Value = 2,Count = 1,Win = 1}
            };

        private void Shuffle(ref int[] array)
        {
            for (int i = 0; i < array.Length / 2; i += 2)
            {
                int tmp = array[i];
                array[i] = array[array.Length - 1 - i];
                array[array.Length - 1 - i] = tmp;
            }
        }

        private string GetCombinationString(WinCombination combination)
        {
            return $"{combination.Count}x{symbols[combination.Value]} Win {combination.Win}";
        }
        public string GetWinTable()
        {
            string result = "Win Table\n";
            for (int i = 0; i < winCombinations.Length; ++i)
            {
                result += $"{GetCombinationString(winCombinations[i])}\n";
            }

            result += "Multiply the win by the value you want to bet to find out your winnings.";
            ; return result;
        }



        public int CountValue(int value, int[] values)
        {
            int count = 0;
            for (int i = 0; i < values.Length; ++i)
            {
                if (values[i] == value)
                {
                    ++count;
                }
            }

            return count;
        }
        private WinCombination IsWinCombination(int[] values)
        {
            foreach (var winCombination in winCombinations)
            {
                int count = CountValue(winCombination.Value, values);
                {
                    if (winCombination.Count == count)
                        return winCombination;
                }
            }
            return null;
        }
        private Random random;
        public SlotMachine(Random random)
        {
            this.random = random;
            Shuffle(ref slot1);
            Shuffle(ref slot2);
            Shuffle(ref slot3);

        }

        public string GetSymbol(int index, int[] slot)
        {
            return symbols[slot[index]];
        }

        public void Spin(int bet)
        {
            LastWin = 0;
            string result = "";
            int s1 = random.Next(0, 23);
            int s2 = random.Next(0, 23);
            int s3 = random.Next(0, 23);
            int win = 0;
            int[] combination = new int[] { slot1[s1], slot2[s2], slot3[s3] };
            WinCombination resultCombination = IsWinCombination(combination);
            if (resultCombination != null)
            {
                win = resultCombination.Win;
            }

            LastCombination = $"{GetSymbol(s1, slot1)}{GetSymbol(s2, slot2)}{GetSymbol(s3, slot3)}";
            if (win > 0)
            {
                LastWin = win * bet;
            }
        }
    }
}