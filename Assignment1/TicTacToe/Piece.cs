namespace TicTacToe;

/// <summary>
/// Which half of the numbers a player owns. In Numerical Tic Tac Toe the
/// numbers 1..n^2 are split between the two players by parity: one takes the
/// odd numbers, the other the even numbers.
/// </summary>
public enum Parity
{
    /// <summary>The odd numbers (1, 3, 5, ...).</summary>
    Odd,

    /// <summary>The even numbers (2, 4, 6, ...).</summary>
    Even
}

/// <summary>
/// Display helpers for <see cref="Parity"/>.
/// </summary>
public static class ParityExtensions
{
    /// <summary>A human-readable name for the set, e.g. "odds".</summary>
    public static string Label(this Parity parity) =>
        parity == Parity.Odd ? "odds" : "evens";
}

/// <summary>
/// Represents a playing piece that can occupy a cell on the board. A piece is
/// one of the numbers 1..n^2, and the number itself identifies which player
/// played it: odd numbers belong to the odd player, even numbers to the even
/// player.
/// An empty cell is represented by a null Piece rather than a special value.
/// </summary>
public class Piece
{
    /// <summary>
    /// The number written on this piece. This is what the winning algorithm
    /// adds up.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Which player's set this piece came from, derived from whether its number
    /// is odd or even.
    /// </summary>
    public Parity Parity => Value % 2 == 0 ? Parity.Even : Parity.Odd;

    /// <summary>
    /// Creates a new piece carrying the given number.
    /// </summary>
    /// <param name="value">The number on the piece; must be 1 or greater.</param>
    public Piece(int value)
    {
        if (value < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value), value, "A piece's number must be 1 or greater.");
        }

        Value = value;
    }

    public override string ToString() => Value.ToString();
}
