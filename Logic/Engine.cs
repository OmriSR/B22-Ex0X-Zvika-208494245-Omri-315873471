using System;
using System.Collections.Generic;
using System.Text;
using Ex02.ConsoleUtils;

namespace Logic
{
    public enum eDirection { UpRight, UpLeft, DownRight, DownLeft, NullDirection }
    class Engine
    {
        private enum eGameMode { SinglePlayer, TwoPlayers }

        bool m_toQuit = false, m_GameOver = false;
        eCellOwner m_curPlayer = eCellOwner.Player1;
        eGameMode m_GameMove;
        private string m_PlayerOneName, m_PlayerTwoName = "Computer";
        private short m_PlayerOnePoints = 0, m_PlayerTwoPoints = 0;

        public bool StartGame(short i_BoardSize, UserInterface i_UserConnections, DataProcessor i_InputHandler, short i_NumOfPlayers, string i_PlayerOneName, string i_PlayerTwoName)
        {
            GameBoard gameBoard = new GameBoard(i_BoardSize);
            m_curPlayer = eCellOwner.Player1;
            m_PlayerOneName = i_PlayerOneName;
            Cell srcCell, dstCell;
            Coin coinThatHasToEatAgain = null;
            bool isValid, thisStepIncludesASpecificCoinEat = false;
            m_GameMove = i_NumOfPlayers == 1 ? eGameMode.SinglePlayer : eGameMode.TwoPlayers;

            if (i_PlayerTwoName != null)
            {
                m_PlayerTwoName = i_PlayerTwoName;
            }

            do
            {
                Ex02.ConsoleUtils.Screen.Clear();
                gameBoard.CheckAndUpdateKings();
                i_UserConnections.PrintBoard(gameBoard.Board, i_BoardSize);
                updateAllGotMoves(gameBoard);
                gameBoard.updateAllEatingSteps();
                i_UserConnections.PrintPlayersTurn(m_curPlayer, i_PlayerOneName, i_PlayerTwoName);

                if (checkIfPlayerOutOfMoves(gameBoard, m_curPlayer))
                {
                    eCellOwner checkOtherPlayerTurns = eCellOwner.Empty;

                    checkOtherPlayerTurns = m_curPlayer == eCellOwner.Player1 ? eCellOwner.Player2 : eCellOwner.Player1;

                    if(checkIfPlayerOutOfMoves(gameBoard, checkOtherPlayerTurns))
                    {
                        i_UserConnections.PrintTie();
                    }
                    else
                    {
                        i_UserConnections.PrintWin(checkOtherPlayerTurns, i_PlayerOneName, i_PlayerTwoName, m_PlayerOnePoints, m_PlayerTwoPoints);
                        UpdatePlayersPoints(gameBoard, ref m_PlayerTwoPoints, ref m_PlayerOnePoints, checkOtherPlayerTurns);
                    }

                    m_GameOver = true;

                    break;
                }

                if (m_GameMove == eGameMode.SinglePlayer && m_curPlayer == eCellOwner.Player2)
                {
                    bool didCoinEatThisTurn = false;
                    Coin coinThatMovedInComputerMove = computerMove(gameBoard, ref didCoinEatThisTurn, i_InputHandler);

                    if (gameBoard.CanCoinEat(coinThatMovedInComputerMove) && didCoinEatThisTurn)
                    {
                        coinThatHasToEatAgain = coinThatMovedInComputerMove;
                        thisStepIncludesASpecificCoinEat = true;
                        i_UserConnections.PrintErrorMassage(3);
                        continue;
                    }

                    thisStepIncludesASpecificCoinEat = false;
                    changePlayersTurn(ref m_curPlayer);
                    continue;
                }

                m_toQuit = i_InputHandler.GetInputIfValidTranslateToCells(gameBoard, out srcCell, out dstCell, out isValid, i_UserConnections);

                    if(!isValid)
                    {
                        i_UserConnections.PrintErrorMassage(1);
                        continue; 
                    }

                    if(m_toQuit)
                    {
                        i_UserConnections.PrintQuitMassage();
                        break;
                    }

                    if(!i_InputHandler.MoveValidation(gameBoard, srcCell, dstCell, m_curPlayer))
                    {
                        i_UserConnections.PrintErrorMassage(1);
                        continue;
                    }

                    if(thisStepIncludesASpecificCoinEat && srcCell.Coin != coinThatHasToEatAgain)
                    {
                        i_UserConnections.PrintErrorMassage(2);
                        continue;
                    }

                    if(gameBoard.CanCoinEat(srcCell.Coin)) 
                    {
                        Coin coinToMove = gameBoard.Board[srcCell.Row, srcCell.Col].Coin;
                        eDirection directionToMove = getEatingDirection(srcCell.Row, srcCell.Col, dstCell.Row, dstCell.Col);

                        if(coinToMove.Player == eCellOwner.Player2)
                        {
                            gameBoard.mirrorDirection(directionToMove);
                        }

                        if(checkIfGivenDirectionIsPossibleEating(directionToMove, coinToMove))
                        {
                            moveCoin(gameBoard.Board, ref coinToMove, srcCell.Row, srcCell.Col, dstCell.Row, dstCell.Col);
                            moveCoinWithEatingStep(gameBoard.Board, coinToMove, srcCell.Row, srcCell.Col, directionToMove);
                        }
                        else
                        {
                            i_UserConnections.PrintErrorMassage(2);
                            continue;
                        }

                        if(gameBoard.CanCoinEat(dstCell.Coin))
                        {
                            coinThatHasToEatAgain = dstCell.Coin;
                            thisStepIncludesASpecificCoinEat = true;
                            i_UserConnections.PrintErrorMassage(4);
                            continue;
                        }                        
                    }
                    else if(gameBoard.CanOtherCoinsEat(m_curPlayer))
                    {
                        i_UserConnections.PrintErrorMassage(2);
                        continue; 
                    }
                    else if(gameBoard.Board[dstCell.Row, dstCell.Col].Coin != null
                            && gameBoard.Board[dstCell.Row, dstCell.Col].Coin.Player != eCellOwner.Empty)
                    {
                        i_UserConnections.PrintErrorMassage(1);
                        continue; 
                    }
                    else
                    {
                        Coin playerToMove = gameBoard.Board[srcCell.Row, srcCell.Col].Coin;
                        moveCoin(gameBoard.Board, ref playerToMove, srcCell.Row, srcCell.Col, dstCell.Row, dstCell.Col);
                    }

                    thisStepIncludesASpecificCoinEat = false;
                    changePlayersTurn(ref m_curPlayer);
            }
            while(!m_toQuit || !m_GameOver);

            if(i_UserConnections.DoYouWantToPlayAnotherGame())
            { 
                m_toQuit = false;
            }

            return m_toQuit;
        }

