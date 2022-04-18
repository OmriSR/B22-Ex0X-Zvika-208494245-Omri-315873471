using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class Engine
    {
        public static void TranslateCharPositionToIndices(string i_charPos, ref int o_col, ref int o_row) 
        {
            o_col = Convert.ToInt32(i_charPos[0]);
            o_row = Convert.ToInt32(i_charPos[1]);
        }
    }
}
