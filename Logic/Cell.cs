using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public enum eCellOwner { Empty, Player1, Player2 };

    class Cell
    { 
        short m_row, m_col;
        bool m_isEmpty;
        Coin m_coin = null;

        public Cell( eCellOwner i_CellOwner, short i_Col, short i_Row, short i_BoardSize)
        {
            m_isEmpty = (i_CellOwner == eCellOwner.Empty);

            if (!m_isEmpty)
            {
                short possibleMovesCount = countInitialPossibleMovesFromCell(i_Col, i_Row, i_BoardSize);
                m_coin = new Coin(i_CellOwner, possibleMovesCount,i_Col,i_Row);
            }
        }

        public short Col
        {
            get
            {
                return m_col;
            }
            set
            {
                m_row = value;
            }
        }

        public short Row
        {
            get
            {
                return m_row;
            }
            set
            {
                m_row = value;
            }
        }

        private bool isFrontLine(short i_Row, short i_BoardSize)
        {
            return ((i_Row == i_BoardSize / 2 - 2) || (i_Row == i_BoardSize / 2 + 1));
        }

        private bool isOuterColumn(short i_Col, short i_BoardSize)
        {

            return ((i_Col == (i_BoardSize - 1)) || (i_Col == 0));
        }

        private short countInitialPossibleMovesFromCell(short i_Col, short i_Row, short i_BoardSize)
        {
            short possibleMoves = 0;

            if (isFrontLine(i_Row, i_BoardSize))
            {
                possibleMoves = Convert.ToInt16( isOuterColumn(i_Col, i_BoardSize) ? 1 : 2 );
            }

            return possibleMoves;
        }

        public Coin Coin
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

        public bool IsEmpty
        {
            get
            {
                return m_isEmpty;
            }
            set
            {
                m_isEmpty = value;
            }
        }
    }
}
