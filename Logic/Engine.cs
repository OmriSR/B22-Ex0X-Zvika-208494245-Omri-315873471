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

        public void StartGame()
        {   
            while(/*not quit*/ true)
            {
                // whos turn is it?
                //get move
                //validate move
                //can coin eat?
                //move coin
                // going back to whos turn is it -- if the player ate the other he gets one more turn
            }
        }

        public static void TranslatePositionToIndices(string i_CharPos, ref short o_Col, ref short o_Row)
        { 
            o_Col = Convert.ToInt16(i_CharPos[0] - 'A');
            o_Row = Convert.ToInt16(i_CharPos[1] - 'a');
        }
    }
}
