using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public enum eDirection { UpRight, UpLeft, DownRight, DownLeft, NullDirection }
    class Engine
    {
        bool m_toQuit = false;
        DataProcessor m_inputChecker = null;
        short m_whosTurnIsIt = 0;

        public void StartGame(short boardSize)
        {
            GameBoard gameBoard = new GameBoard(boardSize);
            bool isSingleLetter;
            string COLrow;

            while (m_toQuit == false)
            {
                // whos turn is it?
                COLrow = getMoveUI(out isSingleLetter);
                m_toQuit = isSingleLetter && m_inputChecker.CheckIfQuit(COLrow);   
                
                if(!m_toQuit)
                {

                }
           //     {
                    // any valid moves left? if not - its a tie  ---- checkIfTie method  
                    //can coin eat?     ---- canCoinEat method
                    //move coin    
                    //do we have a winner/loser?       ---- checkIfWon method
                    // going back to whos turn is it -- if the player ate the other needs to check if can eat again
            //    }
            }
        }

        /*UI method!*/
        private string getMoveUI(out bool o_isSingleLetter)
        {
            Console.WriteLine("Please enter input....");
            string COLrow = Console.ReadLine();

            while (m_inputChecker.IsValidInput(COLrow, out o_isSingleLetter) == false)
            {
                Console.WriteLine("Invalid input! please try again");
                COLrow = getMoveUI(out o_isSingleLetter);
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

        private List<Coin> getCoinsThatCanEat(GameBoard i_GameBoard)
        {
            List<Coin> coinsThatCanEat = new List<Coin>();

            foreach (Coin coin in i_GameBoard.Player1CoinSet)
            {
                if(canCoinEat(i_GameBoard, coin))
                {
                    coinsThatCanEat.Add(coin);
                }
            }

            return coinsThatCanEat;
        }

        private bool canCoinEat(GameBoard i_Gameboard, Coin i_Coin)
        {
            eCellOwner upLeftCellOwner = i_Gameboard.CheckUpLeftCellOwner(i_Coin.col, i_Coin.row);
            eCellOwner upRighttCellOwner = i_Gameboard.CheckUpRightCellOwner(i_Coin.col, i_Coin.row);
            eCellOwner currentCoinOwner = i_Coin.player;
            Coin oppCoin;
            bool canEat;

            if (isCoinOpponentPlayer(currentCoinOwner,upRighttCellOwner))      // if opp to the right
            {
                oppCoin = i_Gameboard.getNieghbourCoin(i_Coin, eDirection.UpRight);        //check if clear after him
                canEat = (i_Gameboard.CheckUpRightCellOwner(oppCoin.col, oppCoin.row) == eCellOwner.Empty);
            }

            if (isCoinOpponentPlayer(currentCoinOwner, upLeftCellOwner))    // if opp to the left
            { 
                oppCoin = i_Gameboard.getNieghbourCoin(i_Coin, eDirection.UpLeft);   // same
                canEat = (i_Gameboard.CheckUpLeftCellOwner(oppCoin.col, oppCoin.row) == eCellOwner.Empty);
            }

            return true;
        }
        
        private bool isEatingBlocked(Coin i_CoinToEat, eDirection eatingDirection)
        {
            return true;
        }
        private bool isCoinOpponentPlayer(eCellOwner i_CurrentPlayer, eCellOwner i_CellOwnerToCheck)
        {
            return (i_CellOwnerToCheck != i_CurrentPlayer && i_CellOwnerToCheck != eCellOwner.Empty);
        }
    }
}
