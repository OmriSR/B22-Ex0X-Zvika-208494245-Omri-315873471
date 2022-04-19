using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class DataProcessor
    {
        public enum eTranslatedUserPlayInput { Quit = 1, ValidMove = 2, InvalidInput = 3}
        public static bool CheckUserPlayInput(string i_UserInput, out eTranslatedUserPlayInput o_PlayOutput)  // this is 'isInputValid' method. enum not needed
        {
            o_PlayOutput = eTranslatedUserPlayInput.InvalidInput;
            bool isValidInput = true;

            if(i_UserInput.Length == 6)
            {
                if(!checkSixLengthInput(i_UserInput))
                {
                    isValidInput = false;
                }
            }

            else if (i_UserInput == "Q")
            {
                o_PlayOutput = eTranslatedUserPlayInput.Quit;
            }

            else
            {
                    isValidInput = false;
            }
            
            return isValidInput;
        }

        private static bool checkSixLengthInput(string i_UserInput)
        {
            bool isValidInput = false;

            if(i_UserInput[0] >= 'A' && i_UserInput[0] <= 'Z' && (i_UserInput[3] >= 'A' && i_UserInput[3] <= 'Z'))
            {
                if(i_UserInput[1] >= 'a' && i_UserInput[1] <= 'z' && i_UserInput[4] >= 'a' && i_UserInput[4] <= 'z')
                {
                    if(i_UserInput[2] == '>' && i_UserInput[5] == '\0')
                    {
                        isValidInput = true;
                    }
                }
            }

            return isValidInput;
        }

        public static bool CheckSoldierInputLocation(short i_PlayerNumber, short i_Row, short i_Col, GameBoard i_GameBoard)
        {
            return (i_PlayerNumber == i_GameBoard.GetItemOnPosition(i_Row, i_Col));
        }
        
    }
}
