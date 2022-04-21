using System;
using System.Data;
using Logic;

namespace Logic
{
    class GameRules // mostly validation
    {

        //private short countPossibleSkipsFromCoin(Cell[,] i_GameBoard, short i_Col, short i_Row)
        //{
        //    short skipsCount = 0;
        //    eCellOwner currentPlayer = i_GameBoard[i_Col, i_Row].coin.player;
        //    bool isKing = i_GameBoard[i_Col, i_Row].coin.isKing;

        //    if (isCoinOpponentPlayer(currentPlayer, checkUpRightCellOwner(i_GameBoard,i_Col,i_Row)))
        //    {
        //        ++skipsCount;
        //    }

        //    if (isCoinOpponentPlayer(currentPlayer, checkUpLeftCellOwner(i_GameBoard, i_Col, i_Row)))
        //    {
        //        ++skipsCount;
        //    }


        //    return skipsCount;
        //}

        //        private short coutPossibleSkipsFromKINGCoin(Cell[,] i_GameBoard, short i_Col, short i_Row)   for later implementation  -- just add check for back steps

        //public bool CheckIfValidMove(Cell[,] i_GameBoard, short i_Col, short i_Row, string i_Input, eDirection i_MoveDirection)
        //{
        //    bool moveIsValid = false;
        //    short skipsPossible = countPossibleSkipsFromCoin(i_GameBoard, i_Col, i_Row);

        //    if(skipsPossible == 1)
        //    {

        //    }

        //    switch(i_MoveDirection)
        //    {
        //        case eDirection.UpRight:
        //            checkUpRightCellOwner(i_GameBoard, i_Col, i_Row);
        //            break;
        //        case eDirection.UpLeft:
        //            checkUpLeftCellOwner(i_GameBoard, i_Col, i_Row);
        //            break;
        //        case eDirection.DownRight:
        //            break;
        //        case eDirection.DownLeft:
        //            break;
        //        case eDirection.NullDirection:
        //            break;
        //    }

        /*
         check if can eat around me and how many ---
        if 1 ==> check if move is in that direction,
            if yes ==> valid                                          need to think of logic --- many diffrent methods are required!
            else  ==> not valid.
        else ==> 


         check if empty cell - if it is ==> valid move.
         if not empty,
         if coin is same player as me ==> invalid move
         */

        // return true;
        //  }


        public static bool CheckValidMove(
            GameBoard i_GameBoard,
            Coin i_Player,
            short i_CurrentRow,
            short i_CurrentCol,
            short i_NewRow,
            short i_NewCol,
            ref bool possibleEat)

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
                    if ((!i_Player.isKing
                        && (inputDirection == eDirection.DownLeft || inputDirection == eDirection.DownRight)))
                    {
                        theMoveIsValid = false;
                    }
                    else
                    {
                        theMoveIsValid = checkValidMoveByGivenDirection(
                            i_NewRow,
                            i_NewCol,
                            i_Player,
                            i_GameBoard,
                            ref possibleEat,
                            inputDirection);
                    }
                }
            }

            return theMoveIsValid;


        }

        private static bool checkValidMoveByGivenDirection(short i_Row, short i_Col, Coin i_Player, GameBoard i_GameBoard, ref bool i_PossibleEat, eDirection i_Direction)
        {
            eCellOwner itemInNextStep = i_GameBoard.gameBoard[i_Row, i_Col].coin.player;
            bool isNextMoveValid = true;
            updateNewRowAndCol(i_Direction, ref i_Row, ref i_Col); // What's sitting in my step?    
            if (itemInNextStep == eCellOwner.Empty)
            {
                isNextMoveValid = true;
            }
            else
            {
                if (itemInNextStep == i_Player.player)
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
                        if (i_GameBoard.gameBoard[i_Row, i_Col].coin.player == eCellOwner.Empty)
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
    