namespace TicTacToe;

/// <summary>
/// An <see cref="IPlayer"/> controlled by the computer.
///
/// The strategy is deliberately simple: if any of the computer's remaining
/// numbers would immediately win the game in any empty cell, it plays that move.
/// Otherwise it picks one of its numbers at random and plays it in a randomly
/// chosen empty cell.
/// </summary>
public class Computer : PlayerBase, IPlayer
{
    /// <summary>
    /// Creates a new computer player.
    /// </summary>
    /// <param name="numbers">Which half of the numbers this player owns.</param>
    /// <param name="boardSize">The number of cells along one side of the board.</param>
    /// <param name="name">A display name for the player. Defaults to "Computer".</param>
    public Computer(Parity numbers, int boardSize, string name = "Computer")
        : base(name, numbers, boardSize)
    {
    }

    /// <summary>
    /// Chooses the computer's next move: an immediately winning one if it can
    /// find it, otherwise a random valid one.
    ///
    /// Assumes the game is not already over — that there is at least one empty
    /// cell and at least one number left to play, which the game loop checks
    /// before asking for a move.
    /// </summary>
    public override Move GetMove(Board board)
    {
        var cells = board.EmptyCells().ToList();

        // Look for a move that wins on the spot: any of this player's numbers, in
        // any empty cell, that completes a line adding up to the target sum.
        foreach ((int row, int column) in cells)
        {
            foreach (int number in Remaining)
            {
                if (board.IsWinningMove(row, column, number))
                {
                    return new Move(row, column, number);
                }
            }
        }

        // No win available, so play randomly.
        (int Row, int Column) cell = cells[Random.Shared.Next(cells.Count)];
        int pick = Remaining[Random.Shared.Next(Remaining.Count)];

        return new Move(cell.Row, cell.Column, pick);
    }
}
