using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    class GameStarter
    {
        UserInterface m_UserConnection = new UserInterface();
        Engine m_GameEngine = new Engine();
        DataProcessor m_InputHandler = new DataProcessor();
        private short Player1Points = 0, Player2Points = 0;

        public GameStarter()
        {
            while(true)
            {
                bool quit;
                string player2Name = null, player1Name = m_UserConnection.GetPlayerName();
                short boardSize = m_UserConnection.GetBoardSize();
                short numOfPlayers = m_UserConnection.GetNumOfPlayers();

                if (numOfPlayers == 2)
                {
                    player2Name = m_UserConnection.GetPlayerName();
                }

                quit = m_GameEngine.StartGame(boardSize, m_UserConnection, m_InputHandler, numOfPlayers, player1Name, player2Name);
                // Do you want to quit?????
                if(!quit)
                {
                    break;
                }
            }
        }


    }
}
