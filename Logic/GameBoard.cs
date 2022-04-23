using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace Logic
{
    class GameBoard
    {
        public Cell[,] m_GameBoard; 
        public readonly short r_BoardSize;
        private readonly short m_numOfCoins;
        Coin[] m_Player1CoinSet, m_Player2CoinSet;

        private void initBoard(short i_BoardSize)
        {
            short coinsCounter = 0;
            int indexForPlayer1CoinSet = 0;
            for (short row = 0; row < i_BoardSize; row++)
            {
                for (short col = 0; col < i_BoardSize; col++)
                {
                    if ((row < (i_BoardSize / 2) - 1) && ((row % 2 == 0 && col % 2 != 0) || (row % 2 != 0 && col % 2 == 0)))
                    {
                        m_GameBoard[row, col] = new Cell(eCellOwner.Player2, col, row, r_BoardSize);
                        m_Player2CoinSet[coinsCounter++] = m_GameBoard[row, col].Coin;

                    }

                    else if (row > (i_BoardSize / 2) && ((row % 2 == 0 && col % 2 != 0) || (row % 2 != 0 && col % 2 == 0)))
                    {
                        m_GameBoard[row, col] = new Cell(eCellOwner.Player1, col, row, r_BoardSize);
                      //  m_Player1CoinSet[coinsCounter++ - m_numOfCoins] = m_GameBoard[row, col].Coin;
                      m_Player1CoinSet[indexForPlayer1CoinSet] = m_GameBoard[row, col].Coin;
                      indexForPlayer1CoinSet++;
                        coinsCounter++;

                    }

                    else
                    {
                        m_GameBoard[row, col] = new Cell(eCellOwner.Empty, col, row, r_BoardSize);
                    }
                }
            }
        }

        public GameBoard(short i_BoardSize) 
        {
            r_BoardSize = i_BoardSize;
            m_GameBoard = new Cell[r_BoardSize, r_BoardSize];
            m_numOfCoins = getNumOfCoinsByBoardSize(i_BoardSize);
            m_Player1CoinSet = new Coin[m_numOfCoins / 2];
            m_Player2CoinSet = new Coin[m_numOfCoins / 2];

            initBoard(i_BoardSize);
        }

        private short getNumOfCoinsByBoardSize(short i_BoardSize)
        {
            short numOfCoins = -1;
            switch(i_BoardSize)
            {
                case 6:
                    {
                        numOfCoins = 16;
                        break;
                    }
                case 8:
                    {
                        numOfCoins = 24;
                        break;
                    }
                case 10:
                    {
                        numOfCoins = 32;
                        break;
                    }
                default:
                    {
                        numOfCoins = -1;
                        break;
                    }
            }

            return numOfCoins;

        }

        //------------- propertise -----------------------

        public Cell[,] Board
        {
            get
            {
                return m_GameBoard;
            }
        }

        public Coin[] Player1CoinSet
        {
            get
            {
                return m_Player1CoinSet;
            }
        }

        public Coin[] Player2CoinSet
        {
            get
            {
                return m_Player2CoinSet;
            }
        }

        //------------- Check Neighbour Board Cell -------------------------

        public Cell GetSubjectiveNeighbourCell(Coin i_CoinToEat, eDirection i_Direction, ref bool i_IsValidMove) // moving buttom to top
        {
            Cell neighbour = null;
            eDirection subjectiveDirection = eDirection.NullDirection;
            if (i_CoinToEat != null)
            { 
                subjectiveDirection = GetSubjectiveDirection(i_Direction, i_CoinToEat.Player);
            }

            switch (subjectiveDirection)
            {
                case eDirection.UpLeft:
                    if(i_CoinToEat.Row > 0 && i_CoinToEat.Col > 0)
                    {
                        neighbour = m_GameBoard[i_CoinToEat.Row - 1, i_CoinToEat.Col - 1];
                    }
                    else
                    {
                        i_IsValidMove = false;
                    }
                    break;

                case eDirection.UpRight:
                    if(i_CoinToEat.Row > 0 && i_CoinToEat.Col < r_BoardSize - 1)
                    {
                        neighbour = m_GameBoard[i_CoinToEat.Row - 1, i_CoinToEat.Col + 1];
                    }
                    else
                    {
                        i_IsValidMove = false;
                    }
                    break;

                case eDirection.DownLeft:
                    if(i_CoinToEat.Row < r_BoardSize - 1 && i_CoinToEat.Col > 0)
                    {
                        neighbour = m_GameBoard[i_CoinToEat.Row + 1, i_CoinToEat.Col - 1];
                    }
                    else
                    {
                        i_IsValidMove = false;
                    }
                    break;

                case eDirection.DownRight:
                    if(i_CoinToEat.Row < r_BoardSize - 1 && i_CoinToEat.Col < r_BoardSize - 1)
                    {
                        neighbour = m_GameBoard[i_CoinToEat.Row + 1, i_CoinToEat.Col + 1];
                    }
                    else
                    {
                        i_IsValidMove = false;
                    }
                    break;
            }

            return neighbour;
        }   

        public eDirection GetSubjectiveDirection(eDirection i_SubjectiveDirection, eCellOwner i_PlayerNumberOfCoinToEat)
        {
            return (i_PlayerNumberOfCoinToEat == eCellOwner.Player1) ? i_SubjectiveDirection : mirrorDirection(i_SubjectiveDirection);
        }

        private eDirection mirrorDirection(eDirection dirToFlip)    // for moving player2 coins ( he moves Top to Buttom )
        {
            eDirection mirroredDic;

            switch (dirToFlip)
            {
                case eDirection.DownLeft:
                    mirroredDic = eDirection.UpRight;
                    break;
                case eDirection.DownRight:
                    mirroredDic = eDirection.UpLeft;
                    break;
                case eDirection.UpLeft:
                    mirroredDic = eDirection.DownRight;
                    break;
                case eDirection.UpRight:
                    mirroredDic = eDirection.DownLeft;
                    break;
                default:
                    mirroredDic = eDirection.NullDirection;
                    break;
            }
            return mirroredDic;
        }

        //----------- Check for possible 'eat' moves on board -----------

        private List<Coin> getCoinsThatCanEat(eCellOwner i_CurPlayer)
        {
            List<Coin> coinsThatCanEat = new List<Coin>();

            foreach (Coin coin in ((i_CurPlayer == eCellOwner.Player1) ? Player1CoinSet : Player2CoinSet))
            {
                if(coin.isAlive && CanCoinEat(coin))
                {
                    coinsThatCanEat.Add(coin);
                    coin.GotMoves = true;
                }
            }

            return coinsThatCanEat;
        }



        public bool CanOtherCoinsEat(eCellOwner i_CurPlayer)
        {
            List<Coin> ableToEatCoins;
            ableToEatCoins = getCoinsThatCanEat(i_CurPlayer);
            return (ableToEatCoins.Count > 0);
        }

        private bool canKingCoinEat(Coin i_Coin)
        {
            bool isValidMove = true;
            Cell downLeftCell = GetSubjectiveNeighbourCell(i_Coin, eDirection.DownLeft, ref isValidMove);
            Cell downRightCell = GetSubjectiveNeighbourCell(i_Coin, eDirection.DownRight, ref isValidMove);
            eCellOwner currentCoinOwner = i_Coin.Player;
            Coin oppCoin;

            if (downRightCell.Coin != null && isCoinOpponentPlayer(currentCoinOwner, downRightCell.Coin.Player))
            {
                oppCoin = downRightCell.Coin;
                i_Coin.CanEatDownRight = isEatingPassClear(oppCoin, eDirection.DownRight);
            }
            else
            {
                i_Coin.CanEatDownRight = false;
            }

            if (downLeftCell.Coin != null && isCoinOpponentPlayer(currentCoinOwner, downLeftCell.Coin.Player))
            {
                oppCoin = downLeftCell.Coin;
                i_Coin.CanEatDownLeft = isEatingPassClear(oppCoin, eDirection.DownLeft);
            }
            else
            {
                i_Coin.CanEatDownLeft = false;
            }

            return i_Coin.CanEatDownLeft || i_Coin.CanEatDownRight;
        }

        private bool canRegularCoinEat(Coin i_Coin)
        {
            bool isValidMove = true;
            Cell upLeftCell = GetSubjectiveNeighbourCell(i_Coin, eDirection.UpLeft, ref isValidMove);
            Cell upRightCell = GetSubjectiveNeighbourCell(i_Coin, eDirection.UpRight, ref isValidMove);
            eCellOwner currentCoinOwner = eCellOwner.Empty;
            Coin oppCoin;
            bool canRegularCoinEat = false;
            if (i_Coin != null)
            {
                currentCoinOwner = i_Coin.Player;
                if (upRightCell != null && !upRightCell.IsEmpty)
                {
                    if (isCoinOpponentPlayer(currentCoinOwner, upRightCell.Coin.Player)) // if opp to the right
                    {
                        oppCoin = upRightCell.Coin; // check if clear after him

                        i_Coin.CanEatUpRight = isEatingPassClear(oppCoin, eDirection.UpRight);
                    }
                }
                else
                {
                    i_Coin.CanEatUpRight = false;
                }

                if (upLeftCell != null && !upLeftCell.IsEmpty)
                {
                    if (isCoinOpponentPlayer(currentCoinOwner, upLeftCell.Coin.Player)) // if opp to the left
                    {
                        oppCoin = upLeftCell.Coin;
                        i_Coin.CanEatUpLeft = isEatingPassClear(oppCoin, eDirection.UpLeft);
                    }
                }
                else
                {
                    i_Coin.CanEatUpLeft = false;
                }

                canRegularCoinEat = (i_Coin.CanEatUpRight || i_Coin.CanEatUpLeft);
            }
            else
            {
                canRegularCoinEat = false;
            }

            return canRegularCoinEat;

        }

        public bool CanCoinEat(Coin i_Coin)
        {
            bool canEat = canRegularCoinEat(i_Coin);
            if(i_Coin != null)
            {
                canEat = i_Coin.IsKing ? canKingCoinEat(i_Coin) : canEat;
            }
            return canEat;
        }

        private bool isEatingPassClear(Coin i_CoinToEat, eDirection eatingDirection)
        {
            eDirection directionToCheck = mirrorDirection(eatingDirection);
            bool isValidMove = true;
            //    return GetSubjectiveNeighbourCell(i_CoinToEat, eatingDirection).IsEmpty;
            Cell subjectiveNeighbourCell = GetSubjectiveNeighbourCell(i_CoinToEat, directionToCheck, ref isValidMove);
            bool isEmpty = true;
            if(subjectiveNeighbourCell != null)
            {
                isEmpty = subjectiveNeighbourCell.IsEmpty;
            }

            return isEmpty && isValidMove;
        }

        private bool isCoinOpponentPlayer(eCellOwner i_CurrentPlayer, eCellOwner i_CellOwnerToCheck)
        {
            return (i_CellOwnerToCheck != i_CurrentPlayer && i_CellOwnerToCheck != eCellOwner.Empty);
        }


        //-------------------printing methods---------------------------

        public void PrintBoard(Cell[,] i_GameBoard, short i_GameSize)
        {
            short currentCol = 0;
            printFirstRow(i_GameSize);
            
            for (short row = 1; row < i_GameSize * 2 + 2; row++)
            {
                for (short col = 0; col < i_GameSize * 4 + 2; col++)
                {
                    if(!printFirstCol(row, col,ref currentCol))
                    {
                        if(!printShaveSign(row))
                        {
                            if(!printCommaSign(row, col))
                            {
                                if(!checkIfToPrintCoinsOnBoard(i_GameBoard, row, col, i_GameSize))
                                {
                                    Console.Write(' ');
                                }
                            }
                        }
                    }
                }
                Console.Write(Environment.NewLine);
            }

        }

        private bool checkIfToPrintCoinsOnBoard(Cell[,] i_GameBoard, short i_Row, short i_Col, short i_GameSize)
        {
            bool didPrint = false;
            if(i_Row != 0 && i_Row % 2 == 0)
            {
                short rowCheck = (short)(i_Row / 2 - 1);
                short colCheck = (short)(i_Col / 4);
                if(colCheck == i_GameSize)
                {
                    colCheck--;
                }

                if(i_GameBoard[rowCheck, colCheck].Coin == null)
                {
                    didPrint = false;
                }
                else if (!i_GameBoard[rowCheck, colCheck].IsEmpty && i_Col % 4 == 3)
                {
                    didPrint = printCoinsOnBoard(i_GameBoard, rowCheck, colCheck);
                }
            }

            return didPrint;
        }

        private bool printCoinsOnBoard(Cell[,] i_GameBoard, short i_RowToCheck, short i_ColToCheck)
        {
            bool didPrint = false;
            if (i_GameBoard[i_RowToCheck, i_ColToCheck].Coin.Player == eCellOwner.Player1)
            {
                if (i_GameBoard[i_RowToCheck, i_ColToCheck].Coin.IsKing)
                {
                    Console.Write('K');
                }
                else
                {
                    Console.Write('X');
                }

                didPrint = true;
            }
            else if (i_GameBoard[i_RowToCheck, i_ColToCheck].Coin.Player == eCellOwner.Player2)
            {
                if (i_GameBoard[i_RowToCheck, i_ColToCheck].Coin.IsKing)
                {
                    Console.Write('U');
                }
                else
                {
                    Console.Write('O');
                }
                didPrint = true;
            }

            return didPrint;
        }

        private void printFirstRow(short i_GameSize)
        {
            short row = 0;
            for (short i = 0; i < i_GameSize * 4 + 1; i++)
            {
                if (i % 4 == 0 && i != 0) // First Row drawing
                {
                    Console.Write((char)(row + 'A'));
                    row++;
                }
                else if (i!=0)
                {
                    Console.Write(' ');
                }
            }
            Console.Write(Environment.NewLine);
        }

        private bool printFirstCol(short i_Row, short i_Col, ref short i_CurrentCol)
        {
            bool didPrint = false;
            if (i_Col == 0 && i_Row % 2 == 0 && i_Row != 0)
            {
                Console.Write((char)(i_CurrentCol + 'a'));
                i_CurrentCol++;
                didPrint = true;
            }

            return didPrint;
        }

        private bool printShaveSign(short i_currentRow)
        {
            bool didPrint = false;
            if (i_currentRow % 2 != 0)
            {
                didPrint = true;
                Console.Write('=');
            }

            return didPrint;
        }

        private bool printCommaSign(short i_Row, short i_Col)
        {
            bool didPrint = false;
            if (i_Col % 4 == 1 && i_Row != 0 && i_Row % 2 == 0)
            {
                Console.Write('|');
                didPrint = true;
            }

            return didPrint;
        }

        public void CheckAndUpdateKings()
        {
            for(short row = 0; row < r_BoardSize; row++)
            {
                for(short col = 0; col < r_BoardSize; col++)
                {
                    if(m_GameBoard[row, col].Coin != null)
                    {
                        if(m_GameBoard[row, col].Coin.Player == eCellOwner.Player1 && row == 0)
                        {
                            m_GameBoard[row, col].Coin.IsKing = true;
                        }
                        else if(m_GameBoard[row, col].Coin.Player == eCellOwner.Player2 && row == r_BoardSize - 1)
                        {
                            m_GameBoard[row, col].Coin.IsKing = true;
                        }
                    }
                }
            }
        }
    }
}
