namespace TicTacToe;

/// <summary>
/// The application's single, shared configuration (the Singleton pattern): the
/// board-size bounds and the default player names. There is exactly one settings
/// object for the whole run, reached through <see cref="Instance"/>, so the
/// values are defined in one place instead of being hard-coded at each call site,
/// and every part of the app reads the same configuration.
///
/// A singleton is appropriate here because these are genuinely process-wide
/// settings with no meaningful "second instance": two different sets of board
/// bounds in one run would be a bug, not a feature. The console app is
/// single-threaded, so no locking is needed; the instance is created lazily and
/// only once by the static initialiser.
/// </summary>
public sealed class GameSettings
{
    /// <summary>
    /// The one and only instance. Created on first access and shared thereafter.
    /// </summary>
    public static GameSettings Instance { get; } = new GameSettings();

    /// <summary>
    /// Private so nothing outside can construct a second instance; the only way to
    /// reach the settings is through <see cref="Instance"/>.
    /// </summary>
    private GameSettings()
    {
    }

    /// <summary>The smallest allowed board size (cells per side).</summary>
    public int MinBoardSize { get; } = 3;

    /// <summary>The largest allowed board size (cells per side).</summary>
    public int MaxBoardSize { get; } = 9;

    /// <summary>The default display name for the first (human) player.</summary>
    public string PlayerOneName { get; } = "Player 1";

    /// <summary>The default display name for the second human player.</summary>
    public string PlayerTwoName { get; } = "Player 2";

    /// <summary>The default display name for the computer opponent.</summary>
    public string ComputerName { get; } = "Computer";
}
