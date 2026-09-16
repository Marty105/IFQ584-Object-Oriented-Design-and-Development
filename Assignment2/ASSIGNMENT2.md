# Assignment 2 — Numerical Tic Tac Toe

## How to run

**Requires:** the [.NET 10 SDK](https://dotnet.microsoft.com/download). No other
dependencies or setup — all libraries used are part of the .NET base class
library and are restored automatically on first build.

From the `Assignment2/TicTacToe` directory (or point `--project` at it):

```bash
dotnet run --project TicTacToe
```

The program is an interactive console app. It prompts for:

1. **New game or load** a saved `.json` game.
2. **Board size** (3–9; 3 is classic Numerical Tic Tac Toe).
3. **Mode** — human vs human, or human vs computer.

On a turn, press **Enter** to play then type a move as `row column` (0-based),
or `u` undo, `r` redo, `s` save and quit, `h` help. The number you place is the
game's next shared number; complete a row, column or diagonal summing to the
board's magic constant to win.

## External libraries and frameworks

Only the **.NET base class library** is used — no third-party packages. The
reused framework types are:

| Namespace | Class / interface | Used for |
|-----------|-------------------|----------|
| `System.Collections.Generic` | `Stack<T>` | undo and redo command histories |
| `System.Collections.Generic` | `IReadOnlyList<T>`, `IEnumerable<T>`, `List<T>` | exposing and iterating moves, lines and empty cells |
| `System` | `ArraySegment<T>` | read-only view over the played moves |
| `System.Text.Json` | `JsonSerializer`, `JsonSerializerOptions` | save / load game state as JSON |
| `System.Text` | `StringBuilder` | rendering the board as text |
| `System.IO` | `File`, `Directory`, `Path` | reading and writing save files |
| `System` | `Console`, `Environment`, `Random` | console I/O, exit, computer's random move |
