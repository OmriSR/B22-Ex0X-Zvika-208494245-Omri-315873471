using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public enum eDirection { UpRight, UpLeft, DownRight, DownLeft, NullDirection }
    class Engine
    {
        bool m_toQuit = false;
        DataProcessor m_inputChecker = null;
        short m_whosTurnIsIt = 0;

        public void StartGame(short boardSize)
        {
            GameBoard gameBoard = new GameBoard(boardSize);

            while (m_toQuit == false)
            {

                // whos turn is it?

                string COLrow = getMove();

             //   if ( m_inputChecker.IsValidInput(COLrow) == false )
            //    {
                    /* a method containing a loop of getMove() until it is corrected*/  //omri
           //     }

            //    if (m_inputChecker.CheckIfQuit(COLrow) == true)
            //    {
            //        m_toQuit = false;
           //     }

            //    else
           //     {
                    // any valid moves left? if not - its a tie  ---- checkIfTie method  
                    //can coin eat?     ---- canCoinEat method
                    //move coin    
                    //do we have a winner/loser?       ---- checkIfWon method
                    // going back to whos turn is it -- if the player ate the other needs to check if can eat again
            //    }
            }
        }


        private string getMove()
        {
            /*get move from UI
              translate given move to indices
              assign via output parametters new pos*/

            return "needs to be implimented";
        }


        // Moves coin after we know that the move is valid
        public void MoveCoin(Cell[,] i_GameBoard, Coin i_Player, short i_OldRow, short i_OldCol, short i_NewRow, short i_NewCol)
        {
            i_GameBoard[i_OldRow, i_OldCol].isEmpty = true;
            i_GameBoard[i_NewRow, i_NewCol].isEmpty = false;
            i_GameBoard[i_NewRow, i_NewCol].coin = i_Player;
        }


        private bool checkIfNextMoveMakesAKing(short i_BoardSize, short i_NewRow, eCellOwner i_PlayerNumber)
        {
            return ((i_NewRow == 0 && i_PlayerNumber == eCellOwner.Player1) ||
                          (i_NewRow == i_BoardSize - 1 && i_PlayerNumber == eCellOwner.Player2));
        }

        // we use this function when we know that in the current step a player does, he makes his coin a king.
        public static void makeKing(Cell[,] i_GameBoard, short i_Row, short i_Col) // CHANGE TO PRIVATE!!! ITS PUBLIC FOR TESTINGS
        {
            i_GameBoard[i_Row, i_Col].coin.isKing = true;
        }

        public bool ValidBoardSize(string i_UserInput)
        {
            return (i_UserInput != "1" && i_UserInput != "2" && i_UserInput != "3");
        }

    }
}
