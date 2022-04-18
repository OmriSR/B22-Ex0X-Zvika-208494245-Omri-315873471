using System;
using Logic;

namespace Logic
{
    class GameRules
    {
        public static bool CheckValidMove(
                short i_CurrentRow,
                short i_CurrentCol,
                short i_NewRow,
                short i_NewCol,
                short i_PlayerNumber,
                short i_BoardSize,
                ref bool i_PossibleEat,
                GameBoard i_GameBoard)
            // we assume newRow and newCol are valid inputs. ---- CHECK IN PREVIOUS CALLS THAT INDEED HAPPENS ----
        {
            bool theMoveIsValid = true;
            i_PossibleEat = false;
            theMoveIsValid = i_NewRow < i_BoardSize && i_NewCol < i_BoardSize; // Checking out of bounds
            Engine.eDirection inputDirection = getDirectionFromInput(i_CurrentRow, i_CurrentCol, i_NewRow, i_NewCol);
            if(inputDirection != Engine.eDirection.NullDirection)
            {
                theMoveIsValid = checkValidMoveToAllDirections(
                    i_NewRow,
                    i_NewCol,
                    i_PlayerNumber,
                    i_BoardSize,
                    i_GameBoard,
                    ref i_PossibleEat,
                    inputDirection);
            }
            else
            {
                theMoveIsValid = false;
            }
            return theMoveIsValid;
        }

        private static bool checkValidMoveToAllDirections(
            short i_NewRow,
            short i_NewCol,
            short i_PlayerNumber,
            short i_BoardSize,
            GameBoard i_GameBoard,
            ref bool i_PossibleEat,
            Engine.eDirection i_Direction)
        {
            // Input is valid thanks to function getDirectionFromInput
            short itemInNextStep = i_GameBoard.GetItemOnPosition(i_NewRow, i_NewCol);
            bool isNextMoveValid = true;
            updateNewRowAndCol(i_Direction, ref i_NewRow, ref i_NewCol); // Checking the step AFTERWARDS the one we take.
            if (itemInNextStep == 0)
            {
                isNextMoveValid = true;
            }

            if(i_NewCol < 0 || i_NewCol >= i_BoardSize || i_NewRow < 0 || i_NewRow >= i_BoardSize) // Going out of bounds for next step
            {
                isNextMoveValid = false;
            }

            else if (itemInNextStep != i_PlayerNumber && itemInNextStep != 0) // Is there an enemy in next step?
            {

                if (i_GameBoard.GetItemOnPosition((short)(i_NewRow), (short)(i_NewCol))
                    == 0) // can I eat the enemy?
                {
                    isNextMoveValid = true;
                    i_PossibleEat = true;
                }

                else
                {
                    isNextMoveValid = false;
                    i_PossibleEat = false;
                }
            }

            return isNextMoveValid;
        }

        private static void updateNewRowAndCol(Engine.eDirection i_Direction, ref short i_NewRow, ref short i_NewCol)
        {
            switch (i_Direction)
            {
                case Engine.eDirection.UpRight:
                    {
                        i_NewRow--;
                        i_NewCol++;
                        break;
                    }
                case Engine.eDirection.UpLeft:
                    {
                        i_NewRow--;
                        i_NewCol--;
                        break;
                    }
                case Engine.eDirection.DownRight:
                    {
                        i_NewRow++;
                        i_NewCol++;
                        break;
                    }
                case Engine.eDirection.DownLeft:
                    {
                        i_NewRow++;
                        i_NewCol--;
                        break;
                    }
            }
        }

        private static Engine.eDirection getDirectionFromInput(
            short i_CurrentRow,
            short i_CurrentCol,
            short i_NewRow,
            short i_NewCol)
        {
            Engine.eDirection resultDirection = Engine.eDirection.NullDirection;
            short rows = (short)(i_CurrentRow - i_NewRow);
            short cols = (short)(i_CurrentCol - i_CurrentCol);

            //  UP              RIGHT
            if(rows == 1 && cols == -1)
            {
                resultDirection = Engine.eDirection.UpRight;
            }

            //  UP              LEFT
            if(rows == 1 && cols == 1)
            {
                resultDirection = Engine.eDirection.UpLeft;
            }

            // DOWN             RIGHT
            if(rows == -1 && cols == -1)
            {
                resultDirection = Engine.eDirection.DownRight;
            }

            // DOWN            LEFT
            if(rows == -1 && cols == 1)
            {
                resultDirection = Engine.eDirection.DownLeft;
            }

            return resultDirection;
        }
    }
}
    