namespace TicTacToe;

/// <summary>
/// An <see cref="IPlayer"/> controlled by the computer.
///
/// The strategy is deliberately simple: if playing the next number in any empty
/// cell would win the game on the spot, it plays that cell. Otherwise it plays
/// the number in a randomly chosen empty cell.
/// </summary>
public class Computer : PlayerBase, IPlayer
{
    /// <summary>
    /// Creates a new computer player.
    /// </summary>
    /// <param name="name">A display name for the player. Defaults to "Computer".</param>
    public Computer(string name = "Computer")
        : base(name)
    {
    }

    /// <summary>
    /// Chooses the computer's next move: an immediately winning cell if it can
    /// find one, otherwise a random empty cell.
    ///
    /// Assumes the game is not already over — that there is at least one empty
    /// cell — which the game loop checks before asking for a move.
    /// </summary>
    public override Move GetMove(Board board)
    {
        var cells = board.EmptyCells().ToList();

        // The number is the board's next number, so the computer only chooses the
        // cell. Look for a cell where playing it wins on the spot: it completes a
        // line adding up to the target sum.
        int number = board.NextNumber;

        // TODO: Board.IsWinningMove no longer exists - win check moves to GameVariant //
        // foreach ((int row, int column) in cells)
        // {
        //     if (board.IsWinningMove(row, column, number))
        //     {
        //         return new Move(row, column, number);
        //     }
        // }

        // No winning cell, so play the number in a random empty cell.
        (int Row, int Column) cell = cells[Random.Shared.Next(cells.Count)];

        return new Move(cell.Row, cell.Column, number);
    }
}
