using System;
using System.Collections.Generic;
using System.Text;

namespace WolfGoatCabbage
{
    class Program
    {
        internal enum Entities { Empty, Wolf, Goat, Cabbage };
        internal enum MessageLevels { 
            Success = ConsoleColor.Green, 
            Info = ConsoleColor.Cyan,
            Warn = ConsoleColor.Yellow,
            Error = ConsoleColor.Red
        };

        static void Main(string[] args)
        {
            bool success;
            int actionIndex, sideIndex, itemIndex;

            var sides = new List<List<Entities>>()
            {
                new List<Entities>(){
                    Entities.Wolf,
                    Entities.Goat,
                    Entities.Cabbage
                },
                new List<Entities>(),
            };

            var boat = Entities.Empty;

            Console.OutputEncoding = Encoding.UTF8;

            do
            {
                Console.Clear();

                PrintStringsWithIndexes(new string[] {
                    "Left side: " + string.Join(", ", sides[0]),
                    "Right side: " + string.Join(", ", sides[1])
                });

                Console.WriteLine("Boat: " + boat);
                Console.WriteLine();

                Output("Actions", MessageLevels.Info);
                PrintStringsWithIndexes(new string[] {
                    "Move item to boat",
                    "Move item from boat",
                    "Switch items"
                });

                Console.WriteLine();

                do
                {
                    Console.Write("action: ");
                    success = int.TryParse(Console.ReadLine(), out actionIndex);
                }
                while (success == false);

                switch (actionIndex)
                {
                    //move item to boat
                    case 0:

                        if (boat != Entities.Empty)
                        {
                            Output("I can't move item to boat, boat is not empty", MessageLevels.Error);
                            Console.ReadKey();
                            break;
                        }

                        do
                        {
                            Console.Write("select side from: ");
                            success = int.TryParse(Console.ReadLine(), out sideIndex) && sideIndex >= 0 && sideIndex < 2;

                            if (success && sides[sideIndex].Count == 0)
                            {
                                Output("Can't move item from selected side. Side is empty.\r\n", MessageLevels.Error);
                                success = false;
                            }
                        }
                        while (success == false);

                        Console.WriteLine();

                        for (int i = 0; i < sides[sideIndex].Count; i++)
                            Console.WriteLine(i + ". " + sides[sideIndex][i]);

                        Console.WriteLine();

                        do
                        {
                            Console.Write("select item: ");
                            success = int.TryParse(Console.ReadLine(), out itemIndex) && itemIndex >= 0 && itemIndex < sides[sideIndex].Count;

                            var temp = new List<Entities>(sides[sideIndex]);
                            temp.RemoveAt(itemIndex);

                            var canMoveFrom =
                                (temp.Contains(Entities.Wolf) && temp.Contains(Entities.Goat)
                                ||
                                temp.Contains(Entities.Goat) && temp.Contains(Entities.Cabbage)) == false;

                            if (canMoveFrom == false)
                            {
                                Output("Can't move item from selected side. It's not safe.\r\n", MessageLevels.Error);
                                success = false;
                            }
                        }
                        while (success == false);

                        boat = sides[sideIndex][itemIndex];
                        sides[sideIndex].RemoveAt(itemIndex);

                        Output("Moved", MessageLevels.Success);
                        Console.ReadKey();
                        break;

                    //move item from boat
                    case 1:
                        if (boat == Entities.Empty)
                        {
                            Output("I can't move item from boat, boat is empty", MessageLevels.Error);
                            Console.ReadKey();
                            break;
                        }

                        do
                        {
                            Console.Write("select side to: ");
                            success = int.TryParse(Console.ReadLine(), out sideIndex) && sideIndex >= 0 && sideIndex < 2;
                        }
                        while (success == false);

                        if (CanMoveItemToSide(boat, sides[sideIndex], sideIndex == 1))
                        {
                            sides[sideIndex].Add(boat);
                            boat = Entities.Empty;

                            Output("Moved", MessageLevels.Success);
                        }

                        break;

                    //switch items
                    case 2:

                        if (boat == Entities.Empty)
                        {
                            Output("I can't move item from boat, boat is empty", MessageLevels.Error);
                            Console.ReadKey();
                            break;
                        }

                        do
                        {
                            Console.Write("select side: ");
                            success = int.TryParse(Console.ReadLine(), out sideIndex) && sideIndex >=0 && sideIndex < 2;

                            if (success && sides[sideIndex].Count == 0)
                            {
                                Output("Can't move item from selected side. Side is empty.\r\n", MessageLevels.Error);
                                success = false;
                            }
                        }
                        while (success == false);

                        Console.WriteLine();

                        for (int i = 0; i < sides[sideIndex].Count; i++)
                            Console.WriteLine(i + ". " + sides[sideIndex][i]);

                        Console.WriteLine();

                        do
                        {
                            Console.Write("select item: ");

                            success = int.TryParse(Console.ReadLine(), out itemIndex) && itemIndex >= 0 && itemIndex < sides[sideIndex].Count;

                            if (success)
                            {
                                var temp = new List<Entities>(sides[sideIndex]);
                                temp.RemoveAt(itemIndex);

                                var canMoveFrom =
                                    (temp.Contains(Entities.Wolf) && temp.Contains(Entities.Goat)
                                    ||
                                    temp.Contains(Entities.Goat) && temp.Contains(Entities.Cabbage)) == false;

                                if (canMoveFrom == false)
                                {
                                    Output("Can't move item from selected side. It's not safe.\r\n", MessageLevels.Error);
                                    success = false;
                                }
                                else
                                {
                                    success = CanMoveItemToSide(boat, temp);
                                }
                            }
                        }
                        while (success == false);

                        var movedItem = sides[sideIndex][itemIndex];
                        sides[sideIndex][itemIndex] = boat;
                        boat = movedItem;

                        break;
                }
            }
            while (sides[1].Count < 3);

            Output("You win!", MessageLevels.Success);
            Console.ReadKey();
        }
        private static void Output(string message, MessageLevels level)
        {
            Console.ForegroundColor = level switch
            {
                MessageLevels.Info => Console.ForegroundColor = ConsoleColor.Cyan,
                MessageLevels.Warn => Console.ForegroundColor = ConsoleColor.Yellow,
                MessageLevels.Error => Console.ForegroundColor = ConsoleColor.Red,
                MessageLevels.Success => Console.ForegroundColor = ConsoleColor.Green,
                _ => ConsoleColor.Gray,
            };
            Console.WriteLine(message);
            Console.ResetColor();
        }
        private static void PrintStringsWithIndexes(IEnumerable<string> list)
        {
            var i = 0;
            foreach (var item in list)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("{0}. ", i++);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(item);
                Console.ResetColor();
            }
        }
        private static bool CanMoveItemToSide(Entities item, List<Entities> side, bool isRightSide = false)
        {
            if (isRightSide && side.Count == 2)
                return true;

            bool canMoveTo = item switch
            {
                Entities.Wolf => side.Contains(Entities.Goat) == false,
                Entities.Goat => side.Contains(Entities.Wolf) == false && side.Contains(Entities.Cabbage) == false,
                Entities.Cabbage => side.Contains(Entities.Goat) == false,
                _ => throw new NotImplementedException(),
            };

            if (canMoveTo == false)
            {
                Output("Can't move item to selected side. It's not safe.\r\n", MessageLevels.Error);
                Console.ReadKey();
            }
                

            return canMoveTo;
        }
    }
}
