//using System.Text.Json;

//namespace TicTacToe;

/// <summary>
/// Coordinates a game of Numerical Tic Tac Toe between two players on a board.
/// Players share a single run of numbers played in order — the 1st move plays 1,
/// the 2nd plays 2, and so on — so the only per-turn state is whose turn it is.
/// </summary>
//public class Game : IGame
//{
    /// <summary>
    /// The two players in the game. Player one moves first.
    /// </summary>
    //public IPlayer PlayerOne { get; private set; }
    //public IPlayer PlayerTwo { get; private set; }

    /// <summary>
    /// The game type.
    /// </summary>
    //public string GameType { get; private set; }

    /// <summary>
    /// The board the game is played on.
    /// </summary>
    //public Board Board { get; private set; }

    /// <summary>
    /// Whose turn it is: true for player one, false for player two. Held as state
    /// (and saved) rather than derived, so a loaded game resumes with the right
    /// player and undo/redo simply flip it.
    /// </summary>
    //private bool _playerOnesTurn = true;

    /// <summary>
    /// The commands that have been played, most recent on top (the Command
    /// pattern's history). Undoing pops from here; a fresh move pushes onto it.
    /// </summary>
 //   private readonly Stack<ICommand> _undo = new();

    /// <summary>
    /// Commands that have been undone and can be redone, most recent on top. A
    /// fresh move clears this, since redoing onto a board that has moved on no
    /// longer makes sense.
    /// </summary>
 //   private readonly Stack<ICommand> _redo = new();

    /// <summary>
    /// Creates a new game between two players on a board of the given size.
    /// </summary>
    /// <param name="playerOne">The first player.</param>
    /// <param name="playerTwo">The second player.</param>
    /// <param name="boardSize">The number of cells along one side of the board.</param>
  //  public Game(IPlayer playerOne, IPlayer playerTwo, int boardSize, string gameType)
   // {
     //   PlayerOne = playerOne;
    //    PlayerTwo = playerTwo;
     //   Board = new Board(boardSize);
      //  GameType = gameType;
  //  }

    /// <summary>The player whose turn it is right now.</summary>
   // public IPlayer CurrentPlayer => _playerOnesTurn ? PlayerOne : PlayerTwo;

    /// <summary>
    /// The number the next move will place. Numbers are played in order and
    /// shared between the players, so it is simply one more than the number of
    /// moves made so far: the 1st move plays 1, the 2nd plays 2, and so on.
    /// </summary>
    /*public int NextNumber => Board.Moves.Count + 1;

    /// <summary>Hands the turn to the other player.</summary>
    private void SwapTurn() => _playerOnesTurn = !_playerOnesTurn;

    /// <inheritdoc />
    /// <remarks>
    /// Both players are rebuilt as human <see cref="Player"/>s: the saved state
    /// does not record whether a side was human or computer, so it cannot tell
    /// them apart.
    /// </remarks>
    public void Load(string data)
    {
        GameState? state = JsonSerializer.Deserialize<GameState>(data, SerializerOptions);

        if (state is null)
        {
            throw new ArgumentException("The game state is not valid JSON.", nameof(data));
        }

        // The saved state does not record whether a side was human or computer,
        // so both are rebuilt as humans through the same factory the game uses.
        PlayerOne = PlayerFactory.Create(PlayerKind.Human, state.PlayerOne);
        PlayerTwo = PlayerFactory.Create(PlayerKind.Human, state.PlayerTwo);
        Board = RestoreBoard(state);
        _playerOnesTurn = state.PlayerOnesTurn;
        GameType = state.GameType;
        // A loaded game starts a fresh command history: the moves that rebuilt the
        // board were replayed directly, not through commands, and there is nothing
        // meaningful to undo back past the saved position.
        _undo.Clear();
        _redo.Clear();
    }

    /// <summary>
    /// Rebuilds the board from its saved state by replaying every occupied cell
    /// through <see cref="Board.PlacePiece"/>, which also restores the move
    /// history in reading order.
    /// </summary>
    private static Board RestoreBoard(GameState state)
    {
        var board = new Board(state.BoardSize);

        for (int row = 0; row < state.Cells.Length; row++)
        {
            int?[] cellsInRow = state.Cells[row];

            for (int column = 0; column < cellsInRow.Length; column++)
            {
                int? value = cellsInRow[column];

                if (value is int number)
                {
                    board.PlacePiece(row, column, new Piece(number));
                }
            }
        }

        return board;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Prompts at the console for a save name and writes the current
    /// <see cref="State"/> to "&lt;name&gt;.json" in the working directory.
    /// </remarks>
    public void Save()
    {
        Console.Write("Save as (name, no extension): ");
        string? name = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("No name given; not saved.");
            return;
        }

        string path = SaveFilePath(name);
        File.WriteAllText(path, State);
        Console.WriteLine($"Saved to {path}.");
    }

    /// <summary>
    /// The path a save with the given name is written to and loaded from:
    /// "&lt;name&gt;.json" in the current working directory.
    /// </summary>
    public static string SaveFilePath(string name) =>
        Path.Combine(Directory.GetCurrentDirectory(), name + ".json");

    /// <summary>
    /// Plays the current player's number in the given cell by building and
    /// executing a <see cref="MoveCommand"/>, then recording it on the undo
    /// history. Returns false (without changing anything) if the cell was taken.
    /// A fresh move retires any commands that were waiting to be redone.
    /// </summary>
    public bool PlayMove(int row, int column)
    {
        var command = new MoveCommand(Board, SwapTurn, row, column);

        if (!command.Execute())
        {
            return false;
        }

        _undo.Push(command);
        _redo.Clear();
        return true;
    }

    /// <summary>True if there is a command that can be undone.</summary>
    public bool CanUndo => _undo.Count > 0;

    /// <summary>True if there is an undone command that can be redone.</summary>
    public bool CanRedo => _redo.Count > 0;

    /// <summary>
    /// Undoes the most recent command: reverses it (taking its piece off the board
    /// and handing the turn back) and moves it onto the redo history.
    /// Returns the undone move, or null if there was nothing to undo.
    /// </summary>
    public Move? Undo()
    {
        if (_undo.Count == 0)
        {
            return null;
        }

        ICommand command = _undo.Pop();
        command.Undo();
        _redo.Push(command);

        return (command as MoveCommand)?.Move;
    }

    /// <summary>
    /// Redoes the most recently undone command: re-executes it and moves it back
    /// onto the undo history.
    /// Returns the redone move, or null if there was nothing to redo.
    /// </summary>
    public Move? Redo()
    {
        if (_redo.Count == 0)
        {
            return null;
        }

        ICommand command = _redo.Pop();
        command.Execute();
        _undo.Push(command);

        return (command as MoveCommand)?.Move;
    }

    /// <inheritdoc />
    public string State => JsonSerializer.Serialize(Snapshot(), SerializerOptions);

    /// <summary>Options used when serialising the game state.</summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Builds a plain, serialisable snapshot of the whole game: the board size,
    /// both players' names, whose turn it is, and the grid of numbers (null for
    /// empty cells).
    /// </summary>
    private GameState Snapshot()
    {
        var cells = new int?[Board.Height][];

        for (int row = 0; row < Board.Height; row++)
        {
            cells[row] = new int?[Board.Width];

            for (int column = 0; column < Board.Width; column++)
            {
                cells[row][column] = Board.GetCell(row, column)?.Value;
            }
        }

        return new GameState(
            GameType,
            Board.Size,
            PlayerOne.Name,
            PlayerTwo.Name,
            _playerOnesTurn,
            cells);
    }

    /// <summary>The serialisable shape of a whole game.</summary>
    private sealed record GameState(
        string GameType,
        int BoardSize,
        string PlayerOne,
        string PlayerTwo,
        bool PlayerOnesTurn,
        int?[][] Cells);
}
    */