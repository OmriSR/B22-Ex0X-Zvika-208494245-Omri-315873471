using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Logic
{
    class GameBoard
    {
        private Cell[,] m_GameBoard; 
        public readonly short r_BoardSize;

        private void initBoard(short i_BoardSize)
        {
            for (short row = 0; row < i_BoardSize; row++)
            {
                for (short col = 0; col < i_BoardSize; col++)
                {
                    if ((row < (i_BoardSize / 2) - 1) && ((row % 2 == 0 && col % 2 != 0) || (row % 2 != 0 && col % 2 == 0)))
                    {
                        m_GameBoard[row, col] = new Cell(eCellOwner.Player2, col, row, r_BoardSize);
                    }

                    else if (row > (i_BoardSize / 2) && ((row % 2 == 0 && col % 2 != 0) || (row % 2 != 0 && col % 2 == 0)))
                    {
                        m_GameBoard[row, col] = new Cell(eCellOwner.Player1, col, row, r_BoardSize);
                    }

                    else
                    {
                        m_GameBoard[row, col] = new Cell(eCellOwner.Empty, col, row, r_BoardSize);
                    }
                }
            }
        }

        public GameBoard(short i_BoardSize) // We assume that board size is valid
        {
            r_BoardSize = i_BoardSize;
            m_GameBoard = new Cell[r_BoardSize, r_BoardSize];
            initBoard(i_BoardSize);
        }

        internal short CountPossibleMoves(Cell i_cell)
        {
            


            return 0;
        }

        public Cell[,] gameBoard
        {
            get
            {
                return m_GameBoard;
            }
        }
    }

    
}
