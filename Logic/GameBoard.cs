using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Logic
{
    class GameBoard
    {
        private short[,] m_GameBoard; // 0 = blank, 1 = player1 = X, 2 = player2 = O
        private readonly short r_BoardSize;

        static void Main() // We will need to delete.
        {
            Console.WriteLine("Testing printing the 2d array to make sure it's good. Please enter size>0 of table: ");
            string val = Console.ReadLine();
            short res = Convert.ToInt16(val);
            GameBoard abc = new GameBoard(res);
            for(short i = 0; i < res; i++)
            {
                for(short j = 0; j < res; j++)
                {
                    Console.Write(abc.GetItemOnPosition(i,j));
                }

                Console.Write(Environment.NewLine);
            }
        }

        public short GetItemOnPosition(short i_Row, short i_Col)
        {
            return m_GameBoard[i_Row, i_Col];
        }

        public GameBoard(short i_BoardSize) // We assume that board size is valid
        {
            r_BoardSize = i_BoardSize;
            m_GameBoard = new short[r_BoardSize, r_BoardSize];
            for(short row = 0; row < i_BoardSize; row++)
            {
                for(short col = 0; col < i_BoardSize; col++)
                {
                    if((row < (i_BoardSize / 2) - 1) && ((row % 2 == 0 && col % 2 != 0) || (row % 2 != 0 && col % 2 == 0)))
                    {
                        m_GameBoard[row, col] = 2;
                    }

                    else if (row > (i_BoardSize / 2) && ((row % 2 == 0 && col % 2 != 0) || (row % 2 != 0 && col % 2 == 0)))
                    {
                        m_GameBoard[row, col] = 1;
                    }

                    else
                    {
                        m_GameBoard[row, col] = 0;
                    }
                }
            }
        }
    }

    
}
