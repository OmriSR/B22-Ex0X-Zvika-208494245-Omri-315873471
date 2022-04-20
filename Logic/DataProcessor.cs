using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class DataProcessor  // input handler --- with help from game rules for validation
    {
        private bool isCapitalLetter(char i_CharToCheck)
        {
            return (i_CharToCheck >= 'A' && i_CharToCheck <= 'Z');
        }

        private bool islowerCaseLetter(char i_CharToCheck)
        {
            return (i_CharToCheck >= 'a' && i_CharToCheck <= 'z');
        }

        private bool isValidInput(string i_UserInput)
        {
            bool isValid = false;

            if (i_UserInput.Length == 5)
            {
                bool colsValid = (isCapitalLetter(i_UserInput[0]) && isCapitalLetter(i_UserInput[3]));
                bool rowsValid = (islowerCaseLetter(i_UserInput[1]) && islowerCaseLetter(i_UserInput[4]));
                bool signValid = i_UserInput[2] == '>';

                isValid = colsValid && rowsValid && signValid;
            }

            else if(i_UserInput.Length == 1)
            {
                isValid = isCapitalLetter(i_UserInput[0]) || islowerCaseLetter(i_UserInput[0]);
            }

            return isValid;
        }

        private bool checkIfQuit(string i_UserInput)
        {
            return (i_UserInput[0] == 'q' || i_UserInput[0] == 'Q');
        }

        private void translateCharPositionToIndices(string i_CharPos, ref short o_col, ref short o_row)
        {
            o_col = Convert.ToInt16(i_CharPos[0] - 'A');
            o_row = Convert.ToInt16(i_CharPos[1] - 'a');
        }

        //private void translateStringInput(string i_input, out short o_curCol, out short o_curRow, out short o_nextCol, out short o_nextRow)
        //{
        //    /* break input to pices and send to method above*/
        //}

    }
}
