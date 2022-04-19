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

        public static void TranslateCharPositionToIndices(string i_CharPos, ref short o_CurrentCol, ref short o_CurrentRow,
                                                          ref short o_NewRow, ref short o_NewCol) 
        {
            //o_Col = Convert.ToInt16(i_CharPos[0]) - 'A';
            //o_Row = Convert.ToInt16(i_CharPos[1]) - 'a';
            o_CurrentCol = (short)(i_CharPos[0] - 'A');
            o_CurrentRow = (short)(i_CharPos[1] - 'a');
            o_NewCol = (short)(i_CharPos[3] - 'A');
            o_NewRow = (short)(i_CharPos[4] - 'a');
        }


    }
}
