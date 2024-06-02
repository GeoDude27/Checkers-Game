using Checkers.Services;
using Checkers.ViewModels;

namespace Checkers.Helpers
{
    public static class CheckerHelper
    {
        public static PlayerType GetPlayerTypeFromChecker(CheckerTypes checkerType)
        {
            switch (checkerType)
            {
                case CheckerTypes.WhitePawn:
                case CheckerTypes.WhiteKing:
                    return PlayerType.White;

                case CheckerTypes.BlackPawn:
                case CheckerTypes.BlackKing:

                    return PlayerType.Black;

                default:
                    return PlayerType.None;

            }
        }
    }
}
