using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace Logic
{
    class GameBoard
    {
        /*??internal??*/ Cell[,] m_GameBoard; 
        public readonly short r_BoardSize;
        private readonly short m_numOfCoins;
        Coin[] m_coinsPlayer1, m_coinsPlayer2;

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
                        m_coinsPlayer2[coinsCounter++] = m_GameBoard[row, col].coin;

                    }

                    else if (row > (i_BoardSize / 2) && ((row % 2 == 0 && col % 2 != 0) || (row % 2 != 0 && col % 2 == 0)))
                    {
                        m_GameBoard[row, col] = new Cell(eCellOwner.Player1, col, row, r_BoardSize);
                        m_coinsPlayer1[coinsCounter++ - m_numOfCoins] = m_GameBoard[row, col].coin;

                    }

                    else
                    {
                        m_GameBoard[row, col] = new Cell(eCellOwner.Empty, col, row, r_BoardSize);
                    }
                }
            }
        }

        public GameBoard(short i_BoardSize) // We assume that board size is valid
        {
            r_BoardSize = i_BoardSize;
            m_GameBoard = new Cell[r_BoardSize, r_BoardSize];
            m_numOfCoins = Convert.ToInt16(r_BoardSize / 2 * (r_BoardSize / -1));
            m_coinsPlayer1 = new Coin[m_numOfCoins / 2];
            m_coinsPlayer2 = new Coin[m_numOfCoins / 2];

            initBoard(i_BoardSize);
        }

        public Cell[,] gameBoard
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

        private static Coin getCoinFromCell(Cell[,] i_Board, int i_Row, int i_Col)
        {
            return i_Board[i_Row, i_Col].coin;
        }

        //------------- gameboard utilities ---------------------------
        public Coin getNieghbourCoin(Coin i_Coin, eDirection oppDirection)
        {
            Coin neighbour;

            switch (oppDirection)
            {
                case eDirection.UpLeft:
                    neighbour = m_GameBoard[i_Coin.row - 1, i_Coin.col - 1].coin;
                    break;

                case eDirection.UpRight:
                    neighbour = m_GameBoard[i_Coin.row - 1, i_Coin.col + 1].coin;
                    break;

                case eDirection.DownLeft:
                    neighbour = m_GameBoard[i_Coin.row + 1, i_Coin.col - 1].coin;
                    break;

                case eDirection.DownRight:
                    neighbour = m_GameBoard[i_Coin.row + 1, i_Coin.col + 1].coin;
                    break;
            }

            return neighbour;
        }
        //------------- check board cell for owner --------------------
        public eCellOwner CheckUpRightCellOwner(short i_Col, short i_Row)
        {
            eCellOwner playerInCell;

            if (m_GameBoard[i_Row, i_Col].coin.player == eCellOwner.Player2)
            {
                playerInCell = m_GameBoard[i_Row + 1, i_Col - 1].coin.player;
            }
            else
            {
                playerInCell = m_GameBoard[i_Row - 1, i_Col + 1].coin.player;
            }

            return playerInCell;
        }

        public eCellOwner CheckUpLeftCellOwner(short i_Col, short i_Row)
        {
            eCellOwner playerInCell;

            if (m_GameBoard[i_Row, i_Col].coin.player == eCellOwner.Player2)
            {
                playerInCell = m_GameBoard[i_Row + 1, i_Col + 1].coin.player;
            }
            else
            {
                playerInCell = m_GameBoard[i_Row - 1, i_Col - 1].coin.player;
            }

            return playerInCell;
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

                if(!i_GameBoard[rowCheck, colCheck].isEmpty && i_Col % 4 == 3)
                {
                    didPrint = printCoinsOnBoard(i_GameBoard, rowCheck, colCheck);
                }
            }

            return didPrint;
        }

        private bool printCoinsOnBoard(Cell[,] i_GameBoard, short i_RowToCheck, short i_ColToCheck)
        {
            bool didPrint = false;
            if (i_GameBoard[i_RowToCheck, i_ColToCheck].coin.player == eCellOwner.Player1)
            {
                if (i_GameBoard[i_RowToCheck, i_ColToCheck].coin.isKing)
                {
                    Console.Write('K');
                }
                else
                {
                    Console.Write('X');
                }

                didPrint = true;
            }
            else if (i_GameBoard[i_RowToCheck, i_ColToCheck].coin.player == eCellOwner.Player2)
            {
                if (i_GameBoard[i_RowToCheck, i_ColToCheck].coin.isKing)
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
