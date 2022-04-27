using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class GameStarter
    {
        readonly UserInterface r_UserConnection = new UserInterface();
        readonly Engine r_GameEngine = new Engine();
        readonly DataProcessor r_InputHandler = new DataProcessor();

        public GameStarter()
        {
            while(true)
            {
                bool quit;
                string player2Name = null, player1Name = r_UserConnection.GetPlayerName();
                short boardSize = r_UserConnection.GetBoardSize();
                short numOfPlayers = r_UserConnection.GetNumOfPlayers();

                player2Name = (numOfPlayers == 2) ? r_UserConnection.GetPlayerName() : "Computer";
                quit = r_GameEngine.StartGame(boardSize, r_UserConnection, r_InputHandler, numOfPlayers, player1Name, player2Name);
                
                if(!quit)
                {
                    break;
                }
            }
        }


    }
}
