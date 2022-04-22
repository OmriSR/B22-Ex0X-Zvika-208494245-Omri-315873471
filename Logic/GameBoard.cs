using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace Logic
{
    class GameBoard
    {
        private Cell[,] m_GameBoard; 
        public readonly short r_BoardSize;
        private readonly short m_numOfCoins;
        Coin[] m_Player1CoinSet, m_Player2CoinSet;

        private void initBoard(short i_BoardSize)
        {
            short coinsCounter = 0;

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
                        m_Player1CoinSet[coinsCounter++ - m_numOfCoins] = m_GameBoard[row, col].Coin;

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
            m_numOfCoins = Convert.ToInt16(r_BoardSize / 2 * (r_BoardSize / -1));
            m_Player1CoinSet = new Coin[m_numOfCoins / 2];
            m_Player2CoinSet = new Coin[m_numOfCoins / 2];

            initBoard(i_BoardSize);
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
                return m_coinsPlayer1;
            }
        }

        public Coin[] Player2CoinSet
        {
            get
            {
                return m_coinsPlayer2;
            }
        }

        //------------- Check Neighbour Board Cell -------------------------

        public Cell GetSubjectiveNeighbourCell(Coin i_Coin, eDirection i_Direction) // moving buttom to top
        {
            Cell neighbour = null;
            eDirection subjectiveDirection = GetSubjectiveDirection(i_Direction, i_Coin.Player);

            switch (subjectiveDirection)
            {
                case eDirection.UpLeft:
                    neighbour = m_GameBoard[i_Coin.Row - 1, i_Coin.Col - 1];
                    break;

                case eDirection.UpRight:
                    neighbour = m_GameBoard[i_Coin.Row - 1, i_Coin.Col + 1];
                    break;

                case eDirection.DownLeft:
                    neighbour = m_GameBoard[i_Coin.Row + 1, i_Coin.Col - 1];
                    break;

                case eDirection.DownRight:
                    neighbour = m_GameBoard[i_Coin.Row + 1, i_Coin.Col + 1];
                    break;
            }

            return neighbour;
        }   

        public eDirection GetSubjectiveDirection(eDirection i_SubjectiveDirection, eCellOwner i_CurrentPlayer)
        {
            return (i_CurrentPlayer == eCellOwner.Player1) ? i_SubjectiveDirection : mirrorDirection(i_SubjectiveDirection);
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
                if (CanCoinEat(coin))
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
            Cell downLeftCell = GetSubjectiveNeighbourCell(i_Coin, eDirection.DownLeft);
            Cell downRightCell = GetSubjectiveNeighbourCell(i_Coin, eDirection.DownRight);
            eCellOwner currentCoinOwner = i_Coin.Player;
            Coin oppCoin;

            if (isCoinOpponentPlayer(currentCoinOwner, downRightCell.Coin.Player))
            {
                oppCoin = downRightCell.Coin;
                i_Coin.CanEatDownRight = isEatingPassClear(oppCoin, eDirection.DownRight);
            }

            if (isCoinOpponentPlayer(currentCoinOwner, downLeftCell.Coin.Player))
            {
                oppCoin = downLeftCell.Coin;
                i_Coin.CanEatDownLeft = isEatingPassClear(oppCoin, eDirection.DownLeft);
            }

            return i_Coin.CanEatDownLeft || i_Coin.CanEatDownRight;
        }

        private bool canRegularCoinEat(Coin i_Coin)
        {
            Cell upLeftCell = GetSubjectiveNeighbourCell(i_Coin, eDirection.UpLeft);
            Cell upRightCell = GetSubjectiveNeighbourCell(i_Coin, eDirection.UpRight);
            eCellOwner currentCoinOwner = i_Coin.Player;
            Coin oppCoin;

            if (isCoinOpponentPlayer(currentCoinOwner, upRightCell.Coin.Player))                    // if opp to the right
            {
                oppCoin = upRightCell.Coin;                                                       // check if clear after him
                i_Coin.CanEatUpRight = isEatingPassClear(oppCoin, eDirection.UpRight);
            }

            if (isCoinOpponentPlayer(currentCoinOwner, upLeftCell.Coin.Player))                      // if opp to the left
            {
                oppCoin = upLeftCell.Coin;
                i_Coin.CanEatUpLeft = isEatingPassClear(oppCoin, eDirection.UpLeft);
            }

            return (i_Coin.CanEatUpRight || i_Coin.CanEatUpLeft);

        }

        public bool CanCoinEat(Coin i_Coin)
        {
            bool canEat = canRegularCoinEat(i_Coin);
            canEat = i_Coin.IsKing ? canKingCoinEat(i_Coin) : canEat;
            return canEat;
        }

        private bool isEatingPassClear(Coin i_CoinToEat, eDirection eatingDirection)
        {
            eDirection directionToCheck = mirrorDirection(eatingDirection);
            return GetSubjectiveNeighbourCell(i_CoinToEat, eatingDirection).IsEmpty;
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

                if(!i_GameBoard[rowCheck, colCheck].IsEmpty && i_Col % 4 == 3)
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
    }
}
