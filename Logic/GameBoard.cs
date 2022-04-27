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
            int indexForPlayer1CoinSet = 0, indexForPlayer2CoinSet = 0;
            for (short row = 0; row < i_BoardSize; row++)
            {
                for (short col = 0; col < i_BoardSize; col++)
                {
                    if ((row < (i_BoardSize / 2) - 1) && ((row % 2 == 0 && col % 2 != 0) || (row % 2 != 0 && col % 2 == 0)))
                    {
                        m_GameBoard[row, col] = new Cell(eCellOwner.Player2, col, row, r_BoardSize);
                        m_Player2CoinSet[indexForPlayer2CoinSet] = m_GameBoard[row, col].Coin;
                        indexForPlayer2CoinSet++;
                    }

                    else if (row > (i_BoardSize / 2) && ((row % 2 == 0 && col % 2 != 0) || (row % 2 != 0 && col % 2 == 0)))
                    {
                        m_GameBoard[row, col] = new Cell(eCellOwner.Player1, col, row, r_BoardSize);
                        //  m_Player1CoinSet[coinsCounter++ - m_numOfCoins] = m_GameBoard[row, col].Coin;
                        m_Player1CoinSet[indexForPlayer1CoinSet] = m_GameBoard[row, col].Coin;
                        indexForPlayer1CoinSet++;
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
            switch (i_BoardSize)
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
                        numOfCoins = 40;
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

        public Cell GetSubjectiveNeighbourCell(Coin i_SrcCoin, eDirection i_Direction, out bool i_IsNeighbourInBounds) // moving buttom to top
        {
            Cell neighbour = null;
            short neighbourRow, neighbourCol;
            eDirection subjectiveDirection = GetSubjectiveDirection(i_Direction, i_SrcCoin.Player);
            i_IsNeighbourInBounds = false;

            switch (subjectiveDirection)
            {
                case eDirection.UpLeft:
                    neighbourRow = Convert.ToInt16(i_SrcCoin.Row - 1);
                    neighbourCol = Convert.ToInt16(i_SrcCoin.Col - 1);
                    break;
                case eDirection.UpRight:
                    neighbourRow = Convert.ToInt16(i_SrcCoin.Row - 1);
                    neighbourCol = Convert.ToInt16(i_SrcCoin.Col + 1);
                    break;
                case eDirection.DownLeft:
                    neighbourRow = Convert.ToInt16(i_SrcCoin.Row + 1);
                    neighbourCol = Convert.ToInt16(i_SrcCoin.Col - 1);
                    break;
                case eDirection.DownRight:
                    neighbourRow = Convert.ToInt16(i_SrcCoin.Row + 1);
                    neighbourCol = Convert.ToInt16(i_SrcCoin.Col + 1);
                    break;
                default:
                    neighbourRow = neighbourCol = -1;
                    break;
            }

            if (CheckIfCellInBoundsByIndices(neighbourCol, neighbourRow, r_BoardSize))
            {
                neighbour = m_GameBoard[neighbourRow, neighbourCol];
                i_IsNeighbourInBounds = true;
            }

            return neighbour;
        }

        public bool CheckIfCellInBoundsByIndices(short i_Col, short i_Row, short i_BoardSize)
        {
            return (i_Row >= 0 && i_Col >= 0 && i_Row <= i_BoardSize - 1 && i_Col <= i_BoardSize - 1);
        }

        public eDirection GetSubjectiveDirection(eDirection i_SubjectiveDirection, eCellOwner i_PlayerNumber)
        {
            return (i_PlayerNumber == eCellOwner.Player1) ? i_SubjectiveDirection : mirrorDirection(i_SubjectiveDirection);
        }

        public eDirection mirrorDirection(eDirection dirToFlip)    // for moving player2 coins ( he moves Top to Buttom )
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

        public List<Coin> getCoinsThatCanEat(eCellOwner i_CurPlayer)
        {
            List<Coin> coinsThatCanEat = new List<Coin>();

            foreach (Coin coin in ((i_CurPlayer == eCellOwner.Player1) ? Player1CoinSet : Player2CoinSet))
            {
                if (coin != null && coin.isAlive && CanCoinEat(coin))
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
            bool isValidMove;
            Cell downLeftCell = GetSubjectiveNeighbourCell(i_Coin, eDirection.DownLeft, out isValidMove);
            Cell downRightCell = GetSubjectiveNeighbourCell(i_Coin, eDirection.DownRight, out isValidMove);
            eCellOwner currentCoinOwner = i_Coin.Player;
            Coin oppCoin;
            bool valueToReturn = false;

            if (downRightCell != null && !downRightCell.IsEmpty && downRightCell.Coin != null && isCoinOpponentPlayer(currentCoinOwner, downRightCell.Coin.Player))
            {
                oppCoin = downRightCell.Coin;
                if(i_Coin.Player == eCellOwner.Player2)
                {
                    i_Coin.CanEatUpLeft = isEatingPathClear(oppCoin, eDirection.DownRight);
                }
                else
                {
                    i_Coin.CanEatDownRight = isEatingPathClear(oppCoin, eDirection.DownRight);
                }
            }
            else
            {
                if(i_Coin.Player == eCellOwner.Player2)
                {
                    i_Coin.CanEatUpLeft = false;
                }
                else
                {
                    i_Coin.CanEatDownRight = false;
                }
            }

            if (downLeftCell != null && !downLeftCell.IsEmpty && downLeftCell.Coin != null && isCoinOpponentPlayer(currentCoinOwner, downLeftCell.Coin.Player))
            {
                oppCoin = downLeftCell.Coin;
                if(i_Coin.Player == eCellOwner.Player2)
                {
                    i_Coin.CanEatUpRight = isEatingPathClear(oppCoin, eDirection.DownLeft);
                }
                else
                {
                    i_Coin.CanEatDownLeft = isEatingPathClear(oppCoin, eDirection.DownLeft);
                }
            }
            else
            {
                if(i_Coin.Player == eCellOwner.Player2)
                {
                    i_Coin.CanEatUpRight = false;
                }
                else
                {
                    i_Coin.CanEatDownLeft = false;
                }
            }

            if(i_Coin.Player == eCellOwner.Player2)
            {
                valueToReturn = i_Coin.CanEatUpRight || i_Coin.CanEatUpLeft;
            }
            else
            {
                valueToReturn = i_Coin.CanEatDownLeft || i_Coin.CanEatDownRight;
            }

            if((i_Coin != null) && (i_Coin.CanEatDownLeft || i_Coin.CanEatDownRight || i_Coin.CanEatUpLeft
                                    || i_Coin.CanEatUpRight))
            {
                i_Coin.GotMoves = true;
                valueToReturn = true;
            }
            return valueToReturn;
        }

        private bool canRegularCoinEat(Coin i_SrcCell)
        {
            bool neighbourInBounds, canRegularCoinEat = false;
            Coin oppCoin;
            eCellOwner currentCoinOwner = eCellOwner.Empty;
            Cell upLeftCell = GetSubjectiveNeighbourCell(i_SrcCell, eDirection.UpLeft, out neighbourInBounds);
            Cell upRightCell = GetSubjectiveNeighbourCell(i_SrcCell, eDirection.UpRight, out neighbourInBounds);
            bool valueToReturn = false;

            currentCoinOwner = i_SrcCell.Player;
            if(upRightCell != null && !upRightCell.IsEmpty
                                   && isCoinOpponentPlayer(
                                       currentCoinOwner,
                                       upRightCell.Coin.Player)) // if opp to the right
            {
                oppCoin = upRightCell.Coin; // check if clear after him
                if(i_SrcCell.Player == eCellOwner.Player2)
                {
                    i_SrcCell.CanEatDownLeft = isEatingPathClear(oppCoin, eDirection.UpRight);
                }
                else
                {
                    i_SrcCell.CanEatUpRight = isEatingPathClear(oppCoin, eDirection.UpRight);
                }
            }
            else
            {
                if(i_SrcCell.Player == eCellOwner.Player2)
                {
                    i_SrcCell.CanEatDownLeft = false;
                }
                else
                {
                    i_SrcCell.CanEatUpRight = false;
                }
            }


            if(upLeftCell != null && !upLeftCell.IsEmpty
                                  && isCoinOpponentPlayer(
                                      currentCoinOwner,
                                      upLeftCell.Coin.Player)) // if opp to the left
            {
                oppCoin = upLeftCell.Coin;
                if (i_SrcCell.Player == eCellOwner.Player2)
                {
                    i_SrcCell.CanEatDownRight = isEatingPathClear(oppCoin, eDirection.UpLeft);
                }
                else
                {
                    i_SrcCell.CanEatUpLeft = isEatingPathClear(oppCoin, eDirection.UpLeft);
                }
            }
            else
            {
                if(i_SrcCell.Player == eCellOwner.Player2)
                {
                    i_SrcCell.CanEatDownRight = false;
                }
                else
                {
                    i_SrcCell.CanEatUpLeft = false;
                }
            }

            if(i_SrcCell.Player == eCellOwner.Player2)
            {
                valueToReturn = i_SrcCell.CanEatDownLeft || i_SrcCell.CanEatDownRight;
            }
            else
            {
                valueToReturn = i_SrcCell.CanEatUpRight || i_SrcCell.CanEatUpLeft;
            }

            return valueToReturn;
        }

        public bool CanCoinEat(Coin i_SrcCell)
        {
            bool canEat = canRegularCoinEat(i_SrcCell);

            //if(i_Coin != null)
            
            canEat = i_SrcCell.IsKing ? canKingCoinEat(i_SrcCell) : canEat;

            
            return canEat;
        }

        private bool isEatingPathClear(Coin i_CoinToEat, eDirection eatingDirection)
        {
            bool eatingPathInBounds;
            eDirection directionToCheck = mirrorDirection(eatingDirection);
            Cell subjectiveNeighbourCell = GetSubjectiveNeighbourCell(i_CoinToEat, directionToCheck, out eatingPathInBounds);
         
            return eatingPathInBounds && subjectiveNeighbourCell.IsEmpty;
        }

        private bool isCoinOpponentPlayer(eCellOwner i_CurrentPlayer, eCellOwner i_CellOwnerToCheck)
        {
            return (i_CellOwnerToCheck != i_CurrentPlayer && i_CellOwnerToCheck != eCellOwner.Empty);
        }

        public bool CheckIfComputerHasToEatThisTurn()
        {
            return CanOtherCoinsEat(eCellOwner.Player2);
        }


        public void updateAllEatingSteps()
        {
            foreach (Coin coin in m_Player1CoinSet)
            {
                if(coin != null && coin.isAlive)
                {
                    CanCoinEat(coin);
                }
            }
            foreach (Coin coin in m_Player2CoinSet)
            {
                if (coin != null && coin.isAlive)
                {
                    CanCoinEat(coin);
                }
            }
        }


        public void CheckAndUpdateKings()
        {
            for (short row = 0; row < r_BoardSize; row++)
            {
                for (short col = 0; col < r_BoardSize; col++)
                {
                    if (m_GameBoard[row, col].Coin != null)
                    {
                        if (m_GameBoard[row, col].Coin.Player == eCellOwner.Player1 && row == 0)
                        {
                            m_GameBoard[row, col].Coin.IsKing = true;
                        }
                        else if (m_GameBoard[row, col].Coin.Player == eCellOwner.Player2 && row == r_BoardSize - 1)
                        {
                            m_GameBoard[row, col].Coin.IsKing = true;
                        }
                    }
                }
            }
        }





        //-------------------printing methods---------------------------



    }
}
