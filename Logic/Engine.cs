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
        bool m_GameOver = false;

        //------------- Main Game Loop ---------------------
        public void StartGame(short boardSize)
        {
            GameBoard gameBoard = new GameBoard(boardSize);
            Cell srcCell, dstCell;

            while (!m_toQuit && !m_GameOver)
            {
                // whos turn is it????

                m_inputHandler.GetInputAndTranslateToCells(gameBoard.Board, out srcCell, out dstCell);
                if(m_toQuit)
                {
                    //print proper massage using UI.
                    break;
                }

                if (!m_inputHandler.MoveValidation(gameBoard, srcCell, dstCell, m_curPlayer))
                {
                    Console.WriteLine("Invalid input! please try again");
                    continue;  // but dont change turn!
                }

                if (gameBoard.CanCoinEat(srcCell.Coin))
                {
                    /*make the move and eat.*/
                }
                else if(gameBoard.CanOtherCoinsEat(m_curPlayer))
                {
                    Console.WriteLine("Invalid input! please try again");
                    continue;  // but dont change turn!
                }
                else if(checkIfPlayerOutOfMoves(gameBoard, m_curPlayer))
                {
                    //GameOver --- check if tie and give points accordinly.
                    break;
                }
                
            }
        }

        private void updateGotMoves(GameBoard i_GameBoard, Coin i_Coin)
        {
            eCellOwner upLeftOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpLeft).Coin.Player;
            eCellOwner upRightOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpRight).Coin.Player;
            eCellOwner downLeftOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownLeft).Coin.Player;
            eCellOwner downRightOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownRight).Coin.Player;
            i_Coin.GotMoves = false;

            // check if can move up
            if (upLeftOwner != i_Coin.Player || upRightOwner != i_Coin.Player)
            {
                i_Coin.GotMoves = true;
            }

            if (i_Coin.IsKing)
            {
                if (downLeftOwner != i_Coin.Player || downRightOwner != i_Coin.Player)
                {
                    i_Coin.GotMoves = true;
                }
            }
        }

        private bool checkIfPlayerOutOfMoves(GameBoard i_GameBoard, eCellOwner i_CurPlayer)
        {
            bool stillGotMoves = false;

            Coin[] curPlayerCoinSet = (i_CurPlayer == eCellOwner.Player1) ? i_GameBoard.Player1CoinSet : i_GameBoard.Player2CoinSet;

            foreach (Coin coin in curPlayerCoinSet)
            {
                updateGotMoves(i_GameBoard, coin);

                if (stillGotMoves = stillGotMoves || coin.GotMoves)
                {
                    break;
                }
                
            }

            return !stillGotMoves;
        }

        private bool srcCoinCanEatOrNoCoinCan(GameBoard i_GameBoard, Coin i_SrcCoin)
        {
            return (i_GameBoard.CanCoinEat(i_SrcCoin) || !i_GameBoard.CanOtherCoinsEat(m_curPlayer));  //   if I cant eat and no one else can it --> it is a valid move
        }

        /*UI method!*/
        private string getMoveUI(out bool o_toQuit)
        {
            Console.WriteLine("Please enter input....");
            string COLrow = Console.ReadLine();

            if (!m_inputHandler.IsValidInput(COLrow, out o_toQuit))
            {
                Console.WriteLine("Invalid input! please try again");
                COLrow = getMoveUI(out o_toQuit);
            }

            return COLrow;
        }

        // Moves coin after we know that the move is valid
        public void MoveCoin(Cell[,] i_GameBoard, Coin i_MovingCoin, short i_SrcRow, short i_SrcCol, short i_DstRow, short i_DstCol)
        {
            i_GameBoard[i_SrcRow, i_SrcCol].IsEmpty = true;
            i_GameBoard[i_DstRow, i_DstCol].IsEmpty = false;
            i_GameBoard[i_DstRow, i_DstCol].Coin = i_MovingCoin;   
        }

        private bool checkIfNextMoveMakesAKing(short i_BoardSize, short i_NewRow, eCellOwner i_PlayerNumber)
        {
            return ((i_NewRow == 0 && i_PlayerNumber == eCellOwner.Player1) ||
                          (i_NewRow == i_BoardSize - 1 && i_PlayerNumber == eCellOwner.Player2));
        }

        // we use this function when we know that in the current step a player does, he makes his coin a king.
        public static void MakeKing(Cell[,] i_GameBoard, short i_Row, short i_Col) // CHANGE TO PRIVATE!!! ITS PUBLIC FOR TESTINGS
        {
            i_GameBoard[i_Row, i_Col].Coin.IsKing = true;
        }

        public bool ValidBoardSize(string i_UserInput)   
        {
            return (i_UserInput != "1" && i_UserInput != "2" && i_UserInput != "3");
        }
    }
}
