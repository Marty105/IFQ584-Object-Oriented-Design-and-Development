namespace TicTacToe;

/// <summary>
/// The Command for playing one move into a cell. It captures a move as a single
/// object that can undo itself, so <see cref="Game"/> can treat "play", "undo"
/// and "redo" uniformly as pushing and popping commands.
///
/// Whose turn it is follows from the game's move count, so playing or taking
/// back the move also hands the turn over.
/// </summary>
public sealed class MoveCommand : ICommand
{
    private readonly Game _game;
    private readonly int _row;
    private readonly int _column;
    private readonly int _boardIndex; // Note - piece/number choice is made by the game type //

    public MoveOutcome Outcome { get; private set; } // makes outcome visible //

    /// <summary>
    /// Creates a command that will play the current player's move into the given
    /// cell.
    /// </summary>
    /// <param name="game">The game the move is played in.</param>
    /// <param name="row">The row to play in.</param>
    /// <param name="column">The column to play in.</param>
    /// <param name="boardIndex">Which of the game's boards to play on.</param>
    public MoveCommand(Game game, int row, int column, int boardIndex = 0)
    {
        _game = game;
        _row = row;
        _column = column;
        _boardIndex = boardIndex;
    }

    /// <summary>The move this command plays, once created.</summary>
    public Placement Placement => new(_row, _column, _boardIndex);

    /// <inheritdoc />
    public bool Execute()
    {
        Outcome = _game.Play(Placement);
        return Outcome != MoveOutcome.Illegal; // illegal: the move did not happen
    }

    /// <inheritdoc />
    public void Undo()
    {
        // The move being undone is always the most recent one, so taking the last
        // move back removes exactly this command's piece.
        _game.Unplay();
    }
}
