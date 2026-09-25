namespace TicTacToe;

public interface IGame
{
    /// <summary>Any extra rule a game places on moves.</summary>
    bool IsLegal(Placement p);

    /// <summary>Puts the move's piece on the board.</summary>
    // void Apply(Placement p);

    /// <summary>
    /// Decides what the move just applied means for the player who made it.
    /// </summary>
    MoveOutcome PlayMove(Placement p);

    /// <summary>
    /// Persists the current game so it can be reloaded later.
    /// </summary>
    void Save();

    /// <summary>
    /// The current game serialised as JSON: the game type, the two players and
    /// every move played.
    /// </summary>
    string State { get; }

    bool CanUndo { get; }

    bool CanRedo { get; }

    public Placement? Undo();

    public Placement? Redo();

    public IPlayer PlayerOne { get; }

    public IPlayer PlayerTwo { get; }

    public GameType Type { get; }

    // remove this, should be private
    public IReadOnlyList<Board> Boards { get; }

    public IPlayer CurrentPlayer { get; }
}
