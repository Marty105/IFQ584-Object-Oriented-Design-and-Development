namespace TicTacToe;

public interface IGame
{
    /// <summary>
    /// Restores a game from its serialised <see cref="State"/> representation.
    /// </summary>
    /// <param name="data">A JSON document previously produced by <see cref="State"/>.</param>
    void Load(string data);

    /// <summary>
    /// Persists the current game so it can be reloaded later.
    /// </summary>
    void Save();

    /// <summary>
    /// The current game serialised as JSON: the two players (with the numbers
    /// they still hold) and the board.
    /// </summary>
    string State { get; }
}