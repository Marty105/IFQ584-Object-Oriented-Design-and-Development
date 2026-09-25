namespace TicTacToe;

/// <summary>
/// Represents a playing piece that can occupy a cell on the board. A piece is
/// one of the numbers 1..n^2 that have been played, in order, over the game.
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
    /// Which player's mark this piece shows: 'X' for player one, 'O' for player
    /// two. Numbers are played in order, so odd-numbered moves (1st, 3rd, ...)
    /// are player one and even-numbered moves are player two.
    /// </summary>
    public char Mark => Value % 2 == 1 ? 'X' : 'O';

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

    public override string ToString() => Mark.ToString();
}
