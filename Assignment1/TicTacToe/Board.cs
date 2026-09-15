namespace TicTacToe;

/// <summary>
/// Represents the Numerical Tic Tac Toe playing board.
/// The board is a square grid of size n x n whose cells hold the numbers
/// 1..n^2, and it owns the winning algorithm for the game (see
/// <see cref="TargetSum"/> and <see cref="HasWinningLine()"/>).
/// </summary>
public class Board
{
    /// <summary>
    /// The grid of cells. Each cell holds a <see cref="Piece"/>,
    /// or null when the cell is empty.
    /// </summary>
    private readonly Piece?[,] _cells;

    /// <summary>
    /// The size of the board (the number of cells along one side).
    /// Set once via the constructor and cannot be changed afterwards.
    /// </summary>
    public readonly int Size;

    /// <summary>
    /// The height of the board in cells. For a square board this equals Size.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The width of the board in cells. For a square board this equals Size.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Creates a new square board of the given size. All cells start empty.
    /// </summary>
    /// <param name="n">The number of cells along one side of the board.</param>
    public Board(int n)
    {
        Size = n;
        Height = n;
        Width = n;
        _cells = new Piece?[n, n];
        // A new Piece?[,] defaults every element to null,
        // so the board already starts empty.
    }

    /// <summary>
    /// The number every winning line has to add up to: n(n^2 + 1) / 2, the magic
    /// constant of an n x n square. For the classic 3x3 game this is 15.
    /// </summary>
    public int TargetSum => Size * (Size * Size + 1) / 2;

    /// <summary>
    /// The largest number in play. The numbers 1..HighestNumber are split
    /// between the two players as odds and evens.
    /// </summary>
    public int HighestNumber => Size * Size;

    /// <summary>
    /// Gets the piece at the given row and column, or null if the cell is empty.
    /// </summary>
    public Piece? GetCell(int row, int column)
    {
        if (!IsInBounds(row, column))
        {
            throw new ArgumentOutOfRangeException(
                $"Cell ({row}, {column}) is outside the {Size}x{Size} board.");
        }

        return _cells[row, column];
    }

    /// <summary>
    /// Places a piece at the given row and column.
    /// Returns true if the move was made, false if the cell was already taken.
    /// </summary>
    public bool PlacePiece(int row, int column, Piece piece)
    {
        if (!IsInBounds(row, column))
        {
            throw new ArgumentOutOfRangeException(
                $"Cell ({row}, {column}) is outside the {Size}x{Size} board.");
        }

        if (GetCell(row, column) != null)
        {
            return false; // cell already occupied
        }

        _cells[row, column] = piece;
        return true;
    }

    /// <summary>
    /// Returns true if the given row and column fall within the board.
    /// </summary>
    public bool IsInBounds(int row, int column)
    {
        return row >= 0 && row < Height && column >= 0 && column < Width;
    }

    /// <summary>True when no empty cells remain.</summary>
    public bool IsFull()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                if (_cells[row, column] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>The coordinates of every empty cell, in reading order.</summary>
    public IEnumerable<(int Row, int Column)> EmptyCells()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                if (_cells[row, column] == null)
                {
                    yield return (row, column);
                }
            }
        }
    }

    /// <summary>
    /// Every line that can win the game: each of the n rows, each of the n
    /// columns, and the two long diagonals.
    /// </summary>
    public IEnumerable<(int Row, int Column)[]> Lines()
    {
        int n = Size;

        for (int i = 0; i < n; i++)
        {
            yield return Enumerable.Range(0, n).Select(j => (i, j)).ToArray();  // row i
            yield return Enumerable.Range(0, n).Select(j => (j, i)).ToArray();  // column i
        }

        yield return Enumerable.Range(0, n).Select(i => (i, i)).ToArray();          // top-left to bottom-right
        yield return Enumerable.Range(0, n).Select(i => (i, n - 1 - i)).ToArray();  // top-right to bottom-left
    }

    /// <summary>
    /// The winning algorithm. A line wins when it is completely filled and its n
    /// numbers add up to <see cref="TargetSum"/>.
    ///
    /// Note that who owns those numbers does not matter: a winning line may well
    /// mix odd and even numbers. The winner is whoever plays the number that
    /// completes such a line, which is why the game loop tests this straight
    /// after each move and credits the win to the player who just moved.
    /// </summary>
    public bool IsWinningLine((int Row, int Column)[] line)
    {
        int sum = 0;

        foreach ((int row, int column) in line)
        {
            Piece? piece = _cells[row, column];

            if (piece == null)
            {
                return false; // the line isn't finished yet
            }

            sum += piece.Value;
        }

        return sum == TargetSum;
    }

    /// <summary>
    /// True if any row, column or diagonal is a winning line, i.e. the game has
    /// been won. Tested after every move, so the winner is the player who just
    /// moved.
    /// </summary>
    public bool HasWinningLine() => Lines().Any(IsWinningLine);

    /// <summary>
    /// True if playing the given number in the given cell would win the game
    /// straight away. Used by the computer player to spot a winning move.
    ///
    /// The number is part of the move as well as the cell, because in Numerical
    /// Tic Tac Toe what wins a line is the total, so which number goes in the cell
    /// decides whether it wins. The move is tried on the board and then taken back
    /// off, leaving the board exactly as it was.
    /// </summary>
    public bool IsWinningMove(int row, int column, int number)
    {
        if (!PlacePiece(row, column, new Piece(number)))
        {
            return false; // cell already taken, so it isn't a move at all
        }

        bool wins = HasWinningLine();
        _cells[row, column] = null;

        return wins;
    }

    /// <summary>
    /// Renders the board as text, e.g. for a 3x3 board:
    ///
    ///   2 |  7 |
    /// ----+----+----
    ///     |  5 |
    /// ----+----+----
    ///   9 |    |  1
    /// </summary>
    public override string ToString()
    {
        // Numbers run up to n^2, so size every cell to the widest of them and the
        // columns stay lined up on any board size.
        int cellWidth = HighestNumber.ToString().Length;
        var builder = new System.Text.StringBuilder();

        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                // An empty (null) cell renders as blanks; otherwise the piece's
                // ToString() supplies its number.
                string text = _cells[row, column]?.ToString() ?? string.Empty;

                builder.Append(' ');
                builder.Append(text.PadLeft(cellWidth));
                builder.Append(' ');

                if (column < Width - 1)
                {
                    builder.Append('|');
                }
            }

            builder.AppendLine();

            if (row < Height - 1)
            {
                // Row separator, e.g. "----+----+----" sized to the board width.
                builder.AppendLine(string.Join(
                    "+", Enumerable.Repeat(new string('-', cellWidth + 2), Width)));
            }
        }

        return builder.ToString();
    }
}
