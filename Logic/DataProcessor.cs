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

        private void translateInputToCells(Cell [,] board, string i_Input, out Cell o_curCell, out Cell o_dstCell)
        {
            short curCol, curRow, dstCol, dstRow;

            string[] positions = i_Input.Split(">");
            
            translatePositionToIndices(positions[0], out curCol, out curRow);
            translatePositionToIndices(positions[1], out dstCol, out dstRow);

            o_curCell = board[curCol, curRow];
            o_dstCell = board[dstCol, dstRow];
        }

        public void GetInputAndTranslateToCells(Cell[,] i_Board, out Cell o_CurCell, out Cell o_DstCell)     
        {
            bool toQuit;
            string COLrow = getMoveUI(out toQuit);           // UI metod is used!!! handle it
            translateInputToCells(i_Board, COLrow, out o_CurCell, out o_DstCell);
        }

        //----------------- Given Move Validation ------------------------

        public bool MoveValidation(GameBoard i_GameBoard, Cell i_SrcCell, Cell i_DstCell, eCellOwner i_CurPlayer)
        {
            bool moveInBounds = isMoveInBoardBounds(i_SrcCell, i_DstCell, i_GameBoard.r_BoardSize);                                    // move is in board
            bool srcCellIsOwnedByCurrentPlayer = i_SrcCell.Coin.Player == i_CurPlayer;  
            bool dstCellIsntOwnedByCurrentPlayer = i_DstCell.Coin.Player != i_CurPlayer;                                             //  source cell current players coin and dest is empty/opp coin

            return (moveInBounds && srcCellIsOwnedByCurrentPlayer && dstCellIsntOwnedByCurrentPlayer);           
        }

        
        private bool isMoveInBoardBounds(Cell i_srcCell,Cell i_dstCell, short i_BoardSize)
        {
            return (isCellInBoardBounds(i_srcCell, i_BoardSize) && isCellInBoardBounds(i_dstCell, i_BoardSize));
        }

        private bool isCellInBoardBounds(Cell i_Cell, short i_BoardSize)
        {
            bool colSizeValid = i_Cell.Col >= 0 && i_Cell.Col < i_BoardSize;
            bool rowSizeValid = i_Cell.Row >= 0 && i_Cell.Row < i_BoardSize;

            return (colSizeValid && rowSizeValid);
        }
    }
}
