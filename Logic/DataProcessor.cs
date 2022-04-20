using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class DataProcessor  // input handler --- with help from game rules for validation
    {
        private bool isCapitalLetter(char i_charToCheck)
        {
            return (i_charToCheck >= 'A' && i_charToCheck <= 'Z');
        }

        private bool islowerCaseLetter(char i_charToCheck)
        {
            return (i_charToCheck >= 'a' && i_charToCheck <= 'z');
        }

        public bool IsValidInput(string i_UserInput)
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

        public bool CheckIfQuit(string i_userInput)
        {
            return (i_userInput[0] == 'q' || i_userInput[0] == 'Q');
        }


        //public static bool CheckIfGivenPositionIsMyCoin(short i_PlayerNumber, short i_Row, short i_Col, GameBoard i_GameBoard)
        //{
        //    return (i_PlayerNumber == i_GameBoard.GetItemOnPosition(i_Row, i_Col));
        //}
        
    }
}
