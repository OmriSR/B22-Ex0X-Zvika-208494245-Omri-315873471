using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class UserInterface
    {

        public string getMoveUI()
        {
            Console.WriteLine("Please enter input....");
            string COLrow = Console.ReadLine();

            return COLrow;
        }

        public void PrintQuitMassage()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            Console.WriteLine("Hope you had fun! \n See you next time");
        }

        //-----------------  Main Menu  -----------

        public short GetNumOfPlayers()
        {
            string numOfPlayers;

            do
            {
                Console.WriteLine("Please enter the number of players ( 1/2 ): ");
                numOfPlayers = Console.ReadLine();
            }
            while (numOfPlayers != "1" && numOfPlayers != "2");

            return Convert.ToInt16(numOfPlayers);
        }

        public string GetPlayerName()
        {
            Console.WriteLine("Please enter player's name: ");
            return Console.ReadLine();
        }

        public short GetBoardSize()
        {
            string boardSize;
            do
            {
                Console.WriteLine("Please enter board size ( 6/8/10 ) : ");
                boardSize = Console.ReadLine();
            }
            while (boardSize != "6" && boardSize != "8" && boardSize != "10");

            return Convert.ToInt16(boardSize);
        }

        //----------------- Turn Info Printing ---------
        public void PrintPlayersTurn(eCellOwner i_CurrentPlayer)
        {
            Console.WriteLine("This is {0}'s turn!", i_CurrentPlayer);
        }

        public void PrintErrorMassage(ushort typeOfInvalid)
        {
            switch (typeOfInvalid)
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
                        Console.WriteLine("Computer can eat again. Turn stays with computer. ");  
                        break;
                    }
                    case 4:
                        Console.WriteLine("You have another step with eating possible. Turn stays with you. You have to play it.");
                    break;
            }

            System.Threading.Thread.Sleep(2000);
        }

        //------------------ Board Printing------------
        public void PrintBoard(Cell[,] i_GameBoard, short i_GameSize)
        {
            short currentCol = 0;
            printFirstRow(i_GameSize);

            for (short row = 1; row < i_GameSize * 2 + 2; row++)
            {
                for (short col = 0; col < i_GameSize * 4 + 2; col++)
                {
                    if (!printFirstCol(row, col, ref currentCol))
                    {
                        if (!printShaveSign(row))
                        {
                            if (!printCommaSign(row, col))
                            {
                                if (!checkIfToPrintCoinsOnBoard(i_GameBoard, row, col, i_GameSize))
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
            if (i_Row != 0 && i_Row % 2 == 0)
            {
                short rowCheck = (short)(i_Row / 2 - 1);
                short colCheck = (short)(i_Col / 4);
                if (colCheck == i_GameSize)
                {
                    colCheck--;
                }

                if (i_GameBoard[rowCheck, colCheck].Coin == null)
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
                else if (i != 0)
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
