using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessGame2
{
    class Game
    {
        private Object _lock;
        private int winningNumber;
        private int triesLeft;
        private int currentGuess;
        private IBroadcaster broadcaster;

        public Game(IBroadcaster broadcaster)
        {
            this.broadcaster = broadcaster;
            _lock = new Object();
            Initialize();
        }

        private void Initialize()
        {
            winningNumber = new Random().Next(1,10);
            triesLeft = 9;
            Console.WriteLine("New Game Starting : Winning Number = " + winningNumber);
        }

        internal void PlayerJoin(Client client)
        {
            client.Write("Welcome to the game");
            lock (_lock)
            {
                client.Write("You have "+triesLeft+" left to guess the number");
            }

            while(client.IsActive()){ ReadResponse(client); }       
        }

        private void ReadResponse(Client client)
        {
            string response = client.Read();

            try
            {
                if (Convert.ToInt32(response) > 0 && Convert.ToInt32(response) < 10)
                    Guess(client, response);
                else if (response == "exit")
                    client.Disconnect();
                else
                    client.Write("Unknown command");
            }
            catch (Exception e) { client.Write("Unknown command"); }
        }

        private void Guess(Client client, string response)
        {
            int guess = Convert.ToInt32(response);

            lock (_lock)
            {
                if(guess == currentGuess)
                    client.Write("Same as current guess, try again");
                else if (guess == winningNumber)
                {
                    broadcaster.Broadcast("Congratulation! You have won, " + guess + " is correct.");
                    Initialize();
                    broadcaster.Broadcast("A new game has started");
                }                    
                else
                {
                    client.Write("Wrong guess! Try again.");
                    broadcaster.BroadcastExcept("Another player guessed " + guess + ", and it is wrong. Try again", client);
                    triesLeft--;
                    broadcaster.Broadcast(triesLeft + " tries left");
                    currentGuess = guess;
                }                
            }
        }
    }
}
