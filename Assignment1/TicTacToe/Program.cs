using TicTacToe;

// Assignment 1 - Numerical Tic Tac Toe console app
// Entry point. Set up a game between two players and run the game loop until
// someone completes a line adding up to the target sum, or the board fills up.
//
// The game generalises to any n x n board: the numbers 1..n^2 are split between
// the two players as odds and evens, the players take turns placing one of their
// numbers in an empty cell, and the first to complete a line (row, column or
// diagonal) of n numbers adding up to n(n^2 + 1) / 2 wins.

// The board size fixes which numbers are in play, so it is chosen first.
int boardSize = ChooseBoardSize();
bool againstComputer = ChooseComputerOpponent();

// Player 1 is always a human at the console, playing the odd numbers, and moves
// first. Player 2 plays the evens, and is either another human or the computer.
IPlayer playerOne = new Player("Player 1", Parity.Odd, boardSize);

IPlayer playerTwo = againstComputer
    ? new Computer(Parity.Even, boardSize)
    : new Player("Player 2", Parity.Even, boardSize);

var game = new Game(playerOne, playerTwo, boardSize);
var board = game.Board;

Console.WriteLine();
Console.WriteLine("Numerical Tic Tac Toe");
Console.WriteLine($"{game.PlayerOne} ({game.PlayerOne.Numbers.Label()}) vs {game.PlayerTwo} ({game.PlayerTwo.Numbers.Label()})");
Console.WriteLine($"Board size: {board.Size}x{board.Size}, using the numbers 1 to {board.HighestNumber}");
Console.WriteLine(
    $"Complete a row, column or diagonal of {board.Size} numbers adding up to " +
    $"{board.TargetSum} to win.");
Console.WriteLine($"Rows and columns are numbered from 0 to {board.Size - 1}.");
Console.WriteLine();
Console.WriteLine(board);

// Player 1 (the odds) always goes first; turns then alternate.
IPlayer current = game.PlayerOne;

while (true)
{
    // Ask the current player for a move, retrying until a legal one is played.
    PlayTurn(current);

    Console.WriteLine();
    Console.WriteLine(board);

    // Whoever completes a line adding up to the target sum wins, no matter who
    // played the other numbers in it — so the check is made straight after each
    // move and the win goes to the player who just moved.
    if (board.HasWinningLine())
    {
        Console.WriteLine($"{current} ({current.Numbers.Label()}) wins!");
        break;
    }

    // Hand over to the other player.
    IPlayer next = current == game.PlayerOne ? game.PlayerTwo : game.PlayerOne;

    if (board.IsFull() || next.Remaining.Count == 0)
    {
        Console.WriteLine("It's a draw.");
        break;
    }

    current = next;
}

// --- Local helpers -------------------------------------------------------

// Ask the user for the board size (cells per side). Keeps asking until the
// input is a whole number within the supported range.
int ChooseBoardSize()
{
    const int min = 3;
    const int max = 9;

    while (true)
    {
        Console.Write($"Enter board size ({min}-{max}, 3 = classic Numerical Tic Tac Toe): ");

        if (int.TryParse(Console.ReadLine()?.Trim(), out int size) &&
            size >= min && size <= max)
        {
            return size;
        }

        Console.WriteLine($"Please enter a whole number from {min} to {max}.");
    }
}

// Ask the user which mode to play. Returns true for human vs computer,
// false for human vs human. Keeps asking until the input is understood.
bool ChooseComputerOpponent()
{
    while (true)
    {
        Console.WriteLine("Choose a mode:");
        Console.WriteLine("  1) Human vs Human");
        Console.WriteLine("  2) Human vs Computer");
        Console.Write("Enter 1 or 2: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                return false;
            case "2":
                return true;
            default:
                Console.WriteLine("Please enter 1 or 2.");
                break;
        }
    }
}

// Repeatedly ask a player for a move until one is legal, then play it. Legality
// (the cell being on the board and empty, and the number being one the player
// still holds) is enforced here so a bad move from either a human or the
// computer re-prompts rather than crashing the game.
void PlayTurn(IPlayer player)
{
    while (true)
    {
        Move move;
        try
        {
            move = player.GetMove(board);
        }
        catch (Exception ex)
        {
            // A player may fail to produce a usable move.
            Console.WriteLine($"Couldn't read a move ({ex.Message}). Trying again.");
            continue;
        }

        if (!board.IsInBounds(move.Row, move.Column))
        {
            Console.WriteLine($"({move.Row}, {move.Column}) is off the board. Try again.");
            continue;
        }

        if (!player.HasNumber(move.Number))
        {
            Console.WriteLine(
                $"{move.Number} isn't one of your numbers. You have: " +
                $"{string.Join(", ", player.Remaining)}. Try again.");
            continue;
        }

        if (!board.PlacePiece(move.Row, move.Column, new Piece(move.Number)))
        {
            Console.WriteLine($"({move.Row}, {move.Column}) is already taken. Try again.");
            continue;
        }

        // The number is on the board now, so the player can no longer play it.
        player.UseNumber(move.Number);
        Console.WriteLine($"{player} plays {move.Number} at ({move.Row}, {move.Column}).");
        return;
    }
}
