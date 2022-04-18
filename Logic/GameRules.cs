using System;
using GameBoardNamespace;

namespace GameRulesNamespace
{
    class GameRules
    {
        public static bool CheckValidMove(
            ushort i_CurrentRow,
            ushort i_CurrentCol,
            ushort i_NewRow,
            ushort i_NewCol,
            ushort i_PlayerNumber,
            ushort i_BoardSize,
            ref bool i_PossibleEat,
            GameBoard i_GameBoard)
        {
            bool theMoveIsValid = true;
            i_PossibleEat = false;
            theMoveIsValid = i_NewRow < i_BoardSize && i_NewCol < i_BoardSize;

            // Was the given move RIGHT-UP?                                              
            if(theMoveIsValid && i_CurrentCol - i_NewCol == -1 && i_CurrentRow - i_NewRow == 1)
            {
                theMoveIsValid = CheckValidMoveToRightUp(
                    i_NewRow,
                    i_NewCol,
                    i_PlayerNumber,
                    i_BoardSize,
                    i_GameBoard,
                    ref i_PossibleEat);
            }

            // Was the given move LEFT-UP?
            if (theMoveIsValid && i_CurrentCol - i_NewCol == 1 && i_CurrentRow - i_NewRow == 1)
            {
                theMoveIsValid = CheckValidMoveToLeftUp(
                    i_NewRow,
                    i_NewCol,
                    i_PlayerNumber,
                    i_GameBoard,
                    ref i_PossibleEat);
            }

            return theMoveIsValid;
        }
       
        public static bool CheckValidMoveToRightUp(
            ushort i_NewRow,
            ushort i_NewCol,
            ushort i_PlayerNumber,
            ushort i_BoardSize,
            GameBoard i_GameBoard, ref bool i_PossibleEat)
        {
            bool theMoveIsValid = true;
            ushort itemInNextStep = i_GameBoard.GetItemOnPosition(i_NewRow, i_NewCol);

                if (itemInNextStep == 0)
                {
                    theMoveIsValid = true;
                }

                else if (itemInNextStep != i_PlayerNumber && i_NewRow - 1 >= 0 && i_NewCol + 1 < i_BoardSize) // Is there an enemy in right up?
                {

                    if (i_GameBoard.GetItemOnPosition((ushort)(i_NewRow - 1), (ushort)(i_NewCol + 1)) == 0) // can I eat the enemy?
                    {
                        theMoveIsValid = true;
                        i_PossibleEat = true;
                    }

                    else
                    {
                        theMoveIsValid = false;
                        i_PossibleEat = false;
                    }
                }

                else
                {
                    theMoveIsValid = false;
                    i_PossibleEat = false;
                }

                return theMoveIsValid;
        }


        public static bool CheckValidMoveToLeftUp(
            ushort i_NewRow,
            ushort i_NewCol,
            ushort i_PlayerNumber,
            GameBoard i_GameBoard, ref bool i_PossibleEat)
        {
            bool theMoveIsValid = true;
            ushort itemInNextStep = i_GameBoard.GetItemOnPosition(i_NewRow, i_NewCol);

            if (itemInNextStep == 0)
            {
                theMoveIsValid = true;
            }

            else if (itemInNextStep != i_PlayerNumber && i_NewRow - 1 >= 0 && i_NewCol - 1 >= 0) // Is there an enemy in left up?
            {

                if (i_GameBoard.GetItemOnPosition((ushort)(i_NewRow - 1), (ushort)(i_NewCol - 1)) == 0) // can I eat the enemy?
                {
                    theMoveIsValid = true;
                    i_PossibleEat = true;
                }

                else
                {
                    theMoveIsValid = false;
                    i_PossibleEat = false;
                }
            }

            else
            {
                theMoveIsValid = false;
                i_PossibleEat = false;
            }

            return theMoveIsValid;
        }
    }
}