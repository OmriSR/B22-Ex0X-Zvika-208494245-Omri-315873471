using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class DataProcessor
    {
        public enum eTranslatedUserPlayInput { Quit = 1, Valid = 2, Invalid = 3}
        public static bool CheckUserPlayInput(string i_UserInput, out eTranslatedUserPlayInput o_PlayOutput)
        {
            o_PlayOutput = eTranslatedUserPlayInput.Invalid;
            bool isValidInput = true;

            if(i_UserInput.Length == 6)
            {
                isValidInput = checkSixLengthInput((i_UserInput));
            }

            else if (i_UserInput == "Q" || i_UserInput == "q")
            {
                o_PlayOutput = eTranslatedUserPlayInput.Quit;
            }

            else
            {
                    isValidInput = false;
            }

            if (isValidInput)
            {
                o_PlayOutput = eTranslatedUserPlayInput.Valid;
            }

            return isValidInput;
        }

        private static bool checkSixLengthInput(string i_UserInput)
        {
            return i_UserInput[2] == '>' && i_UserInput[5] == '\0' && checkTwoUpperLetters(i_UserInput)
                   && checkTwoSmallLetters(i_UserInput);
        }

        private static bool checkTwoUpperLetters(string i_StringInput)
        {
            return (i_StringInput[0] >= 'A' && i_StringInput[3] <= 'Z');
        }

        private static bool checkTwoSmallLetters(string i_StringInput)
        {
            return (i_StringInput[1] >= 'a' && i_StringInput[4] <= 'z');
        }

        // האם בנקודה הזו יש חייל שלי
        public static bool CheckIfGivenPositionIsMyCoin(short i_PlayerNumber, short i_Row, short i_Col, GameBoard i_GameBoard)
        {
            return (i_PlayerNumber == i_GameBoard.GetItemOnPosition(i_Row, i_Col));
        }
        
    }
}
