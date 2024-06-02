using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Checkers.Helpers;
using Checkers.Models;
using Checkers.ViewModels;
using System.Collections.Generic;
 

namespace Checkers.Services
{
    public class GameService
    {
        private int whiteCheckers = 12;
        private int blackCheckers = 12;



        private (int line, int column)? _currentCell, _newCell;

        private PlayerType _currentPlayer = PlayerType.Black;

        public event Action PlayerChanged;
        public PlayerType CurrentPlayer
        {
            get { return _currentPlayer; }

            set
            {
                _currentPlayer = value;
                PlayerChanged?.Invoke();
            }
        }

        public ObservableCollection<Cell> Cells { get; set; }
        public GameService(bool allowMultiJump = false)
        {
            _currentCell = null;
            _newCell = null;
            _currentPlayer = PlayerType.Black;

        }

        public bool IsMoveValid(ObservableCollection<Cell> cells)
        {
            if (_currentCell == null || _newCell == null)
                return false;

            if (cells[_currentCell.Value.line * 8 + _currentCell.Value.column].Content == CheckerTypes.WhiteKing ||
                               cells[_currentCell.Value.line * 8 + _currentCell.Value.column].Content == CheckerTypes.BlackKing)
            {
                return IsKingSimpleMoveValid() || IsKingJumpMoveValid(cells) || IsKingMultiJumpValid(cells);
            }

            return IsMultiJumpValid(cells) || IsJumpMoveValid(cells) || IsSimpleMoveValid();
        }

        private (int, int) identifyOpponentPieces(int firstMiddleCellLine, int firstMiddleCellColumn, int secondMiddleCellLine, int secondMiddleCellColumn)
        {
            if (_currentPlayer == PlayerType.Black)
            {
                firstMiddleCellLine = _currentCell.Value.line - 1;
                secondMiddleCellLine = _currentCell.Value.line - 3; //black

                if (_currentCell.Value.column - _newCell.Value.column == -4)
                {
                    firstMiddleCellColumn = _currentCell.Value.column + 1;
                    secondMiddleCellColumn = _currentCell.Value.column + 3;
                }

                else if (_currentCell.Value.column - _newCell.Value.column == 4)
                {
                    firstMiddleCellColumn = _currentCell.Value.column - 1;
                    secondMiddleCellColumn = _currentCell.Value.column - 3;
                }
            }
            else if (_currentPlayer == PlayerType.White)
            {
                firstMiddleCellLine = _currentCell.Value.line + 1;
                secondMiddleCellLine = _currentCell.Value.line + 3; //white

                if (_currentCell.Value.column - _newCell.Value.column == -4)
                {
                    firstMiddleCellColumn = _currentCell.Value.column + 1;
                    secondMiddleCellColumn = _currentCell.Value.column + 3;
                }

                else if (_currentCell.Value.column - _newCell.Value.column == 4)
                {
                    firstMiddleCellColumn = _currentCell.Value.column - 1;
                    secondMiddleCellColumn = _currentCell.Value.column - 3;
                }
            }

            if (firstMiddleCellLine < 0 || firstMiddleCellLine > 7 ||
                firstMiddleCellColumn < 0 || firstMiddleCellColumn > 7 ||
                    secondMiddleCellLine < 0 || secondMiddleCellLine > 7 ||
                     secondMiddleCellColumn < 0 || secondMiddleCellColumn > 7)
            {
                return (-1, -1);
            }

            int firstMiddleCellIndex = firstMiddleCellLine * 8 + firstMiddleCellColumn;
            int secondMiddleCellIndex = secondMiddleCellLine * 8 + secondMiddleCellColumn;

            return (firstMiddleCellIndex, secondMiddleCellIndex);
        }

