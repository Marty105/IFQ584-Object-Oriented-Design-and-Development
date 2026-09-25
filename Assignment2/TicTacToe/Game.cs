using System.Text.Json;

namespace TicTacToe;

/// <summary>
/// Base class for every game type. Coordinates a game between two players on one
/// or more boards: whose turn it is, the move history, undo/redo and saving.
///
/// Playing a move follows the Template Method pattern: <see cref="Play"/> fixes
/// the steps (check the cell, apply the move, record it, evaluate the result) and
/// each concrete game supplies its own rules through <see cref="IsLegal"/>,
/// <see cref="Apply"/> and <see cref="Evaluate"/>.
/// </summary>
public abstract class Game : IGame
{
    /// <summary>
    /// The two players in the game. Player one moves first.
    /// </summary>
    public IPlayer PlayerOne { get; }
    public IPlayer PlayerTwo { get; }

    /// <summary>The boards the game is played on. Most games have just one.</summary>
    private readonly Board[] _boards;

    /// <summary>Every move played so far, in order.</summary>
    private readonly List<Placement> _history = new();

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

    /// <summary>
    /// Initialises the shared game state. Called by subclass constructors.
    /// </summary>
    /// <param name="playerOne">The first player.</param>
    /// <param name="playerTwo">The second player.</param>
    /// <param name="boards">The boards the game is played on; at least one.</param>
    protected Game(IPlayer playerOne, IPlayer playerTwo, params Board[] boards)
    {
        if (boards.Length == 0)
        {
            throw new ArgumentException("A game needs at least one board.", nameof(boards));
        }

        PlayerOne = playerOne;
        PlayerTwo = playerTwo;
        _boards = boards;
    }

    /// <summary>Which kind of game this is.</summary>
    public abstract GameType Type { get; }

    public IReadOnlyList<Board> Boards => _boards;
    public IReadOnlyList<Placement> History => _history;
    public int MoveCount => _history.Count;

    /// <summary>
    /// The player whose turn it is right now. Players alternate, so this follows
    /// from how many moves have been played.
    /// </summary>
    public IPlayer CurrentPlayer => MoveCount % 2 == 0 ? PlayerOne : PlayerTwo;
    public IPlayer OtherPlayer => MoveCount % 2 == 0 ? PlayerTwo : PlayerOne;

    // Move Template //

    /// <summary>
    /// Plays a move: checks it, applies it, records it and evaluates the result.
    /// Returns <see cref="MoveOutcome.Illegal"/> (without changing anything) if
    /// the cell is taken or the game's rules forbid the move.
    /// </summary>
    internal MoveOutcome Play(Placement placement)
    {
        if (!IsOnAnEmptyCell(placement) || !IsLegal(placement))
        {
            return MoveOutcome.Illegal;
        }

        Apply(placement);
        _history.Add(placement);
        return Evaluate(placement);
    }

    /// <summary>
    /// Reverses the last move made. Returns it, or null if there were no moves.
    /// </summary>
    internal Placement? Unplay()
    {
        if (_history.Count == 0)
        {
            return null;
        }

        Placement last = _history[^1];
        _history.RemoveAt(_history.Count - 1);
        _boards[last.BoardIndex].UndoLastMove();
        return last;
    }

    /// <summary>True if the placement names a real board and an empty cell on it.</summary>
    private bool IsOnAnEmptyCell(Placement p)
    {
        if (p.BoardIndex < 0 || p.BoardIndex >= _boards.Length)
        {
            return false;
        }

        Board board = _boards[p.BoardIndex];
        return board.IsInBounds(p.Row, p.Column) && board.GetCell(p.Row, p.Column) is null;
    }

    // Parts game types supply //

    /// <summary>Any extra rule a game places on moves. Allows everything by default.</summary>
    protected virtual bool IsLegal(Placement p) => true;

    /// <summary>Puts the move's piece on the board.</summary>
    protected abstract void Apply(Placement p);

    /// <summary>
    /// Decides what the move just applied means for the player who made it.
    /// </summary>
    protected abstract MoveOutcome Evaluate(Placement p);

    // Additional helpers //

    /// <summary>
    /// Every straight run of <paramref name="length"/> cells on the board: across
    /// rows, down columns and along both diagonal directions.
    /// </summary>
    protected static IEnumerable<(int Row, int Column)[]> Lines(Board board, int length)
    {
        (int Row, int Column)[] directions = { (0, 1), (1, 0), (1, 1), (1, -1) };

        for (int row = 0; row < board.Height; row++)
        {
            for (int column = 0; column < board.Width; column++)
            {
                foreach ((int dRow, int dColumn) in directions)
                {
                    var line = new (int Row, int Column)[length];
                    for (int i = 0; i < length; i++)
                    {
                        line[i] = (row + i * dRow, column + i * dColumn);
                    }

                    if (board.IsInBounds(line[^1].Row, line[^1].Column))
                    {
                        yield return line;
                    }
                }
            }
        }
    }

    // Commands (undo / redo) //

    /// <summary>
    /// Plays the current player's move in the given cell by building and
    /// executing a <see cref="MoveCommand"/>, then recording it on the undo
    /// history. Returns <see cref="MoveOutcome.Illegal"/> (without changing
    /// anything) if the move was not allowed. A fresh move retires any commands
    /// that were waiting to be redone.
    /// </summary>
    public MoveOutcome PlayMove(int row, int column, int boardIndex = 0)
    {
        var command = new MoveCommand(this, row, column, boardIndex);

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
    /// Undoes the most recent command and moves it onto the redo history.
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

    // Saving and loading //

    /// <summary>
    /// Restores a game from its serialised <see cref="State"/>. The game type is
    /// read from the data, so this builds the right kind of game through
    /// <see cref="GameFactory"/> and replays every saved move onto it.
    /// </summary>
    /// <remarks>
    /// Both players are rebuilt as human <see cref="Player"/>s: the saved state
    /// does not record whether a side was human or computer. A loaded game starts
    /// with an empty undo history.
    /// </remarks>
    public static Game Load(string data)
    {
        GameState? state = JsonSerializer.Deserialize<GameState>(data, SerializerOptions);

        if (state is null)
        {
            throw new ArgumentException("The game state is not valid JSON.", nameof(data));
        }

        Game game = GameFactory.CreateGame(
            state.GameType,
            PlayerFactory.Create(PlayerKind.Human, state.PlayerOne),
            PlayerFactory.Create(PlayerKind.Human, state.PlayerTwo),
            state.BoardSize);

        foreach (Placement placement in state.Moves)
        {
            if (game.Play(placement) == MoveOutcome.Illegal)
            {
                throw new InvalidDataException($"The saved move {placement} is not legal.");
            }
        }

        return game;
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

    /// <inheritdoc />
    public string State => JsonSerializer.Serialize(Snapshot(), SerializerOptions);

    /// <summary>Options used when serialising the game state.</summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Builds a plain, serialisable snapshot of the whole game: the game type, the
    /// board size, both players' names and every move played, in order.
    /// </summary>
    private GameState Snapshot() => new(
        Type,
        _boards[0].Size,
        PlayerOne.Name,
        PlayerTwo.Name,
        _history.ToArray());

    /// <summary>The serialisable shape of a whole game.</summary>
    private sealed record GameState(
        GameType GameType,
        int BoardSize,
        string PlayerOne,
        string PlayerTwo,
        Placement[] Moves);
}
