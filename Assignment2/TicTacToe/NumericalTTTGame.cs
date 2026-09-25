namespace TicTacToe;

/// <summary>
/// Numerical Tic Tac Toe on an n x n board. The players share the numbers
/// 1..n^2 and play them in order; whoever completes a line of n numbers adding
/// up to the board's target sum wins, no matter who played the other numbers.
/// </summary>
public sealed class NumericalTTTGame : Game
{
    public NumericalTTTGame(IPlayer playerOne, IPlayer playerTwo, int size = 3)
        : base(playerOne, playerTwo, new Board(size))
    {
    }

    public override GameType Type => GameType.NumericalTicTacToe;

    protected override void Apply(Placement p)
    {
        Board board = Boards[p.BoardIndex];
        board.PlacePiece(p.Row, p.Column, new Piece(board.NextNumber));
    }

    protected override MoveOutcome Evaluate(Placement p)
    {
        Board board = Boards[p.BoardIndex];

        if (Lines(board, board.Size).Any(line => IsMatch(board, line)))
        {
            return MoveOutcome.CurrentPlayerWins;
        }

        // The numbers run out exactly when the board fills.
        return board.IsFull() ? MoveOutcome.Draw : MoveOutcome.Continue;
    }

    /// <summary>True if every cell in the line is filled and they add up to the target sum.</summary>
    private static bool IsMatch(Board board, (int Row, int Column)[] line)
    {
        int sum = 0;

        foreach ((int row, int column) in line)
        {
            Piece? piece = board.GetCell(row, column);
            if (piece is null)
            {
                return false;
            }

            sum += piece.Value;
        }

        return sum == board.TargetSum;
    }
}