        public (int, int) identifyOpponentPiecesForKing(int firstMiddleCellLine, int firstMiddleCellColumn, int secondMiddleCellLine, int secondMiddleCellColumn, Cell selectedCell, Cell destinationCell)
        {
            if (destinationCell.Line < selectedCell.Line)
            {
                firstMiddleCellLine = selectedCell.Line - 1;
                secondMiddleCellLine = selectedCell.Line - 3;

                if (selectedCell.Column - destinationCell.Column == -4)
                {
                    firstMiddleCellColumn = selectedCell.Column + 1;
                    secondMiddleCellColumn = selectedCell.Column + 3;
                }

                else if (selectedCell.Column - destinationCell.Column == 4)
                {
                    firstMiddleCellColumn = selectedCell.Column - 1;
                    secondMiddleCellColumn = selectedCell.Column - 3;
                }
            }
            else if (destinationCell.Line > selectedCell.Line)
            {
                firstMiddleCellLine = selectedCell.Line + 1;
                secondMiddleCellLine = selectedCell.Line + 3;

                if (selectedCell.Column - destinationCell.Column == -4)
                {
                    firstMiddleCellColumn = selectedCell.Column + 1;
                    secondMiddleCellColumn = selectedCell.Column + 3;
                }

                else if (selectedCell.Column - destinationCell.Column == 4)
                {
                    firstMiddleCellColumn = selectedCell.Column - 1;
                    secondMiddleCellColumn = selectedCell.Column - 3;
                }
            }



            int firstMiddleCellIndex = firstMiddleCellLine * 8 + firstMiddleCellColumn;
            int secondMiddleCellIndex = secondMiddleCellLine * 8 + secondMiddleCellColumn;

            return (firstMiddleCellIndex, secondMiddleCellIndex);
        }
        private bool IsMultiJumpValid(ObservableCollection<Cell> cells)
        {
            if (_currentCell == null || _newCell == null)
                return false;

            bool isMovingFourSpaces = Math.Abs(_currentCell.Value.line - _newCell.Value.line) == 4 &&
                                     Math.Abs(_currentCell.Value.column - _newCell.Value.column) == 4;
            if(!isMovingFourSpaces)
                return false;
            int rowChange = _newCell.Value.line - _currentCell.Value.line;
            int isWhite = (_currentPlayer == PlayerType.White) ? 4 : -4;
            bool isForwardMove = rowChange == isWhite;

            int firstMiddleCellLine = 0, firstMiddleCellColumn = 0;
            int secondMiddleCellLine = 0, secondMiddleCellColumn = 0;

            int firstMiddleCellIndex = identifyOpponentPieces(firstMiddleCellLine, firstMiddleCellColumn, secondMiddleCellLine, secondMiddleCellColumn).Item1;
            int secondMiddleCellIndex = identifyOpponentPieces(firstMiddleCellLine, firstMiddleCellColumn, secondMiddleCellLine, secondMiddleCellColumn).Item2;



            int middleCellLine = (_currentCell.Value.line + _newCell.Value.line) / 2;
            int middleCellColumn = (_currentCell.Value.column + _newCell.Value.column) / 2;

            int middleCellIndex = middleCellLine * 8 + middleCellColumn;
            int newCellIndex = _newCell.Value.line * 8 + _newCell.Value.column;



            bool areBothOpponentPiecesInBetween =
                 CheckerHelper.GetPlayerTypeFromChecker(cells[firstMiddleCellIndex].Content) != _currentPlayer
                   && cells[firstMiddleCellIndex].Content != CheckerTypes.None
                   && CheckerHelper.GetPlayerTypeFromChecker(cells[secondMiddleCellIndex].Content) != _currentPlayer;

            bool isMiddleCellFree = cells[middleCellIndex].Content == CheckerTypes.None;

            bool isDestinationCellFree = cells[newCellIndex].Content == CheckerTypes.None;


            return isMovingFourSpaces && isForwardMove && isDestinationCellFree && isMiddleCellFree && areBothOpponentPiecesInBetween;
        }
        private bool IsJumpMoveValid(ObservableCollection<Cell> cells)
        {
            if (_currentCell == null || _newCell == null)
                return false;

            bool isMovingTwoSpaces = Math.Abs(_currentCell.Value.line - _newCell.Value.line) == 2 &&
                                     Math.Abs(_currentCell.Value.column - _newCell.Value.column) == 2;
            if(!isMovingTwoSpaces)
                return false;
            int rowChange = _newCell.Value.line - _currentCell.Value.line;
            int isWhite = (_currentPlayer == PlayerType.White) ? 2 : -2;

            bool isForwardMove = rowChange == isWhite;

            int middleCellLine = (_currentCell.Value.line + _newCell.Value.line) / 2;
            int middleCellColumn = (_currentCell.Value.column + _newCell.Value.column) / 2;

            int middleCellIndex = middleCellLine * 8 + middleCellColumn;
            int newCellIndex = _newCell.Value.line * 8 + _newCell.Value.column;



            bool isOpponentPieceInBetween =
                CheckerHelper.GetPlayerTypeFromChecker(cells[middleCellIndex].Content) != _currentPlayer && cells[middleCellIndex].Content != CheckerTypes.None;

            bool isDestinationCellFree = cells[newCellIndex].Content == CheckerTypes.None;

            return isMovingTwoSpaces && isOpponentPieceInBetween && isDestinationCellFree && isForwardMove;

        }

