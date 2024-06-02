using Checkers.Models;
using Checkers.Services;
using System.Collections.ObjectModel;
using System.IO;
using System;
using System.Linq;


namespace Checkers.Repositories
{
    public class GamesDataRepository
    {
        public GamesDataRepository()
        {
            GameStatistics = LoadGameStatistics();
        }

        public ObservableCollection<GameStatistics> GameStatistics { get; set; }
        public void SaveGameStatistics(GameStatistics gameStatistics)
        {
            var json = JsonService.Serialize(gameStatistics);
            var fileName = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.json";
            File.WriteAllText($"Data/{fileName}", json);
        }

        public void AddGameStatistics(GameStatistics gameStatistics)
        {
            GameStatistics.Add(gameStatistics);
            SaveGameStatistics(gameStatistics);
        }

        public ObservableCollection<GameStatistics> LoadGameStatistics()
        {
            var gameStatistics = new ObservableCollection<GameStatistics>();
            var files = Directory.GetFiles("Data", "*.json");

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                gameStatistics.Add(JsonService.Deserialize<GameStatistics>(json));
            }

            return gameStatistics;
        }

        public int WhiteWins()
        {
            return GameStatistics.Count(gs => gs.Winner == PlayerType.White);
        }

        public int BlackWins()
        {
            return GameStatistics.Count(gs => gs.Winner == PlayerType.Black);
        }

        public int TotalGames()
        {
            return GameStatistics.Count;
        }


    }
}
