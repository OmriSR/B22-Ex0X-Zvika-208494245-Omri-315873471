using System;
using System.Data;
using Logic;

namespace Logic
{
    class GameRules   // mostly validation
    {
        private eCellOwner checkUpRightCellOwner(Cell[,] i_GameBoard, short i_Col, short i_Row)  
        {
            eCellOwner playerInCell;

            if(i_GameBoard[i_Row, i_Col].coin.player == eCellOwner.Player2)
            {
                playerInCell = i_GameBoard[i_Row + 1, i_Col - 1].coin.player; // Zvika: "row+1 col-1 ze down-left. lo evanti ma kore po"
            }
            else
            {
                playerInCell = i_GameBoard[i_Row - 1, i_Col + 1].coin.player;
            }

            return playerInCell;
        }

        private eCellOwner checkUpLeftCellOwner(Cell[,] i_GameBoard, short i_Col, short i_Row) 
        {
            eCellOwner playerInCell;

            if (i_GameBoard[i_Row, i_Col].coin.player == eCellOwner.Player2)
            {
                playerInCell = i_GameBoard[i_Row + 1, i_Col + 1].coin.player;
            }
            else
            {
                playerInCell = i_GameBoard[i_Row - 1, i_Col - 1].coin.player;
            }

            return playerInCell;
        }

        private bool isCoinOpponentPlayer(eCellOwner i_CurrentPlayer, eCellOwner i_CellOwnerToCheck)
        {
            return (i_CellOwnerToCheck != i_CurrentPlayer && i_CellOwnerToCheck !=eCellOwner.Empty);
        }

        private short countPossibleSkipsFromCoin(Cell[,] i_GameBoard, short i_Col, short i_Row)
        {
            short skipsCount = 0;
            eCellOwner currentPlayer = i_GameBoard[i_Col, i_Row].coin.player;
            bool isKing = i_GameBoard[i_Col, i_Row].coin.isKing;

            if (isCoinOpponentPlayer(currentPlayer, checkUpRightCellOwner(i_GameBoard,i_Col,i_Row)))
            {
                ++skipsCount;
            }

            if (isCoinOpponentPlayer(currentPlayer, checkUpLeftCellOwner(i_GameBoard, i_Col, i_Row)))
            {
                ++skipsCount;
            }


            return skipsCount;
        }

        //        private short coutPossibleSkipsFromKINGCoin(Cell[,] i_GameBoard, short i_Col, short i_Row)   for later implementation  -- just add check for back steps

        public bool CheckIfValidMove(Cell[,] i_GameBoard, short i_Col, short i_Row, string i_Input, eDirection i_MoveDirection)
        {
            bool moveIsValid = false;
            short skipsPossible = countPossibleSkipsFromCoin(i_GameBoard, i_Col, i_Row);

            if(skipsPossible == 1)
            {

            }

            switch(i_MoveDirection)
            {
                case eDirection.UpRight:
                    checkUpRightCellOwner(i_GameBoard, i_Col, i_Row);
                    break;
                case eDirection.UpLeft:
                    checkUpLeftCellOwner(i_GameBoard, i_Col, i_Row);
                    break;
                case eDirection.DownRight:
                    break;
                case eDirection.DownLeft:
                    break;
                case eDirection.NullDirection:
                    break;
            }

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

            return true;
        }











        // The upper function is much much better^


        // Need to delete the down function VVVV
        // Keeping for now, in case we run into bugs and need to see another implementation. 

        //public static bool CheckValidMove(short i_CurrentRow, short i_CurrentCol, short i_NewRow, short i_NewCol, short i_PlayerNumber, short i_BoardSize ,ref bool i_PossibleEat,
        //        bool i_IsKing,
        //        GameBoard i_GameBoard)
        //    // we assume newRow and newCol are valid inputs. ---- CHECK IN PREVIOUS CALLS THAT INDEED HAPPENS ----
        //{
        //    bool theMoveIsValid = true;
        //    i_PossibleEat = false;
        //    theMoveIsValid = i_NewRow < i_BoardSize && i_NewCol < i_BoardSize; // Checking out of bounds
        //    eDirection inputDirection = getDirectionFromInput(i_CurrentRow, i_CurrentCol, i_NewRow, i_NewCol);

        //    if(inputDirection != eDirection.NullDirection)
        //    {
        //        if ((!i_IsKing && (inputDirection == eDirection.DownLeft || inputDirection == eDirection.DownRight)))
        //        {
        //            theMoveIsValid = false;
        //        }

        //        else
        //        {
        //            //theMoveIsValid = checkValidMoveToAllDirections(i_NewRow, i_NewCol, i_PlayerNumber, i_BoardSize, i_GameBoard, ref i_PossibleEat, inputDirection);
        //        }
        //    }

        //    return theMoveIsValid;
        //}

        /*private static bool checkValidMoveToAllDirections(
            short i_NewRow,
            short i_NewCol,
            short i_PlayerNumber,
            short i_BoardSize,
            GameBoard i_GameBoard,
            ref bool i_PossibleEat,
            eDirection i_Direction)    --------------- need to be fixed
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
        */

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
            if(rows == 1 && cols == -1)
            {
                resultDirection = eDirection.UpRight;
            }

            //  UP              LEFT
            if(rows == 1 && cols == 1)
            {
                resultDirection = eDirection.UpLeft;
            }

            // DOWN             RIGHT
            if(rows == -1 && cols == -1)
            {
                resultDirection = eDirection.DownRight;
            }

            // DOWN            LEFT
            if(rows == -1 && cols == 1)
            {
                resultDirection = eDirection.DownLeft;
            }

            return resultDirection;
        }





    }
}
    