        private bool IsSimpleMoveValid()
        {

            int rowChange = _newCell.Value.line - _currentCell.Value.line;
            int isWhite = (_currentPlayer == PlayerType.White) ? 1 : -1;

            bool isDiagonalMove = Math.Abs(_currentCell.Value.line - _newCell.Value.line) == 1 &&
                                  Math.Abs(_currentCell.Value.column - _newCell.Value.column) == 1;

            bool isForwardMove = rowChange == isWhite;

            return isDiagonalMove && isForwardMove;
        }


        public void AssignCheckerType(IList<Cell> cells, (int line, int column) cell, CheckerTypes checkerType)
        {
            var cellIndex = cell.line * 8 + cell.column;
            cells[cellIndex].Content = checkerType;
        }

        public void MovePiece(ObservableCollection<Cell> cells, ref int whiteCheckers, ref int blackCheckers)
        {
            var currentCellIndex = _currentCell.Value.line * 8 + _currentCell.Value.column;
            var newCellIndex = _newCell.Value.line * 8 + _newCell.Value.column;

            cells[newCellIndex].Content = cells[currentCellIndex].Content;
            cells[newCellIndex].IsOccupied = true;
            cells[currentCellIndex].IsOccupied = false;
            cells[currentCellIndex].Content = CheckerTypes.None;

            if (Math.Abs(_currentCell.Value.line - _newCell.Value.line) == 2)
            {
                int midLine = (_currentCell.Value.line + _newCell.Value.line) / 2;
                int midColumn = (_currentCell.Value.column + _newCell.Value.column) / 2;
                Cell middleCell = cells.FirstOrDefault(c => c.Line == midLine && c.Column == midColumn);

                middleCell.Content = CheckerTypes.None;
                middleCell.IsOccupied = false;
                if(CurrentPlayer == PlayerType.White)
                {
                    blackCheckers--;
                }
                else
                {
                    whiteCheckers--;
                }
            }
            else if (Math.Abs(_currentCell.Value.line - _newCell.Value.line) == 4 && cells[newCellIndex].Content != CheckerTypes.WhiteKing && cells[newCellIndex].Content != CheckerTypes.BlackKing)
            {
                int firstMiddleCellLine = 0, firstMiddleCellColumn = 0;
                int secondMiddleCellLine = 0, secondMiddleCellColumn = 0;

                int firstMiddleCellIndex = identifyOpponentPieces(firstMiddleCellLine, firstMiddleCellColumn, secondMiddleCellLine, secondMiddleCellColumn).Item1;
                int secondMiddleCellIndex = identifyOpponentPieces(firstMiddleCellLine, firstMiddleCellColumn, secondMiddleCellLine, secondMiddleCellColumn).Item2;



                cells[firstMiddleCellIndex].Content = CheckerTypes.None;
                cells[firstMiddleCellIndex].IsOccupied = false;

                cells[secondMiddleCellIndex].Content = CheckerTypes.None;
                cells[secondMiddleCellIndex].IsOccupied = false;


                if (_currentPlayer == PlayerType.White)
                {
                    blackCheckers -= 2;
                }
                else
                {
                    whiteCheckers -= 2;
                }
            }
            else if (Math.Abs(_currentCell.Value.line - _newCell.Value.line) == 4 && (cells[newCellIndex].Content == CheckerTypes.WhiteKing || cells[newCellIndex].Content == CheckerTypes.BlackKing))
            {
                Cell selectedCell = cells.First(c => c.Line == _currentCell.Value.line && c.Column == _currentCell.Value.column);
                Cell destinationCell = cells.First(c => c.Line == _newCell.Value.line && c.Column == _newCell.Value.column);

                int firstMiddleCellLine = 0, firstMiddleCellColumn = 0;
                int secondMiddleCellLine = 0, secondMiddleCellColumn = 0;

                int firstMiddleCellIndex = identifyOpponentPiecesForKing(firstMiddleCellLine, firstMiddleCellColumn, secondMiddleCellLine, secondMiddleCellColumn, selectedCell, destinationCell).Item1;
                int secondMiddleCellIndex = identifyOpponentPiecesForKing(firstMiddleCellLine, firstMiddleCellColumn, secondMiddleCellLine, secondMiddleCellColumn, selectedCell, destinationCell).Item2;



                cells[firstMiddleCellIndex].Content = CheckerTypes.None;
                cells[firstMiddleCellIndex].IsOccupied = false;

                cells[secondMiddleCellIndex].Content = CheckerTypes.None;
                cells[secondMiddleCellIndex].IsOccupied = false;


                if (_currentPlayer == PlayerType.White)
                {
                    blackCheckers -= 2;
                }
                else
                {
                    whiteCheckers -= 2;
                }
            }

            CheckForKing(cells, newCellIndex);

            _currentCell = null;

            _newCell = null;

            if (whiteCheckers == 0 || blackCheckers == 0)
                GameOver(whiteCheckers, blackCheckers);

        }

