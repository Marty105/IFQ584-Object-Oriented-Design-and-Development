namespace TicTacToe;

/// <summary>
/// A human player that chooses its moves by typing them at the console.
/// </summary>
public class Player : PlayerBase, IPlayer
{
    /// <summary>
    /// Creates a new human player with the given name.
    /// </summary>
    public Player(string name)
        : base(name)
    {
    }

    /// <summary>
    /// Prompts this human player for a move at the console.
    /// Expects a "row column" pair, both 0-based; the number to place is the
    /// game's next number, so the player only chooses the cell. Keeps asking
    /// until the input parses. Legality against the board is checked by the game
    /// loop, not here.
    /// </summary>
    public override Move GetMove(Board board)
    {
        while (true)
        {
            Console.WriteLine($"{Name}'s turn. You play the number {board.NextNumber}.");
            Console.Write("Enter your move as \"row column\": ");
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

            if (parts is { Length: 2 } &&
                int.TryParse(parts[0], out int row) &&
                int.TryParse(parts[1], out int column))
            {
                // The player chooses only the cell; the board's next number is
                // what gets placed.
                return new Move(row, column, board.NextNumber);
            }

            Console.WriteLine("Please enter two numbers, e.g. \"1 2\".");
        }
    }
}
