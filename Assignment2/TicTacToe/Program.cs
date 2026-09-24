using TicTacToe;

// Assignment 1 - Numerical Tic Tac Toe console app
// Entry point. Set up a game between two players and run the game loop until
// someone completes a line adding up to the target sum, or the board fills up.
//
// The game generalises to any n x n board: the players share the numbers 1..n^2
// and play them in order — the 1st move plays 1, the 2nd plays 2, and so on —
// taking turns placing the next number in an empty cell. The first to complete a
// line (row, column or diagonal) of n numbers adding up to n(n^2 + 1) / 2 wins.

// Start a brand new game, or pick up a saved one from disk.
Game game = ChooseLoadGame() ? LoadGame() : NewGame();
var board = game.Board;

Console.WriteLine();
Console.WriteLine($"You chose {game.GameType}.");
Console.WriteLine($"{game.PlayerOne} vs {game.PlayerTwo}");
Console.WriteLine($"Board size: {board.Size}x{board.Size}, using the numbers 1 to {board.HighestNumber}");
Console.WriteLine(
    $"Complete a row, column or diagonal of {board.Size} numbers adding up to " +
    $"{board.TargetSum} to win.");
Console.WriteLine($"Rows and columns are numbered from 0 to {board.Size - 1}.");
Console.WriteLine();
Console.WriteLine(board);

// Player 1 goes first in a new game; a loaded game resumes with whoever is due
// to move. Turns then alternate.
while (true)
{
    IPlayer current = game.CurrentPlayer;

    // A human may undo, redo, save or ask for help before moving. Only when they
    // actually play (or the computer moves) do we check for a win or a draw.
    if (!PlayTurn(current))
    {
        // An undo or redo happened: redraw and let the loop re-pick the player.
        Console.WriteLine();
        Console.WriteLine(board);
        continue;
    }

    Console.WriteLine();
    Console.WriteLine(board);

    // Whoever completes a line adding up to the target sum wins, no matter who
    // played the other numbers in it — so the check is made straight after each
    // move and the win goes to the player who just moved.
    if (board.HasWinningLine())
    {
        Console.WriteLine($"{current} wins!");
        break;
    }

    // The numbers run out exactly when the board fills, so a full board with no
    // winning line is a draw.
    if (board.IsFull())
    {
        Console.WriteLine("It's a draw.");
        break;
    }
}

// --- Local helpers -------------------------------------------------------

// Ask whether to start a new game or load a saved one. Returns true to load.
// Help just prints the rules and re-shows the menu. Keeps asking until the
// input is understood.
bool ChooseLoadGame()
{
    while (true)
    {
        Console.WriteLine("  1) New game");
        Console.WriteLine("  2) Load a saved game");
        Console.WriteLine("  3) Help");
        Console.Write("Enter 1, 2 or 3: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                return false;
            case "2":
                return true;
            case "3":
                ShowHelp();
                break;
            default:
                Console.WriteLine("Please enter 1, 2 or 3.");
                break;
        }
    }
}

// Print how the game is played and how to drive the console app.
void ShowHelp()
{
    Console.WriteLine();
    Console.WriteLine("How to play Numerical Tic Tac Toe");
    Console.WriteLine("---------------------------------");
    Console.WriteLine("The board is an n x n grid played with the numbers 1 to n^2.");
    Console.WriteLine("The players share the numbers and play them in order: the 1st");
    Console.WriteLine("move plays 1, the 2nd plays 2, and so on. Player 1 moves first.");
    Console.WriteLine();
    Console.WriteLine("Players take turns placing the next number in an empty cell. The");
    Console.WriteLine("first to complete a row, column or diagonal whose numbers add up");
    Console.WriteLine("to n(n^2 + 1) / 2 (15 on a 3x3 board) wins — no matter who played");
    Console.WriteLine("the other numbers in that line.");
    Console.WriteLine();
    Console.WriteLine("On your turn:");
    Console.WriteLine("  - Press Enter, then type your move as \"row column\",");
    Console.WriteLine("    e.g. \"1 2\". Rows and columns are numbered from 0. The");
    Console.WriteLine("    game's next number is played in that cell.");
    Console.WriteLine("  - Or type 's' to save the game and quit.");
    Console.WriteLine();
    Console.WriteLine("Saved games are stored as .json files you can load again later.");
    Console.WriteLine();
}

// Set up a brand new game from the user's chosen board size and opponent.
// Player 1 is always a human and moves first; player 2 is either another human
// or the computer.
Game NewGame()
{
    GameType gameType = ChooseGameType();
    int boardSize = ChooseBoardSize();
    bool againstComputer = ChooseComputerOpponent();

    // Players are built through the factory, so this loop never names the concrete
    // Player / Computer types: it just asks for the kind it wants. Names come from
    // the shared settings rather than being hard-coded here.
    GameSettings settings = GameSettings.Instance;
    IPlayer playerOne = PlayerFactory.Create(PlayerKind.Human, settings.PlayerOneName);

    IPlayer playerTwo = PlayerFactory.Create(
        againstComputer ? PlayerKind.Computer : PlayerKind.Human,
        againstComputer ? settings.ComputerName : settings.PlayerTwoName);

    return new Game(playerOne, playerTwo, boardSize, gameType);
}