        public void GameOver(int whiteCheckerNumber, int blackCheckerNumber)
        {
            if (whiteCheckerNumber == 0)
                MessageBox.Show("Game Over: Castiga jucatorul cu piese rosii!!");
            if (blackCheckerNumber == 0)
                MessageBox.Show("Game Over: Castiga jucatorul cu piese albe!!");
        }

        private void CheckForKing(ObservableCollection<Cell> cells, int newIndex)
        {
            if (_currentPlayer == PlayerType.White && _newCell.Value.line == 7)
            {
                
                cells[newIndex].Content = CheckerTypes.WhiteKing;
            }
            else if (_currentPlayer == PlayerType.Black && _newCell.Value.line == 0)
            {
                cells[newIndex].Content = CheckerTypes.BlackKing;
            }
        }

        public void CellClicked(Cell cell)
        {
           
                if (cell == null) throw new ArgumentNullException(nameof(cell));

                if (cell.IsOccupied && CheckerHelper.GetPlayerTypeFromChecker(cell.Content) == _currentPlayer && _currentCell == null)
                {
                    _currentCell = (cell.Line, cell.Column);
                }
                else if (_currentCell.HasValue && !cell.IsOccupied)
                {
                    _newCell = (cell.Line, cell.Column);
                    if (IsMoveValid(Cells))
                    {
                        int whiteCheckers = this.whiteCheckers;
                        int blackCheckers = this.blackCheckers;
                        MovePiece(Cells, ref whiteCheckers, ref blackCheckers);
                        this.whiteCheckers = whiteCheckers;
                        this.blackCheckers = blackCheckers;
                        CurrentPlayer = (CurrentPlayer == PlayerType.White) ? PlayerType.Black : PlayerType.White;
                        _currentCell = null;
                        _newCell = null;
                        PlayerChanged?.Invoke();
                    }
                    else if (_newCell.HasValue)
                    {
                        _newCell = null;
                        _currentCell = null;
                    }
                }
                else if (!cell.IsOccupied || CheckerHelper.GetPlayerTypeFromChecker(cell.Content) != _currentPlayer)
                {
                    _currentCell = null;
                    _newCell = null;
                }
           
        }

