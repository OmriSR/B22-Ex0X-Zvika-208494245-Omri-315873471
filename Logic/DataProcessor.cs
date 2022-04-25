using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class DataProcessor  
    {
        //----------- initial input validation ---------------------

        private bool isCapitalLetter(char i_CharToCheck)
        {
            return (i_CharToCheck >= 'A' && i_CharToCheck <= 'Z');
        }

        private bool islowerCaseLetter(char i_CharToCheck)
        {
            return (i_CharToCheck >= 'a' && i_CharToCheck <= 'z');
        }

        public bool IsValidInput(string i_UserInput, out bool o_toQuit)
        {
            bool isValid = false;
            o_toQuit = false;

            if (i_UserInput.Length == 5)
            {
                bool colsValid = (isCapitalLetter(i_UserInput[0]) && isCapitalLetter(i_UserInput[3]));
                bool rowsValid = (islowerCaseLetter(i_UserInput[1]) && islowerCaseLetter(i_UserInput[4]));
                bool signValid = i_UserInput[2] == '>';

                isValid = colsValid && rowsValid && signValid;
            }

            else if(i_UserInput.Length == 1)
            {
                if (isCapitalLetter(i_UserInput[0]) || islowerCaseLetter(i_UserInput[0]))
                {
                    if (isValid = checkIfQuit(i_UserInput[0]))
                    {
                        o_toQuit = true;
                    }
                }
            }

            return isValid;
        }

        private bool checkIfQuit(char i_UserInput)      // maybe should be in UI? (q is console based)      
        {
            return (i_UserInput == 'q' || i_UserInput == 'Q');
        }
      
        private void translatePositionToIndices(string i_CharPos, out short o_col, out short o_row)
        {
            o_col = Convert.ToInt16(i_CharPos[0] - 'A');
            o_row = Convert.ToInt16(i_CharPos[1] - 'a');
        }

        private void translateInputToCells(GameBoard i_Gameboard, string i_Input, out Cell o_curCell, out Cell o_dstCell)
        {
            short curCol, curRow, dstCol, dstRow;
            Cell testSrc, testDst;
            o_curCell = o_dstCell = null;

            string[] positions = i_Input.Split(">");
            
            translatePositionToIndices(positions[0], out curCol, out curRow);
            translatePositionToIndices(positions[1], out dstCol, out dstRow);

            testSrc = new Cell(eCellOwner.Empty, curCol, curRow, i_Gameboard.r_BoardSize);
            testDst = new Cell(eCellOwner.Empty, dstCol, dstRow, i_Gameboard.r_BoardSize);

            if (isCellInBoardBounds(testSrc, i_Gameboard.r_BoardSize) && isCellInBoardBounds(testDst, i_Gameboard.r_BoardSize))
            {
                o_curCell = i_Gameboard.Board[curRow, curCol];
                o_dstCell = i_Gameboard.Board[dstRow, dstCol];
            }

            //    o_curCell.Row = curRow;
            //o_curCell.Col = curCol;
            //o_dstCell.Row = dstRow;
            //o_dstCell.Col = dstCol;
        }

        public void GetInputIfValidTranslateToCells(GameBoard i_GameBoard, out Cell o_CurCell, out Cell o_DstCell)
        {
            Engine testToDelete = new Engine();
            bool toQuit;
            string COLrow = testToDelete.getMoveUI(out toQuit);           // UI method is used!!! handle it

            if (COLrow == "Invalid input! please try again")
            {
                o_CurCell = o_DstCell = null;
            }
            else
            {
                translateInputToCells(i_GameBoard, COLrow, out o_CurCell, out o_DstCell);
            }
        }

        //----------------- Given Move Validation ------------------------

        public bool MoveValidation(GameBoard i_GameBoard, Cell i_SrcCell, Cell i_DstCell, eCellOwner i_CurPlayer)
        {
            bool moveInBounds = IsMoveInBoardBounds(i_SrcCell, i_DstCell, i_GameBoard.r_BoardSize); // move is in board
            bool srcCellIsOwnedByCurrentPlayer = false;
            bool moveIsDiagonalizedAndValid = checkSourceAndDstValidMove(i_GameBoard, i_SrcCell, i_DstCell, i_CurPlayer);
            //  source cell current players coin and dest is empty/opp coin
            if(!i_SrcCell.IsEmpty)
            {
                srcCellIsOwnedByCurrentPlayer = i_SrcCell.Coin.Player == i_CurPlayer;
            }

            return (moveInBounds && srcCellIsOwnedByCurrentPlayer && i_DstCell.IsEmpty && moveIsDiagonalizedAndValid);           
        }

        
        public bool IsMoveInBoardBounds(Cell i_srcCell,Cell i_dstCell, short i_BoardSize)
        {
            return (isCellInBoardBounds(i_srcCell, i_BoardSize) && isCellInBoardBounds(i_dstCell, i_BoardSize));
        }

        private bool isCellInBoardBounds(Cell i_Cell, short i_BoardSize)
        {
            bool colSizeValid = i_Cell.Col >= 0 && i_Cell.Col < i_BoardSize;
            bool rowSizeValid = i_Cell.Row >= 0 && i_Cell.Row < i_BoardSize;

            return (colSizeValid && rowSizeValid);
        }

        private bool checkSourceAndDstValidMove(GameBoard i_GameBoard, Cell i_SrcCell, Cell i_DstCell, eCellOwner i_PlayerNum)
        {
            bool moveIsValid = false;
            short playerNum = -1;
            short srcRow = i_SrcCell.Row, srcCol= i_SrcCell.Col;
            short dstRow = i_DstCell.Row, dstCol = i_DstCell.Col;

            if(i_PlayerNum == eCellOwner.Player1) // player1 = 1, player2 = -1
            {
                playerNum = 1;
            }

            if((srcRow - dstRow == -2 && srcCol - dstCol == -2) || (srcRow - dstRow == -2 && srcCol - dstCol == 2))
            {
                if(playerNum == -1 || i_SrcCell.Coin.IsKing)
                {
                    moveIsValid = i_GameBoard.CanCoinEat(i_SrcCell.Coin) ? true : false;
                }
            }

            else if((srcRow - dstRow == 2 && srcCol - dstCol == -2)
                    || (srcRow - dstRow == 2 && srcCol - dstCol == 2))
                {
                    if(playerNum == 1 || i_SrcCell.Coin.IsKing)
                    {
                        moveIsValid = i_GameBoard.CanCoinEat(i_SrcCell.Coin) ? true : false;
                    }
                }

                else if((srcRow - dstRow == -1 && srcCol-dstCol == -1) || (srcRow - dstRow == -1 && srcCol - dstCol == 1))
                {
                    if (playerNum == -1 || i_SrcCell.Coin.IsKing)
                    {
                        moveIsValid = true;
                    }
                }

                else if ((srcRow - dstRow == 1 && srcCol - dstCol == 1) || (srcRow - dstRow == 1 && srcCol - dstCol == -1)) 
                {
                         if (playerNum == 1 || i_SrcCell.Coin.IsKing)
                         {
                                moveIsValid = true;
                         }
                }

                return moveIsValid;
        }
    }
}
