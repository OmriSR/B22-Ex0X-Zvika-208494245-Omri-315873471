using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class Engine
    {
        public enum eDirection
        {
            UpRight = 1,
            UpLeft = 2,
            DownRight = 3,
            DownLeft = 4, 
            NullDirection = 5
        }
        public static void TranslateCharPositionToIndices(string i_CharPos, ref int o_Col, ref int o_Row) 
        {
            o_Col = Convert.ToUInt16(i_CharPos[0]);
            o_Row = Convert.ToUInt16(i_CharPos[1]);
        }
    }
}
