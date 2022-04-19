using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public enum eCellOwner { Empty, Player1, Player2 };

    class Cell
    {
        //short m_row, m_col;
        bool m_isEmpty;
        Coin m_coin = null;

        public Cell( eCellOwner i_cellOwner, short i_col, short i_row, short i_boardSize)
        {
            m_isEmpty = (i_cellOwner == eCellOwner.Empty);

            if (!m_isEmpty)
            {
                short possibleMovesCount = countInitialPossibleMovesFromCell(i_col, i_row, i_boardSize);

                m_coin = new Coin(i_cellOwner, possibleMovesCount);
            }
        }

        private bool isFrontLine(short i_row, short i_boardSize)
        {
            return ((i_row == i_boardSize / 2 - 2) || (i_row == i_boardSize / 2 + 1));
        }

        private bool isOuterColumn(short i_col, short i_boardSize)
        {

            return ((i_col == (i_boardSize - 1)) || (i_col == 0));
        }

        short countInitialPossibleMovesFromCell(short i_col, short i_row, short i_boardSize)
        {
            short possibleMoves = 0;

            if (isFrontLine(i_row, i_boardSize))
            {
                possibleMoves = Convert.ToInt16( isOuterColumn(i_col, i_boardSize) ? 1 : 2 );
            }

            return possibleMoves;
        }

        public Coin coin
        {
            get
            {
                return m_coin;
            }
            set
            {
                m_coin = value;
            }
        }
    }
}
