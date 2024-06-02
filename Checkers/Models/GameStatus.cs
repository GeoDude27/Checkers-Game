using Checkers.Services;
using Checkers.ViewModels;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Checkers.Models
{
    public class GameStatus
    {
        public ObservableCollection<Cell> Cells { get; set; }

        public PlayerType CurrentPlayer { get; set; }

        public bool IsMultiJump { get; set; }

        public int WhiteCheckers { get; set; }

        public int BlackCheckers { get; set; }
        public GameStatus()
        {
        }

        public GameStatus(PlayerType currentPlayer, bool isMultiJump)
        {
            Cells = new ObservableCollection<Cell>();
            CurrentPlayer = currentPlayer;
            IsMultiJump = isMultiJump;
            WhiteCheckers = 12;
            BlackCheckers = 12;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bool isCellDark = (i + j) % 2 == 1;
                    if (i < 3 && isCellDark)
                    {
                        Cells.Add(new Cell(isCellDark, i, j, CheckerTypes.WhitePawn));
                    }
                    else if (i > 4 && isCellDark)
                    {
                        Cells.Add(new Cell(isCellDark, i, j, CheckerTypes.BlackPawn));
                    }

                    else
                    {
                        Cells.Add(new Cell(isCellDark, i, j));
                    }
                }
            }
        }


        public GameStatus(ObservableCollection<Cell> cells, PlayerType currentPlayer, bool isMultiJump)
        {
            Cells = cells;
            CurrentPlayer = currentPlayer;
            IsMultiJump = isMultiJump;
        }

        [JsonConstructor]
        public GameStatus(ObservableCollection<Cell> cells, PlayerType currentPlayer, bool isMultiJump, int whiteCheckers, int blackCheckers)
        {
            Cells = cells;
            CurrentPlayer = currentPlayer;
            IsMultiJump = isMultiJump;
            WhiteCheckers = whiteCheckers;
            BlackCheckers = blackCheckers;
        }

       

    }
}
