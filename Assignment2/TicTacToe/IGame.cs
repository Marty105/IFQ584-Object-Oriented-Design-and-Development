namespace TicTacToe;

public interface IGame
{
    /// <summary>Any extra rule a game places on moves.</summary>
    bool IsLegal(Placement p);

    /// <summary>Puts the move's piece on the board.</summary>
    void Apply(Placement p);

    /// <summary>
    /// Decides what the move just applied means for the player who made it.
    /// </summary>
    MoveOutcome Evaluate(Placement p);

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
