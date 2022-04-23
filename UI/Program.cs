using System;

namespace UI
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");
            string test = Console.ReadLine();
            Console.WriteLine(test[5]);
        }

        public static void PrintMainScreen()
        {
            printFirstText();
            string boardSize = Console.ReadLine();
            //while (!Logic.ValidBoardSize(boardSize)) // why doesn't it go to Engine.ValidBoardSize?
            //{
            //    Console.WriteLine("You have entered illegal value! Please try again.");
            //}

            //Start game(string to ushort --> boardSize);
        }

        private static void printFirstText()
        {
            Console.WriteLine("Hello! Welcome to Omri and Zvika's Checkers game! Please enter size of board: ");
            Console.WriteLine("1. 6X6");
            Console.WriteLine("2. 8X8");
            Console.WriteLine("3. 10X10");
        }
    }
}
