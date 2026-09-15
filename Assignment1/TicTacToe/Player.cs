namespace TicTacToe;

/// <summary>
/// A human player that chooses its moves by typing them at the console.
/// </summary>
public class Player : PlayerBase, IPlayer
{
    /// <summary>
    /// Creates a new human player with the given name and set of numbers.
    /// </summary>
    public Player(string name, Parity numbers, int boardSize)
        : base(name, numbers, boardSize)
    {
    }

    /// <summary>
    /// Prompts this human player for a move at the console.
    /// Expects a "row column number" triple, where row and column are 0-based
    /// and number is one of the player's remaining numbers; keeps asking until
    /// the input parses. Legality against the board is checked by the game loop,
    /// not here.
    /// </summary>
    public override Move GetMove(Board board)
    {
        while (true)
        {
            Console.WriteLine(
                $"{Name} ({Numbers.Label()}) — your numbers: {string.Join(", ", Remaining)}");
            Console.Write("Enter your move as \"row column number\": ");
            string? line = Console.ReadLine();

            // Console.ReadLine returns null at end of input (e.g. Ctrl-D, or a
            // closed/redirected stream). There is no more input to read, so
            // looping would spin forever; end the program cleanly instead.
            if (line == null)
            {
                Console.WriteLine();
                Console.WriteLine("No more input. Goodbye.");
                Environment.Exit(0);
            }

            var parts = line.Split(
                new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts is { Length: 3 } &&
                int.TryParse(parts[0], out int row) &&
                int.TryParse(parts[1], out int column) &&
                int.TryParse(parts[2], out int number))
            {
                return new Move(row, column, number);
            }

            Console.WriteLine("Please enter three numbers, e.g. \"1 2 5\".");
        }
    }
}