// Load a saved game the user picks from the .json save files in the working
// directory. Falls back to a new game if there are none, or if loading fails.
Game LoadGame()
{
    string[] saves = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.json");

    if (saves.Length == 0)
    {
        Console.WriteLine("No saved games found. Starting a new game.");
        return NewGame();
    }

    Console.WriteLine("Saved games:");
    for (int i = 0; i < saves.Length; i++)
    {
        Console.WriteLine($"  {i + 1}) {Path.GetFileNameWithoutExtension(saves[i])}");
    }

    // Keep asking until the user picks a listed save by number.
    string chosen;
    while (true)
    {
        Console.Write($"Choose a game to load (1-{saves.Length}): ");

        if (int.TryParse(Console.ReadLine()?.Trim(), out int pick) &&
            pick >= 1 && pick <= saves.Length)
        {
            chosen = saves[pick - 1];
            break;
        }

        Console.WriteLine($"Please enter a number from 1 to {saves.Length}.");
    }

    // The game needs a board size to be constructed; Load then replaces its
    // players and board wholesale from the file, so these initial values are
    // just placeholders. A minimum-size 3x3 board is enough to build one.
    var loaded = new Game(
        PlayerFactory.Create(PlayerKind.Human, "Player 1"),
        PlayerFactory.Create(PlayerKind.Human, "Player 2"),
        3, GameType.NumericalTicTacToe);

    try
    {
        loaded.Load(File.ReadAllText(chosen));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Couldn't load that game ({ex.Message}). Starting a new game.");
        return NewGame();
    }

    Console.WriteLine($"Loaded {Path.GetFileNameWithoutExtension(chosen)}.");
    return loaded;
}

// Ask a human what they want to do before their move: play, undo the last move,
// redo an undone move, or save and quit. 'h' shows the rules and re-asks. Undo
// and redo are only offered when there is something to undo or redo. Keeps
// asking until the input is understood.
TurnChoice AskTurnChoice()
{
    while (true)
    {
        // Build the prompt from just the options that apply right now.
        var options = new List<string> { "Enter to play" };
        if (game.CanUndo) options.Add("'u' undo");
        if (game.CanRedo) options.Add("'r' redo");
        options.Add("'s' save and quit");
        options.Add("'h' help");
        Console.Write(string.Join(", ", options) + ": ");

        string? answer = Console.ReadLine()?.Trim();

        // End of input (Ctrl-D / redirected stream): let the move prompt handle
        // it, matching how Player.GetMove treats a closed input stream.
        if (answer is null || answer.Length == 0)
        {
            return TurnChoice.Play;
        }

        if (answer.Equals("u", StringComparison.OrdinalIgnoreCase))
        {
            if (game.CanUndo) return TurnChoice.Undo;
            Console.WriteLine("There's nothing to undo.");
            continue;
        }

        if (answer.Equals("r", StringComparison.OrdinalIgnoreCase))
        {
            if (game.CanRedo) return TurnChoice.Redo;
            Console.WriteLine("There's nothing to redo.");
            continue;
        }

        if (answer.Equals("s", StringComparison.OrdinalIgnoreCase))
        {
            return TurnChoice.SaveAndQuit;
        }

        if (answer.Equals("h", StringComparison.OrdinalIgnoreCase))
        {
            ShowHelp();
            continue;
        }

        Console.WriteLine("Sorry, I didn't understand that.");
    }
}

// Ask the user for the game type ( Numerical Tic-Tac-Toe, Notakto or Gomoku)
// input is a correct number that correspondes to a game type
// Returns the chosen <see cref="GameType"/>

static GameType ChooseGameType()
{
    while (true)
    {
        Console.WriteLine("Choose a game:");
        Console.WriteLine("  1) Numerical Tic-Tac-Toe");
        Console.WriteLine("  2) Notakto");
        Console.WriteLine("  3) Gomoku");
        Console.Write("Enter 1, 2 or 3: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                return GameType.NumericalTicTacToe;
            case "2":
                return GameType.Notakto;
            case "3":
                return GameType.Gomoku;
            default:
                Console.WriteLine("Incorrect input. Please enter 1, 2 or 3.");
                break;
        }
    }
}


// Ask the user for the board size (cells per side). Keeps asking until the
// input is a whole number within the supported range.
int ChooseBoardSize()
{
    int min = GameSettings.Instance.MinBoardSize;
    int max = GameSettings.Instance.MaxBoardSize;

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

// Take one turn for a player. Returns true when an actual move was played (so
// the loop should check for a win or draw), false when the human undid or redid
// a move instead (so the loop should just redraw and carry on).
//
// Legality (the cell being on the board and empty, and the number being one the
// player still holds) is enforced here so a bad move from either a human or the
// computer re-prompts rather than crashing the game.
bool PlayTurn(IPlayer player)
{
    // Before a human's move, offer to undo, redo, save or ask for help. The
    // computer plays straight away, with nothing to prompt.
    if (player is Player)
    {
        switch (AskTurnChoice())
        {
            case TurnChoice.Undo:
                Move? undone = game.Undo();
                if (undone is Move u)
                {
                    Console.WriteLine($"Undid {u.Number} at ({u.Row}, {u.Column}).");
                }
                return false;

            case TurnChoice.Redo:
                Move? redone = game.Redo();
                if (redone is Move r)
                {
                    Console.WriteLine($"Redid {r.Number} at ({r.Row}, {r.Column}).");
                }
                return false;

            case TurnChoice.SaveAndQuit:
                game.Save();
                Console.WriteLine("Game saved. Goodbye.");
                Environment.Exit(0);
                break;
        }
    }

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

        // The number played is the game's next number, so we only pass the cell.
        // PlayMove places it, hands over the turn, and clears the redo history.
        int number = move.Number;

        if (!game.PlayMove(move.Row, move.Column))
        {
            Console.WriteLine($"({move.Row}, {move.Column}) is already taken. Try again.");
            continue;
        }

        Console.WriteLine($"{player} plays {number} at ({move.Row}, {move.Column}).");
        return true;
    }
}

// What a human chose to do at the start of their turn.
enum TurnChoice { Play, Undo, Redo, SaveAndQuit }
