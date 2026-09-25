namespace TicTacToe;

// Results from a move //
public enum MoveOutcome { Illegal, Continue, CurrentPlayerWins, CurrentPlayerLoses, Draw }

// Where a move is played: a cell on one of the game's boards. //
public readonly record struct Placement(int Row, int Column, int BoardIndex = 0);
