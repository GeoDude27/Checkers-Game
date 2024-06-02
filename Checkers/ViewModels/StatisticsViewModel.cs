using Checkers.Repositories;

namespace Checkers.ViewModels
{
    public class StatisticsViewModel
    {
        GamesDataRepository _gamesDataRepository;

        public int TotalGames => _gamesDataRepository.TotalGames();

        public int WhiteWins => _gamesDataRepository.WhiteWins();

        public int BlackWins => _gamesDataRepository.BlackWins();

        public StatisticsViewModel()
        {
            _gamesDataRepository = new GamesDataRepository();
        }

    }
}
