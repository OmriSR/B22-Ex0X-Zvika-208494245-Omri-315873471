using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public enum eDirection { UpRight, UpLeft, DownRight, DownLeft, NullDirection }
    class Engine
    {
        private enum eGameMode { SinglePlayer, TwoPlayers }

        bool m_toQuit = false;
        DataProcessor m_inputHandler = new DataProcessor();
        eCellOwner m_curPlayer = eCellOwner.Player1;
        bool m_GameOver = false;
        eGameMode m_GameMove;
        //------------- Main Game Loop ---------------------
        public void StartGame(short i_BoardSize)
        {
            GameBoard gameBoard = new GameBoard(i_BoardSize);
            Cell srcCell, dstCell;
            m_GameMove = eGameMode.SinglePlayer;
            //m_GameMove = GetGameMode();    UI func needs to be written
            do
            {
                Ex02.ConsoleUtils.Screen.Clear();
                gameBoard.PrintBoard(gameBoard.Board, gameBoard.r_BoardSize);
                updateAllGotMoves(gameBoard);
                gameBoard.updateAllEatingSteps();
                PrintPlayersTurn(m_curPlayer);
                
                if (m_GameMove == eGameMode.SinglePlayer && m_curPlayer == eCellOwner.Player2)
                {
                    //    // AI computer move
                    ComputerMove(gameBoard);
                    //    changePlayersTurn(ref m_curPlayer);
                    //    continue;
                }
                else
                {
                    m_inputHandler.GetInputIfValidTranslateToCells(gameBoard, out srcCell, out dstCell);

                    if(srcCell == null || dstCell == null)
                    {
                        PrintInvalidInput(1);
                        continue; // but dont change turn!
                    }

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

                    if(gameBoard.CanCoinEat(srcCell.Coin)) // If a player can eat on left, but more both left+right, it can go right.
                    {
                        //if(getEatingDirection(srcCell.Row, srcCell.Col, dstCell.Row, dstCell.Col) != eDirection.NullDirection)
                        //{
                        /*make the move and eat.*/
                        Coin coinToMove = gameBoard.Board[srcCell.Row, srcCell.Col].Coin;
                        eDirection directionToMove = getEatingDirection(
                            srcCell.Row,
                            srcCell.Col,
                            dstCell.Row,
                            dstCell.Col);
                        if(checkIfGivenDirectionIsPossibleEating(directionToMove, coinToMove))
                        {
                            MoveCoin(gameBoard.Board, ref coinToMove, srcCell.Row, srcCell.Col, dstCell.Row, dstCell.Col);
                            MoveCoinWithEatingStep(gameBoard.Board, coinToMove, srcCell.Row, srcCell.Col, directionToMove);
                        }
                        else
                        {
                            PrintInvalidInput(2);
                            continue; // but dont change turn!
                        }

                        if(gameBoard.CanCoinEat(dstCell.Coin))
                        {
                            Console.WriteLine("You have another step with eating possible. Turn stays with you. You have to play it.");
                           System.Threading.Thread.Sleep(2000);
                            continue;
                        }
                        // directionToMove = gameBoard.GetSubjectiveDirection(directionToMove, srcCell.Coin.Player);
                        


                        //Zvika: if you don't switch turns, it happens that X eats Y, X has no eatings left, but turn is still with X.


                        //Omri:
                        // continue; // dont switch turns


                        //}
                        //else
                        //{
                        //    // the move was valid and in bounds. But in the other spot was an enemy that we couldn't eat.
                        //PrintInvalidInput(1);
                        //continue; // but dont change turn!
                        //}

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
                    else if(gameBoard.Board[dstCell.Row, dstCell.Col].Coin != null
                            && gameBoard.Board[dstCell.Row, dstCell.Col].Coin.Player != eCellOwner.Empty)
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
                }

                gameBoard.CheckAndUpdateKings();
                changePlayersTurn(ref m_curPlayer);
            }
            while(!m_toQuit && !m_GameOver);
        }

        private void updateAllGotMoves(GameBoard i_GameBoard)
        {
            foreach (Coin coin in i_GameBoard.Player1CoinSet)
            {
                if (coin != null && coin.isAlive)
                {
                    updateGotMoves(i_GameBoard, coin);
                }
            }
            foreach (Coin coin in i_GameBoard.Player2CoinSet)
            {
                if (coin != null && coin.isAlive)
                {
                    updateGotMoves(i_GameBoard, coin);
                }
            }
        }
        private void updateGotMoves(GameBoard i_GameBoard, Coin i_Coin)
        {
            bool isValidMoveUpRight, isValidMoveUpLeft, isValidMoveDownRight, isValidMoveDownLeft;
            eCellOwner upLeftOwner = eCellOwner.Empty;
            eCellOwner upRightOwner = eCellOwner.Empty;
            eCellOwner downLeftOwner = eCellOwner.Empty;
            eCellOwner downRightOwner = eCellOwner.Empty;

            if(i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpLeft, out isValidMoveUpLeft) != null && !i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpLeft, out isValidMoveUpLeft).IsEmpty)
            {
                upLeftOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpLeft, out isValidMoveUpLeft).Coin.Player;
            }

            if(i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpRight, out isValidMoveUpRight) != null && !i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpRight, out isValidMoveUpRight).IsEmpty)
            { 
                upRightOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.UpRight, out isValidMoveUpRight).Coin.Player;
            }

            if(i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownLeft, out isValidMoveDownLeft) != null && !i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownLeft, out isValidMoveDownLeft).IsEmpty)
            {
                downLeftOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownLeft, out isValidMoveDownLeft).Coin.Player;
            }

            if(i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownRight, out isValidMoveDownRight) != null && !i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownRight, out isValidMoveDownRight).IsEmpty)
            {
                downRightOwner = i_GameBoard.GetSubjectiveNeighbourCell(i_Coin, eDirection.DownRight, out isValidMoveDownRight).Coin.Player;
            }

            if(i_Coin != null)
            {
                i_Coin.GotMoves = false;
                // check if can move up
                //if (upLeftOwner != i_Coin.Player || upRightOwner != i_Coin.Player)
                //{
                //    i_Coin.GotMoves = true;
                //}

                //if (i_Coin.IsKing)
                //{
                //    if (downLeftOwner != i_Coin.Player || downRightOwner != i_Coin.Player)
                //    {
                //        i_Coin.GotMoves = true;
                //    }
                //}

                if(upLeftOwner != i_Coin.Player && isValidMoveUpLeft)
                {
                    if(i_Coin.Player == eCellOwner.Player2)
                    {
                        i_Coin.CanMoveDownRight = true;
                    }
                    else
                    {
                        i_Coin.CanMoveUpLeft = true;
                    }
                }

                if(upRightOwner != i_Coin.Player && isValidMoveUpRight)
                {
                    if(i_Coin.Player == eCellOwner.Player2)
                    {
                        i_Coin.CanMoveDownLeft = true;
                    }
                    else
                    {
                        i_Coin.CanMoveUpRight = true;
                    }
                        
                }

                if (i_Coin.IsKing)
                {
                    if (downLeftOwner != i_Coin.Player && isValidMoveDownLeft)
                    {
                        if(i_Coin.Player == eCellOwner.Player2)
                        {
                            i_Coin.CanMoveUpRight = true;
                        }
                        else
                        {
                            i_Coin.CanMoveDownLeft = true;
                        }
                       
                    }

                    if(downRightOwner != i_Coin.Player && isValidMoveDownRight)
                    {
                        if(i_Coin.Player == eCellOwner.Player2)
                        {
                            i_Coin.CanMoveUpLeft = true;
                        }
                        else
                        {
                            i_Coin.CanMoveDownRight = true;
                        }
                            
                    }
                }

                if(i_Coin.CanMoveDownRight || i_Coin.CanMoveUpLeft || i_Coin.CanMoveDownLeft || i_Coin.CanMoveUpRight)
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


        private bool checkIfGivenDirectionIsPossibleEating(eDirection i_DirectionToMove, Coin i_CoinToMove)
        {
            bool directionIsEating = false;
            switch(i_DirectionToMove)
            {
                case eDirection.DownLeft:
                    {
                        directionIsEating = i_CoinToMove.CanEatDownLeft;
                        break;
                    }
                case eDirection.DownRight:
                    {
                        directionIsEating = i_CoinToMove.CanEatDownRight;
                        break;
                    }
                case eDirection.UpLeft:
                    {
                        directionIsEating = i_CoinToMove.CanEatUpLeft;
                        break;
                    }
                case eDirection.UpRight:
                    {
                        directionIsEating = i_CoinToMove.CanEatUpRight;
                        break;
                    }
            }
            return directionIsEating;
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
                COLrow = "Invalid input! please try again";
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

        private void changePlayersTurn(ref eCellOwner i_CurrentPlayer)
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

        private eDirection getEatingDirection(short i_SrcRow, short i_SrcCol, short i_DstRow, short i_DstCol)
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

        public void ComputerMove(GameBoard i_GameBoard)
        {
            Random rnd = new Random();
            short dstCellRow, dstCellCol, indexOfCoinToMove;
            bool computerHasToEatThisTurn = (i_GameBoard.CheckIfComputerHasToEatThisTurn());
            List<Coin> possibleEatingCurrentCoins = new List<Coin>();
            List<Coin> coinsThatCanMove = new List<Coin>();
            Coin coinToMove;
            eDirection directionToMove;

            if (computerHasToEatThisTurn)
            {
                possibleEatingCurrentCoins = i_GameBoard.getCoinsThatCanEat(eCellOwner.Player2);
                indexOfCoinToMove = Convert.ToInt16(rnd.Next(possibleEatingCurrentCoins.Count));
                coinToMove = possibleEatingCurrentCoins[indexOfCoinToMove];
            }
            else
            {
                coinsThatCanMove = GetCoinsThatCanMoveWithoutEat(i_GameBoard);
                // IF NO MOVES AVAILABLE ------- COMPUTER LOSES. NEED TO CHECK WE DONT GET HERE WITH 0 MOVES POSSIBLE.
                indexOfCoinToMove = Convert.ToInt16(rnd.Next(coinsThatCanMove.Count));
                coinToMove = coinsThatCanMove[indexOfCoinToMove];
                //Cell srcCell = i_GameBoard.Board[coinToMove.Row, coinToMove.Col];
                //eDirection directionToMove = getRandomPossibleDirection(coinToMove, toMoveWithoutEat);
                //getDstRowAndDstColFromDirection(srcCell, directionToMove, out dstCellRow, out dstCellCol, toMoveWithoutEat);
                //MoveCoin(i_GameBoard.Board, ref coinToMove, srcCell.Row, srcCell.Col, dstCellRow, dstCellCol);

            }
            Cell srcCell = i_GameBoard.Board[coinToMove.Row, coinToMove.Col];
            directionToMove = getRandomPossibleDirection(coinToMove, computerHasToEatThisTurn);
            // directionToMove = i_GameBoard.mirrorDirection(directionToMove);
            getDstRowAndDstColFromDirection(srcCell, directionToMove, out dstCellRow, out dstCellCol, computerHasToEatThisTurn);
            if (computerHasToEatThisTurn)
            {
                MoveCoinWithEatingStep(i_GameBoard.Board, coinToMove, coinToMove.Row, coinToMove.Col, directionToMove);
            }
            MoveCoin(i_GameBoard.Board, ref coinToMove, coinToMove.Row, coinToMove.Col, dstCellRow, dstCellCol);

            //if(i_GameBoard.CanCoinEat(coinToMove))
            //{
            //    Console.WriteLine("COMPUTER HAS ANOTHER STEP THAT CAN EAT. AND WILL DO IT.");
            //}

           System.Threading.Thread.Sleep(2000);
        }


        private eDirection getRandomPossibleDirection(Coin i_Coin, bool i_ComputerHasToEatThisTurn)
        {
            List<eDirection> directionsList = new List<eDirection>();
            Random rnd = new Random();
            fillDirectionsList(ref directionsList, i_Coin, i_ComputerHasToEatThisTurn);
            short randomIndexFromList = Convert.ToInt16(rnd.Next(directionsList.Count));

            return directionsList[randomIndexFromList];
        }

        private void fillDirectionsList(ref List<eDirection> i_ListOfDirections, Coin i_Coin, bool i_ComputerHasToEatThisTurn)
        {
            if(i_ComputerHasToEatThisTurn)
            {
                if (i_Coin.CanEatDownLeft)
                {
                    i_ListOfDirections.Add(eDirection.DownLeft);
                }

                if (i_Coin.CanEatDownRight)
                {
                    i_ListOfDirections.Add(eDirection.DownRight);
                }

                if (i_Coin.CanEatUpLeft)
                {
                    i_ListOfDirections.Add(eDirection.UpLeft);
                }

                if (i_Coin.CanEatUpRight)
                {
                    i_ListOfDirections.Add(eDirection.UpRight);
                }
            }
            else
            {
                if (i_Coin.CanMoveDownLeft)
                {
                    i_ListOfDirections.Add(eDirection.DownLeft);
                }

                if (i_Coin.CanMoveDownRight)
                {
                    i_ListOfDirections.Add(eDirection.DownRight);
                }

                if (i_Coin.CanMoveUpLeft)
                {
                    i_ListOfDirections.Add(eDirection.UpLeft);
                }

                if (i_Coin.CanMoveUpRight)
                {
                    i_ListOfDirections.Add(eDirection.UpRight);
                }
            }
        }

        public List<Coin> GetCoinsThatCanMoveWithoutEat(GameBoard i_GameBoard)
        {
            List<Coin> listOfCoins = new List<Coin>();
            
            foreach (Coin coin in i_GameBoard.Player2CoinSet)
            {
                updateGotMoves(i_GameBoard, coin);
                if (coin.isAlive && (coin.CanMoveDownRight || coin.CanMoveDownLeft || coin.CanMoveUpLeft || coin.CanMoveUpRight))
                {
                    listOfCoins.Add(coin);
                }
            }

            return listOfCoins;
        }

        private void getDstRowAndDstColFromDirection(
            Cell i_SrcCell,
            eDirection i_Direction,
            out short o_dstRow,
            out short o_dstCol, bool i_ComputerEatsThisTurn)
        {
            o_dstRow = i_SrcCell.Row;
            o_dstCol = i_SrcCell.Col;
            if(i_ComputerEatsThisTurn)
            {
                switch (i_Direction)
                {
                    case eDirection.DownLeft:
                        {
                            o_dstRow += 2;
                            o_dstCol -= 2;
                            break;
                        }
                    case eDirection.DownRight:
                        {
                            o_dstRow += 2;
                            o_dstCol += 2;
                            break;
                        }
                    case eDirection.UpLeft:
                        {
                            o_dstRow -= 2;
                            o_dstCol -= 2;
                            break;
                        }
                    case eDirection.UpRight:
                        {
                            o_dstRow -= 2;
                            o_dstCol += 2;
                            break;
                        }
                }
            }
            else
            {
                switch (i_Direction)
                {
                    case eDirection.DownLeft:
                        {
                            o_dstRow += 1;
                            o_dstCol -= 1;
                            break;
                        }
                    case eDirection.DownRight:
                        {
                            o_dstRow += 1;
                            o_dstCol += 1;
                            break;
                        }
                    case eDirection.UpLeft:
                        {
                            o_dstRow -= 1;
                            o_dstCol -= 1;
                            break;
                        }
                    case eDirection.UpRight:
                        {
                            o_dstRow -= 1;
                            o_dstCol += 1;
                            break;
                        }
                }
            }
        }

        


        // MOVE THIS FUNCTION TO UI !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // AND CHANGE ECELLOWNER TO STRING PLAYERNAME!!!!!!!!!!!!!!!!!!!!!!!!!
        public void PrintPlayersTurn(eCellOwner i_CurrentPlayer)
        {
            Console.WriteLine("This is {0}'s turn!", i_CurrentPlayer);
        }

        public void PrintInvalidInput(ushort typeOfInvalid)
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

