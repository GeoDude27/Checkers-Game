using Checkers.Services;

namespace Checkers.Models
{
    public class GameStatistics
    {
        public GameStatistics(PlayerType winner, int whiteCheckers, int blackCheckers)
        {
            Winner = winner;
            WhiteCheckers = whiteCheckers;
            BlackCheckers = blackCheckers;
        }

        public GameStatistics(GameStatus gameStatus)
        {
            Winner = gameStatus.CurrentPlayer;
            WhiteCheckers = gameStatus.WhiteCheckers;
            BlackCheckers = gameStatus.BlackCheckers;
        }

        public PlayerType Winner { get; set; }

        public int WhiteCheckers { get; set; }

        public int BlackCheckers { get; set; }
    }
}
