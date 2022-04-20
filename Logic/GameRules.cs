using System;
using System.Data;
using Logic;

namespace Logic
{
    class GameRules   // mostly valitition
    {
        private eCellOwner checkUpRightCellOwner(Cell[,] i_gameBoard, short i_col, short i_row)   
        {
            eCellOwner playerInCell;

            if(i_gameBoard[i_row, i_col].coin.player == eCellOwner.Player2)
            {
                playerInCell = i_gameBoard[i_row + 1, i_col - 1].coin.player;
            }
            else
            {
                playerInCell = i_gameBoard[i_row - 1, i_col + 1].coin.player;
            }

            return playerInCell;
        }

        private eCellOwner checkUpLeftCellOwner(Cell[,] i_gameBoard, short i_col, short i_row)
        {
            eCellOwner playerInCell;

            if (i_gameBoard[i_row, i_col].coin.player == eCellOwner.Player2)
            {
                playerInCell = i_gameBoard[i_row + 1, i_col + 1].coin.player;
            }
            else
            {
                playerInCell = i_gameBoard[i_row - 1, i_col - 1].coin.player;
            }

            return playerInCell;
        }

        private bool isCoinOponnentPlayer(eCellOwner i_currentPlayer, eCellOwner i_cellOwnerToCheck)
        {
            return (i_cellOwnerToCheck != i_currentPlayer && i_cellOwnerToCheck !=eCellOwner.Empty);
        }

        private short coutPossibleSkipsFromCoin(Cell[,] i_gameBoard, short i_col, short i_row)
        {
            short skipsCount = 0;
            eCellOwner currentPlayer = i_gameBoard[i_col, i_row].coin.player;

            if (isCoinOponnentPlayer(currentPlayer, checkUpRightCellOwner(i_gameBoard,i_col,i_row)))
            {
                ++skipsCount;
            }

            if (isCoinOponnentPlayer(currentPlayer, checkUpLeftCellOwner(i_gameBoard, i_col, i_row)))
            {
                ++skipsCount;
            }

            return skipsCount;
        }

        //        private short coutPossibleSkipsFromKINGCoin(Cell[,] i_gameBoard, short i_col, short i_row)   for later implementation  -- just add check for back steps

        public bool CheckIfValidMove(Cell[,] i_gameBoard, short i_col, short i_row, string i_input, eDirection i_moveDirection)
        {
            bool moveIsValid = false;
            short skipsPossible = coutPossibleSkipsFromCoin(i_gameBoard, i_col, i_row);

            if(skipsPossible == 1)
            {

            }

            switch (i_moveDirection)
            {
                case eDirection.UpRight:
                    checkUpRightCellPlayer(i_gameBoard, i_col, i_row);
                    break;
                case eDirection.UpLeft:
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














        public static bool CheckValidMove(short i_CurrentRow, short i_CurrentCol, short i_NewRow, short i_NewCol, short i_PlayerNumber, short i_BoardSize ,ref bool i_PossibleEat,
                bool i_IsKing,
                GameBoard i_GameBoard)
            // we assume newRow and newCol are valid inputs. ---- CHECK IN PREVIOUS CALLS THAT INDEED HAPPENS ----
        {
            bool theMoveIsValid = true;
            i_PossibleEat = false;
            theMoveIsValid = i_NewRow < i_BoardSize && i_NewCol < i_BoardSize; // Checking out of bounds
            Engine.eDirection inputDirection = getDirectionFromInput(i_CurrentRow, i_CurrentCol, i_NewRow, i_NewCol);

            if(inputDirection != Engine.eDirection.NullDirection)
            {
                if ((!i_IsKing && (inputDirection == Engine.eDirection.DownLeft || inputDirection == Engine.eDirection.DownRight)))
                {
                    theMoveIsValid = false;
                }

                else
                {
                    //theMoveIsValid = checkValidMoveToAllDirections(i_NewRow, i_NewCol, i_PlayerNumber, i_BoardSize, i_GameBoard, ref i_PossibleEat, inputDirection);
                }
            }

            return theMoveIsValid;
        }

        /*private static bool checkValidMoveToAllDirections(
            short i_NewRow,
            short i_NewCol,
            short i_PlayerNumber,
            short i_BoardSize,
            GameBoard i_GameBoard,
            ref bool i_PossibleEat,
            Engine.eDirection i_Direction)    --------------- need to be fixed
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
            short cols = (short)(i_CurrentCol - i_NewCol);

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
    