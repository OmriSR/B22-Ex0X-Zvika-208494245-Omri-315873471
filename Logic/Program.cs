using System;

namespace Logic
{
    class Program
    {
        static void Main()
        {

            /*new line text*/
            string test = Console.ReadLine();
            
            for(int i=0;i<5;++i)
            {
                if(test[i] == '\n') Console.WriteLine($"{i} index has \\n! \n");
            }
            Console.WriteLine("all is well");

        }

        //static void Main() // We will need to delete.
        //{
        //    Console.WriteLine("Testing printing the 2d array to make sure it's good. Please enter size>0 of table: ");
        //    string val = Console.ReadLine();
        //    short res = Convert.ToInt16(val);
        //    GameBoard abc = new GameBoard(res);
        //    for(short i = 0; i < res; i++)
        //    {
        //        for(short j = 0; j < res; j++)
        //        {
        //            Console.Write(abc.GetItemOnPosition(i,j));
        //        }

        //        Console.Write(Environment.NewLine);
        //    }
        //}
    }


}
