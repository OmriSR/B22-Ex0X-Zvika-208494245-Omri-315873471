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
        bool m_canEatUpLeft, m_canEatUpRight, m_canEatDownRight, m_canEatDownLeft;
        bool m_GotMoves;
        bool m_IsAlive = true;

        public Coin(eCellOwner i_Player, short i_NumOfPossibleMoves,short i_col, short i_row)
        {
            m_row = i_row;
            m_col = i_col;
            r_player = i_Player;
            // m_possibleMovesCount = i_NumOfPossibleMoves;
            // m_canEat = new bool[4];
            m_canEatUpLeft = m_canEatUpRight = m_canEatDownRight = m_canEatDownLeft = false;
        }

        //----------- properties ----------------
        public bool isAlive
        {
            get
            {
                return m_IsAlive;
            }
            set
            {
                m_IsAlive = value;
            }
        }
        public bool CanEatDownRight
        {
            get
            {
                return m_canEatDownRight;
            }
            set
            {
                m_canEatDownRight = value;
            }
        }


        public bool CanEatDownLeft
        {
            get
            {
                return m_canEatDownLeft;
            }
            set
            {
                m_canEatDownLeft = value;
            }
        }


        public bool CanEatUpLeft
        {
            get
            {
                return m_canEatUpLeft;
            }
            set
            {
                m_canEatUpLeft = value;
            }
        }

        public bool CanEatUpRight
        {
            get
            {
                return m_canEatUpRight;
            }
            set
            {
                m_canEatUpRight = value;
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
                m_col = value;
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

        public bool IsKing
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

        public bool GotMoves
        {
            get
            {
                return m_GotMoves;
            }
            set
            {
                m_GotMoves = value;
            }
        }

        public eCellOwner Player
        {
            get
            {
                return r_player;
            }
        }

        //------------------------------








        
    }
}
