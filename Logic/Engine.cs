using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public enum eDirection { UpRight, UpLeft, DownRight, DownLeft, NullDirection }
    class Engine
    {
        bool m_toQuit = false;
        DataProcessor m_inputChecker = null;
        short m_whosTurnIsIt = 0;

        public void StartGame()
        {
            while (m_toQuit == false)
            {
                // whos turn is it?

                string COLrow = getMove();

                if ( m_inputChecker.IsValidInput(COLrow) == false )
                {
                    /* a method containing a loop of getMove() until it is corrected*/  //omri
                }

                if (m_inputChecker.CheckIfQuit(COLrow) == true)
                {
                    m_toQuit = false;
                }

                else
                {
                    // any valid moves left? if not - its a tie  ---- checkIfTie method  
                    //can coin eat?     ---- canCoinEat method
                    //move coin    
                    //do we have a winner/loser?       ---- checkIfWon method
                    // going back to whos turn is it -- if the player ate the other needs to check if can eat again
                }
            }
        }


        private string getMove()
        {
            /*get move from UI
              translate given move to indices
              assign via output parametters new pos*/

            return "needs to be implimented";
        }










        public static void TranslateCharPositionToIndices(string i_CharPos, ref short o_col, ref short o_row) 
        {
             o_col = Convert.ToInt16(i_CharPos[0] - 'A');
             o_row = Convert.ToInt16(i_CharPos[1] - 'a');
        }


    }
}
