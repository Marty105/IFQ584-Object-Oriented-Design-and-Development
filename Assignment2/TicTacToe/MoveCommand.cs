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
    private readonly Board _board;
    private readonly Action _swapTurn;
    private readonly int _row;
    private readonly int _column;
    private readonly int _number;

    /// <summary>
    /// Creates a command that will play the board's current next number into the
    /// given cell and hand the turn over via <paramref name="swapTurn"/>.
    /// </summary>
    /// <param name="board">The board the move is played on.</param>
    /// <param name="swapTurn">Hands the turn to the other player (and back, on undo).</param>
    /// <param name="row">The row to play in.</param>
    /// <param name="column">The column to play in.</param>
    public MoveCommand(Board board, Action swapTurn, int row, int column)
    {
        _board = board;
        _swapTurn = swapTurn;
        _row = row;
        _column = column;
        // Numbers are played in order, so the move plays whatever number is next
        // right now; fixing it here keeps a later redo identical to the original.
        _number = board.NextNumber;
    }

    /// <summary>The move this command plays, once created.</summary>
    public Move Move => new(_row, _column, _number);

    /// <inheritdoc />
    public bool Execute()
    {
        if (!_board.PlacePiece(_row, _column, new Piece(_number)))
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
        _board.UndoLastMove();
        _swapTurn();
    }
}
