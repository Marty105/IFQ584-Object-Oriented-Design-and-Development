using System.Text.Json;

namespace TicTacToe;

/// <summary>
/// Coordinates a game of Numerical Tic Tac Toe between two players on a board.
/// Players share a single run of numbers played in order — the 1st move plays 1,
/// the 2nd plays 2, and so on — so the only per-turn state is whose turn it is.
/// </summary>
public class Game : IGame
{
    /// <summary>
    /// The two players in the game. Player one moves first.
    /// </summary>
    public IPlayer PlayerOne { get; private set; }
    public IPlayer PlayerTwo { get; private set; }

    /// <summary>
    /// The game variant.
    /// </summary>
    public GameVariant Variant { get; private set; }


    /// <summary>
    /// The commands that have been played, most recent on top (the Command
    /// pattern's history). Undoing pops from here; a fresh move pushes onto it.
    /// </summary>
    private readonly Stack<ICommand> _undo = new();

    /// <summary>
    /// Commands that have been undone and can be redone, most recent on top. A
    /// fresh move clears this, since redoing onto a board that has moved on no
    /// longer makes sense.
    /// </summary>
    private readonly Stack<ICommand> _redo = new();

    /// <Whicplayer's turn it is> //
    private bool _playerOnesTurn = true;

    /// <summary>
    /// Creates a new game between two players on a board of the given size.
    /// </summary>
    /// <param name="playerOne">The first player.</param>
    /// <param name="playerTwo">The second player.</param>
    /// <param name="variant">The type of game. </param>
    /// 
    public Game(IPlayer playerOne, IPlayer playerTwo, GameVariant variant)
    {
        PlayerOne = playerOne;
        PlayerTwo = playerTwo;
        Variant = variant;
    }

    /// <summary>The player whose turn it is right now.</summary>
    public IPlayer CurrentPlayer => _playerOnesTurn ? PlayerOne : PlayerTwo;
    public IPlayer OtherPlayer => _playerOnesTurn ? PlayerTwo : PlayerOne;


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

        GameVariant variant = GameFactory.CreateGame(state.GameType, state.BoardSize);

        foreach (Placement placement in state.Moves)
        {
            variant.Play(placement);
        }

        // The saved state does not record whether a side was human or computer,
        // so both are rebuilt as humans through the same factory the game uses.
        PlayerOne = PlayerFactory.Create(PlayerKind.Human, state.PlayerOne);
        PlayerTwo = PlayerFactory.Create(PlayerKind.Human, state.PlayerTwo);
        Variant = variant;
        _playerOnesTurn = variant.MoveCount % 2 == 0;
        //GameType = state.GameType;
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
    public MoveOutcome PlayMove(int row, int column, int boardIndex = 0)
    {
        var command = new MoveCommand(Variant, SwapTurn, row, column, boardIndex);

        if (!command.Execute())
        {
            return MoveOutcome.Illegal;
        }

        _undo.Push(command);
        _redo.Clear();
        return command.Outcome;
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
    public Placement? Undo()
    {
        if (_undo.Count == 0)
        {
            return null;
        }

        ICommand command = _undo.Pop();
        command.Undo();
        _redo.Push(command);

        return (command as MoveCommand)?.Placement;
    }

    /// <summary>
    /// Redoes the most recently undone command: re-executes it and moves it back
    /// onto the undo history.
    /// Returns the redone move, or null if there was nothing to redo.
    /// </summary>
    public Placement? Redo()
    {
        if (_redo.Count == 0)
        {
            return null;
        }

        ICommand command = _redo.Pop();
        command.Execute();
        _undo.Push(command);

        return (command as MoveCommand)?.Placement;
    }

    /// <inheritdoc />
    public string State => JsonSerializer.Serialize(Snapshot(), SerializerOptions);

    /// <summary>Options used when serialising the game state.</summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Builds a plain, serialisable snapshot of the whole game: the game variant, the board size and count,
    /// both players' names, whose turn it is, and the grid of numbers (null for
    /// empty cells).
    /// </summary>
    private GameState Snapshot() => new(
        Variant.Type,
        Variant.Boards[0].Size,
        PlayerOne.Name,
        PlayerTwo.Name,
        Variant.History.ToArray());

    /// <summary>The serialisable shape of a whole game.</summary>
    private sealed record GameState(
        GameType GameType,
        int BoardSize,
        string PlayerOne,
        string PlayerTwo,
      Placement[] Moves);

}
    