        private bool IsKingSimpleMoveValid()
        {
            var rowChange = Math.Abs(_currentCell.Value.line - _newCell.Value.line);
            var columnChange = Math.Abs(_currentCell.Value.column - _newCell.Value.column);
            return (rowChange == 1 && columnChange == 1);
        }

        private bool IsKingJumpMoveValid(ObservableCollection<Cell> cells)
        {
            if (_currentCell == null || _newCell == null)
                return false;

            bool isMovingTwoSpaces = Math.Abs(_currentCell.Value.line - _newCell.Value.line) == 2 &&
                                     Math.Abs(_currentCell.Value.column - _newCell.Value.column) == 2;

            int middleCellLine = (_currentCell.Value.line + _newCell.Value.line) / 2;
            int middleCellColumn = (_currentCell.Value.column + _newCell.Value.column) / 2;

            int middleCellIndex = middleCellLine * 8 + middleCellColumn;
            int newCellIndex = _newCell.Value.line * 8 + _newCell.Value.column;



            bool isOpponentPieceInBetween =
                CheckerHelper.GetPlayerTypeFromChecker(cells[middleCellIndex].Content) != _currentPlayer && cells[middleCellIndex].Content != CheckerTypes.None;

            bool isDestinationCellFree = cells[newCellIndex].Content == CheckerTypes.None;

            return isMovingTwoSpaces && isOpponentPieceInBetween && isDestinationCellFree;

        }

        private bool IsKingMultiJumpValid(ObservableCollection<Cell> cells)
        {
            if (_currentCell == null || _newCell == null)
                return false;

            bool isMovingFourSpaces = Math.Abs(_currentCell.Value.line - _newCell.Value.line) == 4 &&
                                     Math.Abs(_currentCell.Value.column - _newCell.Value.column) == 4;

            Cell selectedCell = cells.First(c => c.Line == _currentCell.Value.line && c.Column == _currentCell.Value.column);
            Cell destinationCell = cells.First(c => c.Line == _newCell.Value.line && c.Column == _newCell.Value.column);

            int firstMiddleCellLine = 0, firstMiddleCellColumn = 0;
            int secondMiddleCellLine = 0, secondMiddleCellColumn = 0;

             int firstMiddleCellIndex = identifyOpponentPiecesForKing(firstMiddleCellLine, firstMiddleCellColumn, secondMiddleCellLine, secondMiddleCellColumn, selectedCell, destinationCell).Item1;
                int secondMiddleCellIndex = identifyOpponentPiecesForKing(firstMiddleCellLine, firstMiddleCellColumn, secondMiddleCellLine, secondMiddleCellColumn, selectedCell, destinationCell).Item2;



            int middleCellLine = (_currentCell.Value.line + _newCell.Value.line) / 2;
            int middleCellColumn = (_currentCell.Value.column + _newCell.Value.column) / 2;

            int middleCellIndex = middleCellLine * 8 + middleCellColumn;
            int newCellIndex = _newCell.Value.line * 8 + _newCell.Value.column;



            bool areBothOpponentPiecesInBetween =
              CheckerHelper.GetPlayerTypeFromChecker(cells[firstMiddleCellIndex].Content) != _currentPlayer
                && cells[firstMiddleCellIndex].Content != CheckerTypes.None
                && CheckerHelper.GetPlayerTypeFromChecker(cells[secondMiddleCellIndex].Content) != _currentPlayer;

            bool isMiddleCellFree = cells[middleCellIndex].Content == CheckerTypes.None;

            bool isDestinationCellFree = cells[newCellIndex].Content == CheckerTypes.None;


            return isMovingFourSpaces && isDestinationCellFree && isMiddleCellFree && areBothOpponentPiecesInBetween;
        }
    }
}
