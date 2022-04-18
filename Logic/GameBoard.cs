using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoardNamespace
{
    class GameBoard
    {
        private ushort[][] m_GameBoard; // 0 = blank, 1 = player1 = X, 2 = player2 = O
        private readonly ushort r_BoardSize;

        static void Main()
        {
            Console.WriteLine("Hayde");
        }

        public ushort GetItemOnPosition(ushort i_Row, ushort i_Col)
        {
            return m_GameBoard[i_Row][i_Col];
        }
    }

    
}
