using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public enum eDirection { UpRight, UpLeft, DownRight, DownLeft, NullDirection }
    class Engine
    {
        bool m_toQuit = false;
        DataProcessor m_inputHandler = null;
        eCellOwner m_curPlayer = eCellOwner.Empty;

        public void StartGame(short boardSize)
        {
            GameBoard gameBoard = new GameBoard(boardSize);
            string COLrow;
            Cell srcCell, dstCell;

            while (m_toQuit == false)
            {
                // whos turn is it????
                m_inputHandler.GetInputAndTranslateToCells(gameBoard.gameBoard, out srcCell, out dstCell);

                if (!m_toQuit)
                {
                    if(!m_inputHandler.MoveValidation(gameBoard, srcCell, dstCell, m_curPlayer))
                    {
                        Console.WriteLine("Invalid input! please try again");
                        continue;
                    }

                 //should we check for tie pre-move or post-move?

                }
             
            }
        }

        /*UI method!*/
        private string getMoveUI(out bool o_toQuit)
        {
            Console.WriteLine("Please enter input....");
            string COLrow = Console.ReadLine();

            if (m_inputHandler.IsValidInput(COLrow, out o_toQuit) == false)
            {
                Console.WriteLine("Invalid input! please try again");
                COLrow = getMoveUI(out o_toQuit);
            }

            return COLrow;
        }


        // Moves coin after we know that the move is valid
        public void MoveCoin(Cell[,] i_GameBoard, Coin i_Player, short i_OldRow, short i_OldCol, short i_NewRow, short i_NewCol)
        {
            i_GameBoard[i_OldRow, i_OldCol].isEmpty = true;
            i_GameBoard[i_NewRow, i_NewCol].isEmpty = false;
            i_GameBoard[i_NewRow, i_NewCol].coin = i_Player;   //???
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
