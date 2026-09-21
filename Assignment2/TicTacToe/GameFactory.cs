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
    /// <returns>The new game type in "Board".</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If <paramref name="gameType"/> is not a known <see cref="GameType"/>.
    /// </exception>

    public static GameVariant CreateGame(GameType gameType)
    {

        return gameType switch
        {
            GameType.NumericalTicTacToe => new NumericalTTTGame(),
            GameType.Notakto => new NotaktoGame(),
            GameType.Gomoku => new GomokuGame(),
            _ => throw new ArgumentOutOfRangeException(
                nameof(gameType), gameType, "Unkown game type selecion")
        };
    }
}
