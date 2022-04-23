using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public enum eCellOwner { Empty, Player1, Player2 };

    class Cell
    { 
        short m_Row, m_Col;
        bool m_IsEmpty;
        Coin m_Coin = null;

        public Cell( eCellOwner i_CellOwner, short i_Col, short i_Row, short i_BoardSize)
        {
            m_IsEmpty = (i_CellOwner == eCellOwner.Empty);
            m_Row = i_Row;
            m_Col = i_Col;

            if (!m_IsEmpty)
            {
                short possibleMovesCount = countInitialPossibleMovesFromCell(i_Col, i_Row, i_BoardSize);
                m_Coin = new Coin(i_CellOwner, possibleMovesCount,i_Col,i_Row);
            }
        }

        public short Col
        {
            get
            {
                return m_Col;
            }
            set
            {
                m_Col = value;
            }
        }

        public short Row
        {
            get
            {
                return m_Row;
            }
            set
            {
                m_Row = value;
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
                return m_Coin;
            }
            set
            {
                m_Coin = value;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return m_IsEmpty;
            }
            set
            {
                m_IsEmpty = value;
            }
        }
    }
}
