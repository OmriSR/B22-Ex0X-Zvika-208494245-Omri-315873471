using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public enum eDirection { UpRight, UpLeft, DownRight, DownLeft, NullDirection }
    class Engine
    {
        bool m_toQuit = false;
        DataProcessor m_inputHandler = new DataProcessor();
        eCellOwner m_curPlayer = eCellOwner.Player1;
        bool m_GameOver = false;

        //------------- Main Game Loop ---------------------
        public void StartGame(short i_BoardSize)
        {
            GameBoard gameBoard = new GameBoard(i_BoardSize);
            Cell srcCell, dstCell;
            
            do
            {
                Ex02.ConsoleUtils.Screen.Clear();
                gameBoard.CheckAndUpdateKings();
                gameBoard.PrintBoard(gameBoard.Board, gameBoard.r_BoardSize);
                PrintPlayersTurn(m_curPlayer);
                m_inputHandler.GetInputAndTranslateToCells(gameBoard.Board, out srcCell, out dstCell);
                if(m_toQuit)
                {
                    //print proper massage using UI.
                    break;
                }

                if(!m_inputHandler.MoveValidation(gameBoard, srcCell, dstCell, m_curPlayer))
                {
                    PrintInvalidInput(1);
                    continue; // but dont change turn!
                }

                if(gameBoard.CanCoinEat(srcCell.Coin))
                {
                    if(getEatingDirection(srcCell.Row, srcCell.Col, dstCell.Row, dstCell.Col) != eDirection.NullDirection)
                    {
                        /*make the move and eat.*/
                        Coin playerToMove = gameBoard.Board[srcCell.Row, srcCell.Col].Coin;
                        eDirection directionToMove = getEatingDirection(srcCell.Row, srcCell.Col, dstCell.Row, dstCell.Col);
                        MoveCoin(gameBoard.Board, ref playerToMove, srcCell.Row, srcCell.Col, dstCell.Row, dstCell.Col);
                        MoveCoinWithEatingStep(gameBoard.Board, playerToMove, srcCell.Row, srcCell.Col, directionToMove);
                    }
                    else
                    {
                        // the move was valid and in bounds. But in the other spot was an enemy that we couldn't eat.
                    PrintInvalidInput(1);
                    continue; // but dont change turn!
                    }
                    
                }
                else if(gameBoard.CanOtherCoinsEat(m_curPlayer))
                {
                    PrintInvalidInput(2);
                    continue; // but dont change turn!
                }
                else if(checkIfPlayerOutOfMoves(gameBoard, m_curPlayer))
                {
                    //GameOver --- check if tie and give points accordinly.
                    break;
                }
                else if (gameBoard.Board[dstCell.Row, dstCell.Col].Coin != null && gameBoard.Board[dstCell.Row, dstCell.Col].Coin.Player != eCellOwner.Empty)
                {
                    // the move was valid and in bounds. But in the other spot was an enemy that we couldn't eat.
                    PrintInvalidInput(1);
                    continue; // but dont change turn!
                }
                else
                {
                    // If the move is valid, and there is no possible eat move, so you can do the move you requested.
                    Coin playerToMove = gameBoard.Board[srcCell.Row, srcCell.Col].Coin;
                    MoveCoin(gameBoard.Board, ref playerToMove, srcCell.Row, srcCell.Col, dstCell.Row, dstCell.Col);
                }
                changePlayersTurn(ref m_curPlayer);
            }
            while(!m_toQuit && !m_GameOver);
        }

        private void updateGotMoves(GameBoard i_GameBoard, Coin i_Coin)
        {
            bool isValidMove = true;
            eCellOwner upLeftOwner = eCellOwner.Empty;
            eCellOwner upRightOwner = eCellOwner.Empty;
            eCellOwner downLeftOwner = eCellOwner.Empty;
            eCellOwner downRightOwner = eCellOwner.Empty;
            if(i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpLeft, ref isValidMove) != null && !i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpLeft, ref isValidMove).IsEmpty)
            {
                upLeftOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpLeft, ref isValidMove).Coin.Player;
            }

            if(i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpRight, ref isValidMove) != null && !i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpRight, ref isValidMove).IsEmpty)
            { 
                upRightOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpRight, ref isValidMove).Coin.Player;
            }

            if(i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownLeft, ref isValidMove) != null && !i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownLeft, ref isValidMove).IsEmpty)
            {
                downLeftOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownLeft, ref isValidMove).Coin.Player;
            }

            if(i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownRight, ref isValidMove) != null && !i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownRight, ref isValidMove).IsEmpty)
            {
                downRightOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownRight, ref isValidMove).Coin.Player;
            }

            if(i_Coin != null)
            {
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
        }

        private bool checkIfPlayerOutOfMoves(GameBoard i_GameBoard, eCellOwner i_CurPlayer)
        {
            bool stillGotMoves = false;

            Coin[] curPlayerCoinSet = (i_CurPlayer == eCellOwner.Player1) ? i_GameBoard.Player1CoinSet : i_GameBoard.Player2CoinSet;

            foreach (Coin coin in curPlayerCoinSet)
            {
                if(coin != null)
                {
                    updateGotMoves(i_GameBoard, coin);

                    if (stillGotMoves = stillGotMoves || coin.GotMoves)
                    {
                        break;
                    }

                }

            }

            return !stillGotMoves;
        }

        private bool srcCoinCanEatOrNoCoinCan(GameBoard i_GameBoard, Coin i_SrcCoin)
        {
            return (i_GameBoard.CanCoinEat(i_SrcCoin) || !i_GameBoard.CanOtherCoinsEat(m_curPlayer));  //   if I cant eat and no one else can it --> it is a valid move
        }

        /*UI method!*/
        public string getMoveUI(out bool o_toQuit)
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
        public void MoveCoin(Cell[,] i_GameBoard, ref Coin i_MovingCoin, short i_SrcRow, short i_SrcCol, short i_DstRow, short i_DstCol)
        {
            i_GameBoard[i_SrcRow, i_SrcCol].IsEmpty = true;
            i_GameBoard[i_SrcRow, i_SrcCol].Coin = null;
            i_GameBoard[i_DstRow, i_DstCol].IsEmpty = false;
            i_MovingCoin.Row = i_DstRow;
            i_MovingCoin.Col = i_DstCol;
            i_GameBoard[i_DstRow, i_DstCol].Coin = i_MovingCoin;
        }

        public void MoveCoinWithEatingStep(Cell[,] i_GameBoard, Coin i_MovingCoin, short i_SrcRow, short i_SrcCol, eDirection i_Direction)
        {
            short dstRow = i_SrcRow, dstCol = i_SrcCol;

            switch (i_Direction)
            {
                case eDirection.UpRight:
                    {
                        dstCol++;
                        dstRow--;
                        break;
                    }
                case eDirection.DownRight:
                    {
                        dstCol++;
                        dstRow++;
                        break;
                    }
                case eDirection.UpLeft:
                    {
                        dstCol--;
                        dstRow--;
                        break;
                    }
                case eDirection.DownLeft:
                    {
                        dstCol--;
                        dstRow++;
                        break;
                    }

            }
            if(i_GameBoard[dstRow, dstCol].Coin != null)
            {
                i_GameBoard[dstRow, dstCol].Coin.isAlive = false;
            }
            i_GameBoard[dstRow, dstCol].IsEmpty = true;
            i_GameBoard[dstRow, dstCol].Coin = null;
            

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
        private static void changePlayersTurn(ref eCellOwner i_CurrentPlayer)
        {
            if (i_CurrentPlayer == eCellOwner.Player1)
            {
                i_CurrentPlayer = eCellOwner.Player2;
            }
            else
            {
                i_CurrentPlayer = eCellOwner.Player1;
            }
        }

        private static eDirection getEatingDirection(short i_SrcRow, short i_SrcCol, short i_DstRow, short i_DstCol)
        {
            eDirection directionToReturn = eDirection.NullDirection;
            if(i_SrcRow - i_DstRow == -2 && i_SrcCol - i_DstCol == 2) // down left
            {
                directionToReturn = eDirection.DownLeft;
            }
            if (i_SrcRow - i_DstRow == -2 && i_SrcCol - i_DstCol == -2) // down right
            {
                directionToReturn = eDirection.DownRight;
            }
            if (i_SrcRow - i_DstRow == 2 && i_SrcCol - i_DstCol == 2) // up left
            {
                directionToReturn = eDirection.UpLeft;
            }
            if (i_SrcRow - i_DstRow == 2 && i_SrcCol - i_DstCol == -2) // up right
            {
                directionToReturn = eDirection.UpRight;
            }
            return directionToReturn;
        }


        // MOVE THIS FUNCTION TO UI !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // AND CHANGE ECELLOWNER TO STRING PLAYERNAME!!!!!!!!!!!!!!!!!!!!!!!!!
        public static void PrintPlayersTurn(eCellOwner i_CurrentPlayer)
        {
            Console.WriteLine("This is {0}'s turn!", i_CurrentPlayer);
        }

        public static void PrintInvalidInput(ushort typeOfInvalid)
        {
            switch(typeOfInvalid)
            {
                case 1:
                    {
                        Console.WriteLine("Invalid input! please try again.");
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("Invalid input! You have another move with a possible eat!");
                        break;
                    }
                case 3:
                    {
                        break;
                    }
            }
            System.Threading.Thread.Sleep(2000);

        }
    }

    
}

