using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public class Checkers
    {
        int m_row;
        int m_col;
        bool isKing = false;

        public int Row
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

        public void MoveChecker(string i_COLrow)   // Assuming that validation was checked by GameRules and only next position was given.
        {
            int col = -1 , row = -1;
            Engine.TranslateCharPositionToIndices(i_COLrow, ref col, ref row);
            m_row = (row != -1) ? row : m_row;
            m_col = (col != -1) ? col : m_col;
        }
    }
}