        private void updateAllGotMoves(GameBoard i_GameBoard)
        {
            foreach (Coin coin in i_GameBoard.Player1CoinSet)
            {
                if (coin != null && coin.isAlive)
                {
                    updateGotMoves(i_GameBoard, coin);
                }

                if(coin != null && !coin.isAlive)
                {
                    killCoin(coin);
                }
            }

            foreach (Coin coin in i_GameBoard.Player2CoinSet)
            {
                if (coin != null && coin.isAlive)
                {
                    updateGotMoves(i_GameBoard, coin);
                }

                if(coin != null && !coin.isAlive)
                {
                    killCoin(coin);
                }
                
            }
        }

        private void killCoin(Coin i_Coin)
        {
            i_Coin.GotMoves = false;
            i_Coin.CanMoveUpLeft = false;
            i_Coin.CanMoveUpRight = false;
            i_Coin.CanMoveDownLeft = false;
            i_Coin.CanMoveDownRight = false;
            i_Coin.CanEatUpLeft = false;
            i_Coin.CanEatUpRight = false;
            i_Coin.CanEatDownLeft = false;
            i_Coin.CanEatDownRight = false;
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
            bool gotMoves = false;

            Coin[] curPlayerCoinSet = (i_CurPlayer == eCellOwner.Player1) ? i_GameBoard.Player1CoinSet : i_GameBoard.Player2CoinSet;

            foreach (Coin coin in curPlayerCoinSet)
            {
                if((coin != null && coin.isAlive) || (coin !=null && coin.GotMoves))
                {
                    gotMoves = true;
                }
            }

            return !gotMoves;
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

        private void moveCoin(Cell[,] i_GameBoard, ref Coin i_MovingCoin, short i_SrcRow, short i_SrcCol, short i_DstRow, short i_DstCol)
        {
            i_GameBoard[i_SrcRow, i_SrcCol].IsEmpty = true;
            i_GameBoard[i_SrcRow, i_SrcCol].Coin = null;
            i_GameBoard[i_DstRow, i_DstCol].IsEmpty = false;
            i_MovingCoin.Row = i_DstRow;
            i_MovingCoin.Col = i_DstCol;
            i_GameBoard[i_DstRow, i_DstCol].Coin = i_MovingCoin;
        }

        private void moveCoinWithEatingStep(Cell[,] i_GameBoard, Coin i_MovingCoin, short i_SrcRow, short i_SrcCol, eDirection i_Direction)
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

        private Coin computerMove(GameBoard i_GameBoard, ref bool i_DidEat, DataProcessor i_InputHandler)
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
                i_DidEat = true;
                possibleEatingCurrentCoins = i_GameBoard.getCoinsThatCanEat(eCellOwner.Player2);
                indexOfCoinToMove = Convert.ToInt16(rnd.Next(possibleEatingCurrentCoins.Count));
                coinToMove = possibleEatingCurrentCoins[indexOfCoinToMove];
            }
            else
            {
                i_DidEat = false;
                coinsThatCanMove = getCoinsThatCanMoveWithoutEat(i_GameBoard);
                indexOfCoinToMove = Convert.ToInt16(rnd.Next(coinsThatCanMove.Count));
                coinToMove = coinsThatCanMove[indexOfCoinToMove];
            }

