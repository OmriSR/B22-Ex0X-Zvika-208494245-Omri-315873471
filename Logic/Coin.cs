using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class Coin
    {
        bool m_isKing = false;
        readonly eCellOwner r_player;
        short m_possibleMovesCount;

        public Coin(eCellOwner i_player, short i_numOfPossibleMoves)
        {
            r_player = i_player;
            m_possibleMovesCount = i_numOfPossibleMoves;
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
