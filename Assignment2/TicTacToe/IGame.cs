namespace TicTacToe;

public interface IGame
{
    /// <summary>
    /// Persists the current game so it can be reloaded later.
    /// </summary>
    void Save();

    /// <summary>
    /// The current game serialised as JSON: the game type, the two players and
    /// every move played.
    /// </summary>
    string State { get; }
}
