using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class Coin
    {
        short m_row, m_col;
        bool m_isKing = false;
        readonly eCellOwner r_player;
        bool m_canEatLeft, m_canEatRight;
        
        short m_possibleMovesCount;

        public Coin(eCellOwner i_Player, short i_NumOfPossibleMoves,short i_col, short i_row)
        {
            m_row = i_row;
            m_col = i_col;
            r_player = i_Player;
            m_possibleMovesCount = i_NumOfPossibleMoves;
            m_canEatLeft = m_canEatRight = false;
        }

        public bool canEatLeft
        {
            get
            {
                return m_canEatLeft;
            }
            set
            {
                m_canEatLeft = value;
            }
        }
        public bool canEatRight
        {
            get
            {
                return m_canEatRight;
            }
            set
            {
                m_canEatRight = value;
            }
        }
        public short col
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

        public short row
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

        public bool isKing
        {
            get
            {
                return m_isKing;
            }
            set
            {
                m_isKing = true;
            }
        }

        public short possibleMoveCount
        {
            get
            {
                return m_possibleMovesCount;
            }
            set
            {
                m_possibleMovesCount = value;
            }
        }

        public eCellOwner player
        {
            get
            {
                return r_player;
            }
        }

        
    }
}
