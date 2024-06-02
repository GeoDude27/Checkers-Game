using Checkers.Models;
using Checkers.Services;
using Checkers.Views;
using System.Windows;
using System.Windows.Input;

namespace Checkers.ViewModels
{
    public class BoardViewModel : BaseViewModel
    {
        private readonly GameService _gameService;

        private readonly FilesService _jsonService;

        private GameStatus _gameStatus;

        public GameStatus GameStatus
        {
            get => _gameStatus;
            set
            {
                if (_gameStatus != value)
                {
                    _gameStatus = value;
                    OnPropertyChanged(nameof(GameStatus));
                    OnPropertyChanged(nameof(GameStatus.Cells));
                    _gameService.Cells = value.Cells;
                }
            }
        }


        public PlayerType CurrentPlayer
        {
            get { return _gameService.CurrentPlayer; }

            set
            {
                if (_gameService.CurrentPlayer != value)
                {
                    _gameService.CurrentPlayer = value;
                    OnPropertyChanged(nameof(CurrentPlayer));
                }
            }
        }

        public BoardViewModel()
        {

            _gameService = new GameService();
            GameStatus = new GameStatus(PlayerType.White, false);
            _gameService.Cells = GameStatus.Cells;
            _gameService.PlayerChanged += OnPlayerChanged; 
            _jsonService = new FilesService();

            ClickCellCommand = new RelayCommand(ClickCell);
            MovePieceCommand = new RelayCommand(MovePiece, isMoveValid);
            SaveGameCommand = new RelayCommand(SaveGame);
            LoadGameCommand = new RelayCommand(LoadGame);
            NewGameCommand = new RelayCommand(NewGame);
            DisplayInfoCommand = new RelayCommand(DisplayInfo);
            DisplayStatisticsCommand = new RelayCommand(DisplayStatistics);
        }


        public ICommand ClickCellCommand { get; set; }

        public ICommand MovePieceCommand { get; set; }

        public ICommand SaveGameCommand { get; set; }

        public ICommand LoadGameCommand { get; set; }

        public ICommand NewGameCommand { get; set; }

        public ICommand DisplayInfoCommand { get; set; }

        public ICommand DisplayStatisticsCommand { get; set; }

        public void DisplayInfo(object parameter)
        {
            About aboutWindow = new About();
            aboutWindow.Show();
        }

        public void DisplayStatistics(object parameter)
        {
            Statistics statisticsWindow = new Statistics();
            statisticsWindow.Show();
        }

        public void NewGame(object parameter)
        {
            MessageBoxResult result = MessageBox.Show(
            "Want to start a new game?",
            "New Game",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                GameStatus = new GameStatus(PlayerType.White, true);
            }
            else if (result == MessageBoxResult.No)
            {
                GameStatus = new GameStatus(PlayerType.White, false);
            }


        }

        public void SaveGame(object parameter)
        {
            _jsonService.SaveObjectToFile(GameStatus);
        }

        public void LoadGame(object parameter)
        {
            GameStatus = _jsonService.LoadObjectFromFile<GameStatus>();
        }

        public void ClickCell(object parameter)
        {

            var cell = parameter as Cell;

            if (cell != null)
            {
                _gameService.CellClicked(cell);
            }
        }

        public void MovePiece(object parameter)
        {
            int whiteCheckers = GameStatus.WhiteCheckers;
            int blackCheckers = GameStatus.BlackCheckers;

            _gameService.MovePiece(GameStatus.Cells, ref whiteCheckers, ref blackCheckers);

            GameStatus.WhiteCheckers = whiteCheckers;
            GameStatus.BlackCheckers = blackCheckers;
            _gameService.GameOver(GameStatus.WhiteCheckers, GameStatus.BlackCheckers);
        }

        public bool isMoveValid(object parameter)
        {
            return _gameService.IsMoveValid(GameStatus.Cells);
        }

        private void OnPlayerChanged()
        {
            CurrentPlayer = _gameService.CurrentPlayer;
        }

    }
}
