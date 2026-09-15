namespace TicTacToe;

/// <summary>
/// A single move: the number a player wants to play, and the row and column to
/// play it in.
/// </summary>
public readonly record struct Move(int Row, int Column, int Number);

/// <summary>
/// A participant in a game of Numerical Tic Tac Toe.
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// The player's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Which half of the numbers this player owns (the odds or the evens).
    /// </summary>
    Parity Numbers { get; }

    /// <summary>
    /// The numbers this player still has left to play, in ascending order.
    /// </summary>
    IReadOnlyList<int> Remaining { get; }

    /// <summary>
    /// True if this player still holds the given number.
    /// </summary>
    bool HasNumber(int number);

    /// <summary>
    /// Spends one of this player's numbers, so it cannot be played again.
    /// Returns false if the player did not hold it.
    /// </summary>
    bool UseNumber(int number);

    /// <summary>
    /// Chooses the next move for this player given the current board.
    /// The returned move is not guaranteed to be legal; the caller (the game
    /// loop) is responsible for validating it against the board and against the
    /// player's remaining numbers.
    /// </summary>
    /// <param name="board">The board as it currently stands.</param>
    Move GetMove(Board board);
}
