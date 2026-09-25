namespace TicTacToe;

/// <summary>The kinds of games able to be created.</summary>
public enum GameType
{
    NumericalTicTacToe,
    Notakto,
    Gomoku
}

/// <summary>
///  Creates the game types
/// </summary>
public static class GameFactory
{
    /// <summary>
    /// Creates a game of the specified type
    /// </summary>
    /// <param name="gameType">Which kind of game to create.</param>
    /// <param name="playerOne">The first player, who moves first.</param>
    /// <param name="playerTwo">The second player.</param>
    /// <param name="boardSize">Cells per side, for games whose board size can be chosen.</param>
    /// <returns>The new game.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If <paramref name="gameType"/> is not a known <see cref="GameType"/>.
    /// </exception>
    public static Game CreateGame(GameType gameType, IPlayer playerOne, IPlayer playerTwo, int boardSize = 3)
    {
        return gameType switch
        {
            GameType.NumericalTicTacToe => new NumericalTTTGame(playerOne, playerTwo, boardSize),
            GameType.Notakto => new NotaktoGame(playerOne, playerTwo),
            GameType.Gomoku => new GomokuGame(playerOne, playerTwo),
            _ => throw new ArgumentOutOfRangeException(
                nameof(gameType), gameType, "Unknown game type selection, please try again.")
        };
    }
}
