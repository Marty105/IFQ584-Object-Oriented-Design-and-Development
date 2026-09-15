namespace TicTacToe;

/// <summary>
/// A single reversible action in the game (the Command pattern). Encapsulating a
/// request as an object lets the game keep a history of what has been done and
/// walk back and forth through it, which is exactly what undo/redo needs: an
/// <see cref="ICommand"/> knows both how to carry itself out and how to reverse
/// itself, so the game loop never has to special-case each kind of action.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Carries the action out. Returns true if it took effect, false if it could
    /// not be applied (for example, playing into a cell that is already taken), in
    /// which case nothing changes and the command is not added to the history.
    /// </summary>
    bool Execute();

    /// <summary>
    /// Reverses the action, leaving the game exactly as it was before
    /// <see cref="Execute"/> ran.
    /// </summary>
    void Undo();
}
