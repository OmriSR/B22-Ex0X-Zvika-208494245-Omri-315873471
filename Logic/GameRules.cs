using System;
using System.Data;
using Logic;

namespace Logic
{
    class GameRules // mostly validation
    {
        public static bool CheckValidMove( GameBoard i_GameBoard, Coin i_Player, short i_CurrentRow, short i_CurrentCol, short i_NewRow, short i_NewCol, ref bool possibleEat)
        {
            // we assume that newrow, newcol is a legit input, such as "Gf" in Af>Gf.
            // In this position, we don't know if Af>Gf is a valid move. but we know it's a valid input.
            bool theMoveIsValid = checkNewPositionIsNotOutOfBounds(i_GameBoard, i_NewRow, i_NewCol);

            if (theMoveIsValid)
            // ok, i didn't get out of bounds. give me the direction im going.
            {
                eDirection inputDirection = getDirectionFromInput(i_CurrentRow, i_CurrentCol, i_NewRow, i_NewCol);
                if (inputDirection != eDirection.NullDirection)
                {
                    if ((!i_Player.IsKing && (inputDirection == eDirection.DownLeft || inputDirection == eDirection.DownRight)))
                    {
                        theMoveIsValid = false;
                    }

                    else
                    {
                        theMoveIsValid = checkValidMoveByGivenDirection( i_NewRow, i_NewCol, i_Player, i_GameBoard, ref possibleEat, inputDirection);
                    }
                }
            }

            return theMoveIsValid;

        }

        private static bool checkValidMoveByGivenDirection(short i_Row, short i_Col, Coin i_Player, GameBoard i_GameBoard, ref bool i_PossibleEat, eDirection i_Direction)
        {
            eCellOwner itemInNextStep = i_GameBoard.Board[i_Row, i_Col].Coin.Player;
            bool isNextMoveValid = true;
            updateNewRowAndCol(i_Direction, ref i_Row, ref i_Col); // What's sitting in my step?    
            if (itemInNextStep == eCellOwner.Empty)
            {
                isNextMoveValid = true;
            }
            else
            {
                if (itemInNextStep == i_Player.Player)
                {
                    // If in my next step sitting my soldier, I can't try eating it.
                    isNextMoveValid = false;
                }
                else
                {
                    // There is an enemy in my next step. I want to check if behind him there's space, so I can eat him.
                    updateNewRowAndCol(i_Direction, ref i_Row, ref i_Col);
                    if (checkNewPositionIsNotOutOfBounds(i_GameBoard, i_Row, i_Col))
                    {
                        if (i_GameBoard.Board[i_Row, i_Col].Coin.Player == eCellOwner.Empty)
                        {
                            isNextMoveValid = true;
                            i_PossibleEat = true;
                        }
                        else
                        {
                            isNextMoveValid = false;
                        }
                    }
                    else
                    {
                        isNextMoveValid = false;
                    }
                }
            }

            return isNextMoveValid;
        }

        private static void updateNewRowAndCol(eDirection i_Direction, ref short i_NewRow, ref short i_NewCol)
        {
            switch (i_Direction)
            {
                case eDirection.UpRight:
                    {
                        i_NewRow--;
                        i_NewCol++;
                        break;
                    }
                case eDirection.UpLeft:
                    {
                        i_NewRow--;
                        i_NewCol--;
                        break;
                    }
                case eDirection.DownRight:
                    {
                        i_NewRow++;
                        i_NewCol++;
                        break;
                    }
                case eDirection.DownLeft:
                    {
                        i_NewRow++;
                        i_NewCol--;
                        break;
                    }
            }
        }

        private static eDirection getDirectionFromInput(
            short i_CurrentRow,
            short i_CurrentCol,
            short i_NewRow,
            short i_NewCol)
        {
            eDirection resultDirection = eDirection.NullDirection;
            short rows = (short)(i_CurrentRow - i_NewRow);
            short cols = (short)(i_CurrentCol - i_NewCol);

            //  UP              RIGHT
            if (rows == 1 && cols == -1)
            {
                resultDirection = eDirection.UpRight;
            }

            //  UP              LEFT
            if (rows == 1 && cols == 1)
            {
                resultDirection = eDirection.UpLeft;
            }

            // DOWN             RIGHT
            if (rows == -1 && cols == -1)
            {
                resultDirection = eDirection.DownRight;
            }

            // DOWN            LEFT
            if (rows == -1 && cols == 1)
            {
                resultDirection = eDirection.DownLeft;
            }

            return resultDirection;
        }



        
    }
}
    