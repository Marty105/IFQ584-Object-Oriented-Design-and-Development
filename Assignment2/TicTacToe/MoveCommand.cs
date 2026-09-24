namespace TicTacToe;

/// <summary>
/// The Command for playing one number into a cell. It captures everything a move
/// does — placing the piece on the board and handing the turn to the other
/// player — as a single object that can undo itself, so <see cref="Game"/> can
/// treat "play", "undo" and "redo" uniformly as pushing and popping commands.
///
/// The number played is fixed at the moment the command is created (it is the
/// board's next number then), so redoing a command replays the exact same move.
/// </summary>
public sealed class MoveCommand : ICommand
{
    private readonly GameVariant _variant;
    private readonly Action _swapTurn;
    private readonly int _row;
    private readonly int _column;
    private readonly int _boardIndex; // Note - piece/number choice moved to game variant // 

    public MoveOutcome Outcome { get; private set; } // makes outcome visible //

    /// <summary>
    /// Creates a command that will play the board's current next number into the
    /// given cell and hand the turn over via <paramref name="swapTurn"/>.
    /// </summary>
    /// <param name="board">The board the move is played on.</param>
    /// <param name="swapTurn">Hands the turn to the other player (and back, on undo).</param>
    /// <param name="row">The row to play in.</param>
    /// <param name="column">The column to play in.</param>
    public MoveCommand(GameVariant variant, Action swapTurn, int row, int column, int boardIndex =0)
    {
        _variant = variant;
        _swapTurn = swapTurn;
        _row = row;
        _column = column;
        _boardIndex = boardIndex;
    }

    /// <summary>The move this command plays, once created.</summary>
    public Placement Placement => new(_row, _column, _boardIndex);

    /// <inheritdoc />
    public bool Execute()
    {
        Outcome = _variant.Play(Placement);
        if (Outcome == MoveOutcome.Illegal)
        {
            return false; // cell taken: the move did not happen
        }

        _swapTurn();
        return true;
    }

    /// <inheritdoc />
    public void Undo()
    {
        // The move being undone is always the most recent one on the board, so
        // taking the last move back removes exactly this command's piece; then we
        // hand the turn back to the player who made it.
        _variant.Unplay();
        _swapTurn();
    }
}