            Cell srcCell = i_GameBoard.Board[coinToMove.Row, coinToMove.Col];

            do
            {
                directionToMove = getRandomPossibleDirection(coinToMove, computerHasToEatThisTurn);
                getDstRowAndDstColFromDirection(srcCell, directionToMove, out dstCellRow, out dstCellCol, computerHasToEatThisTurn);
            }
            while (!i_InputHandler.IsIndicesInBoardBounds(dstCellRow, dstCellCol, i_GameBoard.r_BoardSize));

            if (computerHasToEatThisTurn)
            {
                moveCoinWithEatingStep(i_GameBoard.Board, coinToMove, coinToMove.Row, coinToMove.Col, directionToMove);
            }

            moveCoin(i_GameBoard.Board, ref coinToMove, coinToMove.Row, coinToMove.Col, dstCellRow, dstCellCol);

           System.Threading.Thread.Sleep(2000);

           return coinToMove;
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

        private List<Coin> getCoinsThatCanMoveWithoutEat(GameBoard i_GameBoard)
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

        private void getDstRowAndDstColFromDirection( Cell i_SrcCell, eDirection i_Direction, out short o_dstRow, out short o_dstCol, bool i_ComputerEatsThisTurn)
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

        private void UpdatePlayersPoints(GameBoard i_GameBoard, ref short m_PlayerTwoPoints, ref short m_PlayerOnePoints, eCellOwner i_Winner)
        {
            short alivePlayerOneCoins = 0, alivePlayerTwoCoins = 0;

            foreach(Coin coin in i_GameBoard.Player1CoinSet)
            {
                if(coin != null && coin.isAlive)
                    if (coin.IsKing)
                    {
                        alivePlayerOneCoins += 4;
                    }
                    else
                    {
                        alivePlayerOneCoins++;
                    }
            }

            foreach (Coin coin in i_GameBoard.Player2CoinSet)
            {
                if (coin != null && coin.isAlive)
                {
                    if(coin.IsKing)
                    {
                        alivePlayerTwoCoins += 4;
                    }
                    else
                    {
                        alivePlayerTwoCoins++;
                    }
                }    
            }

            short addToPoints = (short)(alivePlayerOneCoins - alivePlayerTwoCoins);

            if (addToPoints < 0)
            {
                addToPoints *= -1;
            }

            if (i_Winner == eCellOwner.Player1)
            {
                
                m_PlayerOnePoints += addToPoints;
            }
            else
            {
                m_PlayerTwoPoints += addToPoints;
            }
        }
    }  
}

