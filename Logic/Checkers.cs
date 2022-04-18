using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public class Checkers
    {
        ushort m_Row;
        ushort m_Col;
        bool isKing = false;

        public int Row
        {
            get
            {
                return m_Row;
            }
            set
            {
                m_Row = Convert.ToUInt16(value);
            }
        }
        public int Col
        {
            get
            {
                return m_Col;
            }
            set
            {
                m_Col = Convert.ToUInt16(value);
            }
        }

        public void MoveChecker(string i_COLrow)   // Assuming that validation was checked by GameRules and only next position was given.
        {
            int col = -1 , row = -1;
            Engine.TranslateCharPositionToIndices(i_COLrow, ref col, ref  row);
            m_Row = (row != -1) ? Convert.ToUInt16(row) : m_Row;
            m_Col = (col != -1) ? Convert.ToUInt16(col) : m_Col;
        }
    }
}
